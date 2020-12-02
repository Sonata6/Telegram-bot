using System.Net.Http;
using Newtonsoft.Json.Linq;
    static class ScheduleQuery
    {
        public static string schedule;
        public static async System.Threading.Tasks.Task SQuery(string weekday, string weeknumber, string studentsgr)
        {
            HttpClient httpClient = new HttpClient();
            string request = $"https://journal.bsuir.by/api/v1/studentGroup/schedule?studentGroup={studentsgr}";
            HttpResponseMessage response = (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var rootObject = JObject.Parse(responseBody);
            schedule = ScheduleParsing.Parse(rootObject, weekday, weeknumber);
        }
    }
