
namespace derpirc.Data
{
    public abstract class BaseModel<T> : IBaseModel where T : BaseModel<T>
    {
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            return obj is T && ((T)obj).Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("{0} with Id {1}", typeof(T).FullName, Id);
        }
    }
}
