using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Portal.Application.Repositories;
using Portal.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Portal.Web.ViewModel;
using ServiceStack;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using System.Reflection;
using static System.Net.WebRequestMethods;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Identity;
using ControlPortal.Persistence;
using BackGroundService;

namespace Portal.Web.Controllers
{
    
    [AllowAnonymous]

    public class LoginController : Controller
    {
       
        readonly private IUserReadRepository _userReadRepository;
        readonly private IUserWriteRepository _userWriteRepository;
       

        public LoginController(IUserReadRepository userReadRepository, IUserWriteRepository userWriteRepository)
        {
            _userReadRepository = userReadRepository;
            _userWriteRepository = userWriteRepository;
           
        }


        public IActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(HomeIndexViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                return View(new HomeIndexViewModel { UserName = model.UserName, HasError = true, Error = "Kullanıcı adı ve şifre alanları boş olamaz!" });

            var datas = _userReadRepository.GetAll();
            var dataSearch = datas.Where(elm => elm.UserName == model.UserName && elm.Password == model.Password);

            if (dataSearch.Count() == 1)
            {
                model.Role = (int)dataSearch.First().Role;
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,model.UserName),
                    new Claim(ClaimTypes.Role,model.Role.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties() { ExpiresUtc = DateTime.UtcNow.AddHours(1) };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                if (model.Role == 2)
                {

                    return RedirectToAction("AdminKullaniciListesi","Home");
                }

                //Console.WriteLine(model.Id)
                return RedirectToAction("Takvim", "Home");
            }

            return View(new HomeIndexViewModel { UserName = model.UserName, HasError = true, Error = "Kullanıcı Adı Veya Şifre Hatalı!" });
        }
        public async Task<IActionResult> CikisYap()
        {
            await HttpContext.SignOutAsync();

            // Prevent caching of the page
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Index", "Login");
        }



    }
}
