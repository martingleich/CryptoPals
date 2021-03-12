using System;

namespace CryptoPals
{
	public static class RandomExtensions
	{
		public static (int, int) NextDiffrentMultiples(this Random self, int max, int step)
		{
			var first = self.Next(0, max / step) * step;
			int second;
			do
			{
				second = self.Next(0, max/step) * step;
			} while (first == second);

			return (first, second);
		}
		public static byte[] NextNBytes(this Random self, int count)
		{
			var result = new byte[count];
			self.NextBytes(result);
			return result;
		}
		public static bool NextBool(this Random self) => self.Next(0, 2) == 0;
	}
}
