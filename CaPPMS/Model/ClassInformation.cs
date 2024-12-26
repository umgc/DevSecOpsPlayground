using CaPPMS.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CaPPMS.Model
{
    /// <summary>
    /// The class information.
    /// </summary>
    [SqlTableName("ClassInformation")]
    public class ClassInformation
    {
        private static readonly List<Tuple<int[], string>> cohortSeasons = new()
        {
            { Tuple.Create<int[], string>([11, 12, 1, 2], "Spring") },
            { Tuple.Create<int[], string>([3, 4, 5, 6], "Summer") },
            { Tuple.Create<int[], string>([7, 8, 9, 10],"Fall") }
        };

        private DateTime startDate;

        public ClassInformation()
        {
            DateTime now = DateTime.Now;
            int year = now.Year;
            if (now.Month > 10)
            {
                year++;
            }
            string season = cohortSeasons.First(x => x.Item1.Contains(now.Month)).Item2;
            Cohort = $"{season} {year}";
            this.StartDate = NearestDay(now, DayOfWeek.Wednesday);
        }

        /// <summary>
        /// The class id.
        /// </summary>
        [SqlIdProperty]
        public long ClassId { get; set; } = -1;

        /// <summary>
        /// The Course name.
        /// </summary>
        [ColumnHeader]
        public string Course { get; set; } = "SWEN 670";

        /// <summary>
        /// The class name.
        /// </summary>
        [ColumnHeader]
        public string Cohort { get; set; }

        /// <summary>
        /// The start of the class.
        /// </summary>
        [ColumnHeader]
        [DisplayName("Start Date")]
        public DateTime? StartDate
        {
            get
            {
                return this.startDate;
            }
            set
            {
                this.startDate = value ?? NearestDay(DateTime.Now, DayOfWeek.Wednesday);
                this.EndDate = CalculateEndDate(this.startDate);
            }
        }

        /// <summary>
        /// The end of the class.
        /// </summary>
        [ColumnHeader]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; } = DateTime.Now.AddDays(83);

        public override string ToString()
        {
            return $"{Course} - {Cohort}";
        }

        private static DateTime NearestDay(DateTime date, DayOfWeek day)
        {
            while (date.DayOfWeek != day)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        private DateTime CalculateEndDate(DateTime startDate)
        {
            DateTime endDate = startDate.AddDays(83);
            return NearestDay(endDate, DayOfWeek.Tuesday);
        }
    }

    public class ClassInformationComparer : IComparer<ClassInformation>
    {
        public int Compare(ClassInformation? x, ClassInformation? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            string xStr = $"{x.ToString()} - Start:{x.StartDate}. End:{x.EndDate}";
            string yStr = $"{y.ToString()} - Start:{y.StartDate}. End:{y.EndDate}";

            return xStr.CompareTo(yStr);
        }
    }
}
