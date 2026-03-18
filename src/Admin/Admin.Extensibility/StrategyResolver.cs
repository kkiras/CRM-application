using Admin.Common;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Admin.Extensibility
{
    public class StrategyResolver
    {
        public IAfterCommitStrategy Resolve(string strategyName)
        {
            string pluginsDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");

            if (!Directory.Exists(pluginsDirectory))
            {
                Console.WriteLine("Plugins folder not found.");
                return null;
            }

            foreach (var dll in Directory.GetFiles(pluginsDirectory, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);

                    var strategyType = assembly.GetExportedTypes()
                        .FirstOrDefault(t =>
                            typeof(IAfterCommitStrategy).IsAssignableFrom(t) &&
                            !t.IsInterface &&
                            !t.IsAbstract);

                    if (strategyType == null)
                        continue;

                    var instance = Activator.CreateInstance(strategyType) as IAfterCommitStrategy;

                    if (instance != null && instance.Name == strategyName)
                    {
                        return instance;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Load plugin failed: {dll}. Error: {ex.Message}");
                }
            }

            return null;
        }
    }
}