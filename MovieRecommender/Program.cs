using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommender.Repositories.Movies;
using MovieRecommender.Repositories.Ratings;
using MovieRecommender.Repositories.Users;

namespace MovieRecommender
{
    public class MovieRecommender
    {
        private static UserRepository _usersRepository = null!;
        private static MovieRepository _moviesRepository = null!;
        private static RatingRepository _ratingsRepository = null!;

        private static Recommender _recommender = null!;

        static void Main(string[] args)
        {
            // set up repositories
            _usersRepository = new UserRepository("users.csv");
            _moviesRepository = new MovieRepository("movies.csv");
            _ratingsRepository = new RatingRepository("ratings.csv", 0.8);

            // set up Recommender
            _recommender = new Recommender(_moviesRepository, _ratingsRepository);

            // testing repositories
            TestGetUsersById();
            TestGetUsersByAge();
            TestGetMoviesById();
            TestGetMoviesByGenre();
            TestGetRatingsByUserId();
            TestGetRatingsByMovieId();
            TestGetFavouriteMoviesFromUser();
            TestGetDislikedMoviesFromUser();
            TestGetGenrePreferenceFromUser();


            // recommender
            Tools.SendToUser("Which user would like to recommend a movie? Provide their ID:");
            var userId = Console.ReadLine();
            if (!string.IsNullOrEmpty(userId))
            {
                var user = _usersRepository.GetUserById(userId);
                if (user == null)
                {
                    Tools.SendToUser($"The user ID given '{userId} is not a valid one.");
                    return;
                }
            }

            var movieId = _recommender.RecommendMovie(userId);
            if (!string.IsNullOrEmpty(movieId))
            {
                var movie = _moviesRepository.GetMovieById(movieId).Title;
                Tools.SendToUser($"The user with ID '{userId}' would enjoy watching the movie: '{movie}'.");
            }
            else
            {
                Tools.SendToUser($"Unfortunately, there is no movie to recommend to the user with ID '{userId}'.");
            }

            Tools.SendToUser("End of the program");
        }

        // ---------------------------------------------------------------- Test methods
        private static void TestGetUsersById()
        {
            Tools.SendToUser("What user do you want information on? Provide their ID:");
            var userId = Console.ReadLine();

            while (!string.IsNullOrEmpty(userId))
            {
                var user = _usersRepository.GetUserById(userId);

                Tools.SendToUser(user != null,
                    $"The name of the user with ID '{userId}' is '{user.Name}' and they're '{user.Age}' years old.",
                    $"The ID '{userId}' given does not belong to any user."
                );

                Tools.SendToUser("Provide another user ID or press ENTER to finish:");
                userId = Console.ReadLine();
            }
        }

        private static void TestGetUsersByAge()
        {
            Tools.SendToUser("What age users do you want to know? Provide one age:");
            var age = Console.ReadLine();

            while (!string.IsNullOrEmpty(age))
            {
                var users = _usersRepository.GetUsersByAge(int.Parse(age)).ToArray();
                Tools.SendToUser(!users.Any(),
                    "There are no users with such age.",
                    $"- {string.Join("\n- ", users.Select(u => u.Name))}"
                );

                Tools.SendToUser("Provide another age or press ENTER to finish:");
                age = Console.ReadLine();
            }
        }

        private static void TestGetMoviesById()
        {
            Tools.SendToUser("What movie do you want information on? Provide its ID:");
            var movieId = Console.ReadLine();

            while (!string.IsNullOrEmpty(movieId))
            {
                var movie = _moviesRepository.GetMovieById(movieId);

                Tools.SendToUser(
                    movie != null,
                    $"The title of the movie with ID '{movieId}' is '{movie.Title}' and belongs to the genres: '{string.Join(", ", movie.Genres)}'",
                    $"The ID '{movieId}' given does not belong to any movie."
                );

                Tools.SendToUser("Provide another movie ID or press ENTER to finish:");
                movieId = Console.ReadLine();
            }
        }

        private static void TestGetMoviesByGenre()
        {
            Tools.SendToUser("The movies of what genre do you want to know? Provide one or more genres (with ','):");
            var rawGenre = Console.ReadLine();

            while (!string.IsNullOrEmpty(rawGenre))
            {
                var genres = rawGenre.Split(",");
                var normalizedGenres = NormalizeGenres(genres);
                var movies = _moviesRepository.GetMoviesByGenres(normalizedGenres).ToArray();
                Tools.SendToUser(!movies.Any(),
                    "There are no movies with such genres.",
                    $"- {string.Join("\n- ", movies.Select(m => m.Title))}"
                );

                Tools.SendToUser("Provide more genres or press ENTER to finish:");
                rawGenre = Console.ReadLine();
            }
        }

        private static void TestGetRatingsByUserId()
        {
            Tools.SendToUser("What user do you want to know their ratings? Provide their ID:");
            var userId = Console.ReadLine();

            while (!string.IsNullOrEmpty(userId))
            {
                var userRatings = _ratingsRepository.GetRatingsByUserId(userId);
                if (userRatings.Count == 0)
                {
                    Tools.SendToUser($"The user with ID '{userId}' have not rated any movie yet.");
                }
                else
                {
                    Tools.SendToUser($"The user with ID '{userId}' have rated:");
                    foreach (var (movieId, rate) in userRatings)
                    {
                        var movie = _moviesRepository.GetMovieById(movieId);
                        Tools.SendToUser($"\t- The movie: '{movie?.Title}' with: '{rate}'");
                    }
                }

                Tools.SendToUser("Provide another user ID or press ENTER to finish:");
                userId = Console.ReadLine();
            }
        }

        private static void TestGetRatingsByMovieId()
        {
            Tools.SendToUser("What movie do you want to know its ratings? Provide its ID:");
            var movieId = Console.ReadLine();

            while (!string.IsNullOrEmpty(movieId))
            {
                var movieRatings = _ratingsRepository.GetRatingsByMovieId(movieId);
                if (movieRatings.Count == 0)
                {
                    Tools.SendToUser($"The movie with ID '{movieId}' has not been rated by any user yet.");
                }
                else
                {
                    Tools.SendToUser($"The movie with ID '{movieId}' has been rated by:");
                    foreach (var (userId, rate) in movieRatings)
                    {
                        var user = _usersRepository.GetUserById(userId);
                        Tools.SendToUser($"\t- The user: '{user?.Name}' with: '{rate}'");
                    }
                }

                Tools.SendToUser("Provide another movieId or press ENTER to finish:");
                movieId = Console.ReadLine();
            }
        }

        private static void TestGetFavouriteMoviesFromUser()
        {
            Tools.SendToUser("Which user do you want to know their favorite movies? Provide their ID:");
            var userId = Console.ReadLine();
            while (!string.IsNullOrEmpty(userId))
            {
                var favouriteMovies = _recommender.GetFavouriteMovies(userId)
                    .Select(movieId => _moviesRepository.GetMovieById(movieId).Title).ToList();
                Tools.SendToUser(favouriteMovies.Count == 0,
                    $"The user ID '{userId}' have not rated any movie with 5 stars yet.",
                    $"The user with ID '{userId}' have rated with 5 stars:\n- {string.Join("\n- ", favouriteMovies)}"
                );

                Tools.SendToUser("Provide another user ID or press ENTER to finish:");
                userId = Console.ReadLine();
            }
        }

        private static void TestGetDislikedMoviesFromUser()
        {
            Tools.SendToUser("Which user do you want to know the movies they disliked the most? Provide their ID:");
            var userId = Console.ReadLine();
            while (!string.IsNullOrEmpty(userId))
            {
                var favouriteMovies = _recommender.GetDislikedMovies(userId)
                    .Select(movieId => _moviesRepository.GetMovieById(movieId).Title).ToList();
                Tools.SendToUser(favouriteMovies.Count == 0,
                    $"The user with ID '{userId}' have not rated any movie with 1 star yet.",
                    $"The user with ID '{userId}' have rated with 1 star:\n- {string.Join("\n- ", favouriteMovies)}"
                );

                Tools.SendToUser("Provide another user ID or press ENTER to finish:");
                userId = Console.ReadLine();
            }
        }

        private static void TestGetGenrePreferenceFromUser()
        {
            Tools.SendToUser("Which user do you want to know the genre preferences? Provide their ID:");
            var userId = Console.ReadLine();
            while (!string.IsNullOrEmpty(userId))
            {
                var userRatings = _ratingsRepository.GetRatingsByUserId(userId);
                var genrePreferences = _recommender.GetGenrePreference(userRatings).ToList();
                Tools.SendToUser(genrePreferences.Any(),
                    $"The genre preferences of the user with ID '{userId}' are:\n- {string.Join("\n- ", genrePreferences)}",
                    $"The user with ID '{userId}' have not rated any movie yet"
                );
                Tools.SendToUser("Provide another user ID or press ENTER to finish:");
                userId = Console.ReadLine();
            }
        }

        private static IEnumerable<string> NormalizeGenres(IEnumerable<string> genres)
        {
            var normalizedGenres = new List<string>();
            foreach (var genre in genres)
            {
                if (string.IsNullOrEmpty(genre))
                {
                    continue;
                }

                string normalizedGenre;
                if (genre.Contains('-'))
                {
                    var part1 = NormalizeGenre(genre.Split('-')[0]);
                    var part2 = NormalizeGenre(genre.Split('-')[1]);
                    normalizedGenre = part1 + '-' + part2;
                }
                else
                {
                    normalizedGenre = NormalizeGenre(genre);
                }

                normalizedGenres.Add(normalizedGenre);
            }

            return normalizedGenres;
        }

        private static string NormalizeGenre(string genre)
        {
            return char.ToUpper(genre[0]) + genre.Substring(1).ToLowerInvariant();
        }
    }
}