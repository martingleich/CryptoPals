using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace CryptoPals
{
	public struct Bytes : IEquatable<Bytes>, IReadOnlyCollection<byte>
	{
		private readonly byte[] Raw;

		public int Count => ((IReadOnlyCollection<byte>)Raw).Count;

		public Bytes(byte[] raw)
		{
			Raw = raw;
		}

		public static Bytes FromHex(string input) => new Bytes(Conversions.HexToBytes(input));

		public override bool Equals(object? obj) =>
			obj is Bytes bytes && Equals(bytes);
		public bool Equals([AllowNull] Bytes other)
		{
			if (Raw.Length != other.Raw.Length)
				return false;
			for (int i = 0; i < Raw.Length; ++i)
				if (Raw[i] != other.Raw[i])
					return false;
			return true;
		}

		public string ToBase64() => Conversions.ToBase64(Raw);

		public override string ToString() => ToBase64();
		public override int GetHashCode() => Raw.Aggregate(0, (a,b) => HashCode.Combine(a, b));

		public IEnumerator<byte> GetEnumerator()
		{
			return ((IEnumerable<byte>)Raw).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Raw.GetEnumerator();
		}
	}

	internal static class Conversions
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
	}
}
