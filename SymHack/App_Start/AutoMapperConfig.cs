using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SymHack.Model;
using SymHack.Models;
using SymHack.Repository;

namespace SymHack.App_Start
{
    public class AutoMapperConfig
    {
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile(ModuleManager moduleManager)
            {
                CreateMap<ModuleHints, ModuleHintsViewModels>();
                CreateMap<ModuleHelp, ModuleHelpViewModels>();
                CreateMap<ModuleEmails, ModuleEmailsViewModels>();
                CreateMap<UserModuleEmails, UserModuleEmailsViewModels>();

                CreateMap<Module, ModuleViewModels>()
                    .ForMember(dest => dest.Log, opt => opt.ResolveUsing((src, dest, destMember, context) =>
                        moduleManager.GetUserModuleByModuleAndUserId(src.Id, context.Items["userId"].ToString())?.Log))
                    .ForMember(dest => dest.UserModuleId, opt => opt.ResolveUsing((src, dest, destMember, context) =>
                        moduleManager.GetUserModuleByModuleAndUserId(src.Id, context.Items["userId"].ToString())?.Id))
                    .ForMember(dest => dest.Username,
                        opt => opt.ResolveUsing(
                            (src, dest, destMember, context) => context.Items["username"].ToString()))
                    .ForMember(dest => dest.ModuleType, opt => opt.MapFrom(src => src.Type.Name))
                    .ForMember(dest => dest.Responses,
                        opt => opt.MapFrom(src => src.Responses.ToDictionary(r => r.Request, r => r.Response)))
                    .ForMember(dest => dest.Outbox, opt => opt.ResolveUsing((src, dest, destMember, context) =>
                        moduleManager.GetUserModuleByModuleAndUserId(src.Id, context.Items["userId"].ToString())?.ModuleEmails))
                    .ForMember(dest => dest.Inbox, opt => opt.MapFrom(src => src.Emails))
                    .ForMember(dest => dest.PrerequisiteId, opt => opt.MapFrom(src => src.Prerequisite.Id));

                CreateMap<RegisterViewModel, SymHackUser>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.RequirePasswordChange, opt => opt.UseValue(false));

                CreateMap<StudentViewModel, SymHackUser>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.RequirePasswordChange, opt => opt.UseValue(true))
                    .ForMember(dest => dest.Teacher, opt => opt.ResolveUsing((src, dest, destMember, context) =>
                        context.Items["teacher_user"]));

                CreateMap<SymHackUser, StudentViewModel>()
                    .ForMember(dest => dest.ExternalIdentifier, opt => opt.NullSubstitute("N/A"))
                    .ForMember(dest => dest.Confirmed, opt => opt.MapFrom(src => src.EmailConfirmed ? "Accepted" : "Pending"));
            }
        }
    }
}