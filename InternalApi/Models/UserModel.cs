namespace InternalApi.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PassWord { get; set; } = string.Empty;
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public IFormFile? File { get; set; }
        public string Img { get; set; } = string.Empty;
    }
}
