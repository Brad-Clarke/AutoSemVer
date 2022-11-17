using ASV.Core.Detection.Detectors;
using ASV.Core.Detection.Factory;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace ASV.Core.Detection
{
    public static class ChangeDetectionServiceCollection
    {
        public static IServiceCollection AddChangeDetection(this IServiceCollection services)
        {
            services.AddScoped<IChangeDetectorFactory>(p => new ChangeDetectorFactory(p));

            services.AddScoped<IChangeDetector<Assembly>, AssemblyChangeDetector>();
            services.AddScoped<IChangeDetector<ConstructorInfo>, ConstructorChangeDetector>();
            services.AddScoped<IChangeDetector<EventInfo>, EventChangeDetector>();
            services.AddScoped<IChangeDetector<FieldInfo>, FieldChangeDetector>();
            services.AddScoped<IChangeDetector<MethodInfo>, MethodChangeDetector>();
            services.AddScoped<IChangeDetector<PropertyInfo>, PropertyChangeDetector>();
            services.AddScoped<IChangeDetector<ParameterInfo>, ParameterChangeDetector>();
            services.AddScoped<IChangeDetector<Type>, TypeChangeDetector>();

            return services;
        }
    }
}
