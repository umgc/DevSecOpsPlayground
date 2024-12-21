using System;
using System.Collections.Generic;
using System.Data;

namespace CaPPMS.Data
{
    public class Team
    {
        /// <summary>
        /// Id of the team.
        /// </summary>
        public long TeamId { get; set; }

        /// <summary>
        /// Name of the team.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Class ID.
        /// </summary>
        public long ClassId { get; set; }

        public static IEnumerable<Team> GetTeams(IDataReader dataReader)
        {
            if (dataReader.IsClosed)
            {
                throw new InvalidOperationException("Data Reader is closed.");
            }

            while (dataReader.Read())
            {
                yield return new Team
                {
                    TeamId = Convert.ToInt64(dataReader[nameof(TeamId)]),
                    Name = dataReader[nameof(Name)].ToString() ?? string.Empty,
                    ClassId = Convert.ToInt64(dataReader[nameof(ClassId)])
                };
            }
        }
    }
}