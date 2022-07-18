using Medical.Models;
using Medical.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Controllers
{
    [Area(areaName: "Admin")]
    public class ArticleAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly MedicalContext _medicalContext;
        public ArticleAdminController(MedicalContext medicalContext)
        {
            _medicalContext = medicalContext;
        }
        

        public IActionResult List(AArticleSearchKeywordViewModel vModel)
        {
            
            List<AArticleViewModel> list = new List<AArticleViewModel>();

            if (string.IsNullOrEmpty(vModel.txtKeyword))
            {
                var datas = _medicalContext.Articles.Include(x => x.Doctor);
                foreach(var a in datas)
                {
                    AArticleViewModel ar = new AArticleViewModel();
                    ar.article = a;
                    list.Add(ar);
                }
            }
            else
            {
                var datas = _medicalContext.Articles.Include(x=>x.Doctor).Where(a => 
                a.Articeltitle.Contains(vModel.txtKeyword)||
                a.Doctor.DoctorName.Contains(vModel.txtKeyword));
                foreach (var a in datas)
                {
                    AArticleViewModel ar = new AArticleViewModel();
                    ar.article = a;
                    list.Add(ar);
                }
            }
            return View(list.ToList());
        }
        public IActionResult CreateDoctorCheckBox()
        {
            var data = _medicalContext.Doctors.Select(x => x.DoctorName).Distinct();
            return Json(data);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Article a)
        {
            a.CreateDate = DateTime.Now.ToString("yyyy/MM/dd");
            _medicalContext.Articles.Add(a);
            _medicalContext.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Delete(int? id)
        {

            Article article = _medicalContext.Articles.FirstOrDefault(a => a.ArticleId == id);
            if (article != null)
            {
                _medicalContext.Articles.Remove(article);
                _medicalContext.SaveChanges();
            }
            return RedirectToAction("List");
        }
        public IActionResult Edit(int? id)
        {
            AArticleViewModel article = new AArticleViewModel();
            article.article = _medicalContext.Articles.Include(x=>x.Doctor).FirstOrDefault(a => a.ArticleId == id);
            if (article == null)
            {
                return RedirectToAction("List");
            }
            return View(article);
        }
        [HttpPost]
        public IActionResult Edit(Article a)
        {
            Article ar = _medicalContext.Articles.FirstOrDefault(t => t.ArticleId == a.ArticleId);
            if (ar != null)
            {
                ar.Doctor= a.Doctor;
                ar.CreateDate = DateTime.Now.ToString("yyyy/MM/dd");
                ar.ArticleContent = a.ArticleContent;
                ar.Articeltitle = a.Articeltitle;
            }
            _medicalContext.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
