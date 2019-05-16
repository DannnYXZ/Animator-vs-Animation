using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin {
    public interface IHost {
        string[] Plugins { get; }
        IPlugin getPlugin(string name);
    }
}
