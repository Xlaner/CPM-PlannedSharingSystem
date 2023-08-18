
using LinkedinTextProje;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using static LinkedinTextProje.Body;

namespace ConsoleApp1
{
    internal class MainProgramOnlyTextLinkedin
    {
        static void Main()
        {
            LinkedinURLText URLS = new LinkedinURLText
            {
                accessToken = "AccessToken",//Your accsess Token

                Text = "ExampleText", //Content Text
                contentType = "application/json",

            };

            using (var profile = new HttpClient())
            {
                profile.DefaultRequestHeaders.Add("Authorization", "Bearer " + URLS.accessToken);
                var response = profile.GetAsync("https://api.linkedin.com/v2/me").Result;
                
                var respContent = response.Content.ReadAsStringAsync().Result;

                JToken token = JObject.Parse(respContent);
                string profileId = (string)token["id"];
                if (!string.IsNullOrEmpty(profileId)) 
                {
                    using (var client = new HttpClient())
                    {
                        LinkedinPostImageShareRequest request = new LinkedinPostImageShareRequest
                        { 
                           author= "urn:li:person:" + profileId,
                           lifecycleState= "PUBLISHED",
                           specificContent = new Specificcontent
                           {
                               comlinkedinugcShareContent = new ComLinkedinUgcSharecontent
                               {
                                   shareCommentary = new Sharecommentary
                                   {
                                       text= URLS.Text,
                                   },
                                   shareMediaCategory="NONE"
                               }
                           },
                           visibility= new Visibility
                           {
                               comlinkedinugcMemberNetworkVisibility="PUBLIC"
                           }
                        };
                        
                        var reqShareString = JsonConvert.SerializeObject(request);
                        StringContent contentShare = new StringContent(reqShareString, Encoding.UTF8, URLS.contentType);
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + URLS.accessToken);
                        var responseShare = profile.PostAsync("https://api.linkedin.com/v2/ugcPosts", contentShare).Result;
                        if (responseShare != null && responseShare.IsSuccessStatusCode) 
                        {
                            Console.WriteLine("Paylaşım Yapıldı");
                        }
                        else
                        {
                            Console.WriteLine(responseShare.StatusCode);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(response.StatusCode.ToString());
                }

            }
        }
    }
}
