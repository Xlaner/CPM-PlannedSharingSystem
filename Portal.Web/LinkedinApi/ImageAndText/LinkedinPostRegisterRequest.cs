using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedinImageProje
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
