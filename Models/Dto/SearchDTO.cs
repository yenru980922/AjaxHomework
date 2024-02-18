namespace Fuen31Site.Models.Dto
{
    public class SearchDTO
    {
        //搜尋相關
        public string? keyword { get; set; }
        public int? categoryId { get; set; } = 0; //

        //排序相關
        public string? sortBy { get; set; }
        public string? sortType { get; set; } = "asc";


        //分頁相關
        public int? page { get; set; } = 1;//第一頁
        public int? pageSize { get; set; } = 9;//一頁顯示9筆
    }
}
