using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Facebook
{
    public class FaceBookController
    {
        public class PublishImagesResponseModel
        {
            public string id { get; set; }

        }

        public FaceBookController(string pageId, string accessToken)
        {
            FacebookSettings.FB_PAGE_ID = pageId;
            FacebookSettings.FB_ACCESS_TOKEN = accessToken;
        }

        //this will respond with processed facebook image id that you can save somewhere  then publish later in an array
        public static string PublishedImagesID(string message, string ImageID)
        {



            var postOnWallTask2 = PublishImages(message, ImageID);
            Task.WaitAll(postOnWallTask2);


            var tokenResponse = JsonConvert.DeserializeObject<PublishImagesResponseModel>(postOnWallTask2.Result);
          
            return tokenResponse.id;
        }

        //uploading single image
        public static async Task<string> PublishImages(string message, string ImageID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(FacebookSettings.FB_BASE_ADDRESS);


                FormUrlEncodedContent content = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>>
             {
                new KeyValuePair<string, string>("access_token", FacebookSettings.FB_ACCESS_TOKEN),
                new KeyValuePair<string, string>("published", "false"),
                new KeyValuePair<string, string>("caption", message),
                new KeyValuePair<string, string>("url", "https://homeapi.esquekenya.com/api/MaqaoFile/FBAdImage?ImageID="+ImageID),

             });


                var req = new HttpRequestMessage(HttpMethod.Post, $"{FacebookSettings.FB_PAGE_ID}/photos") { Content = content };
                var res = await httpClient.SendAsync(req);
                return await res.Content.ReadAsStringAsync();
            }

        }
       
    }
}
