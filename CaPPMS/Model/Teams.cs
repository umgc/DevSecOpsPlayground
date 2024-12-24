namespace CaPPMS.Model
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
    }
}