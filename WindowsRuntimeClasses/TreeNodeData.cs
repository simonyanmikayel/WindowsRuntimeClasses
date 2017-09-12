using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeViewControl;

namespace WindowsRuntimeClasses
{
    class TreeNodeData
    {
        public TreeNodeData()
        {
        }

        public string Name { set; get; }

        public bool IsFolder { get; set; }

        public TreeNode TreeNode { get; set; }

        public ClassAndSubclasses NodeClasses { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
