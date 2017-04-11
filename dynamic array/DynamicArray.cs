using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dynamic_array
{
    class DynamicArray
    {
        public delegate void ItemAddedEventHandler(int addedElement, int currentSize);

        public delegate void ArrayResizedEventHandler(int currentSize);

        public event ArrayResizedEventHandler ArrayResized;

        public event ItemAddedEventHandler ItemAdded;

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int FirstIndexInArray = 0;
        private const int DefaultArraySize = 2;
        private int[] _array;
        private int _maxCurrentSize;
        private int _lastFilledIndex;

        public DynamicArray() : this(size: DefaultArraySize)
        {
        }

        public DynamicArray(int size)
        {
            _maxCurrentSize = size;
            _lastFilledIndex = -1;
            _array = new int[_maxCurrentSize];
            Log.Debug("max current size = " + _maxCurrentSize);
            Log.Debug("last fillded index = " + _lastFilledIndex + "\n");
        }

        public int this[int index]
        {
            get
            {
                Log.Debug("get index = " + index);
                if (IsIndexInRange(index))
                {
                    return _array[index];
                }
                throw new IndexOutOfRangeException();
            }

            set
            {
                ResizeIfNecessary(index);
                _array[index] = value;
                _lastFilledIndex = index;
                Log.Debug("set index = " + index + " value = " + value);
                Log.Debug("last filled index = " + _lastFilledIndex);
            }
        }

        public void Add(int item)
        {
            ResizeIfNecessary(++_lastFilledIndex);
            _array[_lastFilledIndex] = item;
            OnItemAdded(item);
            Log.Debug("add element = " + item);
            Log.Debug("last filled index = " + _lastFilledIndex);
        }

        protected virtual void OnItemAdded(int item)
        {
            ItemAdded?.Invoke(item, _maxCurrentSize);
        }

        private bool IsIndexInRange(int index)
        {
            return index <= _lastFilledIndex && index >= FirstIndexInArray;
        }

        private void ResizeIfNecessary(int index)
        {
            if (index >= _maxCurrentSize)
            {
                _maxCurrentSize = index * 2;
                Array.Resize(ref _array, _maxCurrentSize);
                OnArrayResized(_maxCurrentSize);
                Log.Debug("max current size = " + _maxCurrentSize);
            }
        }

        private void OnArrayResized(int maxCurrentSize)
        {
            ArrayResized?.Invoke(maxCurrentSize);
        }
    }
}