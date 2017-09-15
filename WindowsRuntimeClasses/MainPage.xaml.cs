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
        TypeInfo objectTypeInfo = typeof(Object).GetTypeInfo();
        Dictionary<String, NodeData> classesDict;
        NodeData objectNode;

        public MainPage()
        {
            this.InitializeComponent();

            Type type = typeof(Windows.UI.Xaml.DependencyObject);
            DisplayAssembly(type.GetTypeInfo().Assembly);
        }

        private void AddToSubclasses(TypeInfo typeInfo)
        {
            String typeNmae = typeInfo.FullName;
            if (classesDict.ContainsKey(typeNmae))
            {
                return;
            }

            Type baseType = typeInfo.BaseType;
            TypeInfo baseTypeInfo = baseType.GetTypeInfo();
            String baseTypeNmae = baseTypeInfo.FullName;
            if (!classesDict.ContainsKey(baseTypeNmae))
            {
                AddToSubclasses(baseTypeInfo);
            }
            NodeData parent = classesDict[baseTypeNmae];
            NodeData child = new NodeData(typeInfo);
            parent.SubNodes.Add(child);
            classesDict.Add(typeNmae, child);
        }

        private TreeNode AddToTree(NodeData nodeData)
        {
            nodeData.TreeNode = new TreeNode() { Data = nodeData };
            foreach (NodeData subNode in nodeData.SubNodes)
            {
                nodeData.TreeNode.Add(AddToTree(subNode));
            }
            nodeData.Name = String.Format("{0} ({1})", nodeData.TypeInfo.Name, nodeData.SubNodes.Count);
            nodeData.IsFolder = nodeData.SubNodes.Count > 0;
            return nodeData.TreeNode;
        }

        private void SortTree(NodeData nodeData)
        {
            nodeData.Sort();
            foreach (NodeData subNode in nodeData.SubNodes)
            {
                SortTree(subNode);
            }
        }

        private void DisplayAssembly(Assembly assembly)
        {
            objectNode = new NodeData(objectTypeInfo);
            classesDict = new Dictionary<string, NodeData>();
            classesDict.Add(objectTypeInfo.FullName, objectNode);
            foreach (Type type in assembly.ExportedTypes)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                //if (typeInfo.Name.Contains("DependencyObject"))
                {
                    if (typeInfo.IsClass)
                    {
                        AddToSubclasses(typeInfo);
                    }
                }
            }
            SortTree(objectNode);
            sampleTreeView.RootNode.Add(AddToTree(objectNode));
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

        private int maxSuggestion = 50; 
        private void SearchNode(NodeData nodeData, string text, List<NodeData> suggestions, int level)
        {
            if (level == 0)
            {
                if (nodeData.TypeInfo.Name.StartsWith(text))
                    suggestions.Add(nodeData);
            }
            else if (level == 1)
            {
                if (nodeData.TypeInfo.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                    suggestions.Add(nodeData);
            }
            else
            {
                if (nodeData.TypeInfo.Name.ToLower().Contains(text.ToLower()))
                    suggestions.Add(nodeData);
            }
            foreach (NodeData subNode in nodeData.SubNodes)
            {
                SearchNode(subNode, text, suggestions, level);
                if (suggestions.Count > maxSuggestion)
                    break;
            }
        }

        private Task<List<NodeData>> SearchClass(string text)
        {
            return Task.Run(() =>
            {
                var suggestions = new List<NodeData>();
                if (text.Length > 0)
                {
                    //SearchNode(objectNode, text, suggestions, 0);
                    //SearchNode(objectNode, text, suggestions, 1);
                    SearchNode(objectNode, text, suggestions, 2);
                }
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
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is NodeData)
            {
                //User selected an item, take an action
                SelectNode(args.ChosenSuggestion as NodeData);
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

        private void SelectNode(NodeData nodeData)
        {
            sampleTreeView.SelectedItem = nodeData.TreeNode;
            sampleTreeView.ScrollIntoView(nodeData.TreeNode, ScrollIntoViewAlignment.Leading);
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
            var control = args.SelectedItem as NodeData;

            //Don't autocomplete the TextBox when we are showing "no results"
            if (control != null)
            {
                sender.Text = control.ToString();
            }
        }
        #endregion AutoSuggestBox
    }
}
