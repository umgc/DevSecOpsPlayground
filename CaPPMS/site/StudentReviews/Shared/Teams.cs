using CsvHelper.Configuration.Attributes;

namespace StudentReviews.Shared
{
    public class Teams
    {
        [Name("TeamId")]
        public int TeamId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}