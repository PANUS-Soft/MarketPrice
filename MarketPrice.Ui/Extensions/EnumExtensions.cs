using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attr = memberInfo[0].GetCustomAttribute<DisplayAttribute>();
                if (attr != null)
                    return attr.Name;
            }
            return enumValue.ToString();
        }
    }
}
