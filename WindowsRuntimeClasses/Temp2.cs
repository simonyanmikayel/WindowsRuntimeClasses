using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TreeViewControl;

namespace WindowsRuntimeClasses
{
    class Temp2
    {
        private static void SetNodeData(TreeNode treeNode, string format, params object[] param)
        {
            string name = String.Format(format, param);
            TreeNodeData treeNodeData = treeNode.Data as TreeNodeData;
            if (treeNodeData == null)
                treeNode.Data = new TreeNodeData(name);
            else
                treeNodeData.Name = name;
        }

        private static TreeNode CreateFileNode(string format, params object[] param)
        {
            string name = String.Format(format, param);
            return new TreeNode() { Data = new TreeNodeData(name) };
        }

        private static TreeNode CreateFolderNode(string format, params object[] param)
        {
            string name = String.Format(format, param);
            return new TreeNode() { Data = new TreeNodeData(name) { IsFolder = true } };
        }

        private async void DisplayAssembleas()
        {
            //TreeNode workFolder = CreateFolderNode("Work Documents");
            //for (int i = 0; i < 10; i++)
            //{
            //    TreeNode workFolder1 = CreateFolderNode(String.Format("Node {0}", i));
            //    for (int j = 0; j < 10; j++)
            //    {
            //        TreeNode workFolder2 = CreateFolderNode(String.Format("Node {0}-{1}", i, j));
            //        for (int k = 0; k < 10; k++)
            //        {
            //            TreeNode workFolder3 = CreateFileNode(String.Format("Node {0}-{1}-{2}", i, j, k));
            //            workFolder2.Add(workFolder3);
            //        }
            //        workFolder1.Add(workFolder2);
            //    }
            //    workFolder.Add(workFolder1);
            //}
            //sampleTreeView.RootNode.Add(workFolder);

            Type type = typeof(Windows.UI.Xaml.DependencyObject);
            DisplayAssembly(type.GetTypeInfo().Assembly);

            List<Assembly> assembleas = await GetAssemblyList();
            foreach (var assembly in assembleas)
            {
                DisplayAssembly(assembly);
            }
        }

        public static async Task<List<Assembly>> GetAssemblyList()
        {
            List<Assembly> assemblies = new List<Assembly>();

            var files = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync();
            if (files == null)
                return assemblies;

            foreach (var file in files)
            {
                try
                {
                    string fileType = file.FileType.ToLower();
                    if (fileType == ".dll" || fileType == ".exe")
                        assemblies.Add(Assembly.Load(new AssemblyName(file.DisplayName)));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

            }

            return assemblies;
        }

        int assemblyNN;
        private void DisplayAssembly(Assembly assembly)
        {

            TreeNode parentNode = CreateFolderNode("");
            //sampleTreeView.RootNode.Add(parentNode);

            ////Display information about each module of this assembly. 
            //foreach (Module m in a.Modules)
            //{
            //    TreeNode treeNode = CreateFolderNode("Module: {0}", m.Name);
            //    parentNode.Add(treeNode);
            //}

            int typeNN = 0;
            //Display information about each type exported from this assembly.
            foreach (Type type in assembly.ExportedTypes)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                //if (typeInfo.Name.Contains("DependencyObject"))
                if (typeInfo.IsPublic)
                {
                    TreeNode treeNode = CreateFileNode("{0}-{1}", ++typeNN, typeInfo.Name);
                    parentNode.Add(treeNode);
                }
            }
            SetNodeData(parentNode, "{0} - {1} - Assembly={2}", ++assemblyNN, typeNN, assembly.FullName);
        }
    }
}
