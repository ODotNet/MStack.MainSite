using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmitMapper;

namespace MStack.MainSite.WebFramework.Extensions
{
    public static class MapperExtensions
    {
        public static TTo Map<TFrom, TTo>(this TFrom from) where TFrom:class,new() where TTo:class,new()
        {
            return ObjectMapperManager.DefaultInstance.GetMapper<TFrom, TTo>().Map(from);
        }
    }
}