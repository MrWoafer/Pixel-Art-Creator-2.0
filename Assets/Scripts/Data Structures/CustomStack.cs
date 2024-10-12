using System.Collections.Generic;

namespace PAC.Data_Structures
{
    /// <summary>
    /// A custom implementation of a stack to allow removal of items at a specific index.
    /// </summary>
    public class CustomStack<T>
    {
        private List<T> items = new List<T>();

        public int Count => items.Count;

        /// <summary>
        /// Adds the item to the top of the stack.
        /// </summary>
        public void Push(T item)
        {
            items.Insert(0, item);
        }

        /// <summary>
        /// Returns the item on top of the stack.
        /// </summary>
        public T Peek()
        {
            if (Count == 0)
            {
                throw new System.Exception("Cannot peek at an empty stack.");
            }
            return items[0];
        }

        /// <summary>
        /// Removes and returns the item on top of the stack.
        /// </summary>
        public T Pop()
        {
            if (Count == 0)
            {
                throw new System.Exception("Cannot pop from an empty stack.");
            }
            return RemoveAt(0);
        }

        /// <summary>
        /// Removes the item at the given index and returns it.
        /// </summary>
        public T RemoveAt(int index)
        {
            if (index < 0)
            {
                throw new System.IndexOutOfRangeException("index must be non-negative: " + index);
            }
            if (index >= Count)
            {
                throw new System.IndexOutOfRangeException("index must be less than the number of items in stack. index: " + index + "; Count : " + Count);
            }
            T item = items[0];
            items.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Removes the first occurrence (starting from the top) of the item in the stack.
        /// </summary>
        /// <returns>true if the item is successfully removed.</returns>
        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        /// <summary>
        /// Removes all occurrences of the item in the stack.
        /// </summary>
        public void RemoveAll(T item)
        {
            items.RemoveAll(x => x.Equals(item));
        }

        /// <summary>
        /// Removes all items from the stack.
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        public List<T> ToList()
        {
            return items;
        }
        public T[] ToArray()
        {
            return items.ToArray();
        }
    }
}
