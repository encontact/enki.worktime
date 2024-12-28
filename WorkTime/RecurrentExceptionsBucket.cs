using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkTime
{
    /// <summary>
    /// Classe para processar os blocos de feriados existentes.
    /// Esta classe ignora o Ano, ou seja, processa baseado no mês e dia informados.
    /// </summary>
    public class RecurrentExceptionsBucket
    {
        private bool validateUnique = false;
        private readonly List<RecurrentExceptionItem> _bucket = new List<RecurrentExceptionItem>();

        public RecurrentExceptionsBucket() => validateUnique = false;
        /// <summary>
        /// Construtor com opção de bloqueio para único feriado por dia
        /// </summary>
        /// <param name="validadeUnique">Indica se deve bloquear situações onde existe um feriado no mesmo dia</param>
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
            _bucket.Add(new RecurrentExceptionItem(date.Month, date.Day, start, end));
        }

        public bool Has(LocalDateTime date)
        {
            return AlreadyExists(date.Month, date.Day);
        }

        // [Obsolete("Este método deve ser substituido pelo de lista de períodos.")]
        // public Tuple<short, short> GetPeriod(LocalDateTime date)
        // {
        //     if (!AlreadyExists(date.Month, date.Day)) throw new KeyNotFoundException();
        //     var data = _bucket.First(b => b.Month == date.Month && b.Day == date.Day);
        //     return new Tuple<short, short>(data.Start, data.End);
        // }

        public IEnumerable<(short start, short end)> GetPeriods(LocalDateTime date)
        {
            if (!AlreadyExists(date.Month, date.Day)) throw new KeyNotFoundException();
            return _bucket.Where(b => b.Month == date.Month && b.Day == date.Day).Select(b => (b.Start, b.End));
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