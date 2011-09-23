using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models.Settings
{
    [Table]
    public partial class User : BaseNotify, IBaseModel
    {
        [Column(IsVersion = true)]
        private Binary version;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public string Name { get; set; }
        [Column(CanBeNull = false)]
        public string NickName { get; set; }
        [Column(CanBeNull = true)]
        public string NickNameAlternate { get; set; }
        [Column(CanBeNull = true)]
        public string FullName { get; set; }
        [Column(CanBeNull = true)]
        public string Username { get; set; }
        [Column(CanBeNull = true)]
        public bool IsInvisible { get; set; }
        [Column(CanBeNull = true)]
        public string QuitMessage { get; set; }

        #endregion

        public User()
        {
            
        }
    }
}
