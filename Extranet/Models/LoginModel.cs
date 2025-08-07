namespace Extranet.Models
{
    public class LoginModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string society { get; } = "SCHROLL SAS";
    }
}
