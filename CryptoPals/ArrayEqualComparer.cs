using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CryptoPals
{
	public sealed class ArrayEqualComparer<T> : IEqualityComparer<T[]>
	{
		public static readonly ArrayEqualComparer<T> Instance = new ArrayEqualComparer<T>();
		private readonly IEqualityComparer<T> Comparer = EqualityComparer<T>.Default;
		public bool Equals([AllowNull] T[] x, [AllowNull] T[] y)
		{
			if (ReferenceEquals(x, y))
				return true;
			if (x == null || y == null)
				return false;
			if (x.Length != y.Length)
				return false;
			for (int i = 0; i < x.Length; ++i)
			{
				if (!Comparer.Equals(x[i], y[i]))
					return false;
			}
			return true;
		}

		public int GetHashCode([DisallowNull] T[] obj) => 0;
	}
}
