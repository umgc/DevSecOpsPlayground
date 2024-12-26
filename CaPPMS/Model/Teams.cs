using CaPPMS.Attributes;

namespace CaPPMS.Model
{
    [SqlTableName("Teams")]
    public class Team
    {
        /// <summary>
        /// Id of the team.
        /// </summary>
        [SqlIdProperty]
        public long TeamId { get; set; }

        /// <summary>
        /// Name of the team.
        /// </summary>
        [ColumnHeader]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Class ID.
        /// </summary>
        public long ClassId { get; set; }
    }
}