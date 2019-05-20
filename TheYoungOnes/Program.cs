using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TheYoungOnes
{
    class Program
    {
        const string UserListServiceUrl = "https://appsheettest1.azurewebsites.net/sample/list";
        const string UserListServiceUrlQuery = "token={0}";
        const string UserDetailServiceUrl = "http://appsheettest1.azurewebsites.net/sample/detail/{0}";
        const int maxYoungestUsers = 5;

        public static List<User> YoungestUsers = new List<User>();

        static async Task Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                List<Task> userDetailQueries = new List<Task>();
                string token = null;
                do
                {
                    UriBuilder uri = new UriBuilder(UserListServiceUrl); 
                    if (token != null)
                    {
                        uri.Query = string.Format(UserListServiceUrlQuery, token);
                    }

                    var response = await client.GetAsync(uri.ToString());
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error accessing url {0}", uri.ToString());
                        return;
                    }

                    string body = await response.Content.ReadAsStringAsync();
                    PartialUserList userList = null;
                    if (!string.IsNullOrEmpty(body))
                    {
                        userList = JsonConvert.DeserializeObject<PartialUserList>(body);
                    }

                    token = (userList != null && userList.HasMoreUsers()) ? userList.Token : null;

                    foreach(int userId in userList.UserIds)
                    {
                        // separate async task for each user to get details
                        userDetailQueries.Add(GetUserDetailAndUpdate(userId, client));
                    }

                    // purge the completed tasks to keep the list footprint small
                    userDetailQueries = userDetailQueries.FindAll(task => !task.IsCompletedSuccessfully);
                }
                while (token != null);

                await Task.WhenAll(userDetailQueries);

                YoungestUsers.Sort((u1, u2) => string.Compare(u1.Name, u2.Name, true));

                Console.WriteLine("\nThe 5 youngest users are:");
                YoungestUsers.ForEach(u => Console.WriteLine("{0} {1} {2}", u.Name, u.Age, u.Phone));
            }
        }

        static async Task GetUserDetailAndUpdate(int id, HttpClient client)
        {
            try
            {
                var response = await client.GetAsync(string.Format(UserDetailServiceUrl, id));
                response.EnsureSuccessStatusCode();

                string body = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    User user = JsonConvert.DeserializeObject<User>(body);
                    if (user != null && user.IsValidUser())
                    {
                        lock (YoungestUsers)
                        {
                            int index = YoungestUsers.FindIndex(u => u.Age > user.Age);
                            if (index >= 0)
                            {
                                YoungestUsers.Insert(index, user);
                                if (YoungestUsers.Count > maxYoungestUsers)
                                    YoungestUsers.RemoveAt(YoungestUsers.Count - 1);
                            }
                            else if (YoungestUsers.Count < maxYoungestUsers)
                            {
                                // we have less than 5 users so insert anyway
                                YoungestUsers.Add(user);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error accessing user details for {0}\n{1}", id, e.Message);
            }
        }
    }
}
