using CsvHelper.Configuration.Attributes;

namespace MovieRecommender.Repositories.Ratings
{
    public class Rating
    {
        [Name("rate")] public int Rate { get; set; }

        [Name("movie")] public string MovieId { get; set; }

        [Name("user")] public string UserId { get; set; }
    }
}