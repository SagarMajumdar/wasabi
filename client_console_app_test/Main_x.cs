using System.Net.Http;
using System.Net.Http.Headers;

namespace Wasabi {
    public class Main_x {
        public string Wasabi_access_token {get; set;}
        
        public async Task<bool> Register() {
            bool r = false;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("authorization", "Bearer <token....>");
            var stringTask = client.GetStringAsync("https://localhost:7220/api/wasabi/chk");
            var msg = await stringTask;
            Console.Write(msg);

            return r;
        }
    }
}