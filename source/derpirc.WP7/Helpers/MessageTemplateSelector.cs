using System.Windows;

namespace derpirc.Helpers
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TemplateMyMessage
        {
            get;
            set;
        }

        public DataTemplate TemplateTheirMessage
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var entity = item as Data.ChannelMessage;
            if (entity != null)
            {
                if (entity.Source == "w0rd-driven")
                    return TemplateMyMessage;
                else
                    return TemplateTheirMessage;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
