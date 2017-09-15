using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeViewControl;
using System.Reflection;

namespace WindowsRuntimeClasses
{
    class NodeData
    {
        public NodeData(TypeInfo typeInfo)
        {
            this.TypeInfo = typeInfo;
            this.SubNodes = new List<NodeData>();
        }

        public string Name { set; get; }

        public bool IsFolder { get; set; }

        public TreeNode TreeNode { get; set; }

        public TypeInfo TypeInfo { protected set; get; }

        public List<NodeData> SubNodes { protected set; get; }

        public void Sort()
        {
            SubNodes.Sort((t1, t2) =>
            {
                return String.Compare(t1.TypeInfo.Name, t2.TypeInfo.Name);
            });
        }

        public void Find(Type type)
        {
            //NodeData c = SubNodes.Find(x => x.Type == type);
        }

        public override string ToString()
        {
            return this.TypeInfo.Name;
        }
    }
}
