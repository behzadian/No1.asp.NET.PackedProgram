using Autofac;
using Autofac.Extensions.DependencyInjection;
using No1.FaraBank.Api.Contracts;
using No1.FaraBank.Api.Services;
using System.Reflection;

namespace No1.FaraBank.Api.Config;

internal static class ProgramAutofacExtensions
{
	internal static void ConfigureAutofac(this WebApplicationBuilder builder) {
		var thisAssembly = Assembly.GetExecutingAssembly();
		builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
		builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => {
			containerBuilder.RegisterType<ClockService>().AsSelf().As<IClockContract>().InstancePerLifetimeScope();
			foreach (var config in thisAssembly.GetTypes().Where(t => t.Name.EndsWith("Config") && !t.IsAbstract && !t.IsInterface && t.IsClass)) {
				// When registering via a non-generic factory Autofac infers the implementation type as object,
				// so use As(config) to register the service under the actual config Type so it can be resolved.
				containerBuilder.Register(_ => builder.Configuration.GetSection(config.Name).Get(config)!).As(config).SingleInstance();
			}

			containerBuilder.RegisterAssemblyTypes(thisAssembly).Where(t => t.Name.EndsWith("Repo")).AsSelf();
			containerBuilder.RegisterAssemblyTypes(thisAssembly).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces();
		});
	}
}