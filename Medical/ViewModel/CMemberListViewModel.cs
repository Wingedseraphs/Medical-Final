using Medical.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.ViewModel
{
    public class CMemberListViewModel
    {
        public List<Member> mem { get; set; }
        public List<RoleType> roleTypes { get; set; }
        public List<Gender> MemGender { get; set; }
        public List<City> MemCity { get; set; }

        //public List<SelectListItem> selectLists { get; set; }
    }
}
