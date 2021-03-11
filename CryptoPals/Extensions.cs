using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CryptoPals
{
	public static class Extensions
	{
		public static IEnumerable<byte> AllBytes()
		{
			for (byte i = 0; i != 255; ++i)
				yield return i;
			yield return 255;
		}
		public static byte Xor(this byte a, byte b) => (byte)(a ^ b);
		public static int HammingDistance(this byte a, byte b) => BitOperations.PopCount((uint)(a ^ b));
		public static IEnumerable<(T Value, int Id)> AddIDs<T>(this IEnumerable<T> self, int offset = 0, int scale = 1) => self.Select((x, i) => (x, offset + scale*i));
	}
}
