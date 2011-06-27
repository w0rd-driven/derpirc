
namespace derpirc.Data.Settings
{
    public interface IUser
    {
        string NickName { get; set; }
        string NickNameAlternates { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string QuitMessage { get; set; }
    }
}
