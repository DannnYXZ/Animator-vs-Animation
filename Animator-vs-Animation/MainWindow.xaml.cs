using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Characters;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ExtendedMath;
using Rig;
using System.Linq;

namespace Animator_vs_Animation {
    public class MenuItem {
        public MenuItem() {
            Items = new ObservableCollection<MenuItem>();
        }
        public string Title { get; set; }
        public object Data { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
    }
    public partial class MainWindow : Window, INotifyPropertyChanged {
        List<Entity> stageObjects;
        Character orange, theOne, green;
        King king;
        Tentacle tentacle1, tentacle2, tentacle3;
        Drawer drawer;

        public MainWindow() {
            DataContext = this;
            InitializeComponent();
            drawer = new Drawer(worldCanvas);
            orange = new Character("OrangY", TRace.Orange);
            theOne = new Character(TRace.White);
            green = new Character("Mr. Green", TRace.Green);
            king = new King(TRace.Yellow);
            tentacle1 = new Tentacle("Tentacle 1", 4, 70);
            tentacle2 = new Tentacle("Tentacle 2", 4, 70);
            tentacle3 = new Tentacle("Tentacle 3", 4, 70);
            orange.Pivot.Translate(new Vector3(700, 200, 0));
            theOne.Pivot.Translate(new Vector3(400, 200, 0));
            green.Pivot.Translate(new Vector3(700, 400, 0));
            king.Pivot.Translate(new Vector3(500, 100, 0));
            tentacle1.Pivot.Translate(new Vector3(300, 400, 0));
            tentacle2.Pivot.Translate(new Vector3(600, 400, 0));
            tentacle3.Pivot.Translate(new Vector3(500, 400, 0));
            Console.WriteLine(orange.ToString());
            Console.WriteLine(theOne.ToString());
            Console.WriteLine(green.ToString());
            Console.WriteLine(king.ToString());
            Console.WriteLine(tentacle1.ToString());
            Console.WriteLine("Green Says: ");
            green.SaySomething();
            Console.WriteLine("King Says: ");
            king.SaySomething();

            stageObjects = new List<Entity>();
            stageObjects.Add(orange);
            stageObjects.Add(theOne);
            stageObjects.Add(tentacle1);
            stageObjects.Add(tentacle2);
            stageObjects.Add(tentacle3);
            stageObjects.Add(green);
            stageObjects.Add(king);
            foreach (var x in stageObjects)
                trvMenu.Items.Add(ConstructTree(x));

            Thread thr = new Thread(new ThreadStart(Render));
            thr.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Render() {
            while (true) {
                Dispatcher.Invoke(new Action(() => {
                    worldCanvas.Children.Clear();
                    foreach (var obj in stageObjects) {
                        obj.Pivot.UpdatePos();
                        drawer.DrawEntity(obj);
                        if (typeof(Tentacle) == obj.GetType()) {
                            Kinematics.InverseKinematics(obj.Pivot.Joints[0], GetMouseVec3());

                        }
                    }
                }));
                Thread.Sleep(10);
            }
        }
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
                        childNode = ConstructTree((Object)prop.GetValue(rootObj));
                        rootNode.Items.Add(childNode);
                    }
                }
            }
            return rootNode;
        }
        private void TypeSelector_Loaded(object sender, RoutedEventArgs e) {
            string nameSpace = "Characters";
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types) {
                if (type.Namespace == nameSpace && !type.IsEnum)
                    typeSelector.Items.Add(type);
            }
        }
        private void CustomTextBoxBinding(TextBox textBox, string propPath, object context) {
            var binding = new Binding();
            textBox.DataContext = context;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Path = new PropertyPath(propPath);
            textBox.SetBinding(TextBox.TextProperty, binding);
        }
        private void DestroyMenu(MenuItem menu) {
            if (menu.Items.Count > 0) {
                foreach (var item in menu.Items)
                    DestroyMenu(item);
            }
        }
        private void BtnCreate_Click(object sender, RoutedEventArgs e) {
            var type = (Type)typeSelector.SelectedItem;
            if (type == null)
                return;
            var instance = Activator.CreateInstance(type) as Entity;
            stageObjects.Add(instance);
            trvMenu.Items.Clear();
            foreach (var x in stageObjects)
                trvMenu.Items.Add(ConstructTree(x));
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
            pnlEdit.Children.Clear();
            pnlEdit.Children.Add(ConstructEditor(dataContext));
            Console.WriteLine();
        }

        public static Point GetMousePoint() {
            return Mouse.GetPosition(Application.Current.MainWindow);
        }
        public static Vector3 GetMouseVec3() {
            Point pt = Mouse.GetPosition(Application.Current.MainWindow);
            return new Vector3((float)pt.X, (float)pt.Y, 0);
        }
    }
}
