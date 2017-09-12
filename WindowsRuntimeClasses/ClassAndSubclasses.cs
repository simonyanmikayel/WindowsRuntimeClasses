using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace WindowsRuntimeClasses
{
    class ClassAndSubclasses
    {
        public ClassAndSubclasses(Type type)
        {
            this.Type = type;
            this.Subclasses = new List<ClassAndSubclasses>();
        }

        public Type Type { protected set; get; }
        public List<ClassAndSubclasses> Subclasses { protected set; get; }

        public void Sort()
        {
            Subclasses.Sort((t1, t2) =>
            {
                return String.Compare(t1.Type.GetTypeInfo().Name, t2.Type.GetTypeInfo().Name);
            });
        }

        public void Find(Type type)
        {
            ClassAndSubclasses c = Subclasses.Find(x => x.Type == type);
        }
    }
}
