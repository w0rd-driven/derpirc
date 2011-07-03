using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table]
    public partial class User : BaseModel<User>, IUser
    {
        [Column(DbType = "Int NOT NULL", IsPrimaryKey = true)]
        public new int Id { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string NickName { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string NickNameAlternates { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string Name { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string Email { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string QuitMessage { get; set; }

        public User()
        {
            
        }
    }
}
