using System;
using System.ComponentModel;

namespace derpirc.Data
{
    public interface IEntity : INotifyPropertyChanging, INotifyPropertyChanged
    {
        int Id { get; }
    }
}
