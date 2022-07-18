using Medical.Models;
using Medical.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Controllers
{
    [Area(areaName: "Admin")]
    public class AdminClinicDetailController : Controller
    {
        private readonly MedicalContext _medicalContext;
        public AdminClinicDetailController(MedicalContext medicalContext)
        {
            _medicalContext = medicalContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult loadData() {
            IEnumerable<CClinicDetailViewModel> list = null;
            list = _medicalContext.ClinicDetails.Select(x => new CClinicDetailViewModel
            {
                clinicDetail = x,
                Doctor = x.Doctor,
                Department = x.Department,
                Period = x.Period,
                Room = x.Room,
                ClinicDate = x.ClinicDate
            });

            string info = "";
            foreach (var i in list)
            {
                info += $"{i.ClinicDetailId},{i.Doctor.DoctorName},{i.ClinicDate},{i.RoomId},{i.PeriodId}#";
            }
            info = info.Substring(0, info.Length - 1);

            return Content(info, "text/plain", System.Text.Encoding.UTF8);
        }

        public IActionResult CountClinicDetailId()
        {
            List<CClinicDetailViewModel> list = new List<CClinicDetailViewModel>();
            var count = _medicalContext.ClinicDetails.Count().ToString();
            return Content(count, "text/plain", System.Text.Encoding.UTF8);
        }

        public void Method(CClinicDetailViewModel cVM) {
            var result = _medicalContext.ClinicDetails.Where(x => x.ClinicDetailId.Equals(cVM.clinicDetailId));
            var resultDoctor = _medicalContext.Doctors.Where(x => x.DoctorName.Equals(cVM.doctorname)).SingleOrDefault();
            cVM.DoctorId = resultDoctor.DoctorId;
            cVM.DepartmentId = resultDoctor.DepartmentId;

            if (result.Count() > 0)
            { 
                Update(cVM);
            }
            else
            {
                Create(cVM);
            }
        }

        public void Create(CClinicDetailViewModel cVM) 
        {
            ClinicDetail c = new ClinicDetail()
            {
                DoctorId = cVM.DoctorId,
                DepartmentId = cVM.DepartmentId,
                PeriodId = cVM.periodID,
                RoomId = cVM.roomID,
                Online = 0,
                ClinicDate = cVM.date
            };

            _medicalContext.ClinicDetails.Add(c);
            _medicalContext.SaveChanges();

        }
        public void Update(CClinicDetailViewModel cVM)
        {
            ClinicDetail clinicDetail = _medicalContext.ClinicDetails.Where(x => x.ClinicDetailId.Equals(cVM.clinicDetailId)).FirstOrDefault();
            
            if(clinicDetail != null)
            {
                clinicDetail.DoctorId = cVM.DoctorId;
                clinicDetail.PeriodId = cVM.periodID;
                clinicDetail.RoomId = cVM.roomID;
                clinicDetail.ClinicDate = cVM.date;
                _medicalContext.SaveChanges();
            }
        }

        public IActionResult doctorList()
        {
            var doctors = _medicalContext.Doctors.Select( x => x.DoctorName );
            return Json(doctors);
        }

        public IActionResult roomList()
        {
            var rooms = _medicalContext.ClinicRooms.Select(x => x.RoomName);
            return Json(rooms);
        }
    }
}
