using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AutoMapper;
using SymHack.Model;
using SymHack.Repository;

namespace SymHack.Models
{
    public class HomeViewModels
    {
        public List<List<ModuleViewModels>> ModuleGroups { get; set; }
        public List<string> CurrentGame { get; set; }
    }

    public class OptionsViewModel
    {
        [Required]
        [Display(Name="Music Volume")]
        public int Volume { get; set; }
        [Required]
        [Display(Name = "Music Style")]
        public string Style { get; set; }
    }
}