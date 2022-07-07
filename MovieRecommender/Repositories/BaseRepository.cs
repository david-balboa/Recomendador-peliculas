using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace MovieRecommender.Repositories
{
    public class BaseRepository<TItem>
    {
        protected readonly IReadOnlyCollection<TItem> Items;

        internal BaseRepository(string fileName)
        {
            var filePath = $@"{Environment.CurrentDirectory}\..\..\..\..\..\{fileName}";
            using var streamReader = new StreamReader(filePath);
            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            var records = csvReader.GetRecords<TItem>();
            Items = records.ToList();
        }
    }
}