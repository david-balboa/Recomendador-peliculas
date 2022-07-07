using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieRecommender
{
    public static class Tools
    {
        public static IEnumerable<string> OrderDictionaryKeysByValues(IDictionary<string, int> dictionary,
            bool decreaseOrder = false)
        {
            IDictionary<string, double> newDictionary =
                dictionary.ToDictionary(item => item.Key, item => (double) item.Value);
            return OrderDictionaryKeysByValues(newDictionary, decreaseOrder);
        }

        public static IEnumerable<string> OrderDictionaryKeysByValues(IDictionary<string, double> dictionary,
            bool decreaseOrder = false)
        {
            var list = dictionary.ToList();
            if (decreaseOrder)
            {
                list.Sort((keyValue1, keyValue2) => keyValue1.Value.CompareTo(keyValue2.Value));
            }
            else
            {
                list.Sort((keyValue1, keyValue2) => keyValue2.Value.CompareTo(keyValue1.Value));
            }

            return list.Select(keyValue => keyValue.Key);
        }

        public static void SendToUser(string text)
        {
            Console.WriteLine($"> {text}");
        }

        public static void SendToUser(bool condition, string trueText, string falseText)
        {
            SendToUser(condition ? trueText : falseText);
        }

        /*public static void SendToUser(IDictionary<string, int> dictionary, string text)
        {
            foreach (var (key, value) in dictionary)
            {
                var newText = text.Replace("KEY", $"'{key}'").Replace("VALUE", $"'{value}'");
                SendToUser($"\t- {newText}");
            }
        }*/
    }
}