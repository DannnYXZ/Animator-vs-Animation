using Characters;
using ExtendedMath;
using Plugin;
using Rig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HierarchyPlugin {
    public class MenuItem {
        public MenuItem() {
            Items = new ObservableCollection<MenuItem>();
        }
        public string Title { get; set; }
        public object Data { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
    }

    class HierarchyPlugin : IPlugin {
        public string Name { get => "Hierarchy Plugin"; }
        public string Description { get => "Creates tree view"; }
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

        private StackPanel ConstructEditor(MenuItem menuItem) {
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
                    Label lb1 = new Label(); lb1.Content = "X:";
                    Label lb2 = new Label(); lb2.Content = "Y:";
                    Label lb3 = new Label(); lb3.Content = "Z:";
                    TextBox tb1 = new TextBox();
                    TextBox tb2 = new TextBox();
                    TextBox tb3 = new TextBox();
                    CustomTextBoxBinding(tb1, "X", prop.GetValue(rootObj));
                    CustomTextBoxBinding(tb2, "Y", prop.GetValue(rootObj));
                    CustomTextBoxBinding(tb3, "Z", prop.GetValue(rootObj));
                    vectorPanel.Children.Add(lb1);
                    vectorPanel.Children.Add(tb1);
                    vectorPanel.Children.Add(lb2);
                    vectorPanel.Children.Add(tb2);
                    vectorPanel.Children.Add(lb3);
                    vectorPanel.Children.Add(tb3);
                    panel.Children.Add(vectorPanel);
                }
                if (prop.PropertyType == typeof(Quaternion)) {
                    StackPanel quatPanel = new StackPanel();
                    quatPanel.Orientation = Orientation.Horizontal;
                    TextBox tb0 = new TextBox();
                    TextBox tb1 = new TextBox();
                    TextBox tb2 = new TextBox();
                    TextBox tb3 = new TextBox();
                    Label l0 = new Label();
                    Label l1 = new Label();
                    Label l2 = new Label();
                    Label l3 = new Label();
                    l0.Content = "W:";
                    l1.Content = "X:";
                    l2.Content = "Y:";
                    l3.Content = "Z:";
                    CustomTextBoxBinding(tb0, "W", prop.GetValue(rootObj));
                    CustomTextBoxBinding(tb1, "X", prop.GetValue(rootObj));
                    CustomTextBoxBinding(tb2, "Y", prop.GetValue(rootObj));
                    CustomTextBoxBinding(tb3, "Z", prop.GetValue(rootObj));
                    quatPanel.Children.Add(l0);
                    quatPanel.Children.Add(tb0);
                    quatPanel.Children.Add(l1);
                    quatPanel.Children.Add(tb1);
                    quatPanel.Children.Add(l2);
                    quatPanel.Children.Add(tb2);
                    quatPanel.Children.Add(l3);
                    quatPanel.Children.Add(tb3);
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
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e) {
            var baseObj = sender as FrameworkElement;
            var dataContext = baseObj.DataContext as MenuItem;
            //pnlEdit.Children.Clear();
            //pnlEdit.Children.Add(ConstructEditor(dataContext));
            Console.WriteLine();
        }
        TreeView createTreeView() {
            TreeView localTree = new TreeView();
            var dataTemplate = new HierarchicalDataTemplate();
            dataTemplate.DataType = typeof(MenuItem);
            FrameworkElementFactory infoHolder = new FrameworkElementFactory(typeof(TextBlock));
            infoHolder.AddHandler(TextBlock.MouseDownEvent, new MouseButtonEventHandler(TextBlock_MouseDown));
            dataTemplate.ItemsSource = new Binding("Items");
            infoHolder.SetBinding(TextBlock.TextProperty, new Binding("Data.Name"));
            dataTemplate.VisualTree = infoHolder;
            localTree.ItemTemplate = dataTemplate;
            return localTree;
        }
        public object Execute(object obj) {
            TreeView treeView = new TreeView();

            return treeView;
        }
    }
}
