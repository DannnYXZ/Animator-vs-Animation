using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Plugin {
    public interface IPlugin {
        void SetArgument(string key, object value);
        void Initialize(IHost host);
        string Name { get; }
        string Description { get; }
        FrameworkElement GetUI();
        object Execute(object obj);
    }
}
