using Characters;
using ExtendedMath;
using Plugin;
using Rig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Plugin.Hierarchy {
    public class MenuItem {
        public MenuItem() {
            Items = new ObservableCollection<MenuItem>();
        }
        public string Title { get; set; }
        public object Data { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
    }
    class HierarchyPlugin : IPlugin {
        public string Name => "Hierarchy Plugin";
        public string Description => "Creates tree view for List<Entity>";
        MenuItem ConstructTree(object rootObj) {
            MenuItem rootNode = new MenuItem();
            rootNode.Data = rootObj;
            var type = rootObj.GetType();
            var props = type.GetProperties();
            foreach (var prop in props) {
                if (!(prop.PropertyType == typeof(string) ||
                    prop.PropertyType == typeof(int) ||
                    prop.PropertyType == typeof(bool) ||
                    prop.PropertyType == typeof(Vector3) ||
                    prop.PropertyType == typeof(Quaternion) ||
                    prop.PropertyType == typeof(TRace))) {
                    if (prop.GetValue(rootObj) is List<Joint>) {
                        foreach (var item in (List<Joint>)prop.GetValue(rootObj)) {
                            MenuItem childNode = new MenuItem();
                            childNode = ConstructTree(item);
                            rootNode.Items.Add(childNode);
                        }
                    } else {
                        MenuItem childNode = new MenuItem();
                        childNode.Title = prop.Name;
                        childNode = ConstructTree(prop.GetValue(rootObj));
                        rootNode.Items.Add(childNode);
                    }
                }
            }
            return rootNode;
        }
        private void CustomTextBoxBinding(TextBox textBox, string propPath, object context) {
            var binding = new Binding();
            textBox.DataContext = context;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Path = new PropertyPath(propPath);
            textBox.SetBinding(TextBox.TextProperty, binding);
        }
        private StackPanel CreateMenuItemEditor(MenuItem menuItem) {
            object rootObj = menuItem.Data;
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            var type = rootObj.GetType();
            var props = type.GetProperties();
            foreach (var prop in props) {
                if (prop.GetSetMethod(false) == null)
                    continue;
                Label label = new Label();
                label.Content = prop.Name + ":";
                panel.Children.Add(label);
                if (prop.PropertyType == typeof(string)) {
                    TextBox tb = new TextBox();
                    CustomTextBoxBinding(tb, prop.Name, rootObj);
                    panel.Children.Add(tb);
                }
                if (prop.PropertyType == typeof(int)) {
                    TextBox tb = new TextBox();
                    CustomTextBoxBinding(tb, prop.Name, rootObj);
                    panel.Children.Add(tb);
                }
                if (prop.PropertyType == typeof(bool)) {
                    TextBox tb = new TextBox();
                    CustomTextBoxBinding(tb, prop.Name, rootObj);
                    panel.Children.Add(tb);
                }
                if (prop.PropertyType == typeof(Vector3)) {
                    StackPanel vectorPanel = new StackPanel();
                    vectorPanel.Orientation = Orientation.Horizontal;
                    foreach (var text in new string[] { "X", "Y", "Z" }) {
                        var lb = new Label();
                        lb.Content = text + ":";
                        var tb = new TextBox();
                        CustomTextBoxBinding(tb, text, prop.GetValue(rootObj));
                        vectorPanel.Children.Add(lb);
                        vectorPanel.Children.Add(tb);
                    }
                    panel.Children.Add(vectorPanel);
                }
                if (prop.PropertyType == typeof(Quaternion)) {
                    StackPanel quatPanel = new StackPanel();
                    quatPanel.Orientation = Orientation.Horizontal;
                    foreach (var text in new string[] { "W", "X", "Y", "Z" }) {
                        var lb = new Label();
                        lb.Content = text + ":";
                        var tb = new TextBox();
                        CustomTextBoxBinding(tb, text, prop.GetValue(rootObj));
                        quatPanel.Children.Add(lb);
                        quatPanel.Children.Add(tb);
                    }
                    panel.Children.Add(quatPanel);
                }
                if (prop.PropertyType.IsEnum) {
                    ComboBox cb = new ComboBox();
                    cb.ItemsSource = Enum.GetValues(prop.PropertyType).Cast<Enum>();
                    var value = prop.GetValue(rootObj);
                    cb.DataContext = value;
                    cb.SelectionChanged += (object sender, SelectionChangedEventArgs e) => {
                        var comboBox = sender as ComboBox;
                        prop.SetValue(rootObj, comboBox.SelectedItem);
                    };
                    foreach (var item in cb.Items) {
                        if (item.Equals(value)) {
                            cb.SelectedItem = item;
                            break;
                        }
                    }
                    panel.Children.Add(cb);
                }
            }
            return panel;
        }
        private void MenuItem_MouseDown(object sender, MouseButtonEventArgs e) {
            var baseObj = sender as FrameworkElement;
            var dataContext = baseObj.DataContext as MenuItem;
            menuEditorBlock.Children.Clear();
            menuEditorBlock.Children.Add(CreateMenuItemEditor(dataContext));
        }
        private TreeView createTreeView() {
            TreeView newTree = new TreeView();
            var dataTemplate = new HierarchicalDataTemplate();
            dataTemplate.DataType = typeof(MenuItem);
            FrameworkElementFactory infoHolder = new FrameworkElementFactory(typeof(TextBlock));
            infoHolder.AddHandler(TextBlock.MouseDownEvent, new MouseButtonEventHandler(MenuItem_MouseDown));
            dataTemplate.ItemsSource = new Binding("Items");
            infoHolder.SetBinding(TextBlock.TextProperty, new Binding("Data.Name"));
            dataTemplate.VisualTree = infoHolder;
            newTree.ItemTemplate = dataTemplate;
            return newTree;
        }

        Expander expander = null;
        StackPanel pluginBlock, menuEditorBlock;
        TreeView treeView;
        //Button btnBuild;

        public object Execute(object obj) {
            List<Entity> list = (List<Entity>)obj;
            treeView.Items.Clear();
            foreach (var x in list)
                treeView.Items.Add(ConstructTree(x));
            return null;
        }

        public FrameworkElement GetUI() {
            if (expander == null) {
                expander = new Expander();
                expander.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#95A5FF"));
                expander.Header = Name;
                pluginBlock = new StackPanel();
                menuEditorBlock = new StackPanel();
                //btnBuild = new Button();
                //btnBuild.Click += BtnBuild_Click;
                treeView = createTreeView();
                //pluginBlock.Children.Add(btnBuild);
                pluginBlock.Children.Add(treeView);
                pluginBlock.Children.Add(menuEditorBlock);
                expander.Content = pluginBlock;
            }
            return expander;
        }

        public void Initialize(IHost host) {
        }

        public void SetArgument(string key, object value) {
            
        }
    }
}
