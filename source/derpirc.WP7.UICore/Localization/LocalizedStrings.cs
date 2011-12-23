
namespace derpirc.Localization
{
    public class LocalizedStrings
    {
        private AppResources _localizedResources;
        public AppResources LocalizedResources
        {
            get { return _localizedResources; }
            private set { }
        }

        public LocalizedStrings()
        {
            _localizedResources = new AppResources();
        }
    }
}
