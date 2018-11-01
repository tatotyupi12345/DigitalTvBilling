using DigitalTVBilling.CallCenter.InterfaceUser;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class UserViewModel
    {
        private readonly IUser users;
        private readonly UserStatic _userStatic;
        private readonly IUserToGo orderToGo;
        private readonly IUserToGo damageToGo;
        private readonly IUserToGo cancellationToGo;
        private readonly CityRegionList cityRegionList;

        //private readonly CityRegionList cityRegionList;

        public UserViewModel(IUser users,UserStatic userStatic,  IUserToGo orderToGo, IUserToGo damageToGo, IUserToGo cancellationToGo, CityRegionList cityRegionList) {
            this.users = users;
            this._userStatic = userStatic;
            this.orderToGo = orderToGo;
            this.damageToGo = damageToGo;
            this.cancellationToGo = cancellationToGo;
            this.cityRegionList = cityRegionList;
            //this.cityRegionList = cityRegionList;
        }
        public List<CallUser> Result()
        {
            return users.Result().Select(s => new CallUser
            {
                Id = s.Id,
                Name = s.Name,
                Phone = s.Phone,
                Password = s.Password,
                CodeWord = s.CodeWord,
                HardAutorize = s.HardAutorize,
                Logging = s.Logging,
                Login = s.Login,
                SellerObj = s.SellerObj,
                Type = s.Type,
                TypeName = s.TypeName,
                UserType = s.UserType,
                image = s.image,
                Email = s.Email,
                StaticCounts = _userStatic.Result(s.Id),
                start_end = s.start_end,
                OrderToGo = orderToGo.Result(s.Id),
                DamageGoTo = damageToGo.Result(s.Id),
                CancellationToGo = cancellationToGo.Result(s.Id),
                CityRegion = cityRegionList.Result(s.Id)

            }).ToList();
        }
    }
}