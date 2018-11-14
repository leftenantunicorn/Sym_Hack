using System;
using System.Collections.Generic;
using System.Linq;
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
                CreateMap<Module, ModuleViewModels>()
                    .ForMember(dest => dest.Log, opt => opt.ResolveUsing((src, dest, destMember, context) =>
                        moduleManager.GetUserModuleByModuleAndUserId(src.Id, context.Items["userId"].ToString()).Log))
                    .ForMember(dest => dest.UserModuleId, opt => opt.ResolveUsing((src, dest, destMember, context) =>
                        moduleManager.GetUserModuleByModuleAndUserId(src.Id, context.Items["userId"].ToString()).Id))
                    .ForMember(dest => dest.Username,
                        opt => opt.ResolveUsing(
                            (src, dest, destMember, context) => context.Items["username"].ToString()))
                    .ForMember(dest => dest.ModuleType, opt => opt.MapFrom(src => src.Type.Name))
                    .ForMember(dest => dest.Responses, opt => opt.MapFrom(src => src.Responses.ToDictionary(r => r.Request, r=> r.Response)));
            }
        }
    }
}