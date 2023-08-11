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

namespace Portal.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        readonly private IEtkinlikWriteRepository _etkinlikWriteRepository;
        readonly private IEtkinlikReadRepository _etkinlikReadRepository;
        readonly private IUserReadRepository _userReadRepository;
        readonly private IUserWriteRepository _userWriteRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUserWriteRepository userWriteRepository, IUserReadRepository userReadRepository, IEtkinlikWriteRepository etkinlikWriteRepository, IEtkinlikReadRepository etkinlikReadRepository, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _userWriteRepository = userWriteRepository;
            _userReadRepository = userReadRepository;
            _etkinlikWriteRepository = etkinlikWriteRepository;
            _etkinlikReadRepository = etkinlikReadRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        //========================================================================================================================


        public IActionResult GetList()
        {

            var data = _etkinlikReadRepository.GetAll();


            return Json(data);
        }



        public IActionResult _mdletkinlikekleicerik()
        {

            return PartialView("~/Views/PartialView/_mdletkinlikekleicerik.cshtml");
        }

        public PartialViewResult PartialEtkinlikkaydet()
        {
            return PartialView();
        }

        

        //-------------------------------------------------------------

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(HomeIndexViewModel model)
        {
            if(string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                return View(new HomeIndexViewModel { UserName = model.UserName, HasError = true, Error = "Kullanıcı adı ve şifre alanları boş olamaz!" });

            var datas = _userReadRepository.GetAll();
            var dataSearch = datas.Where(elm => elm.UserName == model.UserName && elm.Password == model.Password);

            if (dataSearch.Count() == 1)
            {
                model.Id = dataSearch.First().Id;
                

				if (model.Id== Guid.Parse("a1458335-cbea-4b1f-8746-081b9c131c31"))
                {
					return RedirectToAction("AdminPanel");
                }

                //Console.WriteLine(model.Id)
                return RedirectToAction("Takvim");
            }

            return View(new HomeIndexViewModel { UserName = model.UserName, HasError = true, Error = "Kullanıcı Adı Veya Şifre Hatalı!" });
        }

       
   
        public IActionResult Takvim() 
        {

            return View();
        }

        [HttpPost]
        public IActionResult Tahvim([FromForm]EtkinlikEkleViewModel e)
        {
            Etkinlik Dbe = new Etkinlik();
            if(e.image != null)
            {
                string dosyaadi = Path.GetFileName(e.image.FileName);
                string uzanti = Path.GetExtension(e.image.FileName);
                string wwwRootPath = _hostingEnvironment.WebRootPath;
                string yol = Path.Combine(wwwRootPath, "Images", dosyaadi + uzanti);
                using (var stream = new FileStream(yol, FileMode.Create))
                {
                    e.image.CopyTo(stream);
                }

                Dbe.image = dosyaadi;

            }
            Dbe.title = e.title;
            Dbe.description = e.description;
            Dbe.whatsapp = e.whatsapp;
            Dbe.linkedin = e.linkedin;
            Dbe.start = e.start;
           
            var start = e.start.ToString();
            DateTime end = DateTime.Parse(start);
            Dbe.end =end.AddDays(1);
            _etkinlikWriteRepository.AddAsync(Dbe).Wait();
            _etkinlikWriteRepository.SaveAsync().Wait();
            
            return RedirectToAction("Takvim");
        }

		//-------------------------------------------------------------



		public IActionResult _UserlistAdmin()
		{
			var item = _userReadRepository.Get();
			return PartialView("~/Views/PartialView/_UserlistAdmin.cshtml", item);
		}



		
        public IActionResult Test1()
        {

            return View();
        }



        public IActionResult AdminPanel() 
        {
            return View();
        }

        public IActionResult AdminKullanıcıListesi()
        {
            var items = _userReadRepository.Get().ToList();
            var denem = new UserListPageViewModal { };

            denem.User = items;
            denem.SingleUser = items[0];
            denem.SingleUser.Id  = Guid.NewGuid();
            return View(denem);
        }
        [HttpPost]
		public IActionResult AdminKullanıcıListesi(UserListPageViewModal e)
		{
			var items = _userReadRepository.Get();

			return View(items);
		}


	}
        

   
       
    }
