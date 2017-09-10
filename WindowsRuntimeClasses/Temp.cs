using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;


namespace WindowsRuntimeClasses
{
    class Temp
    {
        Type rootType = typeof(System.Xml.Serialization.XmlAnyAttributeAttribute);//Windows.UI.Xaml.DependencyObject
        TypeInfo rootTypeInfo;
        List<Type> classes = new List<Type>();
        Brush highlightBrush;
        //Object o;
        //StackPanel stackPanel;
        private async void DisplayAssembleas()
        {
            // Display information about the EXE assembly.
            //Assembly a = typeof(MainPage).GetTypeInfo().Assembly;
            List<Assembly> assembleas = await GetAssemblyList();
            int i = 0;
            foreach (var assembly in assembleas)
            {
                Display(0, "{0} - Assembly={1}", ++i, assembly);
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

        private void DisplayAssembly(Assembly a)
        {
            // This variable holds the amount of indenting that  
            // should be used when displaying each line of information.
            int indent = 0;

            // Display information about each module of this assembly. 
            foreach (Module m in a.Modules)
            {
                Display(indent + 1, "Module: {0}", m.Name);
            }

            // Display information about each type exported from this assembly.
            indent += 1;
            //foreach (Type t in a.GetExportedTypes())
            //{
            //    Display(0, "");
            //    Display(indent, "Type: {0}", t);

            //    // For each type, show its members & their custom attributes.
            //}
            indent -= 1;
        }
        void DisplayClassTree()
        {
            rootTypeInfo = rootType.GetTypeInfo();
            highlightBrush = new SolidColorBrush(new UISettings().UIElementColor(UIElementType.Highlight));

            // Accumulate all the classes that derive from rootType 
            AddToClassList(rootType);

            // Sort them alphabetically by name
            classes.Sort((t1, t2) =>
            {
                return String.Compare(t1.GetTypeInfo().Name, t2.GetTypeInfo().Name);
            });

            // Put all these sorted classes into a tree structure
            ClassAndSubclasses rootClass = new ClassAndSubclasses(rootType);
            AddToTree(rootClass, classes);

            // Display the tree using TextBlock's added to StackPanel
            DisplayClass(rootClass, 0);
        }

        void AddToClassList(Type sampleType)
        {
            Assembly assembly = sampleType.GetTypeInfo().Assembly;

            foreach (Type type in assembly.ExportedTypes)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                //if (typeInfo.Name.Contains("DependencyObject"))
                if (typeInfo.IsPublic && rootTypeInfo.IsAssignableFrom(typeInfo))
                    classes.Add(type);
            }
        }

        void AddToTree(ClassAndSubclasses parentClass, List<Type> classes)
        {
            foreach (Type type in classes)
            {
                Type baseType = type.GetTypeInfo().BaseType;

                if (baseType == parentClass.Type)
                {
                    ClassAndSubclasses subClass = new ClassAndSubclasses(type);
                    parentClass.Subclasses.Add(subClass);
                    AddToTree(subClass, classes);
                }
            }
        }

        void DisplayClass(ClassAndSubclasses parentClass, int indent)
        {
            TypeInfo typeInfo = parentClass.Type.GetTypeInfo();

            // Create TextBlock with type name
            TextBlock txtblk = new TextBlock();
            txtblk.Inlines.Add(new Run { Text = new string(' ', 8 * indent) });
            txtblk.Inlines.Add(new Run { Text = typeInfo.Name });

            // Indicate if the class is sealed
            if (typeInfo.IsSealed)
                txtblk.Inlines.Add(new Run
                {
                    Text = " (sealed)",
                    Foreground = highlightBrush
                });

            // Indicate if the class can't be instantiated
            IEnumerable<ConstructorInfo> constructorInfos = typeInfo.DeclaredConstructors;
            int publicConstructorCount = 0;

            foreach (ConstructorInfo constructorInfo in constructorInfos)
                if (constructorInfo.IsPublic)
                    publicConstructorCount += 1;

            if (publicConstructorCount == 0)
                txtblk.Inlines.Add(new Run
                {
                    Text = " (non-instantiable)",
                    Foreground = highlightBrush
                });

            // Add to the StackPanel
            //stackPanel.Children.Add(txtblk);

            // Call this method recursively for all subclasses
            foreach (ClassAndSubclasses subclass in parentClass.Subclasses)
                DisplayClass(subclass, indent + 1);
        }

        void Display(int indent, string format, params object[] param)
        {
            // Create TextBlock with type name
            TextBlock txtblk = new TextBlock();
            txtblk.Inlines.Add(new Run { Text = new string(' ', 8 * indent) });
            txtblk.Inlines.Add(new Run { Text = String.Format(format, param) });

            // Add to the StackPanel
            //stackPanel.Children.Add(txtblk);
        }
    }
}
