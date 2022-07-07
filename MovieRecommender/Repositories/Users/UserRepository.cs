using System.Collections.Generic;
using System.Linq;

namespace MovieRecommender.Repositories.Users
{
    public class UserRepository : BaseRepository<User>
    {
        private readonly IEnumerable<User> _users;

        internal UserRepository(string fileName) : base(fileName)
        {
            _users = Items;
        }

        public User? GetUserById(string id)
        {
            return _users.FirstOrDefault(user => user.Id == id);
        }

        public IEnumerable<User> GetUsersByAge(int age)
        {
            return _users.Where(user => (int)user.Age == age);
        }
    }
}