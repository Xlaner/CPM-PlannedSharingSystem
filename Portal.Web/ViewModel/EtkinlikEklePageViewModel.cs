using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Domain.Entities;


namespace Portal.Web.ViewModel
{
    public class EtkinlikEklePageViewModel 
    {
        public List<SelectListItem> Apiler { get; set; }
        public EtkinlikEkleViewModel EtkinlikEkle { get; set; }
        public List<Etkinlik> etkinliks { get; set; }
    }
}
