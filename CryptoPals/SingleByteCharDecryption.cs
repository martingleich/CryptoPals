using System.Collections.Generic;
using System.Linq;

namespace CryptoPals
{
	public static class SingleByteCharDecryption
	{
		public static readonly ByteFreqMap ENGLISH_FREQ_MAP = ByteFreqMap.FromValues(System.IO.File.ReadAllBytes(@"C:\Home\source\CryptoPals\raw_english.txt"));

		public static Bytes Decrypt(Bytes chiper, byte key) => new Bytes(chiper.Select(c => c.Xor(key)));

		public static IEnumerable<(Bytes, byte, double)> FindDecryptionKeys(Bytes chiperText, int topCount)
		{
			var freqMap = ByteFreqMap.FromValues(chiperText);
			return (from key in Extensions.AllBytes()
					let error = DecryptionError(key, freqMap)
					orderby error
					select (Decrypt(chiperText, key), key, error)).Take(topCount);
		}

		private static double DecryptionError(byte k, ByteFreqMap freqMap)
			=> freqMap.Sum(b =>
			{
				var diff = ENGLISH_FREQ_MAP[k.Xor(b.Key)] - b.Value;
				return diff * diff;
			});
	}
}
