using System;
using System.Linq;

namespace CryptoPals
{
	public static class MyTools
	{
		private static readonly byte[] Random16BytesBlock = new Random(1234).NextNBytes(16);
		public static bool IsEBC(Func<byte[], byte[]> encrypt, int blockSize = 16)
		{
			var input = Enumerable.Repeat(Random16BytesBlock, 3).Concat(); // 3 repeating blocks is the minmal number so that two identical blocks are inside the final input.
			return encrypt(input).Split(blockSize).HasDuplicates(ArrayEqualComparer<byte>.Instance);
		}

		public static int DetectBlockSize(Func<byte[], byte[]> func)
		{
			int size1 = func(Bytes.Repeat(0, 0)).Length;
			int i = 1;
			while (true)
			{
				int size2 = func(Bytes.Repeat(0, i)).Length;
				if (size2 != size1)
					return size2 - size1;
				++i;
			}
		}
	}
}
