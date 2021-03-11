using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CryptoPals
{
	public static class RepeatingKeyXorChiper
	{
		public static byte[] Encrypt(byte[] clearText, byte[] key) => Bytes.Create(clearText.Select((c, id) => c.Xor(key[id % key.Length])));
		public static byte[] Decrypt(byte[] chiperText, byte[] key) => Encrypt(chiperText, key);

		public static int HammingDistance(byte[] a, byte[] b) => a.Zip(b, HammingDistance).Sum();
		public static int HammingDistance(byte a, byte b) => BitOperations.PopCount((uint)(a ^ b));

		public static IEnumerable<(int, double)> FindKeySizes(byte[] chiperText)
		{
			var rnd = new Random();
			for (int keySize = 2; keySize < 40; ++keySize)
			{
				int count = 10;
				double distance = 0;
				for (int i = 0; i < count; ++i)
				{
					var first = rnd.Next(0, chiperText.Length / keySize) * keySize;
					int second;
					do
					{
						second = rnd.Next(0, chiperText.Length / keySize) * keySize;
					} while (first == second);

					for (int j = 0; j < keySize; ++j)
						distance += HammingDistance(chiperText[first + j], chiperText[second + j]);
				}
				distance /= count * keySize;
				yield return (keySize, distance);
			}
		}

		public static IEnumerable<byte[]> Transpose(IEnumerable<byte> values, int step)
		{
			List<byte>[] lists = new List<byte>[step];
			for (int i = 0; i < step; ++i)
				lists[i] = new List<byte>();
			int j = 0;
			foreach (var b in values)
			{
				lists[j].Add(b);
				j = (j + 1) % step;
			}
			return lists.Select(l => l.ToArray()).ToArray();
		}

		public static byte[] FindKey(byte[] chiperText, int keySize)
		{
			List<byte> key = new List<byte>();
			foreach (var b in Transpose(chiperText, keySize))
			{
				var keyByte = SingleByteXorChiper.FindDecryptionKeys(b, 1, 0.05).First().Item1;
				key.Add(keyByte);
			}
			return Bytes.Create(key);
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
