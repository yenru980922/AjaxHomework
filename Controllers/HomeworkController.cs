using Fuen31Site.Models;
using Fuen31Site.Models.Dto;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;

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

        public IActionResult Homework3()
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

        [HttpPost]
        public IActionResult Register(Member member, IFormFile avatar)
        {
            if (avatar == null)
            {
                return NoContent();
            }
            string uploadFile = Path.Combine(_host.WebRootPath, "uploads", avatar.FileName);

            //上傳檔案
            using (var fileStream = new FileStream(uploadFile, FileMode.Create))
            {
                avatar.CopyTo(fileStream);
            }
            member.FileName = avatar.FileName;
            //轉成二進位
            byte[]? imgByte = null;
            using (var memoryStream = new MemoryStream())
            {
                avatar.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }
            member.FileData = imgByte;

            //密碼加鹽          
            // 設定 PBKDF2 參數
            int iterations = 10000;
            int numBytesRequested = 32;
            //產生鹽
            byte[] salt = GenerateRandomSalt();
            // 使用 PBKDF2 加密
            byte[] hashedPassword = KeyDerivation.Pbkdf2(member.Password, salt, KeyDerivationPrf.HMACSHA512, iterations, numBytesRequested);

            // salt 和 Password 可以被儲存為位元組陣列或轉換成十六進位字串
            member.Salt = Convert.ToBase64String(salt);
            member.Password = Convert.ToBase64String(hashedPassword);

            _dbContext.Members.Add(member);
            _dbContext.SaveChanges();
            return Content("存檔成功", "text/plain", System.Text.Encoding.UTF8);
        }

        // 產生鹽
        private static byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[16]; 
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

    }
}
