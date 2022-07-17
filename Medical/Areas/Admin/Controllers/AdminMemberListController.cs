using Medical.Models;
using Medical.ViewModel;
using Medical.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Areas.Admin.Controllers
{[Area(areaName: "Admin")]
    public class AdminMemberListController : Controller
    {
        private readonly MedicalContext _context;

        public AdminMemberListController(MedicalContext context)  //注入
        {
            _context = context;
        }
        //======================================================================
        public IActionResult AdminMemberList(CKeyWordViewModel keyVModel,int? Role)   //管理員帳號登入=>會員清單管理
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USE))  
            {
                if (string.IsNullOrEmpty(keyVModel.txtKeyword))
                {
                    if (Role!=null||Role!=0)   //目前沒用
                    {
                        CRegisterViewModel memVModel = new CRegisterViewModel();

                        memVModel.mem = _context.Members.ToList();
                        memVModel.roleTypes = _context.RoleTypes.Where(n => n.Role == Role).ToList();


                        return View(memVModel);
                    }
                    else
                    {
                        CRegisterViewModel memVModel = new CRegisterViewModel()
                        {
                            mem = _context.Members.ToList(),
                            roleTypes = _context.RoleTypes.ToList()
                        };  
                        return View(memVModel);
                    }
                 
                }
                else
                {
                    CRegisterViewModel AdVModel = new CRegisterViewModel()
                    {
                        mem = _context.Members.Where(t => t.MemberName.Contains(keyVModel.txtKeyword) ||
                          t.Email.Contains(keyVModel.txtKeyword) || t.Phone.Contains(keyVModel.txtKeyword)).ToList(),
                                roleTypes = _context.RoleTypes.ToList()
                    };
                    return View(AdVModel);
                }

            }
            else
                return RedirectToAction("Index", "Home");
        }



        public IActionResult AdminCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AdminCreate(CRegisterViewModel vModel)
        {
            MedicalContext medicalDb = new MedicalContext();
            medicalDb.Members.Add(vModel.member);

            medicalDb.SaveChanges();
            return RedirectToAction("AdminMemberList", "AdminMemberList");
        }
        public IActionResult Delete(int? id)
        {
            MedicalContext db = new MedicalContext();
            Member mem = db.Members.FirstOrDefault(c => c.MemberId == id);
            if (mem != null)
            {
                db.Members.Remove(mem);
                db.SaveChanges();
            }
            return RedirectToAction("AdminMemberList", "AdminMemberList");
        }


        public IActionResult Edit(int? id)
        {      
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USE))
            {
              MedicalContext db = new MedicalContext();
                Member mem = db.Members.FirstOrDefault(c => c.MemberId == id);
        
             return View(mem);
            }
              return RedirectToAction("AdminMemberList", "AdminMemberList");
        }
        [HttpPost]
        public IActionResult Edit(Member p)
        {
            MedicalContext db = new MedicalContext();
            Member mem = db.Members.FirstOrDefault(c => c.MemberId == p.MemberId);
            if (mem != null)
            {
                mem.IdentityId = p.IdentityId;
                mem.Password = p.Password;
                mem.MemberName = p.MemberName;
                mem.BirthDay = p.BirthDay;
                mem.GenderId = p.GenderId;
                mem.BloodType = p.BloodType;
                mem.Weight = p.Weight;
                mem.IcCardNo = p.IcCardNo;
                mem.Phone = p.Phone;
                mem.Email = p.Email;
                mem.Role = p.Role;
                mem.CityId = p.CityId;
                mem.Address = p.Address;


                db.SaveChanges();
            }
            return RedirectToAction("AdminMemberList","AdminMemberList");
        }

        public IActionResult TestRole(CKeyWordViewModel keyVModel)
        {
            IEnumerable<CMemberAdminViewModel> list = null;
            list = _context.Members.Where(n => n.Role == 1).Select(n => new CMemberAdminViewModel
            {
                MemberId=n.MemberId,
                MemberName=n.MemberName,
                Role=n.Role,
                Address=n.Address
            });

            return View(list);
        }
        //=================for Ajax API
  
        //public IActionResult showRolebyAjax(CMemberAdminViewModel AdminVModel)
        //{

        //    Member mb = _context.Members.FirstOrDefault(n => n.Email == AdminVModel.Email);
        //    if (String.IsNullOrEmpty(AdminVModel.Email))
        //    {
        //        AdminVModel.emailState = "請填入信箱";
        //    }
        //    else if (mb != null)
        //    {
        //        AdminVModel.emailState = "帳號已存在";
        //    }
        //    else
        //    {
        //        user.emailState = "帳號可使用";
        //    }

        //    return Content(user.emailState, "text/html", System.Text.Encoding.UTF8);
        //}


    }
}
