using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CryptoPals
{
	public struct ByteFreqMap : IEnumerable<KeyValuePair<byte, double>>
	{
		private readonly ImmutableArray<double> Frequencies;
		public readonly ImmutableArray<byte> BytesByDecreasingFrequencies;

		private ByteFreqMap(ImmutableArray<double> frequencies)
		{
			Frequencies = frequencies;
			BytesByDecreasingFrequencies = Frequencies.AddIDs().OrderByDescending(v => v.Value).Select(v => (byte)v.Id).ToImmutableArray();
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

		public double Sum(Func<KeyValuePair<byte, double>, double> select)
		{
			double sum = 0;
			for (int i = 0; i < 256; ++i)
				sum += select(KeyValuePair.Create((byte)i, Frequencies[i]));
			return sum;
		}
	}
}
