using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;

namespace MovieRecommender.Repositories.Movies
{
    public class Movie
    {
        [Name("id")]
        public string Id { get; set; }
        
        [Name("title")]
        public string Title { get; set; }
        
        [Name("director")]
        public string? DirectorName { get; set; }
        
        [Name("genre")]
        public string RawGenre { get; set; }

        public IEnumerable<string> Genres => RawGenre.Split(",").Select(genre => genre.Trim());
    }
}