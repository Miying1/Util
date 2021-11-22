using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Util.Applications.UnitOfWorks;
using Util.Data.EntityFrameworkCore;
using Util.Helpers;
using Util.Http;
using Xunit.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace Util.Applications {
    /// <summary>
    /// ��������
    /// </summary>
    public class Startup {
        /// <summary>
        /// ��������
        /// </summary>
        public void ConfigureHost( IHostBuilder hostBuilder ) {
            hostBuilder.ConfigureDefaults( null ).ConfigureWebHostDefaults( webHostBuilder => {
                webHostBuilder.UseTestServer()
                    .UseEnvironment( "Development" )
                    .Configure( t => {
                        t.UseRouting();
                        t.UseEndpoints( endpoints => {
                            endpoints.MapControllers();
                        } );
                    } );
            } );
        }

        /// <summary>
        /// ���÷���
        /// </summary>
        public void ConfigureServices( IServiceCollection services ) {
            services.AddControllers();
            services.AddTransient<IHttpClient>( t => {
                var client = new HttpClientService();
                client.SetHttpClient( t.GetService<IHost>().GetTestClient() );
                return client;
            } );
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
            loggerFactory.AddProvider( new XunitTestOutputLoggerProvider( accessor ) );
        }
    }
}
