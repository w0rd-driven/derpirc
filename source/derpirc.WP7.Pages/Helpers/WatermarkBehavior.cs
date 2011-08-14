using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace derpirc.Helpers
{
    // Also includes UpdateOnTextChangedBehavior
    public class WatermarkBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
                   "Watermark",
                   typeof(string),
                   typeof(WatermarkBehavior),
                   new PropertyMetadata("Enter text here ..."));

        [Description("Gets or sets the watermark text")]
        public string Watermark
        {
            get { return GetValue(WatermarkProperty) as string; }
            set { SetValue(WatermarkProperty, value); }
        }

        public Brush Foreground { get; set; }

        private bool _hasWatermark;
        private Brush _textBoxForeground;

        protected override void OnAttached()
        {
            _textBoxForeground = AssociatedObject.Foreground;

            base.OnAttached();
            if (Watermark != null)
                SetWatermarkText();
            AssociatedObject.TextChanged += TextChanged;
            AssociatedObject.GotFocus += GotFocus;
            AssociatedObject.LostFocus += LostFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= TextChanged;
            AssociatedObject.GotFocus -= GotFocus;
            AssociatedObject.LostFocus -= LostFocus;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
                binding.UpdateSource();
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.Text.Length == 0)
                if (Watermark != null)
                    SetWatermarkText();
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (_hasWatermark)
                RemoveWatermarkText();
        }

        private void RemoveWatermarkText()
        {
            AssociatedObject.Foreground = _textBoxForeground;
            AssociatedObject.Text = string.Empty;
            _hasWatermark = false;
        }

        private void SetWatermarkText()
        {
            AssociatedObject.Foreground = Foreground;
            AssociatedObject.Text = Watermark;
            _hasWatermark = true;
        }
    }
}
