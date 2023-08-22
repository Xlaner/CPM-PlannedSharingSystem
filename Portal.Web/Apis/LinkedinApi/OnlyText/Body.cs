using Newtonsoft.Json;


namespace Portal.Web.Apis.LinkedinApi.OnlyText
{
    public class Body
    {
        public class LinkedinPostImageShareRequest
        {
            public string author { get; set; }
            public string lifecycleState { get; set; }
            public Specificcontent specificContent { get; set; }
            public Visibility visibility { get; set; }
        }

        public class Specificcontent
        {
            [JsonProperty("com.linkedin.ugc.ShareContent")]
            public ComLinkedinUgcSharecontent comlinkedinugcShareContent { get; set; }
        }

        public class ComLinkedinUgcSharecontent
        {
            public Sharecommentary shareCommentary { get; set; }
            public string shareMediaCategory { get; set; }
        }

        public class Sharecommentary
        {
            public string text { get; set; }
        }


        public class Description
        {
            public string text { get; set; }
        }


        public class Visibility
        {
            [JsonProperty("com.linkedin.ugc.MemberNetworkVisibility")]
            public string comlinkedinugcMemberNetworkVisibility { get; set; }
        }

    }
}
