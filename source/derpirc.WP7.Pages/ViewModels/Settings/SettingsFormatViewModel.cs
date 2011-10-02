using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using derpirc.Data;
using derpirc.Data.Models.Settings;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels.Settings
{
    public class SettingsFormatViewModel : ViewModelBase
    {
        #region Commands

        #endregion

        #region Properties

        private Formatting _model;
        public Formatting Model
        {
            get { return _model; }
            set
            {
                if (value != null)
                    UpdateViewModel(value);
                if (_model == value)
                    return;

                _model = value;
                RaisePropertyChanged(() => Model);
            }
        }

        private string _fontFamily;
        public string FontFamily
        {
            get { return _fontFamily; }
            set
            {
                if (_fontFamily == value)
                    return;

                _fontFamily = value;
                RaisePropertyChanged(() => FontFamily);
            }
        }

        private string _fontSize;
        public string FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize == value)
                    return;

                _fontSize = value;
                RaisePropertyChanged(() => FontSize);
            }
        }

        private string _fontWeight;
        public string FontWeight
        {
            get { return _fontWeight; }
            set
            {
                if (_fontWeight == value)
                    return;

                _fontWeight = value;
                RaisePropertyChanged(() => FontWeight);
            }
        }

        private ObservableCollection<string> _fontFamilyList;
        public CollectionViewSource FontFamilies { get; set; }

        private ObservableCollection<string> _fontSizeList;
        public CollectionViewSource FontSizes { get; set; }

        private ObservableCollection<string> _fontWeightList;
        public CollectionViewSource FontWeights { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the SettingsFormatViewModel class.
        /// </summary>
        public SettingsFormatViewModel()
        {
            _fontFamilyList = new ObservableCollection<string>();
            _fontSizeList = new ObservableCollection<string>();
            _fontWeightList = new ObservableCollection<string>();

            _fontFamilyList.Add("Monofur");

            _fontSizeList.Add("Small");
            _fontSizeList.Add("Normal");
            _fontSizeList.Add("Medium");
            _fontSizeList.Add("MediumLarge");
            _fontSizeList.Add("Large");

            _fontWeightList.Add("Normal");
            _fontWeightList.Add("Bold");
            _fontWeightList.Add("Light");
            _fontWeightList.Add("Medium");
            _fontWeightList.Add("SemiBold");
            _fontWeightList.Add("Thin");

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

                FontFamily = "Monofur";
                FontSize = "Small";
                FontWeight = "Normal";
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                Load();
            }

            FontFamilies = new CollectionViewSource() { Source = _fontFamilyList };
            FontSizes = new CollectionViewSource() { Source = _fontSizeList };
            FontWeights = new CollectionViewSource() { Source = _fontWeightList };
        }

        private void Load()
        {
            var model = SettingsUnitOfWork.Default.Formatting.FindBy(x => x.Name == "default").FirstOrDefault();
            if (model != null)
                Model = model;
        }

        private void UpdateViewModel(Formatting model)
        {
            FontFamily = model.FontFamily;
            FontSize = model.FontSize;
            FontWeight = model.FontWeight;
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}