using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models.Settings
{
    [Table]
    public partial class Formatting : BaseNotify, IBaseModel
    {
        [Column(IsVersion = true)]
        private Binary version;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public string FontFamily { get; set; }
        [Column(CanBeNull = true)]
        public string FontSize { get; set; }
        [Column(CanBeNull = true)]
        public string FontWeight { get; set; }

        #endregion

        public Formatting()
        {
            
        }
    }
}
