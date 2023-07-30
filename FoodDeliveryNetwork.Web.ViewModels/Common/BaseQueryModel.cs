namespace FoodDeliveryNetwork.Web.ViewModels.Common
{
    public class BaseQueryModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public BaseQueryModelSort SortBy { get; set; } = BaseQueryModelSort.Newest;
        public string SearchTerm { get; set; }
    }
}
