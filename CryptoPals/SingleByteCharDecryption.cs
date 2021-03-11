using System.Collections.Generic;
using System.Linq;

namespace CryptoPals
{
	public static class SingleByteCharDecryption
	{
		public static readonly ByteFreqMap ENGLISH_FREQ_MAP = ByteFreqMap.FromValues(System.IO.File.ReadAllBytes(@"C:\Home\source\CryptoPals\raw_english.txt"));

		public static Bytes Decrypt(Bytes chiper, byte key) => new Bytes(chiper.Select(c => c.Xor(key)));

		public static IEnumerable<(Bytes, byte, double)> FindDecryptionKeys(Bytes chiperText, int topCount, double cutoff)
		{
			var freqMap = ByteFreqMap.FromValues(chiperText);
			return (from key in Extensions.AllBytes()
					let error = DecryptionError(key, freqMap, cutoff)
					where error <= cutoff
					orderby error
					select (Decrypt(chiperText, key), key, error)).Take(topCount);
		}

		private static double DecryptionError(byte k, ByteFreqMap freqMap, double cutoff)
		{
			double sum = 0;
			for (int i = 0; i < 256; ++i)
			{
				byte b = (byte)i;
				var diff = ENGLISH_FREQ_MAP[k.Xor(b)] - freqMap[b];
				sum += diff * diff;
				if (sum > cutoff)
					return cutoff;
			}
			return sum;
		}
	}
}
