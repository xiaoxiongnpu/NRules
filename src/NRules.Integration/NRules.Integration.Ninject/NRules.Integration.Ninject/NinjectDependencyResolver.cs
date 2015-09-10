using System;
using Ninject;
using Ninject.Syntax;

namespace NRules.Integration.Ninject
{
    /// <summary>
    /// Dependency resolver that uses Ninject DI container.
    /// </summary>
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IResolutionRoot _resolutionRoot;

        public NinjectDependencyResolver(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public object Resolve(IResolutionContext context, Type serviceType)
        {
            return _resolutionRoot.Get(serviceType);
        }
    }
}
