using System;
using CsvHelper.Configuration.Attributes;

namespace MovieRecommender.Repositories.Users
{
    public class User
    {
        [Name("id")] public string Id { get; set; }

        [Name("name")] public string Name { get; set; }

        [Name("email")] public string? Email { get; set; }

        [Name("birthday")] public string Birthday { get; set; }

        private DateTime BirthdayDate => DateTime.ParseExact(Birthday, "dd/MM/yyyy", null);

        public double Age => Math.Floor((DateTime.Now - BirthdayDate).TotalDays / 365);
    }
}