﻿using DigitalTVBilling.ListModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTVBilling.Infrastructure.IntefaceClass
{
    interface IOrderUserStatus
    {
        List<IdName> Result();
    }
}
