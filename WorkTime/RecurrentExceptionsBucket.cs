using NodaTime;
using System;
using System.Collections.Generic;

namespace WorkTime
{
    public class RecurrentExceptionsBucket
    {
        private readonly Dictionary<int, Dictionary<int, Tuple<short, short>>> _bucket = new Dictionary<int, Dictionary<int, Tuple<short, short>>>();

        private bool AlreadyExists(int Month, int Day)
        {
            if (!_bucket.ContainsKey(Month)) return false;
            return (_bucket[Month].ContainsKey(Day));
        }

        public void Add(LocalDateTime date, short start, short end)
        {
            if (AlreadyExists(date.Month, date.Day)) throw new Exception("There is already an item to the date indicated in the list.");
            if (!_bucket.ContainsKey(date.Month)) _bucket.Add(date.Month, new Dictionary<int, Tuple<short, short>>());
            _bucket[date.Month].Add(date.Day, new Tuple<short, short>(start, end));
        }

        public bool Has(LocalDateTime date)
        {
            if (!_bucket.ContainsKey(date.Month)) return false;
            if (!_bucket[date.Month].ContainsKey(date.Day)) return false;
            return true;
        }

        public Tuple<short, short> GetPeriod(LocalDateTime date)
        {
            if (!_bucket.ContainsKey(date.Month)) throw new KeyNotFoundException();
            if (!_bucket[date.Month].ContainsKey(date.Day)) throw new KeyNotFoundException();
            return _bucket[date.Month][date.Day];
        }
    }
}