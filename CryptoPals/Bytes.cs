using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoPals
{
	public static class Bytes
	{
		public static byte[] FromRange(IEnumerable<byte> bytes) => bytes.ToArray();
		public static byte[] FromHex(string input) => StringConversion.HexToBytes(input);
		public static byte[] FromASCII(string input) => Encoding.ASCII.GetBytes(input);
		public static byte[] FromBase64(string input) => StringConversion.Base64ToBytes(input);
		public static byte[] FromBase64(params string[] input) => StringConversion.Base64ToBytes(string.Concat(input));

		public static string ToHex(this byte[] self) => StringConversion.ToHex(self);
		public static string ToBase64(this byte[] self) => StringConversion.ToBase64(self);
		public static string ToASCII(this byte[] self) => Encoding.ASCII.GetString(self);
		public static string ToPrintableASCII(this byte[] self)
		{
			var ascii = self.ToASCII();
			var sb = new StringBuilder();
			foreach (var c in ascii)
			{
				if (c == '\n') sb.Append("\\n");
				else if (c == '\r') sb.Append("\\r");
				else if (c == '\t') sb.Append("\\t");
				else if (c == '\\') sb.Append("\\\\");
				else if (char.IsControl(c) || c > 127) sb.Append($"\\x{(int)c:X2}");
				else sb.Append(c);
			}
			return sb.ToString();
		}

		public static byte[] Subrange(this byte[] self, int start, int length)
		{
			var arr = new byte[length];
			Array.Copy(self, start, arr, 0, length);
			return arr;
		}

		public static int HammingDistance(this byte[] a, byte[] b) => a.Zip(b, Extensions.HammingDistance).Sum();

		public static byte[] Xor(this byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
				throw new ArgumentException("Diffrent length of parameters");
			return FromRange(a.Zip(b, (a, b) => (byte)(a ^ b)));
		}

		public static byte[][] Transpose(this byte[] self, int step)
		{
			List<byte>[] lists = new List<byte>[step];
			for (int i = 0; i < step; ++i)
				lists[i] = new List<byte>();
			int j = 0;
			foreach (var b in self)
			{
				lists[j].Add(b);
				j = (j + 1) % step;
			}
			return lists.Select(l => l.ToArray()).ToArray();
		}

		public static byte[][] Split(this byte[] self, int step)
		{
			List<byte[]> blocks = new List<byte[]>();
			for (int i = 0; i < self.Length; i += step)
			{
				var block = new byte[step];
				Array.Copy(self, i, block, 0, step);
				blocks.Add(block);
			}
			return blocks.ToArray();
		}

		public static byte[] Pad_PKCS_7_Multiple(this byte[] data, int blockSize)
			=> data.Pad_PKCS_7(((data.Length + blockSize - 1) / blockSize) * blockSize);
		public static byte[] Pad_PKCS_7(this byte[] data, int paddingLength)
		{
			if (data.Length > paddingLength)
				throw new ArgumentException($"data.Length({data.Length}) > paddingLength({paddingLength})");
			if(data.Length - paddingLength > 255)
				throw new ArgumentException($"data.Length({data.Length}) - paddingLength({paddingLength}) > 255");
			byte[] result = new byte[paddingLength];
			Array.Copy(data, result, data.Length);
			for (int i = data.Length; i < paddingLength; ++i)
				result[i] = (byte)(paddingLength - data.Length);
			return result;
		}
	}
}
