using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;




namespace LinkedinImageProje
{
    public class MainProgramImageLinkedin
    {
       
       
            [STAThread]
        static async Task Main(string[] args)
        {
            LinkedinURL URLS = new LinkedinURL 
            { 
                accessToken = "AccessToken",//Your accsess Token
                
                fileUploadPath = @"/ExapmlePath/Photo.png", //path of the photo
                imageText = "", //Content Text


                contentType = "application/json",
                
                imageText2 = "ExampleText",
                imageTitle = "Title",
            };

            using (var profile = new HttpClient())
            {
                profile.DefaultRequestHeaders.Add("Authorization", "Bearer " + URLS.accessToken);
                var responseProfile = profile.GetAsync("https://api.linkedin.com/v2/me").Result;

                var respContentProfile = responseProfile.Content.ReadAsStringAsync().Result;

                JToken tokenProfile = JObject.Parse(respContentProfile);
                string profileId = (string)tokenProfile["id"];
                using (var client = new HttpClient())
                {
                    LinkedinPostRegisterLinkedin request = new LinkedinPostRegisterLinkedin
                    {
                        registerUploadRequest = new Registeruploadrequest
                        {
                            recipes = new[] { "urn:li:digitalmediaRecipe:feedshare-image" },
                            owner = "urn:li:person:"+profileId,
                            serviceRelationships = new[]
                            {
                            new Servicerelationship
                            {
                                relationshipType = "OWNER",
                                identifier = "urn:li:userGeneratedContent"

                            }
                        }
                        }


                    };
                    var reqString = JsonConvert.SerializeObject(request);
                    StringContent content = new StringContent(reqString, Encoding.UTF8, URLS.contentType);
                    //-------------------------------------------------------------------------------------------------------------------------------

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + URLS.accessToken);
                    //-------------------------------------------------------------------------------------------------------------------------------



                    var response = client.PostAsync("https://api.linkedin.com/v2/assets?action=registerUpload", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var respContent = response.Content.ReadAsStringAsync().Result;


                        JToken token = JObject.Parse(respContent);
                        string uploadUrl = (string)token["value"]["uploadMechanism"]["com.linkedin.digitalmedia.uploading.MediaUploadHttpRequest"]["uploadUrl"];

                        string asset = (string)token["value"]["asset"];
                        //Console.WriteLine(asset);

                        await UploadImage(URLS.fileUploadPath, uploadUrl, URLS.accessToken);

                        using (var clientShare = new HttpClient())
                        {
                            LinkedinPostImageShareRequest requestShare = new LinkedinPostImageShareRequest
                            {
                                author = "urn:li:person:T6hBlHuGR-",
                                lifecycleState = "PUBLISHED",
                                specificContent = new Specificcontent
                                {
                                    comlinkedinugcShareContent = new ComLinkedinUgcSharecontent
                                    {
                                        shareCommentary = new Sharecommentary { text = URLS.imageText },
                                        shareMediaCategory = "IMAGE",
                                        media = new Medium[]
                                        {
                                        new Medium
                                        {
                                            status = "READY",
                                            description = new Description
                                            {
                                                text = URLS.imageText2,
                                            },
                                            media = asset,
                                            title = new Title { text = URLS.imageTitle},

                                        }
                                        }

                                    }

                                },

                                visibility = new Visibility
                                {
                                    comlinkedinugcMemberNetworkVisibility = "PUBLIC"
                                }


                            };

                            var reqShareString = JsonConvert.SerializeObject(requestShare);
                            StringContent contentShare = new StringContent(reqShareString, Encoding.UTF8, URLS.contentType);
                            clientShare.DefaultRequestHeaders.Add("Authorization", "Bearer " + URLS.accessToken);
                            var responseShare = client.PostAsync("https://api.linkedin.com/v2/ugcPosts", contentShare).Result;
                            if (responseShare.IsSuccessStatusCode)
                            {
                                Console.WriteLine("successed");
                            }
                            else
                            {
                                Console.WriteLine(responseShare.StatusCode);
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode);
                    }
                }
            }
        }

        static async Task UploadImage(string filePath, string uploadUrl, string bearerToken)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var requestUpload = new HttpRequestMessage(HttpMethod.Post, uploadUrl))
                    {
                        requestUpload.Headers.Add("Authorization", "Bearer " + bearerToken);

                        using (var content = new MultipartFormDataContent())
                        {
                            var fileStream = File.OpenRead(filePath);
                            var streamContent = new StreamContent(fileStream);

                            content.Add(streamContent, "file", Path.GetFileName(filePath));
                            requestUpload.Content = content;

                            var responseUpload = await httpClient.SendAsync(requestUpload);

                            if (responseUpload.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Image uploaded successfully!");
                            }
                            else
                            {
                                Console.WriteLine($"Image upload failed. Status Code: {responseUpload.StatusCode}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
