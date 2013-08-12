﻿#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.Enums;
using SmartHome.Comunications.Messages;
using DataLayer.Entities.HomeDevices;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessDimmable
    {
        [OperationAttribute]        
        public static OperationMessage On(this Dimmable dimmable)
        {
            return dimmable.PercentageDimmer(100);
        }

        [OperationAttribute]
        public static OperationMessage Off(this Dimmable dimmable)
        {
            return dimmable.PercentageDimmer(0);
        }

        [OperationAttribute]
        public static OperationMessage OnTime(this Dimmable dimmable, byte seconds)
        {
            return dimmable.PercentageDimmer(100, seconds);
        }

        [OperationAttribute]
        public static OperationMessage Switch(this Dimmable dimmable)
        {
            if (dimmable.Value != 0)
            {
                return dimmable.Off();
            }
            else
            {
                return dimmable.PercentageDimmer(dimmable.lastValue);
            }
        }

        [OperationAttribute]
        public static OperationMessage PercentageDimmer(this Dimmable dimmable, int percentage)
        {
            return dimmable.PercentageDimmer(percentage, 0);
        }

        [OperationAttribute]
        public static OperationMessage PercentageDimmer(this Dimmable dimmable, int percentage, byte seconds)
        {
            return OperationMessage.DimmerWrite(dimmable.Id, (byte)(percentage * byte.MaxValue / 100.0), 0);
        }
    }
}