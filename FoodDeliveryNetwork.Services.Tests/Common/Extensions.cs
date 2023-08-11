using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace FoodDeliveryNetwork.Services.Tests.Common
{
    public static class Extensions
    {
        public static void AddRoles(this RoleManager<IdentityRole<Guid>> roleManager)
        {
            Type roleNamesType = typeof(AppConstants.RoleNames);
            FieldInfo[] fields = roleNamesType.GetFields(BindingFlags.Public | BindingFlags.Static);
            var roleFields = fields.Where(f => f.FieldType == typeof(string));

            foreach (var roleField in roleFields)
            {
                var roleName = roleField.GetValue(null) as string;

                if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(new IdentityRole<Guid>(roleName)).GetAwaiter().GetResult();
                }
            }
        }

        
    }
}
