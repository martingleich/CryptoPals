using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
}
