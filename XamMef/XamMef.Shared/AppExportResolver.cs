using System;
using System.Collections.Generic;
using System.Reflection;
using Acr.UserDialogs;
using MFractor.IOC;
using XamMef.Shared;

[assembly: DeclareExportResolver(typeof(AppExportResolver))]

namespace XamMef.Shared
{
    public class AppExportResolver : AssemblyCompositionExportResolver
    {
        public AppExportResolver()
        {
        }

        public override IEnumerable<Assembly> Assemblies
        {
            get
            {
                return new List<Assembly>()
                {
                    this.GetType().Assembly,
                    typeof(App).Assembly,
                };
            }
        }

        protected override void RegisterExternalParts(IExternalPartRegistrar registrar)
        {
            registrar.RegisterSingleton(() => UserDialogs.Instance);
        }
    }
}
