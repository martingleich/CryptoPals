using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CryptoPals
{
	public struct ByteFreqMap : IEnumerable<KeyValuePair<byte, double>>
	{
		private readonly ImmutableArray<double> Frequencies;

		private ByteFreqMap(ImmutableArray<double> frequencies)
		{
			Frequencies = frequencies;
		}

		public static ByteFreqMap FromValues(IEnumerable<byte> bytes)
		{
			var counts = new int[256];
			var total = 0;
			foreach (var b in bytes)
			{
				counts[b] += 1;
				++total;
			}
			return new ByteFreqMap(counts.Select(c => c / (double)total).ToImmutableArray());
		}

		public double this[byte idx] => Frequencies[idx];

		public IEnumerator<KeyValuePair<byte, double>> GetEnumerator() => Frequencies.Select((v, i) => KeyValuePair.Create((byte)i, v)).GetEnumerator();
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
