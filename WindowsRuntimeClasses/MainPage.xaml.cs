using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TreeViewControl;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsRuntimeClasses
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Type objectType = typeof(Object);
        ClassAndSubclasses objectClassAndSubclasses = new ClassAndSubclasses(typeof(Object));
        Dictionary<String, ClassAndSubclasses> classesDict = null;

        public MainPage()
        {
            this.InitializeComponent();

            Type type = typeof(Windows.UI.Xaml.DependencyObject);
            DisplayAssembly(type.GetTypeInfo().Assembly);
        }

        private void AddToSubclasses(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            String typeNmae = typeInfo.FullName;
            if (classesDict.ContainsKey(typeNmae))
            {
                return;
            }

            Type baseType = type.GetTypeInfo().BaseType;
            String baseTypeNmae = baseType.GetTypeInfo().FullName;
            if (!classesDict.ContainsKey(baseTypeNmae))
            {
                AddToSubclasses(baseType);
            }
            ClassAndSubclasses parent = classesDict[baseTypeNmae];
            ClassAndSubclasses child = new ClassAndSubclasses(type);
            parent.Subclasses.Add(child);
            classesDict.Add(typeNmae, child);
        }

        private TreeNode AddToTree(ClassAndSubclasses classAndSubclasses)
        {
            TreeNode treeNode = new TreeNode();
            foreach (ClassAndSubclasses c in classAndSubclasses.Subclasses)
            {
                treeNode.Add(AddToTree(c));
            }
            TypeInfo typeInfo = classAndSubclasses.Type.GetTypeInfo();
            string name = String.Format("{0} ({1})", typeInfo.FullName, treeNode.Count);
            treeNode.Data = new TreeNodeData(name) { IsFolder = treeNode.Count > 0 };
            return treeNode;
        }

        private void DisplayAssembly(Assembly assembly)
        {
            classesDict = new Dictionary<string, ClassAndSubclasses>();
            classesDict.Add(objectType.GetTypeInfo().FullName, objectClassAndSubclasses);
            foreach (Type type in assembly.ExportedTypes)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo.Name.Contains("DependencyObject"))
                {
                    int i = 0;
                    i++;
                }
                if (typeInfo.IsClass)
                {
                    AddToSubclasses(type);
                }
            }
            TreeNode treeNode = AddToTree(objectClassAndSubclasses);
            sampleTreeView.RootNode.Add(treeNode);
            classesDict = null;
        }
    }
}
