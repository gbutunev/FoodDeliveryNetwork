using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FoodDeliveryNetwork.Web.Binders
{
    //https://github.com/KrIsKa7a/CSharpWeb-May2023/blob/f6528f73fcef612a9d1d90f9a71ba85b0f43e10d/ASP.NET%20Advanced/HouseRentingSystem.Web.Infrastructure/ModelBinders/DecimalModelBinderProvider.cs
    public class DecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(decimal) ||
                context.Metadata.ModelType == typeof(decimal?))
            {
                return new DecimalModelBinder();
            }

            return null!;
        }
    }
}
