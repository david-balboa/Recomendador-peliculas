using System.Collections.Generic;
using System.Linq;

namespace MovieRecommender.Repositories.Movies
{
    public class MovieRepository : BaseRepository<Movie>
    {
        private readonly IEnumerable<Movie> _movies;

        internal MovieRepository(string fileName) : base(fileName)
        {
            _movies = Items;
        }

        public Movie? GetMovieById(string id)
        {
            return _movies.FirstOrDefault(movie => movie.Id == id);
        }

        public IEnumerable<Movie> GetMoviesByGenres(IEnumerable<string> genres)
        {
            return _movies.Where(movie => movie.Genres.Intersect(genres).Any());
        }
    }
}