using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SymHack.Models
{
    public class TeacherViewModel
    {
        [Display(Name="Students")]
        public ICollection<StudentViewModel> Students { get; set; }
    }

    public class StudentViewModel
    {
        [Display(Name="Name")]
        public String Name { get; set; }
    }
}