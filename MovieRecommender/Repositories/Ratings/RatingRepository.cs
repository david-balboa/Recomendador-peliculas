using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieRecommender.Repositories.Ratings
{
    public class RatingRepository : BaseRepository<Rating>
    {
        private readonly IEnumerable<Rating> _ratings;
        private readonly double _affinityPercentage;

        internal RatingRepository(string fileName, double affinityPercentage) : base(fileName)
        {
            _ratings = Items;
            _affinityPercentage = affinityPercentage;
        }

        // to get a dictionary with the movies and the punctuation that the userId has given to them
        public IDictionary<string, int> GetRatingsByUserId(string userId)
        {
            return _ratings.Where(r => r.UserId == userId)
                .ToDictionary(rating => rating.MovieId, rating => rating.Rate);
        }

        // to get a dictionary with the users and the punctuation that the movieId has received from them
        public IDictionary<string, int> GetRatingsByMovieId(string movieId)
        {
            return _ratings.Where(r => r.MovieId == movieId)
                .ToDictionary(rating => rating.UserId, rating => rating.Rate);
        }

        // to get a ordered list of user ids that are considered similar to the userId given taking into account the affinity percentage
        public IEnumerable<string> GetSimilarUsers(string userId, bool reverseMode = false)
        {
            IDictionary<string, double> usersAffinity = new Dictionary<string, double>();
            var ratingsComparison = GetRatingsComparison(userId);
            foreach (var (newUserId,similarity) in ratingsComparison)
            {
                var affinity = similarity.CalculateAffinity();
                Console.WriteLine($"- UserID: {newUserId} / Affinity: {affinity}");
                if ((!reverseMode && affinity >= _affinityPercentage) || (reverseMode && affinity <= 1 - affinity))
                {
                    usersAffinity.Add(newUserId, affinity);
                }
            }
            
            return Tools.OrderDictionaryKeysByValues(usersAffinity, reverseMode);
        }

        private IDictionary<string, Similarity> GetRatingsComparison(string userId)
        {
            var userRatings = GetRatingsByUserId(userId);
            var ratingsComparison = new Dictionary<string, Similarity>();
            foreach (var rating in _ratings)
            {
                if (rating.UserId == userId || !userRatings.ContainsKey(rating.MovieId))
                {
                    continue;
                }

                var newUserId = rating.UserId;
                var rateDiscrepancy = Math.Abs(rating.Rate - userRatings[rating.MovieId]);
                if (!ratingsComparison.ContainsKey(newUserId))
                {
                    ratingsComparison.Add(newUserId, new Similarity(rateDiscrepancy));
                }
                else
                {
                    ratingsComparison[newUserId].UpdateSimilarity(rateDiscrepancy);
                }
            }
            return ratingsComparison;
        }
    }

    internal class Similarity
    {
        private int _rateDiscrepancy;
        private int _rateCount;

        private const int MinimumRate = 1;
        private const int MaximumRate = 5;

        public Similarity(int rateDiscrepancy)
        {
            _rateDiscrepancy = rateDiscrepancy;
            _rateCount = 1;
        }

        public void UpdateSimilarity(int rateDiscrepancy)
        {
            _rateDiscrepancy += rateDiscrepancy;
            _rateCount += 1;
        }

        public double CalculateAffinity()
        {
            const int maximumRateDifference = MaximumRate - MinimumRate;
            return (double)(maximumRateDifference * _rateCount - _rateDiscrepancy) / (maximumRateDifference * _rateCount);
        }
    }
}