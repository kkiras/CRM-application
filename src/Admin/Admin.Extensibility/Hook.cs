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
                    // Bỏ qua interface và abstract class (không thể khởi tạo instance)
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    // Tìm method có tên = methodName (vd: "After") 
                    // Chỉ lấy method public instance
                    var method = type.GetMethod(
                        methodName,
                        BindingFlags.Public | BindingFlags.Instance);

                    // Nếu class này không có method cần gọi → bỏ qua
                    if (method == null)
                        continue;

                    // Lấy danh sách tham số của method
                    var methodParams = method.GetParameters();
                    // Số lượng tham số được truyền vào từ Hook
                    int providedCount = parameters?.Length ?? 0;

                    // Nếu số lượng param không khớp → bỏ qua 
                    // (tránh gọi nhầm overload hoặc sai signature)
                    if (methodParams.Length != providedCount)
                        continue;

                    // Tạo instance của class (vd: new SendEmailToCustomer())
                    var instance = Activator.CreateInstance(type);
                    // Nếu tạo instance thất bại → bỏ qua
                    if (instance == null)
                        continue;

                    // Gọi method thông qua reflection 
                    // tương đương: instance.After(parameters...)
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