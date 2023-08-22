namespace Portal.Web.Apis.LinkedinApi.ImageAndText
{

    public class LinkedinPostRegisterLinkedin
    {
        public Registeruploadrequest registerUploadRequest { get; set; }
    }

    public class Registeruploadrequest
    {
        public string[] recipes { get; set; }
        public string owner { get; set; }
        public Servicerelationship[] serviceRelationships { get; set; }
    }

    public class Servicerelationship
    {
        public string relationshipType { get; set; }
        public string identifier { get; set; }
    }

}
