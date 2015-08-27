using System;
using Ninject;

namespace NRules.Integration.Ninject
{
    /// <summary>
    /// Dependency resolver that uses Ninject DI container.
    /// </summary>
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object Resolve(IResolutionContext context, Type serviceType)
        {
            return _kernel.Get(serviceType);
        }
    }
}
