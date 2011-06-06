

namespace derpirc.Data
{
    public class User : BaseModel<User>
    {
        public string NickName { get; set; }
        public string NickNameAlternates { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string QuitMessage { get; set; }

        public User()
        {

        }
    }
}
