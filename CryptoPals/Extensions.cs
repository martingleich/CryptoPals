using System.Collections.Generic;

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
	}
}
