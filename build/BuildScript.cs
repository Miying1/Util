using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Context.Attributes.BuildProperties;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.IO;
using FlubuCore.Scripting;

namespace Build {
    /// <summary>
    /// �����ű�
    /// </summary>
    public class BuildScript : DefaultBuildScript {
        /// <summary>
        /// ��������ļ���
        /// </summary>
        [SolutionFileName]
        public string SolutionFileName { get; set; } = "Util.sln";
        /// <summary>
        /// ��������
        /// </summary>
        [BuildConfiguration]
        public string BuildConfiguration { get; set; } = "Release";
        /// <summary>
        /// Դ����Ŀ¼
        /// </summary>
        public FullPath SourceDir => RootDirectory.CombineWith( "src" );
        /// <summary>
        /// Դ����Ŀ¼
        /// </summary>
        public FullPath TestDir => RootDirectory.CombineWith( "test" );
        /// <summary>
        /// ���Ŀ¼
        /// </summary>
        public FullPath OutputDir => RootDirectory.CombineWith( "output" );
        /// <summary>
        /// ��Ŀ�ļ��б�
        /// </summary>
        public List<FileFullPath> ProjectFiles { get; set; }
        /// <summary>
        /// ������Ŀ�ļ��б�
        /// </summary>
        public List<FileFullPath> TestProjectFiles { get; set; }

        /// <summary>
        /// ����ǰ����
        /// </summary>
        /// <param name="context">��������������</param>
        protected override void BeforeBuildExecution( ITaskContext context ) {
            ProjectFiles = context.GetFiles( SourceDir, "*/*.csproj" );
            TestProjectFiles = context.GetFiles( TestDir, "*/*.csproj" );
        }

        /// <summary>
        /// ���ù���Ŀ��
        /// </summary>
        /// <param name="context">��������������</param>
        protected override void ConfigureTargets( ITaskContext context ) {
            var clean = CreateCleanTarget( context );
            var restore = CreateRestoreTarget( context, clean );
            var build = CreateBuildTarget( context, restore );
            var test = CreateTestTarget( context );
        }

        /// <summary>
        /// ����������
        /// </summary>
        private ITarget CreateCleanTarget( ITaskContext context ) {
            return context.CreateTarget( "Clean" )
                .SetDescription( "Clean the solution." )
                .AddCoreTask( t => t.Clean().Configuration( "Debug" ).CleanOutputDir() )
                .AddCoreTask( t => t.Clean().Configuration( "Release" ).CleanOutputDir() );
        }

        /// <summary>
        /// ��������ԭ����
        /// </summary>
        private ITarget CreateRestoreTarget( ITaskContext context, ITarget target ) {
            return context.CreateTarget( "Restore" )
                .SetDescription( "Restore the solution." )
                .DependsOn( target )
                .AddCoreTask( t => t.Restore() );
        }

        /// <summary>
        /// ������������
        /// </summary>
        private ITarget CreateBuildTarget( ITaskContext context, ITarget target ) {
            return context.CreateTarget( "Build" )
                .SetDescription( "Build the solution." )
                .DependsOn( target )
                .AddCoreTask( t => t.Build() );
        }

        /// <summary>
        /// �������Բ���
        /// </summary>
        private ITarget CreateTestTarget( ITaskContext context ) {
            return context.CreateTarget( "Test" )
                .SetDescription( "Run all tests." )
                .ForEach( TestProjectFiles, ( project, target ) => {
                    target.AddCoreTask( t => t.Test().Project( project ) );
                } );
        }
    }
}