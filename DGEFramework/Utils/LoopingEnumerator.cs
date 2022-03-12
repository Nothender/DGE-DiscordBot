using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DGE.Utils
{
    /// <summary>
    /// A never ending looping over itself enumerator
    /// </summary>
    public class LoopingEnumerator<T> : IEnumerator
    {

        protected readonly T[] array;
        protected int pointer = -1;

        public T Current => array[pointer];

        object IEnumerator.Current => Current;

        public LoopingEnumerator(T[] array)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Array cannot be null or empty");
            this.array = array;
        }

        public bool MoveNext()
        {
            pointer = (pointer + 1) % array.Length;
            return true;
        }

        public void Reset()
        {
            pointer = -1;
        }
    }
}
