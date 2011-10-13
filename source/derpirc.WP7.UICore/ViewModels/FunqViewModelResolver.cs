using System;
using System.Collections.Generic;
using System.Reflection;
using Funq;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    public class FunqViewModelResolver : IViewModelResolver
    {
        private readonly Container container;
        private Dictionary<Type, MethodInfo> methods = null;
        private MethodInfo method = null;

        public FunqViewModelResolver(Container container)
        {
            this.container = container;

            method = container.GetType().GetMethod("Resolve", new Type[0]);

            methods = new Dictionary<Type, MethodInfo>();
        }

        public ViewModelBase Resolve(Type viewModelType)
        {
            MethodInfo genericMethod = null;
            if (!methods.TryGetValue(viewModelType, out genericMethod))
            {
                genericMethod = method.MakeGenericMethod(viewModelType);
                methods.Add(viewModelType, genericMethod);
            }

            var result = genericMethod.Invoke(container, null);
            return (ViewModelBase)result;
        }
    }
}
