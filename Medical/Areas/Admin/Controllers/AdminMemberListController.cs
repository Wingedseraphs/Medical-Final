﻿using Medical.Models;
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
        public IActionResult AdminMemberList(CKeyWordViewModel keyVModel)   //管理員帳號登入=>會員清單管理
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USE))  //TODO 還需要寫一個getSession(登出)/未驗證身分
            {
                MedicalContext medicalContext = new MedicalContext();
                IEnumerable<Member> datas = null;
                if (string.IsNullOrEmpty(keyVModel.txtKeyword))
                {
                    datas = from t in medicalContext.Members
                            select t;
                }
                else
                {
                    datas = medicalContext.Members.Where(t => t.MemberName.Contains(keyVModel.txtKeyword)||t.Email.Contains(keyVModel.txtKeyword)||t.Phone.Contains(keyVModel.txtKeyword));
                }
                return View(datas);
            }
            else
            return RedirectToAction("Index", "Home");
        }

        //public IActionResult showRole(int dd)
        //{
        //    MedicalContext medicalContext = new MedicalContext();
        //    Member mem = new Member()
        //    {
        //        mem = medicalContext.Members.Where(n => n.Role == dd).Distinct().ToList()
        //    };
        //    return View(mem);
        //}

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
            MedicalContext db = new MedicalContext();
            Member mem = db.Members.FirstOrDefault(c => c.MemberId == id);
            if (mem == null)
                return RedirectToAction("AdminMemberList", "AdminMemberList");
            return View(mem);
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
