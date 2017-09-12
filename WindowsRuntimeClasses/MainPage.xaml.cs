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
        Dictionary<String, ClassAndSubclasses> classesDict;
        TreeNodeData objectTreeNodeData; //TODO - delete this

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

        private TreeNodeData AddToTree(ClassAndSubclasses classAndSubclasses)
        {
            TreeNode treeNode = new TreeNode();
            foreach (ClassAndSubclasses c in classAndSubclasses.Subclasses)
            {
                treeNode.Add(AddToTree(c).TreeNode);
            }
            TypeInfo typeInfo = classAndSubclasses.Type.GetTypeInfo();
            string name = String.Format("{0} ({1})", typeInfo.FullName, treeNode.Count);
            TreeNodeData treeNodeData = new TreeNodeData() { IsFolder = (treeNode.Count > 0), TreeNode = treeNode, Name = name, NodeClasses = classAndSubclasses };
            treeNode.Data = treeNodeData;
            return treeNodeData;
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
                    if (typeInfo.IsClass)
                    {
                        AddToSubclasses(type);
                    }
                }
            }
            TreeNodeData treeNodeData = AddToTree(objectClassAndSubclasses);
            objectTreeNodeData = treeNodeData;
            sampleTreeView.RootNode.Add(treeNodeData.TreeNode);
            classesDict = null;
        }

        #region AutoSuggestBox
        //from Windows-universal-samples-master\Samples\XamlUIBasics\cs\AppUIBasics\ControlPages\AutoSuggestBoxPage.xaml.cs

        /// <summary>
        /// This event gets fired anytime the text in the TextBox gets updated.
        /// It is recommended to check the reason for the text changing by checking against args.Reason
        /// </summary>
        /// <param name="sender">The AutoSuggestBox whose text got changed.</param>
        /// <param name="args">The event arguments.</param>
        private async void Search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //We only want to get results when it was a user typing, 
            //otherwise we assume the value got filled in by TextMemberPath 
            //or the handler for SuggestionChosen
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (sender.Text.Length > 0)
                {
                    var suggestions = await SearchClass(sender.Text);

                    if (suggestions.Count > 0)
                        sender.ItemsSource = suggestions;
                    else
                        sender.ItemsSource = new string[] { "No results found" };
                }
                else
                {
                    sender.ItemsSource = null;
                }
            }
        }

        private Task<List<TreeNodeData>> SearchClass(string text)
        {
            return Task.Run(() =>
            {
                var suggestions = new List<TreeNodeData>();
                if (text.Length > 0)
                    suggestions.Add(objectTreeNodeData);
                return suggestions;
            });
        }

        /// <summary>
        /// This event gets fired when:
        ///     * a user presses Enter while focus is in the TextBox
        ///     * a user clicks or tabs to and invokes the query button (defined using the QueryIcon API)
        ///     * a user presses selects (clicks/taps/presses Enter) a suggestion
        /// </summary>
        /// <param name="sender">The AutoSuggestBox that fired the event.</param>
        /// <param name="args">The args contain the QueryText, which is the text in the TextBox, 
        /// and also ChosenSuggestion, which is only non-null when a user selects an item in the list.</param>
        private async void Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is TreeNodeData)
            {
                //User selected an item, take an action
                SelectNode(args.ChosenSuggestion as TreeNodeData);
            }
            else if (!string.IsNullOrEmpty(args.QueryText))
            {
                //Do a fuzzy search based on the text
                var suggestions = await SearchClass(sender.Text);
                if (suggestions.Count > 0)
                {
                    SelectNode(suggestions[0]);
                }
            }
        }

        private void SelectNode(TreeNodeData treeNodeData)
        {
            sampleTreeView.SelectedItem = treeNodeData.TreeNode;
        }

        /// <summary>
        /// This event gets fired as the user keys through the list, or taps on a suggestion.
        /// This allows you to change the text in the TextBox to reflect the item in the list.
        /// Alternatively you can use TextMemberPath.
        /// </summary>
        /// <param name="sender">The AutoSuggestBox that fired the event.</param>
        /// <param name="args">The args contain SelectedItem, which contains the data item of the item that is currently highlighted.</param>
        private void Search_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var control = args.SelectedItem as TreeNodeData;

            //Don't autocomplete the TextBox when we are showing "no results"
            if (control != null)
            {
                sender.Text = control.ToString();
            }
        }
        #endregion AutoSuggestBox
    }
}
