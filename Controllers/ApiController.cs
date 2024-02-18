using Fuen31Site.Models;
using Fuen31Site.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Fuen31Site.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _dbContext;
        private readonly IWebHostEnvironment _host;
        public ApiController(MyDBContext dbContext, IWebHostEnvironment host)
        {
            _dbContext = dbContext;
            _host = host;
        }
        public IActionResult Index()
        {
            System.Threading.Thread.Sleep(5000);
            return Content("<h2>Content,你好</h2>", "text/plain", System.Text.Encoding.UTF8);
        }


        public IActionResult Cities()
        {
            var cities = _dbContext.Addresses.Select(a => a.City).Distinct();
            return Json(cities);
        }
        public IActionResult Districts(string city)
        {
            var districts = _dbContext.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(districts);
        }

        public IActionResult Avatar(int id = 1)
        {
            Member? member = _dbContext.Members.Find(id);
            if (member != null)
            {
                byte[] img = member.FileData;
                return File(img, "image/jpeg");
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Register(UserDTO _user)
        {
            string filename = "empty.jpg";
            if (string.IsNullOrEmpty(_user.name))
            {
                _user.name = "Guest";
            }
            //string uploadPath = @"C:\Users\ispan\Documents\workspace\Fuen31Site\wwwroot\uploads\fileName.jpg";

            //todo 檔案存在的處理
            //todo 限制上傳的檔案類型
            //todo 限制上傳的檔案大小

            string fileName = "empty.jpg";
            if (_user.Avatar != null)
            {
                fileName = _user.Avater.FileName;
            }
            //取得檔案上傳的實際路徑
            string uploadPath = Path.Combine(_host.WebRootPath, "uploads", fileName);
            //檔案上傳
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                _user.Avater.CopyTo(fileStream);
            }

            return Content($"{_user.Avater?.FileName}-{_user.Avater?.Length}-{_user.Avater?.ContentType}");
        }
        public IActionResult First()
        {
            
            return View();
        }

        public IActionResult Address()
        {

            return View();
        }


        [HttpPost]
        public IActionResult spots([FromBody] SearchDTO _search)
        {
            //根據分類編號讀取景點資料
            var spots = _search.categoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots.Where(s => s.CategoryId == _search.categoryId);


            //根據關鍵字搜尋
            if (!string.IsNullOrEmpty(_search.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_search.keyword) || s.SpotDescription.Contains(_search.keyword));
            }

            //排序
            switch (_search.sortBy)
            {
                case "spotTitle":
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                default:
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //總共有多少筆資料
            int TotalCount = spots.Count();

            //設定每頁顯示多少筆資料
            int pageSize = _search.pageSize ?? 9;

            //目前要顯示第幾頁
            int page = _search.page ?? 1;

            //計算總共有幾頁
            int TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize);


            //取得分頁的資料
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);


            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalPages = TotalPages;
            spotsPaging.SpotsResult = spots.ToList();


            return Json(spotsPaging);

        }

        public IActionResult SpotsTitle(string keyword)
        {
            var spots = _dbContext.Spots.Where(s => s.SpotTitle.Contains(keyword))
                               .Select(s => s.SpotTitle).Take(8);
            return Json(spots);

        }
    }
}
