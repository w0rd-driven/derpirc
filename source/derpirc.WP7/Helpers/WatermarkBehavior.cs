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

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
                   "Foreground",
                   typeof(Brush),
                   typeof(WatermarkBehavior),
                   new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        [Description("Gets or sets the watermark foreground brush")]
        public Brush Foreground
        {
            get { return GetValue(ForegroundProperty) as Brush; }
            set { SetValue(ForegroundProperty, value); }
        }

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
