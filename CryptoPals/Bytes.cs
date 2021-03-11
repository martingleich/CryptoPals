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
		public readonly byte[] Raw;

		public int Count => ((IReadOnlyCollection<byte>)Raw).Count;

		public Bytes(params byte[] raw)
		{
			Raw = raw;
		}
		public Bytes(IEnumerable<byte> raw) : this(raw.ToArray())
		{
		}

		public static Bytes FromHex(string input) => new Bytes(StringConversion.HexToBytes(input));
		public static Bytes FromASCII(string input) => new Bytes(Encoding.ASCII.GetBytes(input));
		internal string ToHex() => StringConversion.ToHex(Raw);

		public override bool Equals(object? obj) => obj is Bytes bytes && Equals(bytes);
		public bool Equals([AllowNull] Bytes other)
		{
			if (Raw.Length != other.Raw.Length)
				return false;
			for (int i = 0; i < Raw.Length; ++i)
				if (Raw[i] != other.Raw[i])
					return false;
			return true;
		}

		public string ToBase64() => StringConversion.ToBase64(Raw);
		public string ToASCII() => Encoding.ASCII.GetString(Raw);
		public string ToPrintableASCII()
		{
			var ascii = ToASCII();
			var sb = new StringBuilder();
			foreach (var c in ascii)
			{
				if (c == '\n') sb.Append("\\n");
				else if (c == '\r') sb.Append("\\r");
				else if (c == '\t') sb.Append("\\t");
				else if (c == '\\') sb.Append("\\\\");
				else if (char.IsControl(c) || c > 127) sb.Append($"\\x{(int)c:X4}");
				else sb.Append(c);
			}
			return sb.ToString();
		}

		public override string ToString() => ToBase64();
		public override int GetHashCode() => Raw.Aggregate(0, (a,b) => HashCode.Combine(a, b));

		public byte this[int idx] => Raw[idx];

		public IEnumerator<byte> GetEnumerator()
		{
			return ((IEnumerable<byte>)Raw).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Raw.GetEnumerator();
		}
	}
}
