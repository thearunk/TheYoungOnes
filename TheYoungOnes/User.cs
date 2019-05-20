using System;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace TheYoungOnes
{
    [JsonObject]
    public class User
    {
        // Checks for area code optionally in parentheses, number of digits and separators of - and white space
        static private Regex PhonePattern = new Regex("\\A(?(\\()\\([0-9]{3}\\)|[0-9]{3})[- ]*[0-9]{3}[- ]*[0-9]{4}\\Z");

        public User()
        {
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("number")]
        public string Phone { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }

        public bool IsValidUser()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Phone)) return false;

            return PhonePattern.IsMatch(Phone.Trim()); 
        }
    }
}
