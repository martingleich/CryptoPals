using System;
using System.Linq;

namespace CryptoPals
{
	public static class Operations
	{
		public static Bytes Xor(Bytes a, Bytes b)
		{
			if (a.Count != b.Count)
				throw new ArgumentException("Diffrent length of parameters");
			return new Bytes(a.Zip(b, (a, b) => (byte)(a ^ b)));
		}
	}
}
