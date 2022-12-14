using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace CryptoPals
{
	public static class StringConversion
	{
		private static int ToHexValue(char c)
		{
			if (c >= '0' && c <= '9')
				return c - '0';
			else if (c >= 'a' && c <= 'f')
				return 10 + c - 'a';
			else if (c >= 'A' && c <= 'F')
				return 10 + c - 'A';
			else
				throw new ArgumentException($"{c} is not a valid hex character.");
		}
		public static byte[] HexToBytes(string input)
		{
			if (input is null)
				throw new ArgumentNullException(nameof(input));
			if (input.Length % 2 != 0)
				throw new ArgumentException($"{nameof(input)} must contains a even number of characters.");
			var bytes = new byte[input.Length / 2];
			for (int i = 0; i < input.Length; i += 2)
			{
				bytes[i / 2] = (byte)(ToHexValue(input[i]) << 4 | ToHexValue(input[i + 1]));
			}
			return bytes;
		}
		private static readonly char[] HexChars = Enumerable.Range('0', 10).Concat(Enumerable.Range('A', 7)).Select(c => (char)c).ToArray();
		public static string ToHex(byte[] input)
		{
			if (input is null)
				throw new ArgumentNullException(nameof(input));
			var sb = new StringBuilder();
			foreach (var b in input)
			{
				sb.Append(HexChars[b >> 4]);
				sb.Append(HexChars[b & 0x0F]);
			}
			return sb.ToString();
		}

		private static readonly char[] Base64Chars = Enumerable.Range('A', 26).Concat(Enumerable.Range('a', 26)).Concat(Enumerable.Range('0', 10)).Append('+').Append('/').Select(i => (char)i).ToArray();
		private const int MASK1 = 0xFC;
		private const int MASK2 = 0xF0;
		private const int MASK3 = 0xC0;
		private static void AppendAsBase64(StringBuilder sb, int a, int b, int c)
		{
			// aaaaaaaa bbbbbbbb cccccccc
			// 11111122 22223333 33444444
			sb.Append(Base64Chars[(a & MASK1) >> 2]);
			sb.Append(Base64Chars[((a & ~MASK1) << 4) | ((b & MASK2) >> 4)]);
			sb.Append(Base64Chars[((b & ~MASK2) << 2) | ((c & MASK3) >> 6)]);
			sb.Append(Base64Chars[c & ~MASK3]);
		}
		public static string ToBase64(byte[] input)
		{
			if (input is null)
				throw new ArgumentNullException(nameof(input));
			var sb = new StringBuilder();
			int i = 0;
			for (; i < input.Length / 3; ++i)
				AppendAsBase64(sb, input[3 * i], input[3 * i + 1], input[3 * i + 2]);
			switch (input.Length - i*3)
			{
				case 1:
					AppendAsBase64(sb, input[3*i], 0, 0);
					sb[^1] = '=';
					sb[^2] = '=';
					break;
				case 2:
					AppendAsBase64(sb, input[3*i], input[3*i + 1], 0);
					sb[^1] = '=';
					break;
			}

			return sb.ToString();
		}

		private static readonly ImmutableDictionary<char, byte> Base64Values = Base64Chars.Select((c, i) => KeyValuePair.Create(c, (byte)i)).ToImmutableDictionary();
		public static byte[] Base64ToBytes(string input)
		{
			if (input.Length % 4 != 0)
				throw new ArgumentException(nameof(input));
			List<byte> result = new List<byte>((input.Length / 4) * 3);
			int id = 0;
			for(int i = 0; i < input.Length; ++i)
			{
				char c = input[i];
				if (c == '=')
				{
					int padding = 0;
					for (; i < input.Length; ++i)
					{
						if (input[i] != '=')
							throw new ArgumentException($"Unexpected character '{input[i]}' after start of padding.");
						++padding;
					}
					result.RemoveAt(result.Count - 1);
					break;
				}
				else
				{
					var value = Base64Values[c];
					switch (id)
					{
						case 0:
							result.Add((byte)(value << 2));
							id = 1;
							break;
						case 1:
							result[^1] |= (byte)((value & 0x30) >> 4);
							result.Add((byte)((value & 0x0F) << 4));
							id = 2;
							break;
						case 2:
							result[^1] |= (byte)((value & 0x3C) >> 2);
							result.Add((byte)((value & 0x03) << 6));
							id = 3;
							break;
						case 3:
							result[^1] |= value;
							id = 0;
							break;
					}
				}
			}
			return result.ToArray();
		}
	}
}
