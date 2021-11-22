using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace Util.ObjectMapping.AutoMapper.Tests {
    /// <summary>
    /// ��������
    /// </summary>
    public class Startup {
        /// <summary>
        /// ���÷���
        /// </summary>
        public void ConfigureServices( IServiceCollection services ) {
            services.AddUtil();
        }

        /// <summary>
        /// ������־�ṩ����
        /// </summary>
        public void Configure( ILoggerFactory loggerFactory, ITestOutputHelperAccessor accessor ) {
            loggerFactory.AddProvider( new XunitTestOutputLoggerProvider( accessor ) );
        }
    }
}
