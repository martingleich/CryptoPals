using System.Collections.Generic;
using System.Linq;

namespace CryptoPals
{
	public static class SingleByteCharDecryption
	{
		public static readonly ByteFreqMap ENGLISH_FREQ_MAP = ByteFreqMap.FromValues(System.IO.File.ReadAllBytes(@"C:\Home\source\CryptoPals\raw_english.txt"));

		public static Bytes Decrypt(Bytes chiper, byte key) => new Bytes(chiper.Select(c => c.Xor(key)));

		public static IEnumerable<(Bytes, byte)> FindDecryptionKeys(Bytes chiperText)
		{
			var freqMap = ByteFreqMap.FromValues(chiperText);
			return from key in Extensions.AllBytes()
				   orderby DecryptionError(key, freqMap)
				   select (Decrypt(chiperText, key), key);
		}

		private static double DecryptionError(byte k, ByteFreqMap freqMap)
			=> freqMap.Select(b =>
			{
				var diff = ENGLISH_FREQ_MAP[k.Xor(b.Key)] - b.Value;
				return diff * diff;
			}).Sum();
	}
}
