using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace Chessman
{
    public class TacticsService : ITacticsService
    {
        const string BlundersApiUrl = "https://chessblunders.org/api/mobile/";
        const string Username = "";
        const string Password = "";

        private HttpClient client = null;
        private Task<string> getToken= null;

        private Tactic currentTactic = null;

        public TacticsService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(BlundersApiUrl);

            getToken = Authenticate();
        }

        public async Task<string> Authenticate()
        {
            var payload = new { username = Username, password = Password};
            var response = await client.PostAsync("session/login", GetPayloadContent(payload));
            var responseString = await response.Content.ReadAsStringAsync();

            TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);
            return tokenResponse.token;
        }

        public async Task<Tactic> GetAsync()
        {
            string token = await getToken;

            var payload = new { token, type = "explore" };
            var response = await client.PostAsync("blunder/get", GetPayloadContent(payload));
            var responseString = await response.Content.ReadAsStringAsync();

            TacticResponse<Tactic> tacticsResponse = JsonConvert.DeserializeObject<TacticResponse<Tactic>>(responseString);
            currentTactic = tacticsResponse.data;

            currentTactic.Info = await GetInfoAsync(currentTactic.id);

            return currentTactic;
        }

        private async Task<TacticInfo> GetInfoAsync(string id)
        {
            string token = await getToken;

            var payload = new { token, blunder_id = id };
            var response = await client.PostAsync("blunder/info", GetPayloadContent(payload));
            var responseString = await response.Content.ReadAsStringAsync();

            TacticResponse<TacticInfo> tacticsResponse = JsonConvert.DeserializeObject<TacticResponse<TacticInfo>>(responseString);
            return tacticsResponse.data;
        }

        public async Task Skip()
        {
            if (currentTactic == null)
                throw new NotSupportedException("No current tactic to skip.");

            string token = await getToken;

            var payload = new { token, type = "explore", id = currentTactic.id, line = new [] { "" }, spentTime = 0 };
            var response = await client.PostAsync("blunder/validate", GetPayloadContent(payload));
            await response.Content.ReadAsStringAsync();
        }

        private StringContent GetPayloadContent(object payload)
        {
            return new StringContent(JsonConvert.SerializeObject(payload), Encoding.ASCII, "application/json");
        }
    }


}

//function get()
//{
//    banner.html('');
//    return $.ajax({
//        type: 'POST',
//        crossDomain: true,
//        url: 'https://chessblunders.org/api/mobile/blunder/get',
//        contentType: 'application/json',
//        data: JSON.stringify({ token: token, type: 'explore' })
//        }).done(function(response) {
//        blunder = response.data;
//        banner.html('<div>' + JSON.stringify(response) + "</div>");
//    });
//};
//get();

//function validate()
//{
//    banner.html('');
//    return $.ajax({
//        type: 'POST',
//        crossDomain: true,
//        url: 'https://chessblunders.org/api/mobile/blunder/validate',
//        contentType: 'application/json',
//        data: JSON.stringify({
//            token: token, 
//            type: 'explore', 
//            id: blunder.id,
//            line: [''],
//            spentTime: 0,
//          })
//        }).done(function(data) {
//        banner.html('<div>' + JSON.stringify(data) + "</div>");
//    });