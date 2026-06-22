namespace OnlineExamSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
            = "";

        public string Password { get; set; }
            = "";
        public string FullName { get; set; }
            = "";

        public string Email { get; set; }
            = "";

        public string PhoneNumber { get; set; }
            = "";
        public DateTime CreatedAt
        {
            get; set;
        } = DateTime.Now;

        public DateTime? LastLoginAt
        {
            get; set;
        }

        public int LoginCount
        {
            get; set;
        }
        public Role Role
        {
            get; set;
        }
    }
}
