using System;
using Ninject;
using Ninject.Modules;
using Ninject.Syntax;
using NRules.Fluent;
using NRules.RuleModel;
using IContext = Ninject.Activation.IContext;

namespace NRules.Integration.Ninject
{
    public class NRulesNinjectModule : NinjectModule
    {
        public NRulesNinjectModule()
        {
            RuleScannerAction = s => s.Assembly(AppDomain.CurrentDomain.GetAssemblies());
        }

        public Action<IRuleTypeScanner> RuleScannerAction { get; set; }

        public override void Load()
        {
            Bind<IRuleActivator>().To<NinjectRuleActivator>();
            Bind<IDependencyResolver>().To<NinjectDependencyResolver>();

            ConfigureRuleRepository(Bind<IRuleRepository>());
            ConfigureSessionFactory(Bind<ISessionFactory>());
            ConfigureSession(Bind<ISession>());
        }

        protected virtual void ConfigureRuleRepository(IBindingToSyntax<IRuleRepository> bind)
        {
            bind.ToMethod(CreateRuleRepository)
                .InSingletonScope();
        }

        protected virtual void ConfigureSessionFactory(IBindingToSyntax<ISessionFactory> bind)
        {
            bind.ToMethod(c => CreateSessionFactory(c, c.Kernel.Get<IRuleRepository>()))
                .InSingletonScope();
        }

        protected virtual void ConfigureSession(IBindingToSyntax<ISession> bind)
        {
            bind.ToMethod(c => c.Kernel.Get<ISessionFactory>().CreateSession())
                .InTransientScope();
        }

        private RuleRepository CreateRuleRepository(IContext context)
        {
            var scanner = new RuleTypeScanner();
            RuleScannerAction(scanner);
            var ruleTypes = scanner.GetRuleTypes();
            foreach (var ruleType in ruleTypes)
            {
                Bind(ruleType).To(ruleType);
            }

            var repository = new RuleRepository();
            repository.Activator = context.Kernel.Get<IRuleActivator>();
            repository.Load(x => x.From(ruleTypes));
            return repository;
        }

        private ISessionFactory CreateSessionFactory(IContext context, IRuleRepository repository)
        {
            var factory = repository.Compile();
            factory.DependencyResolver = context.Kernel.Get<IDependencyResolver>();
            return factory;
        }
    }
}
