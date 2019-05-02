﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin {
    interface IPlugin {
        string Name { get; }
        string Description { get; }
        object Execute(object obj);
    }
}
