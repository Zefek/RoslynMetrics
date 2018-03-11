using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    /*
	internal sealed class EmptyEnumerable<T> : IEnumerable<T>, IEnumerable
	{
		private readonly T[] backingField;

		public EmptyEnumerable()
		{
		}

		public IEnumerator<T> GetEnumerator()
		{
			T[] tArray = this.backingField;
			for (int i = 0; i < (int)tArray.Length; i++)
			{
				yield return tArray[i];
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.backingField.GetEnumerator();
		}
	}
    */
}