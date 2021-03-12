using System;
using System.Linq;

namespace CryptoPals
{
	public static class MyTools
	{
		private static readonly byte[] Random16BytesBlock = new Random(1234).NextNBytes(16);
		public static bool IsEBC(Func<byte[], byte[]> encrypt)
		{
			var input = Enumerable.Repeat(Random16BytesBlock, 3).Concat(); // 3 repeating blocks is the minmal number so that two identical blocks are inside the final input.
			return encrypt(input).Split(16).HasDuplicates(ArrayEqualComparer<byte>.Instance);
		}
	}
}
