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

            //Type type = typeof(Windows.UI.Xaml.DependencyObject);
            //DisplayAssembly(type.GetTypeInfo().Assembly);
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

        #region AutoSuggestBox
        //c:\_work\Temp\Windows-universal-samples-master\Samples\XamlUIBasics\cs\AppUIBasics\ControlPages\AutoSuggestBoxPage.xaml.cs

        /// <summary>
        /// This event gets fired anytime the text in the TextBox gets updated.
        /// It is recommended to check the reason for the text changing by checking against args.Reason
        /// </summary>
        /// <param name="sender">The AutoSuggestBox whose text got changed.</param>
        /// <param name="args">The event arguments.</param>
        private void Search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            ////We only want to get results when it was a user typing, 
            ////otherwise we assume the value got filled in by TextMemberPath 
            ////or the handler for SuggestionChosen
            //if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            //{
            //    var suggestions = await SearchControls(sender.Text);

            //    if (suggestions.Count > 0)
            //        sender.ItemsSource = suggestions;
            //    else
            //        sender.ItemsSource = new string[] { "No results found" };
            //}
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
        private void Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //if (args.ChosenSuggestion != null && args.ChosenSuggestion is ControlInfoDataItem)
            //{
            //    //User selected an item, take an action
            //    SelectControl(args.ChosenSuggestion as ControlInfoDataItem);
            //}
            //else if (!string.IsNullOrEmpty(args.QueryText))
            //{
            //    //Do a fuzzy search based on the text
            //    var suggestions = await SearchControls(sender.Text);
            //    if (suggestions.Count > 0)
            //    {
            //        SelectControl(suggestions.FirstOrDefault());
            //    }
            //}
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
            //var control = args.SelectedItem as ControlInfoDataItem;

            ////Don't autocomplete the TextBox when we are showing "no results"
            //if (control != null)
            //{
            //    sender.Text = control.Title;
            //}
        }
        #endregion AutoSuggestBox
    }
}
