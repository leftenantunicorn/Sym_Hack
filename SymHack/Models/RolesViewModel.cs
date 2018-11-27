using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using FileHelpers;
using Microsoft.Ajax.Utilities;

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

    [DelimitedRecord(",")]
    public class StudentViewModel
    {

        [Display(Name = "Student Id")]
        [FieldNullValue(typeof(string), null)]
        public string ExternalIdentifier { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name="Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [FieldHidden]
        [Display(Name = "Confirmed")]
        public string Confirmed { get; set; }
    }

    public class PlayerViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Name")]
        public String Name { get; set; }

        public ICollection<ModuleViewModels> Modules { get; set; }

        public string Labels { get; set; }
        public string Data { get; set; }
        public string BackgroundColour { get; set; }
        public string BorderColour { get; set; }
        public string ColourList { get; set; }
    }

    public static class ChartColouring
    {
        private static readonly string[] borderColour = {"rgba(255, 99, 132, 1)",
            "rgba(54, 162, 235, 1)",
            "rgba(255, 206, 86, 1)",
            "rgba(75, 192, 192, 1)",
            "rgba(153, 102, 255, 1)",
            "rgba(255, 159, 64, 1)"};
        private static readonly string[] backgroundColour = {"rgba(255, 99, 132, 0.2)",
            "rgba(54, 162, 235, 0.2)",
            "rgba(255, 206, 86, 0.2)",
            "rgba(75, 192, 192, 0.2)",
            "rgba(153, 102, 255, 0.2)",
            "rgba(255, 159, 64, 0.2)"};
        private static readonly string[] colourList =  {"Red", "Blue", "Yellow", "Green", "Purple", "Orange"};

    public static string[] BorderColour
        {
            get { return borderColour; }
        }

        public static string[] BackgroundColour
        {
            get { return backgroundColour; }
        }
        public static string[] ColourList
        {
            get { return colourList; }
        }


    }
}