using System.Linq;
using Wintellect.Sterling;
using Wintellect.Sterling.Database;

namespace derpirc.Data
{
    public class IdentityTrigger<T> : BaseSterlingTrigger<T, int> where T : class, IBaseModel, new()
    {
        private static int _idx = 1;

        public IdentityTrigger(ISterlingDatabaseInstance database)
        {
            // if a record exists, set it to the highest value plus 1
            if (database.Query<T, int>().Any())
            {
                _idx = database.Query<T, int>().Max(key => key.Key) + 1;
            }
        }

        public override bool BeforeSave(T instance)
        {
            if (instance.Id < 1)
            {
                instance.Id = _idx++;
            }

            return true;
        }

        public override void AfterSave(T instance)
        {
            return;
        }

        public override bool BeforeDelete(int key)
        {
            return true;
        }
    }
}
