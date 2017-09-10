using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsRuntimeClasses
{
    class TreeNodeData
    {
        public TreeNodeData(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public bool IsFolder { get; set; }
    }
}
