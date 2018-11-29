using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using SymHack.Model;
using SymHack.Models;
using SymHack.Repository;

namespace SymHack.Controllers
{
    public static class Helpers
    {
        public static PlayerStatsViewModel GetPlayerModel(SymHackUser user, ModuleManager moduleManager)
        {
            var player = new PlayerStatsViewModel()
            {
                Email = user.Email,
                Name = user.FirstName + " " + user.LastName,
                Stats = new List<ModuleGroupViewModel>()
            };

            var modules = moduleManager.GetAllModules();
            var initials = modules.Where(m => m.Prerequisite == null).ToList();

            var moduleGroups = new List<OrderedDictionary>();
            string type = "";

            foreach (var module in initials)
            {
                type = module.Type.Name;
                var next = moduleManager.GetNextModuleById(module.Id);
                var group = new OrderedDictionary();
                var status = moduleManager.GetUserModuleByModuleAndUserId(module.Id, user.Id).Status.Status;
                group.Add(module.Title, status.Equals("Not Started") ? -0.1 : (status.Equals("In Progress") ? 1 : 2));
                while (next != null)
                {
                    status = moduleManager.GetUserModuleByModuleAndUserId(next.Id, user.Id).Status.Status;
                    group.Add(next.Title, status.Equals("Not Started") ? -0.1 : (status.Equals("In Progress") ? 1 : 2));
                    next = moduleManager.GetNextModuleById(next.Id);
                }
                moduleGroups.Add(group);
            }

            int index = 0;
            foreach (var moduleGroup in moduleGroups)
            {
                var labels = moduleGroup.Keys;
                var data = moduleGroup.Values;

                string[] colourList = new string[moduleGroup.Count],
                backgroundColours = new string[moduleGroup.Count],
                borderColours = new string[moduleGroup.Count];
                for (int i = 0; i < moduleGroup.Count; i++)
                {
                    colourList[i] = ChartColouring.ColourList[i % ChartColouring.ColourList.Length];
                    backgroundColours[i] =
                        ChartColouring.BackgroundColour[i % ChartColouring.BackgroundColour.Length];
                    borderColours[i] = ChartColouring.BorderColour[i % ChartColouring.BorderColour.Length];
                }

                player.Stats.Add(new ModuleGroupViewModel()
                {
                    Id = index++,
                    Type = type,
                    Labels = labels.Cast<string>().ToArray(),
                    Data = data.Cast<double>().ToArray(),
                    ColourList = colourList,
                    BackgroundColour = backgroundColours,
                    BorderColour = borderColours
                });
            }

            return player;
        }
    }
}