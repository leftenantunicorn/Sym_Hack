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

        [Display(Name="Register Students")]
        public ICollection<StudentViewModel> RegisterStudents { get; set; }

        public TeacherViewModel()
        {
            RegisterStudents = new List<StudentViewModel>();
        } 
    }

    public class StudentViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name="Email")]
        public String Email { get; set; }
    }

    public class PlayerViewModel
    {
        [Display(Name = "Name")]
        public String Name { get; set; }
    }
}