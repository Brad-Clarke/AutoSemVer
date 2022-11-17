using Microsoft.Extensions.DependencyInjection;

namespace ASV.Core.Detection.Factory
{
    public class ChangeDetectorFactory : IChangeDetectorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ChangeDetectorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IChangeDetector<T> Build<T>() where T : class
            => _serviceProvider.GetRequiredService<IChangeDetector<T>>();
    }
}
