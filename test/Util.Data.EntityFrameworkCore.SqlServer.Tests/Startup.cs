using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Util.Aop;
using Util.Data.EntityFrameworkCore.Infrastructure;
using Util.Data.EntityFrameworkCore.UnitOfWorks;
using Util.Helpers;
using Util.Sessions;
using Xunit.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace Util.Data.EntityFrameworkCore {
    /// <summary>
    /// ��������
    /// </summary>
    public class Startup {
        /// <summary>
        /// ��������
        /// </summary>
        public void ConfigureHost( IHostBuilder hostBuilder ) {
            hostBuilder.ConfigureDefaults( null ).UseEnvironment( "Development" ).EnableAop();
        }

        /// <summary>
        /// ���÷���
        /// </summary>
        public void ConfigureServices( IServiceCollection services ) {
            services.AddSingleton<ISession, TestSession>();
            services.AddUtil( options => {
                options.UseSqlServerUnitOfWork<ISqlServerUnitOfWork, SqlServerUnitOfWork>( Config.GetConnectionString( "connection" ) );
            } );
            InitDatabase( services );
        }

        /// <summary>
        /// ��ʼ�����ݿ�
        /// </summary>
        private void InitDatabase( IServiceCollection services ) {
            var unitOfWork = services.BuildServiceProvider().GetService<ISqlServerUnitOfWork>();
            unitOfWork.EnsureDeleted();
            unitOfWork.EnsureCreated();
        }

        /// <summary>
        /// ������־�ṩ����
        /// </summary>
        public void Configure( ILoggerFactory loggerFactory, ITestOutputHelperAccessor accessor ) {
            loggerFactory.AddProvider( new XunitTestOutputLoggerProvider( accessor, ( s, logLevel ) => logLevel >= LogLevel.Trace ) );
        }
    }
}
