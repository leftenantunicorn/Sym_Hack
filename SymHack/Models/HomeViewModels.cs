using System;
using System.Collections.Generic;
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
}