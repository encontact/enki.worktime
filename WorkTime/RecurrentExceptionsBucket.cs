using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTime {
	public class RecurrentExceptionsBucket {
		private readonly Dictionary<int, Dictionary<int, Dictionary<int, Tuple<short, short>>>> _bucket = new Dictionary<int, Dictionary<int, Dictionary<int, Tuple<short, short>>>>();

		public void Add(LocalDateTime date, short start, short end) {
			if (!_bucket.ContainsKey(date.Year)) _bucket.Add(date.Year, new Dictionary<int, Dictionary<int, Tuple<short, short>>>());
			if (!_bucket[date.Year].ContainsKey(date.Month)) _bucket[date.Year].Add(date.Month, new Dictionary<int, Tuple<short, short>>());
			_bucket[date.Year][date.Month].Add(date.Day, new Tuple<short, short>(start, end));
		}

		public bool Has(LocalDateTime date) {
			if (!_bucket.ContainsKey(date.Year)) return false;
			if (!_bucket[date.Year].ContainsKey(date.Month)) return false;
			if (!_bucket[date.Year][date.Month].ContainsKey(date.Day)) return false;
			return true;
		}

		public Tuple<short, short> GetPeriod(LocalDateTime date) {
			if (!_bucket.ContainsKey(date.Year)) throw new KeyNotFoundException();
			if (!_bucket[date.Year].ContainsKey(date.Month)) throw new KeyNotFoundException();
			if (!_bucket[date.Year][date.Month].ContainsKey(date.Day)) throw new KeyNotFoundException();
			return _bucket[date.Year][date.Month][date.Day];
		}
	}
}
