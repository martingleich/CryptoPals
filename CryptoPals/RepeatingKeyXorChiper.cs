using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CryptoPals
{
	public static class RepeatingKeyXorChiper
	{
		public static Bytes Encrypt(Bytes clearText, Bytes key) => new Bytes(clearText.Select((c, id) => c.Xor(key[id % key.Count])));
		public static Bytes Decrypt(Bytes chiperText, Bytes key) => Encrypt(chiperText, key);

		public static int HammingDistance(Bytes a, Bytes b) => a.Zip(b, HammingDistance).Sum();
		public static int HammingDistance(byte a, byte b) => BitOperations.PopCount((uint)(a ^ b));

		public static IEnumerable<(int, double)> FindKeySizes(Bytes chiperText)
		{
			var rnd = new Random();
			for (int keySize = 2; keySize < 40; ++keySize)
			{
				int count = 10;
				double distance = 0;
				for (int i = 0; i < count; ++i)
				{
					var first = rnd.Next(0, chiperText.Count / keySize) * keySize;
					int second;
					do
					{
						second = rnd.Next(0, chiperText.Count / keySize) * keySize;
					} while (first == second);

					for (int j = 0; j < keySize; ++j)
						distance += HammingDistance(chiperText[first + j], chiperText[second + j]);
				}
				distance /= count * keySize;
				yield return (keySize, distance);
			}
		}

		public static IEnumerable<Bytes> Transpose(IEnumerable<byte> values, int step)
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
			return lists.Select(l => new Bytes(l)).ToArray();
		}

		public static Bytes FindKey(Bytes chiperText, int keySize)
		{
			List<byte> key = new List<byte>();
			foreach (var b in Transpose(chiperText, keySize))
			{
				IEnumerable<(Bytes, byte, double)> blub = SingleByteXorChiper.FindDecryptionKeys(b, 5, 1);
				key.Add(blub.First().Item2);
			}
			return new Bytes(key);
		}

		public static IEnumerable<Bytes> FindDecryption(Bytes chiperText)
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
