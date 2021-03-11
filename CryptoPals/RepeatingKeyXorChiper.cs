using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoPals
{
	public static class RepeatingKeyXorChiper
	{
		public static byte[] Encrypt(byte[] clearText, byte[] key) => Bytes.FromRange(clearText.Select((c, id) => c.Xor(key[id % key.Length])));
		public static byte[] Decrypt(byte[] chiperText, byte[] key) => Encrypt(chiperText, key);

		public static IEnumerable<(int, double)> FindKeySizes(byte[] chiperText)
		{
			var rnd = new Random();
			for (int keySize = 2; keySize < 40; ++keySize)
			{
				int count = 10;
				double distance = 0;
				for (int i = 0; i < count; ++i)
				{
					var (first, second) = rnd.NextDiffrentMultiples(chiperText.Length, keySize);
					for (int j = 0; j < keySize; ++j)
						distance += chiperText[first + j].HammingDistance(chiperText[second + j]);
				}
				distance /= count * keySize;
				yield return (keySize, distance);
			}
		}

		public static byte[] FindKey(byte[] chiperText, int keySize)
		{
			List<byte> key = new List<byte>();
			foreach (var b in chiperText.Transpose(keySize))
			{
				var keyByte = SingleByteXorChiper.FindDecryptionKeys(b, 1, 0.05).First().Item1;
				key.Add(keyByte);
			}
			return Bytes.FromRange(key);
		}

		public static IEnumerable<byte[]> FindDecryption(byte[] chiperText)
		{
			var keySizes = FindKeySizes(chiperText).OrderBy(k => k.Item2);
			foreach (var (keySize, prop) in keySizes)
			{
				var key = FindKey(chiperText, keySize);
				var decrypted = Decrypt(chiperText, key);
				yield return decrypted;
			}
		}
	}
}
