using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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





        public static string PublishFBPost(string message, int AdID)
        {



            var postOnWallTask2 = PublishPost(message, AdID);
            Task.WaitAll(postOnWallTask2);
            //  var tokenResponse = JsonConvert.DeserializeObject<PublishImagesResponseModel>(postOnWallTask2.Result);
            return postOnWallTask2.ToString();
        }
        public static async Task<string> PublishPost(string message, int AdID)
        {



            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(FacebookSettings.FB_BASE_ADDRESS);


                List<KeyValuePair<string, string>> bodyProperties = new List<KeyValuePair<string, string>>();
                //Loop over String array and add all instances to our bodyPoperties


                var payments = GetImageList(AdID);
                for (int i = 0; i < payments.Count; i++)
                {
                    var pay = payments[i];

                    bodyProperties.Add(new KeyValuePair<string, string>("attached_media['" + pay.RowID + "']", "{media_fbid:'" + pay.FBImageID + "'}"));

                }

                //Create List of KeyValuePairs

                //Add 'single' parameters
                bodyProperties.Add(new KeyValuePair<string, string>("access_token", FacebookSettings.FB_ACCESS_TOKEN));
                bodyProperties.Add(new KeyValuePair<string, string>("published", "true"));
                bodyProperties.Add(new KeyValuePair<string, string>("message", message));


                //convert your bodyProperties to an object of FormUrlEncodedContent
                var dataContent = new FormUrlEncodedContent(bodyProperties.ToArray());

                var req = new HttpRequestMessage(HttpMethod.Post, $"{FacebookSettings.FB_PAGE_ID}/feed") { Content = dataContent };
                var res = await httpClient.SendAsync(req);


                return await res.Content.ReadAsStringAsync();
            }

        }

        //get list of saved images id in your db
        public static List<ImagesModel> GetImageList(int AdID)
        {
            var payments = new List<ImagesModel>();
            using (SqlConnection conn = new SqlConnection(""))
            {
                using (SqlCommand cmd = new SqlCommand($@"SELECT  ROW_NUMBER() OVER( ORDER BY ID )-1 AS 'RowID',* FROM MyHomeAdFacebookImage WHERE AdID=@AdID AND Status=2", conn))
                {
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@AdID", AdID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var payment = new ImagesModel();
                        payment.RowID = Convert.ToInt32(rdr["RowID"]);
                        payment.ID = Convert.ToInt32(rdr["ID"]);
                        payment.FBImageID = rdr["FBImageID"].ToString();

                        payments.Add(payment);
                    }
                    conn.Close();
                }
            }


            return payments;
        }


        public class ImagesModel
        {
            public int ID { get; set; }
            public int RowID { get; set; }
            public string ImageID { get; set; }
            public string FBImageID { get; set; }
            public string MainMessage { get; set; }
            public int AdID { get; set; }
        }

    }
}
