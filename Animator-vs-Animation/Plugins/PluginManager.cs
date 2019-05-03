using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plugin {

    class PluginManager {
        string PluginPath { get; set; }
        private List<IPlugin> plugins = new List<IPlugin>();
        public PluginManager(string pluginPath) {
            PluginPath = pluginPath;
        }
        public void ReloadPlugins() {
            plugins.Clear();
            DirectoryInfo pluginDirectory = new DirectoryInfo(PluginPath);
            if (!pluginDirectory.Exists)
                pluginDirectory.Create();
            var pluginFiles = Directory.GetFiles(PluginPath, "*.dll");
            foreach (var file in pluginFiles) {
                try {
                    Assembly asm = Assembly.LoadFrom(file);
                    var types = asm.GetTypes().
                                    Where(t => t.GetInterfaces().
                                    Where(i => i.FullName == typeof(IPlugin).FullName).Any());
                    foreach (var type in types) {
                        var plugin = asm.CreateInstance(type.FullName) as IPlugin;
                        plugins.Add(plugin);
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public IPlugin GetPlugin(string name) {
            foreach (var plugin in plugins) {
                if (plugin.Name == name)
                    return plugin;
            }
            return null;
        }
    }
}
