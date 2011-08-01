using System.Windows;
using System.Windows.Controls;

namespace derpirc.Helpers
{
    public abstract class DataTemplateSelector : ContentControl
    {
        public DataTemplateSelector()
        {
            // Set the ContentControl to stretch, otherwise everything is centered
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            this.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
        }

        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }
}
