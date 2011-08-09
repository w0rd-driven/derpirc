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
            var entity = item as Data.ChannelItem;
            if (entity != null)
            {
                switch (entity.Type)
                {
                    case derpirc.Data.MessageType.Mine:
                        return TemplateMyMessage;
                    case derpirc.Data.MessageType.Theirs:
                        return TemplateTheirMessage;
                    default:
                        break;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
