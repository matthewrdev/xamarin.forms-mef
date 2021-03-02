using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using XamMef;

namespace MFractor.IOC
{
    /// <summary>
    /// The <see cref="Resolver"/> is used to resolve implementations of interfaces, abstract classes and types.
    /// <para/>
    /// Under the hood, the <see cref="Resolver"/> uses Managed Extensibility Framework (MEF). See: https://docs.microsoft.com/en-us/dotnet/framework/mef/
    /// <para/>
    /// To become availabe for resolution using the <see cref="Resolver"/>, parts should be exported into MEF using the <see cref="ExportAttribute"/>.
    /// <para/>
    /// To gain access to other services, use the <see cref="ImportingConstructorAttribute"/> for constructor injection and the <see cref="ImportAttribute"/> for property injection.
    /// <para/>
    /// When using constructor injection, always prefer the use of <see cref="Lazy{T}"/> to defer the resolution of systems. This significantly improves performance by 'circuit breaking' the dependency graph resolution and deferring the final resolution of an element until it's actually used. <see cref="Lazy{T}"/> also allows the safe use of circular references between parts.
    /// <para/>
    /// If possible, please use the Resolve methods as a last resort only. They create technical debt by violating the IOC and DI principles and are difficult to unit test.
    /// <para/>
    /// For objects that cannot be exported to MEF, such as controls or IDE integration points, you can mark properties with the <see cref="ImportAttribute"/> and then call <see cref="ComposeParts(object)"/> to trigger MFractor to resolve each property.
    /// </summary>
    public static class Resolver
    {
        static readonly Logging.ILogger log = Logging.Logger.Create();

#pragma warning disable IDE0044 // Add readonly modifier
        /// <summary>
        /// The underlying <see cref="IExportResolver"/> that <see cref="ExportResolver"/> uses to create the export resolver.
        /// <para/>
        /// The <see cref="exportResolver"/> is mutable to allow overloading for unit testing.
        /// </summary>
        internal static Lazy<IExportResolver> exportResolver = new Lazy<IExportResolver>(() =>
#pragma warning restore IDE0044 // Add readonly modifier
        {
            using (Profiler.Profile("Locate export resolver"))
            {
                var candiateAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                                        .Where(a => a.GetCustomAttributes(typeof(DeclareExportResolverAttribute), true).Any())
                                                        .ToList();

                if (candiateAssemblies == null || !candiateAssemblies.Any())
                {
                   var message = $"No assemblies in the AppDomain have a {nameof(DeclareExportResolverAttribute)} defined to declare the backing export resolver. MFractor cannot continue without an IExportResolver implementation.";
                   log?.Error(message);
                   throw new InvalidOperationException(message);
                }

               var assembly = candiateAssemblies.FirstOrDefault();
               if (candiateAssemblies.Count > 1)
               {
                   var message = $"Multiple assemblies in the AppDomain have a {nameof(DeclareExportResolverAttribute)} defined to declare the backing export resolver. MFractor will use the first assembly.";
                   log?.Warning(message);

                   log?.Warning("The following assemblies define an export resolver: " + string.Join(", ", candiateAssemblies.Select(ca => ca.FullName)));

                   assembly = candiateAssemblies.FirstOrDefault(ca => ca.GetName().Name.StartsWith("MFractor", StringComparison.Ordinal)) ?? candiateAssemblies.FirstOrDefault();
               }

               log?.Info("Using " + assembly.FullName + " to create MFractors export resolver.");

                var attribute = (DeclareExportResolverAttribute)assembly.GetCustomAttributes(typeof(DeclareExportResolverAttribute), true).FirstOrDefault();

                var resolver =  (IExportResolver)Activator.CreateInstance(attribute.ExportResolverType);

               resolver.Prepare();

               return resolver;
            }
       });

        /// <summary>
        /// The backing <see cref="IExportResolver"/> that provides parts for the resolver.
        /// <para/>
        /// </summary>
        /// <value>The export resolver.</value>
        public static IExportResolver ExportResolver => exportResolver.Value;

        /// <summary>
        /// Resolves an instance of the provided type.
        /// </summary>
        /// <returns>The resolve.</returns>
        /// <param name="type">Type.</param>
        public static object Resolve(Type type)
        {
            if (!VerifyResolver())
            {
                return default;
            }

            return ExportResolver.GetExportedValue(type);
        }

        /// <summary>
        /// Resolve an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The resolve.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Resolve<T>() where T : class
        {
            if (!VerifyResolver())
            {
                return default;
            }

            return ExportResolver.GetExportedValue<T>();
        }

        /// <summary>
        /// Resolves <typeparamref name="TType"/>, cast as <typeparamref name="TCastType"/>.
        /// </summary>
        /// <returns>The resolve.</returns>
        /// <typeparam name="TType">The 1st type parameter.</typeparam>
        /// <typeparam name="TCastType">The 2nd type parameter.</typeparam>
        public static TCastType Resolve<TType, TCastType>() where TType : class
                                                            where TCastType : class, TType
        {
            if (!VerifyResolver())
            {
                return default;
            }

            var result = ExportResolver.GetExportedValue<TType>();
            return result as TCastType;
        }

        /// <summary>
        /// Resolve all exported implementations of <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The all.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            if (!VerifyResolver())
            {
                return Enumerable.Empty<T>();
            }

            return ExportResolver.GetExportedValues<T>();
        }

        /// <summary>
        /// Gathers the MEF parts to be imported via the <see cref="System.ComponentModel.Composition.ImportAttribute"/> and applies them to the provided <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        public static void ComposeParts(object instance)
        {
            if (!VerifyResolver())
            {
                return;
            }

            ExportResolver.ComposeParts(instance);
        }

        /// <summary>
        /// Verifies that the underlying <see cref="IExportResolver"/> is available for the <see cref="Resolver"/>.
        /// <para/>
        /// If <see cref="VerifyResolver"/> returns false, it indicates an error in the export resolver and all resolve methods will fail.
        /// </summary>
        /// <returns></returns>
        public static bool VerifyResolver()
        {
            if (exportResolver == null)
            {
                log?.Warning("No export resolver defined");
                return false;
            }

            try
            {
                if (ExportResolver == null)
                {
                    log?.Warning("No export resolver defined");
                    return false;
                }
            }
            catch (Exception ex)
            {
                log?.Exception(ex);
                return false;
            }

            return true;
        }
    }
}
