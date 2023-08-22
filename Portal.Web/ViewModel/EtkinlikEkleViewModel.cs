using System.ComponentModel.DataAnnotations;

namespace Portal.Web.ViewModel
{
	public class EtkinlikEkleViewModel
	{
		public string title { get; set; }
		public string description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime start { get; set; } = DateTime.Now.Date;
        public IFormFile? image { get; set; }
		public string Tekrar { get; set; }
		public int TekrarNum { get; set; } = 1;
        public string? ApiId { get; set; }
    }
}
