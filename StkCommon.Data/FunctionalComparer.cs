using System;
using System.Collections;
using System.Collections.Generic;

namespace StkCommon.Data
{
	/// <summary>
	/// Имплементация IComparer, задаваемая функцией или лямбдой
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FunctionalComparer<T> : IComparer<T>, IComparer 
	{
		private readonly Func<T, T, int> _comparer;
		public FunctionalComparer(Func<T, T, int> comparer)
		{
			_comparer = comparer;
		}

		/// <summary>
		/// Создать экземпляр IComparer
		/// </summary>
		public static IComparer Create(Func<T, T, int> comparer)
		{
			return new FunctionalComparer<T>(comparer);
		}

		public int Compare(T x, T y)
		{
			return _comparer(x, y);
		}

		public int Compare(object x, object y)
		{
			return Compare((T)x, (T)y);
		}
	}
}