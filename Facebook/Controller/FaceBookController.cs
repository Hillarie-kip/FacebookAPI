using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Facebook
{
    public class FaceBookController
    {


        public FaceBookController(string pageId, string accessToken)
        {
            FacebookSettings.FB_PAGE_ID = pageId;
            FacebookSettings.FB_ACCESS_TOKEN = accessToken;
        }

        public static async Task<string> PublishMessage(string message)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(FacebookSettings.FB_BASE_ADDRESS);
                var names = new List<string>() { "https://homeapi.esquekenya.com/api/MaqaoFile/PropertyDetailImage?ImageID=1609741506", "https://homeapi.esquekenya.com/api/MaqaoFile/PropertyDetailImage?ImageID=1609741553" };


                var parametters = new Dictionary<string, string>
                {


                    { "access_token", FacebookSettings.FB_ACCESS_TOKEN },
                    { "message", message },
                    { "published", "true" },
                    { "url", "https://homeapi.esquekenya.com/api/MaqaoFile/PropertyDetailImage?ImageID=1609741506" }

                };





                var encodedContent = new FormUrlEncodedContent(parametters);

                var result = await httpClient.PostAsync($"{FacebookSettings.FB_PAGE_ID}/photos", encodedContent);
                var msg = result.EnsureSuccessStatusCode();
                return await msg.Content.ReadAsStringAsync();
            }

        }
    }
}
