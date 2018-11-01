using AutoMapper;
using DigitalTVBilling.CallCenter;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure
{
    public class AutomaperWebProfile : AutoMapper.Profile
    {
        public static void Run()
        {
            AutoMapper.Mapper.Initialize(a =>
            {
                a.AddProfile<AutomaperWebProfile>();

            });
            //AutoMapper.Mapper.Initialize(cfg => cfg.CreateMap<Customer, IdName>());
            //var config = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<Customer, IdName>());
        }

        public AutomaperWebProfile()
        {
            CreateMap<Customer, Card>().PreserveReferences();
                
            //CreateMap<Customer, Card>().ForMember(dest => dest.AbonentNum,
            //        opt => opt.MapFrom(src => src.Cards.FirstOrDefault().AbonentNum));
            //CreateMap<IdName, Customer>();
        }
    }
}