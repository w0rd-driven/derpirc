using System;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    public abstract class ViewModelFactory
    {
        protected static IViewModelResolver resolver = null;
        public static void InitializeResolver(IViewModelResolver resolver)
        {
            // This is where we set the resolver for the ViewModelFactory.
            // This should get set in the Bootstrapper (Composition Root)
            // to the resolver implementation using the
            // IoC container that has been chosen for the application.
            ViewModelFactory.resolver = resolver;
        }
    }

    public abstract class ViewModelFactory<TViewModel, TDesignViewModel> : ViewModelFactory
        where TViewModel : ViewModelBase
        where TDesignViewModel : TViewModel, new()
    {
        public ViewModelFactory()
        {
        }

        public TViewModel ViewModel
        {
            get
            {
                TViewModel viewModel;
                bool designMode = ViewModelBase.IsInDesignModeStatic;

                if (designMode)
                {
                    viewModel = new TDesignViewModel();
                }
                else
                {
                    if (resolver == null)
                    {
                        throw new InvalidOperationException();
                    }

                    // Use the resolver implementation to resolve
                    // the view model requested.
                    viewModel = (TViewModel)resolver.Resolve(typeof(TViewModel));
                }

                //viewModel.OnLocated();
                return viewModel;
            }
        }
    }
}
