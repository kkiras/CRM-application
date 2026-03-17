using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Admin.Extensibility
{
    public class Hook
    {
        public void CreateHook(
            [CallerMemberName] string methodName = null,
            string className = null,
            object[] parameters = null)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                var stackTrace = new StackTrace();
                className = stackTrace.GetFrame(1)?.GetMethod()?.DeclaringType?.Name;
            }

            if (string.IsNullOrWhiteSpace(methodName) || string.IsNullOrWhiteSpace(className))
            {
                throw new ArgumentException("className and methodName cannot be null or empty");
            }

            string pluginsDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");
            string libraryFullPath = Path.Combine(pluginsDirectory, $"{className}Extended.dll");

            if (!File.Exists(libraryFullPath))
            {
                Console.WriteLine($"Hook info: Plugin not found: {libraryFullPath}");
                return;
            }

            ResolveEventHandler resolver = (sender, args) =>
            {
                string assemblyName = new AssemblyName(args.Name).Name + ".dll";
                string dependencyPath = Path.Combine(pluginsDirectory, assemblyName);

                if (File.Exists(dependencyPath))
                {
                    return Assembly.LoadFrom(dependencyPath);
                }

                return null;
            };

            AppDomain.CurrentDomain.AssemblyResolve += resolver;

            try
            {
                var library = Assembly.LoadFrom(libraryFullPath);

                foreach (Type type in library.GetExportedTypes())
                {
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    var method = type.GetMethod(
                        methodName,
                        BindingFlags.Public | BindingFlags.Instance);

                    if (method == null)
                        continue;

                    var methodParams = method.GetParameters();
                    int providedCount = parameters?.Length ?? 0;

                    if (methodParams.Length != providedCount)
                        continue;

                    var instance = Activator.CreateInstance(type);
                    if (instance == null)
                        continue;

                    method.Invoke(instance, parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hook error: {ex}");
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= resolver;
            }
        }
    }
}