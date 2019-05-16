using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Plugin {

    class PluginManager : IHost {
        string[] pluginPaths;
        Dictionary<string, IPlugin> pluginHandler = new Dictionary<string, IPlugin>();
        string[] IHost.Plugins => pluginHandler.Keys.ToArray();

        public PluginManager(string[] paths) {
            pluginPaths = paths;
        }
        public void LoadPlugins() {
            pluginHandler.Clear();
            foreach (var pluginPath in pluginPaths) {
                DirectoryInfo pluginDirectory = new DirectoryInfo(pluginPath);
                if (!pluginDirectory.Exists)
                    pluginDirectory.Create();
                var pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
                foreach (var file in pluginFiles) {
                    try {
                        Assembly asm = Assembly.LoadFrom(file);
                        var types = asm.GetTypes().
                                        Where(t => t.GetInterfaces().
                                        Where(i => i.FullName == typeof(IPlugin).FullName).Any());
                        foreach (var type in types) {
                            var plugin = asm.CreateInstance(type.FullName) as IPlugin;
                            pluginHandler.Add(plugin.Name, plugin);
                            plugin.Initialize(this);
                        }
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        public void InstallUI(StackPanel pnlPlugins) {
            foreach (var plugin in pluginHandler.Values) {
                var ui = plugin.GetUI();
                if (ui != null) {
                    pnlPlugins.Children.Add(ui);
                }
            }
        }

        public IPlugin getPlugin(string name) {
            if (pluginHandler.ContainsKey(name))
                return pluginHandler[name];
            return null;
        }
    }
}
