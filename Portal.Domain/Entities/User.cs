using Portal.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Portal.Domain.Entities
{
    public class User : BaseEntity
    {

        // Arayüz giriş bilgileri
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int? Role { get; set; }

    }
}
