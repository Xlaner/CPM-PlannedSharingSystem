﻿using Microsoft.AspNetCore.Authorization;
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

namespace Portal.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        readonly private IAccessTokenReadRepository _accessTokenReadRepository;
        readonly private IAccessTokenWriteRepository _accessTokenWriteRepository;
        readonly private IEtkinlikWriteRepository _etkinlikWriteRepository;
        readonly private IEtkinlikReadRepository _etkinlikReadRepository;
        readonly private IUserReadRepository _userReadRepository;
        readonly private IUserWriteRepository _userWriteRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUserWriteRepository userWriteRepository, IUserReadRepository userReadRepository, IEtkinlikWriteRepository etkinlikWriteRepository, IEtkinlikReadRepository etkinlikReadRepository, IWebHostEnvironment hostingEnvironment, IAccessTokenReadRepository accessTokenReadRepository, IAccessTokenWriteRepository accessTokenWriteRepository)
        {
            _logger = logger;
            _userWriteRepository = userWriteRepository;
            _userReadRepository = userReadRepository;
            _etkinlikWriteRepository = etkinlikWriteRepository;
            _etkinlikReadRepository = etkinlikReadRepository;
            _hostingEnvironment = hostingEnvironment;
            _accessTokenReadRepository = accessTokenReadRepository;
            _accessTokenWriteRepository = accessTokenWriteRepository;
        }

        //========================================================================================================================
        //Login Ekranı İşlemleri


        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
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

                    return RedirectToAction("AdminKullaniciListesi");
                }

                //Console.WriteLine(model.Id)
                return RedirectToAction("Takvim");
            }

            return View(new HomeIndexViewModel { UserName = model.UserName, HasError = true, Error = "Kullanıcı Adı Veya Şifre Hatalı!" });
        }

        //-------------------------------------------------------------
        //Takvim sayfası işlemleri
        public IActionResult Takvim()
        {
            var denem = _accessTokenReadRepository.Get().ToList();
            List<SelectListItem> ApilerListe = new List<SelectListItem>();

            foreach (var item in denem)
            {
                ApilerListe.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.TokenTitle });
            }



            EtkinlikEklePageViewModel Dbe = new EtkinlikEklePageViewModel()
            {
                Apiler = ApilerListe,
                EtkinlikEkle = new EtkinlikEkleViewModel()
                {
                    //start= DateTime.Now,
                    //title = "",
                    //description="",
                    //Tekrar = "Tek Seferlik",
                    //TekrarNum = 0,
                    //image = null

                }

            };

            return View(Dbe);
        }

        [HttpPost]
        public IActionResult Tahvim([FromForm] EtkinlikEklePageViewModel e)
        {

            if (e.EtkinlikEkle.Tekrar == "Tek Sefer")
            {
                e.EtkinlikEkle.TekrarNum = 1;
            }
            for (var i = 0; i < e.EtkinlikEkle.TekrarNum; i++)
            {
                Etkinlik Dbe = new Etkinlik();
                if (e.EtkinlikEkle.image != null)
                {
                    string dosyaadi = "photoId." + Guid.NewGuid().ToString();
                    string uzanti = Path.GetExtension(e.EtkinlikEkle.image.FileName);
                    string wwwRootPath = _hostingEnvironment.WebRootPath;
                    string yol = Path.Combine(wwwRootPath, "Images", dosyaadi + uzanti);
                    using (var stream = new FileStream(yol, FileMode.Create))
                    {
                        e.EtkinlikEkle.image.CopyTo(stream);
                    }

                    Dbe.image = dosyaadi + uzanti;

                }
                if (e.EtkinlikEkle.Tekrar == "Aylık")
                {
                    Dbe.start = e.EtkinlikEkle.start.AddMonths(i);
                }
                if (e.EtkinlikEkle.Tekrar == "Yıllık")
                {
                    Dbe.start = e.EtkinlikEkle.start.AddYears(i);
                }
                if (e.EtkinlikEkle.Tekrar == "Tek Sefer")
                {
                    Dbe.start = e.EtkinlikEkle.start;
                }
                Dbe.title = e.EtkinlikEkle.title;
                Dbe.description = e.EtkinlikEkle.description;
                Dbe.Tekrar = e.EtkinlikEkle.Tekrar;
                Dbe.TekrarNum = e.EtkinlikEkle.TekrarNum;
                _etkinlikWriteRepository.AddAsync(Dbe).Wait();
                _etkinlikWriteRepository.SaveAsync().Wait();

            }


            return RedirectToAction("Takvim", "Home");
        }

        public async Task<ActionResult> TiklananEtkinlik(string eventId)
        {
            Etkinlik etkinlik = await _etkinlikReadRepository.GetByIdAsync(eventId);


            return View(etkinlik);
        }
        public async Task<ActionResult> TiklananEtkinlikPartial(string eventId)
        {
            Etkinlik etkinlik = await _etkinlikReadRepository.GetByIdAsync(eventId);


            return PartialView("/Views/Home/TiklananEtkinlik.cshtml", etkinlik);
        }

        public IActionResult GetList()
        {

            var data = _etkinlikReadRepository.GetAll();


            return Json(data);
        }

        public async Task<IActionResult> EtkinlikSil(string Id)
        {
            Etkinlik Dbe = await _etkinlikReadRepository.GetByIdAsync(Id);
            if (Dbe.image != null)
            {
                string wwwRootPath = _hostingEnvironment.WebRootPath;
                string yol = Path.Combine(wwwRootPath, "Images", Dbe.image);
                System.IO.File.Delete(yol);


            }

            _etkinlikWriteRepository.RemoveAsync(Id).Wait();
            _etkinlikWriteRepository.SaveAsync().Wait();



            return RedirectToAction("Takvim", "Home");
        }





        //-----------------------------------------------------------------------------------------------------------
        //Admin İşlemleri
        [Authorize]
        public async Task<ActionResult> KullaniciSil(string Id)
        {

            await _userWriteRepository.RemoveAsync(Id);
            await _userWriteRepository.SaveAsync();

            return RedirectToAction("AdminKullaniciListesi", "Home");

        }
        [Authorize]
        public async Task<ActionResult> KullaniciEdit(string Id)
        {
            User Dbu = await _userReadRepository.GetByIdAsync(Id);
            return View(Dbu);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> KullaniciEdit([FromForm] User u)
        {
            string Id = u.Id.ToString();
            User Dbu = await _userReadRepository.GetByIdAsync(Id);
            Dbu = u;
            await _userWriteRepository.RemoveAsync(Id);
            await _userWriteRepository.AddAsync(Dbu);
            await _userWriteRepository.SaveAsync();
            return RedirectToAction("AdminKullaniciListesi");
        }

        [Authorize]
        public IActionResult KullanıcıEkle(UserListPageViewModal u)
        {
            User Dbu = new User { };
            Dbu.UserName = u.SingleUser.UserName;
            Dbu.Password = u.SingleUser.Password;
            Dbu.Role = u.SingleUser.Role;

            _userWriteRepository.AddAsync(Dbu).Wait();
            _userWriteRepository.SaveAsync().Wait();

            return RedirectToAction("AdminKullaniciListesi");
        }
        [Authorize]
        public IActionResult _UserlistAdmin()
        {
            var item = _userReadRepository.Get();
            return PartialView("~/Views/PartialView/_UserlistAdmin.cshtml", item);
        }
        [Authorize]
        public IActionResult AdminKullaniciListesi()
        {
            var items = _userReadRepository.Get().ToList();
            var denem = new UserListPageViewModal()
            {
                User = items,
                SingleUser = new User
                {
                    UserName = null,
                    Password = null,
                    Role = null,
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                },
            };




            return View(denem);
        }
        [Authorize]
        public IActionResult Api()
        {
            var items = _accessTokenReadRepository.Get().ToList();
            var api = new ApiPageViewModel()
            {
                Token = items,
                SingleToken = new AccessToken
                {
                    TokenTitle = null,
                    Token = null,
                    CreatedDate = DateTime.Now,


                }
            };


            return View(api);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ApiEkle(ApiPageViewModel a)
        {
            AccessToken Dba = new AccessToken { };
            Dba.TokenTitle = a.SingleToken.TokenTitle;
            Dba.Token = a.SingleToken.Token;

            _accessTokenWriteRepository.AddAsync(Dba).Wait();
            _accessTokenWriteRepository.SaveAsync().Wait();

            return RedirectToAction("Api");

        }


        [Authorize]
        public async Task<ActionResult> ApiSil(string Id)
        {

            await _accessTokenWriteRepository.RemoveAsync(Id);
            await _accessTokenWriteRepository.SaveAsync();

            return RedirectToAction("Api", "Home");

        }

        [Authorize]
        public async Task<ActionResult> ApiGuncelle(string Id)
        {
            AccessToken Dba = await _accessTokenReadRepository.GetByIdAsync(Id);
            return View(Dba);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ApiGuncelle([FromForm] AccessToken a)
        {
            string Id = a.Id.ToString();
            AccessToken Dba = await _accessTokenReadRepository.GetByIdAsync(Id);
            Dba = a;
            await _accessTokenWriteRepository.RemoveAsync(Id);
            await _accessTokenWriteRepository.AddAsync(Dba);
            await _accessTokenWriteRepository.SaveAsync();
            return RedirectToAction("Api");
        }

    }

}
