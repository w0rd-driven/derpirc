using System.Windows;
using derpirc.Data.Models;

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
            var entity = item as IMessageItem;
            if (entity != null)
            {
                switch (entity.Owner)
                {
                    case Owner.Me:
                        return TemplateMyMessage;
                    case Owner.Them:
                        return TemplateTheirMessage;
                    default:
                        break;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
