using Medical.Models;
using Medical.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Medical.Controllers
{
    public class LoginController : Controller
    {
        private readonly MedicalContext _context;

        public LoginController(MedicalContext context)  //注入
        {
            _context = context;
        }
        //======================================================================
        //public IActionResult AdminLoginMemberList()   //管理員帳號登入=>會員清單管理
        //{
        //    if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USE))  //TODO 還需要寫一個getSession(登出)/未驗證身分
        //    {
        //        MedicalContext medicalContext = new MedicalContext();
        //        //CMemberAdminViewModel vModel = new CMemberAdminViewModel();
        //        IEnumerable<Member> datas = null;

        //        datas = from t in medicalContext.Members
        //                select t;

        //        return View(datas);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}
        public IActionResult LoginSuccess()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USE))
            {
                return RedirectToAction("LoginSuccess");   //已登入的證明
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(CLoginViewModel vModel)
        {
            string jasonUser = "";
            //先設定email登入
            //Member mb = (new MedicalContext()).Members.FirstOrDefault(n => n.Email == vModel.txtAccount);
            Member mb = (new MedicalContext()).Members.FirstOrDefault(n => n.Email.Equals(vModel.txtAccount));  //linQ不分大小寫

            if (mb != null)
            {
                if (mb.Email.Equals(vModel.txtAccount) && mb.Password.Equals(vModel.txtPassword) && mb.Role == 1)
                {
                    //LogbySession(mb);
                    jasonUser = JsonSerializer.Serialize(mb);
                    HttpContext.Session.SetString(CDictionary.SK_LOGINED_USE, jasonUser);
                    return RedirectToAction("Index", "Home");
                }
                else if (mb.Email.Equals(vModel.txtAccount) && mb.Password.Equals(vModel.txtPassword) && mb.Role == 2)
                {
                    jasonUser = JsonSerializer.Serialize(mb);
                    HttpContext.Session.SetString(CDictionary.SK_LOGINED_USE, jasonUser);
                    return RedirectToAction("List", "Consultation", new { area = "Doctors" });
                }
                else if (mb.Email.Equals(vModel.txtAccount) && mb.Password.Equals(vModel.txtPassword) && mb.Role == 3)
                {
                    jasonUser = JsonSerializer.Serialize(mb);
                    HttpContext.Session.SetString(CDictionary.SK_LOGINED_USE, jasonUser);
                    return RedirectToAction("Index", "Home",new {area="Admin" });
                }
            }
            //TODO 身分別登入不同頁面
            return View();
        }



        public IActionResult Register()
        {
            CRegisterViewModel regVModel = new CRegisterViewModel()
            {
                mem = _context.Members.ToList(),
                roleTypes = _context.RoleTypes.ToList(),
                MemCity=_context.Cities.ToList(),
                MemGender=_context.Genders.ToList()
            };
            return View(regVModel);
        }

        [HttpPost]
        public IActionResult Register(CRegisterViewModel vModel)
        {
            if (vModel != null)
            {

                sendMail();

                _context.Members.Add(vModel.member);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }


        private void sendMail()
        {
            string mailContent = String.Empty;
            mailContent += "<h1>請確認您的信箱</h1>";
            mailContent += "此信件由系統自動發送";
       
        }





        //public IActionResult Delete(int? id)
        //{
        //    MedicalContext db = new MedicalContext();
        //    Member mem = db.Members.FirstOrDefault(c => c.MemberId == id);
        //    if (mem != null)
        //    {
        //        db.Members.Remove(mem);
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("AdminLoginMemberList", "Login");
        //}


        public IActionResult Logout()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USE))
            {

                HttpContext.Session.Remove(CDictionary.SK_LOGINED_USE);
                    return RedirectToAction("Index", "Home");
        
            }
            return RedirectToAction("Index", "Home");
        }


            }
}
