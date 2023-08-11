using System.Security.Claims;

namespace FoodDeliveryNetwork.SignalR.Extensions
{
    //https://github.com/KrIsKa7a/CSharpWeb-May2023/blob/main/ASP.NET%20Fundamentals/E03.%20Identity%20Workshop%20-%20TaskBoard/TaskBoardApp/Extensions/ClaimsPrincipalExtensions.cs#L7
    public static class ClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
