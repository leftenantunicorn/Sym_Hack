﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SymHack.Model;
using SymHack.Models;
using SymHack.Repository;

namespace SymHack.Controllers
{
    public class AdminController : CustomController
    {
        private ApplicationUserManager UserManager;
        private IMapper Mapper;

        public AdminController(ApplicationUserManager userManager, IMapper mapper)
        {
            UserManager = userManager;
            Mapper = mapper;
        }

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            var pending = await GetPending();
            return View(pending);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RejectTeacher(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user != null)
            {
                await UserManager.AddToRoleAsync(user.Id, "Student");
                await UserManager.RemoveFromRoleAsync(user.Id, "PendingTeacher");

                try
                {
                    await UserManager.SendEmailAsync(user.Id, "Request Rejection",
                        "Your request to be a teacher in the Sym-Hack game has been rejected.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return PartialView("ListPending", await GetPending());
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ApproveTeacher(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user != null)
            {
                await UserManager.AddToRoleAsync(user.Id, "Teacher");
                await UserManager.RemoveFromRoleAsync(user.Id, "PendingTeacher");

                try
                {
                    await UserManager.SendEmailAsync(user.Id, "Request Approval",
                        "Your request to be a teacher in the Sym-Hack game has been approved.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return PartialView("ListPending", await GetPending());
        }

        private async Task<AdminViewModel> GetPending()
        {
            ICollection<StudentViewModel> pending = new List<StudentViewModel>();
            foreach (var user in UserManager.Users)
            {
                try
                {
                    if (await UserManager.IsInRoleAsync(user.Id, "PendingTeacher"))
                        pending.Add(Mapper.Map<StudentViewModel>(user));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return new AdminViewModel()
            {
                PendingTeachers = pending.ToList()
            };
        }
    }
}