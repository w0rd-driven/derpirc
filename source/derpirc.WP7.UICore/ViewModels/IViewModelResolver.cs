using GalaSoft.MvvmLight;
using System;

namespace derpirc.ViewModels
{
    public interface IViewModelResolver
    {
        ViewModelBase Resolve(Type viewModelType);
    }
}
