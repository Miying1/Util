using System;
using System.Collections.Generic;
using System.IO;
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
        public string SolutionFileName { get; set; } = "../Util.sln";
        /// <summary>
        /// ��������
        /// </summary>
        [FromArg( "c|configuration" )]
        [BuildConfiguration]
        public string BuildConfiguration { get; set; } = "Release";
        /// <summary>
        /// Nuget���͵�ַ
        /// </summary>
        [FromArg( "nugetUrl" )]
        public string NugetUrl { get; set; } = "https://api.nuget.org/v3/index.json";
        /// <summary>
        /// Nuget��Կ
        /// </summary>
        [FromArg( "nugetKey", "Nuget api key for publishing nuget packages." )]
        public string NugetApiKey { get; set; }
        /// <summary>
        /// Դ����Ŀ¼
        /// </summary>
        public FullPath SourceDir => RootDirectory.CombineWith( "../src" );
        /// <summary>
        /// Դ����Ŀ¼
        /// </summary>
        public FullPath TestDir => RootDirectory.CombineWith( "../test" );
        /// <summary>
        /// ���Ŀ¼
        /// </summary>
        public FullPath OutputDir => RootDirectory.CombineWith( "../output" );
        /// <summary>
        /// ��Ŀ�ļ��б�
        /// </summary>
        public List<FileFullPath> Projects { get; set; }
        /// <summary>
        /// ������Ŀ�ļ��б�
        /// </summary>
        public List<FileFullPath> TestProjecs { get; set; }

        /// <summary>
        /// ����ǰ����
        /// </summary>
        /// <param name="context">��������������</param>
        protected override void BeforeBuildExecution( ITaskContext context ) {
            Projects = context.GetFiles( SourceDir, "*/*.csproj" );
            TestProjecs = context.GetFiles( TestDir, "*/*.csproj" );
        }

        /// <summary>
        /// ���ù���Ŀ��
        /// </summary>
        /// <param name="context">��������������</param>
        protected override void ConfigureTargets( ITaskContext context ) {
            var clean = Clean( context );
            var restore = Restore( context, clean );
            var build = Build( context, restore );
            var pack = Pack( context, clean );
            PublishNuGetPackage( context, pack );
            Test( context );
        }

        /// <summary>
        /// ����������
        /// </summary>
        private ITarget Clean( ITaskContext context ) {
            return context.CreateTarget( "clean" )
                .SetDescription( "Clean the solution." )
                .AddCoreTask( t => t.Clean().AddDirectoryToClean( OutputDir, false ) );
        }

        /// <summary>
        /// ��ԭ��
        /// </summary>
        private ITarget Restore( ITaskContext context, ITarget target ) {
            return context.CreateTarget( "restore" )
                .SetDescription( "Restore the solution." )
                .DependsOn( target )
                .AddCoreTask( t => t.Restore() );
        }

        /// <summary>
        /// ����������
        /// </summary>
        private ITarget Build( ITaskContext context, ITarget target ) {
            return context.CreateTarget( "compile" )
                .SetDescription( "Compiles the solution." )
                .DependsOn( target )
                .AddCoreTask( t => t.Build() );
        }

        /// <summary>
        /// ����nuget��
        /// </summary>
        private ITarget Pack( ITaskContext context, ITarget dependTarget ) {
            return context.CreateTarget( "pack" )
                .SetDescription( "Create nuget packages." )
                .DependsOn( dependTarget )
                .ForEach( Projects, ( project, target ) => {
                    target.AddCoreTask( t => t.Pack()
                        .Project( project )
                        .IncludeSymbols()
                        .OutputDirectory( OutputDir ) );
                } );
        }

        /// <summary>
        /// ����nuget��
        /// </summary>
        private void PublishNuGetPackage( ITaskContext context, ITarget dependTarget ) {
            context.CreateTarget( "nuget.publish" )
                .SetDescription( "Publishes nuget packages" )
                .DependsOn( dependTarget )
                .Do( t => {
                    var packages = Directory.GetFiles( OutputDir, "*.nupkg" );
                    foreach ( var package in packages ) {
                        if( package.EndsWith( "symbols.nupkg", StringComparison.OrdinalIgnoreCase ) )
                            continue;
                        context.CoreTasks().NugetPush( package )
                            .DoNotFailOnError( ex => { Console.WriteLine( $"Failed to publish {package}.exception: {ex.Message}" ); } )
                            .ServerUrl( NugetUrl )
                            .ApiKey( NugetApiKey )
                            .Execute( context );
                    }
                } );
        }

        /// <summary>
        /// ���в���
        /// </summary>
        private void Test( ITaskContext context ) {
            context.CreateTarget( "test" )
                .SetDescription( "Run all tests." )
                .ForEach( TestProjecs, ( project, target ) => {
                    target.AddCoreTask( t => t.Test().Project( project ) );
                } );
        }
    }
}