
namespace derpirc.Localization
{
    public class LocalizedStrings
    {
        private AppResources _resources;
        public AppResources Resources
        {
            get { return _resources; }
            private set { }
        }

        public LocalizedStrings()
        {
            _resources = new AppResources();
        }
    }
}
