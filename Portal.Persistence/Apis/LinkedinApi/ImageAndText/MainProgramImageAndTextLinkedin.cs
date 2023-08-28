using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Portal.Application.Repositories;
using Portal.Domain.Entities;
using Portal.Persistence.Context;
using Microsoft.AspNetCore.Hosting;

namespace Portal.Web.Apis.LinkedinApi.ImageAndText
{
    public class MainProgramImageLinkedin
    {
        
        public static async Task RunLinkedInImageShareAsync(string ApiToken,string Text , string İmage)
        {

            try
            {


                LinkedinURL URLS = new LinkedinURL
                {
                    accessToken = ApiToken,
                    fileUploadPath = İmage,
                    imageText = Text,
                    contentType = "application/json",
                    imageText2 = "ExampleText",
                    imageTitle = "Title",
                };

                using (var profile = new HttpClient())
                {
                    profile.DefaultRequestHeaders.Add("Authorization", "Bearer " + URLS.accessToken);
                    var responseProfile = await profile.GetAsync("https://api.linkedin.com/v2/me");
                    var respContentProfile = await responseProfile.Content.ReadAsStringAsync();
                    JToken tokenProfile = JObject.Parse(respContentProfile);
                    string profileId = (string)tokenProfile["id"];

                    using (var client = new HttpClient())
                    {
                        LinkedinPostRegisterLinkedin request = new LinkedinPostRegisterLinkedin
                        {
                            registerUploadRequest = new Registeruploadrequest
                            {
                                recipes = new[] { "urn:li:digitalmediaRecipe:feedshare-image" },
                                owner = "urn:li:person:" + profileId,
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
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + URLS.accessToken);
                        var response = await client.PostAsync("https://api.linkedin.com/v2/assets?action=registerUpload", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var respContent = await response.Content.ReadAsStringAsync();
                            JToken token = JObject.Parse(respContent);
                            string uploadUrl = (string)token["value"]["uploadMechanism"]["com.linkedin.digitalmedia.uploading.MediaUploadHttpRequest"]["uploadUrl"];
                            string asset = (string)token["value"]["asset"];

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
                                                    title = new Title { text = URLS.imageTitle },
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
                                var responseShare = await clientShare.PostAsync("https://api.linkedin.com/v2/ugcPosts", contentShare);

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
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
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
