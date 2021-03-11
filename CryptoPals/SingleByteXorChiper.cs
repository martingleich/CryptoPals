using System.Collections.Generic;
using System.Linq;

namespace CryptoPals
{
	public static class SingleByteXorChiper
	{
		public static byte[] Decrypt(byte[] chiperText, byte key) => Bytes.FromRange(chiperText.Select(c => c.Xor(key)));

		public static IEnumerable<(byte, double)> FindDecryptionKeys(byte[] chiperText, int topCount, double cutoff)
		{
			var freqMap = ByteFreqMap.FromValues(chiperText);
			return (from key in Extensions.AllBytes()
					let error = DecryptionError(key, freqMap, cutoff)
					where error <= cutoff
					orderby error
					select (key, error)).Take(topCount);
		}

		private static double DecryptionError(byte k, ByteFreqMap freqMap, double cutoff)
		{
			double sum = 0;
			for (int i = 0; i < 256; ++i)
			{
				byte b = (byte)i;
				var diff = Facts.ENGLISH_FREQ_MAP[k.Xor(b)] - freqMap[b];
				sum += diff * diff;
				if (sum > cutoff)
					return cutoff;
			}
			return sum;
		}
	}
}
