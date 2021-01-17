using RestSharp;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAntiToxicBot.Services
{
    public static class ToxicScores 
    {
        public static string toxic { get; set; }
        public static string severe_toxic { get; set; }
        public static string obscene { get; set; }
        public static string threat { get; set; }
        public static string insult { get; set; }
        public static string identity_hate { get; set; }
    }
    public class ToxicLangService
    {
       
        public string CheckTextAsync(string text)
        {
            var client = new RestClient("http://localhost:5000/model/predict");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"text\": [\""+text+"\"]}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response.Content;
        }
    }
}
