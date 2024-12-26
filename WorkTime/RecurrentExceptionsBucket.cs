using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkTime
{
    public class RecurrentExceptionsBucket
    {
        private bool validateUnique = false;
        private readonly List<RecurrentExceptionItem> _bucket = new List<RecurrentExceptionItem>();

        public RecurrentExceptionsBucket() => validateUnique = false;
        public RecurrentExceptionsBucket(bool validadeUnique) => this.validateUnique = validadeUnique;

        private bool AlreadyExists(int Month, int Day)
        {
            if (!_bucket.Any(b => b.Month == Month)) return false;
            return _bucket.Where(b => b.Month == Month).Any(b => b.Day == Day);
        }

        public void Add(LocalDateTime date, short start, short end)
        {
            var existsDay = AlreadyExists(date.Month, date.Day);
            if (validateUnique && existsDay) throw new Exception("There is already an item to the date indicated in the list.");
            // if (existsDay)
            // {
            //     _bucket.First(b => b.Month == date.Month && b.Day == date.Day).ExtendTo(start, end);
            // }
            // else
            // {
                _bucket.Add(new RecurrentExceptionItem(date.Month, date.Day, start, end));
            // }
        }

        public bool Has(LocalDateTime date)
        {
            return AlreadyExists(date.Month, date.Day);
        }

        [Obsolete("Este método deve ser substituido pelo de lista de períodos.")]
        public Tuple<short, short> GetPeriod(LocalDateTime date)
        {
            if (!AlreadyExists(date.Month, date.Day)) throw new KeyNotFoundException();
            var data = _bucket.First(b => b.Month == date.Month && b.Day == date.Day);
            return new Tuple<short, short>(data.Start, data.End);
        }

        public IEnumerable<Tuple<short, short>> GetPeriods(LocalDateTime date)
        {
            if (!AlreadyExists(date.Month, date.Day)) throw new KeyNotFoundException();
            return _bucket.Where(b => b.Month == date.Month && b.Day == date.Day).Select(b => new Tuple<short, short>(b.Start, b.End));
        }
    }

    internal class RecurrentExceptionItem
    {
        public int Month { get; private set; }
        public int Day { get; private set; }
        public short Start { get; private set; }
        public short End { get; private set; }

        public RecurrentExceptionItem(int month, int day, short start, short end)
        {
            Month = month;
            Day = day;
            Start = start;
            End = end;
        }

        internal void ExtendTo(short start, short end)
        {
            if(start < Start) Start = start;
            if(end > End) End = end;
        }
    }
}