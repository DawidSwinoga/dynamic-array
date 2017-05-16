using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        protected Object blockingObject;

        public DynamicArray() : this(size: DefaultArraySize)
        {
        }

        public DynamicArray(int size)
        {
            blockingObject = new object();
            _maxCurrentSize = size;
            _lastFilledIndex = -1;
            _array = new int[_maxCurrentSize];
        }

        public int this[int index]
        {
            get
            {
                if (IsIndexInRange(index))
                {
                    return _array[index];
                }
                throw new IndexOutOfRangeException();
            }

            set
            {
                Monitor.Enter(blockingObject);
                try
                {
                    ResizeIfNecessary(index);
                    _array[index] = value;
                    _lastFilledIndex = index;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    Monitor.Exit(blockingObject);
                }
            }
        }

        public bool TryAdd(int item)
        {
            if (!Monitor.TryEnter(blockingObject)) return false;

            try
            {
                Add(item);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Monitor.Exit(blockingObject);
            }
            return true;
        }

        public void BlockingAdd(int item)
        {
            Monitor.Enter(blockingObject);
            try
            {
                Add(item);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Monitor.Exit(blockingObject);
            }
        }

        public void SaveToFile(string fileName)
        {
            File.WriteAllLines(fileName, _array.Select(e => e.ToString()).ToArray());
        }

        private void Add(int item)
        {
            Thread.Sleep(60);
            ResizeIfNecessary(++_lastFilledIndex);
            _array[_lastFilledIndex] = item;
            OnItemAdded(item);
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
                int maxCurrentSize = index * 2;
                Array.Resize(ref _array, maxCurrentSize);
                _maxCurrentSize = maxCurrentSize;
                OnArrayResized(_maxCurrentSize);
            }
        }

        private void OnArrayResized(int maxCurrentSize)
        {
            ArrayResized?.Invoke(maxCurrentSize);
        }
    }
}