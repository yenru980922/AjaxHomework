
namespace Fuen31Site.Models.Dto
{
    public class UserDTO
    {
        public string? name { get; set; }
        public int Age { get; set; } = 29;

        public string? Email { get; set; }

        public IFormFile? Avater { get; set; }
        public object Avatar { get; internal set; }

        internal bool IsAccountExist(object name)
        {
            throw new NotImplementedException();
        }
    }
}
