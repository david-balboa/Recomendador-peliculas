using System.Collections.Generic;
using System.Linq;
using MovieRecommender.Repositories.Movies;
using MovieRecommender.Repositories.Ratings;

namespace MovieRecommender
{
    public class Recommender
    {
        private static MovieRepository _moviesRepository = null!;
        private static RatingRepository _ratingsRepository = null!;

        public Recommender(MovieRepository movieRepository, RatingRepository ratingRepository)
        {
            _moviesRepository = movieRepository;
            _ratingsRepository = ratingRepository;
        }

        public string? RecommendMovie(string userId, bool reverseMode = false)
        {
            var similarUsers = _ratingsRepository.GetSimilarUsers(userId, reverseMode).ToList();
            if (!similarUsers.Any())
            {
                return string.Empty;
            }

            var newUserId = similarUsers.First();
            var userRatings = _ratingsRepository.GetRatingsByUserId(userId);
            var newUserMoviesIds = reverseMode ? GetDislikedMovies(newUserId) : GetFavouriteMovies(newUserId);
            var unseenMovies = newUserMoviesIds.Where(movieId => !userRatings.ContainsKey(movieId));

            var genrePreference = GetGenrePreference(userRatings);
            var orderedMovies = OrderMoviesByGenrePreference(unseenMovies, genrePreference);
            return orderedMovies.FirstOrDefault();
        }

        public IEnumerable<string> GetFavouriteMovies(string userId)
        {
            var userRatings = _ratingsRepository.GetRatingsByUserId(userId);
            return userRatings.Where(rating => rating.Value == 5).Select(rating => rating.Key);
        }

        public IEnumerable<string> GetDislikedMovies(string userId)
        {
            var userRatings = _ratingsRepository.GetRatingsByUserId(userId);
            return userRatings.Where(rating => rating.Value == 1).Select(rating => rating.Key);
        }

        public IEnumerable<string> GetGenrePreference(IDictionary<string, int> userRatings)
        {
            IDictionary<string, int> ratedGenreCounter = new Dictionary<string, int>();
            foreach (var (movieId, _) in userRatings)
            {
                var movieGenres = _moviesRepository.GetMovieById(movieId).Genres;
                foreach (var genre in movieGenres)
                {
                    if (!ratedGenreCounter.ContainsKey(genre))
                    {
                        ratedGenreCounter.Add(genre, 0);
                    }

                    ratedGenreCounter[genre] += 1;
                }
            }

            return Tools.OrderDictionaryKeysByValues(ratedGenreCounter);
        }

        private static IEnumerable<string> OrderMoviesByGenrePreference(IEnumerable<string> movies,
            IEnumerable<string> genrePreference)
        {
            IDictionary<string, int> preferenceValueByMovieId = new Dictionary<string, int>();
            foreach (var movieId in movies)
            {
                var preferenceValue = GetMoviePreferenceValue(movieId, genrePreference);
                preferenceValueByMovieId.Add(movieId, preferenceValue);
            }

            return Tools.OrderDictionaryKeysByValues(preferenceValueByMovieId);
        }

        private static int GetMoviePreferenceValue(string movieId, IEnumerable<string> genrePreference)
        {
            var genrePreferenceList = genrePreference.Reverse().ToList();
            var movieGenres = _moviesRepository.GetMovieById(movieId).Genres;
            return movieGenres.Sum(genre => genrePreferenceList.FindIndex(gen => gen.Equals(genre)));
        }
    }
}