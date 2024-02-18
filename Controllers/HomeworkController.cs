using Fuen31Site.Models;
using Fuen31Site.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Fuen31Site.Controllers
{
    public class HomeworkController : Controller
    {
        private readonly MyDBContext _dbContext;
        private readonly IWebHostEnvironment _host;

        public HomeworkController(MyDBContext dbContext, IWebHostEnvironment host)
        {
            _dbContext = dbContext;
            _host = host;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Homework2()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CheckAccount([FromBody] UserDTO user)
        {
            var isAccountExist = _dbContext.Members.Any(m => m.Name == user.name);

            if (isAccountExist)
            {
                return Json(new { success = false, message = "帳號已存在" });
            }
            else
            {
                // 這裡不直接保存帳號信息，僅返回可使用的信息
                return Json(new { success = true, message = "帳號可使用" });
            }
        }

        [HttpPost]
        public JsonResult SaveAccount([FromBody] UserDTO user)
        {
            var newMember = new Member
            {
                Name = user.name,
                Email = user.Email,
                Age = user.Age
            };

            _dbContext.Members.Add(newMember);
            _dbContext.SaveChanges();

            return Json(new { success = true, message = "用戶資料已保存" });
        }
    }
}
