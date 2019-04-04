using Animator_vs_Animation.Reflection;
using System;
using System.Numerics;
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

namespace Animator_vs_Animation {
    public class MenuItem {
        public MenuItem() {
            this.Items = new ObservableCollection<MenuItem>();
        }
        public string Title { get; set; }
        public Object Data { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
    }
    public partial class MainWindow : Window, INotifyPropertyChanged {
        List<Entity> stageObjects;
        Character orange, theOne, green;
        King king;
        Tentacle tentacle1, tentacle2, tentacle3;
        Drawer drawer;

        public string test = "";
        public string Test {
            get {
                return test;
            }
            set {
                test = value;
                OnPropertyChanged();
            }
        }

        public MainWindow() {
            DataContext = this;
            InitializeComponent();
            orange = new Character("OrangY", TRace.Orange);
            orange.Pivot.Translate(new Vector3(700, 200, 0));
            theOne = new Character(TRace.White);
            theOne.Pivot.Translate(new Vector3(400, 200, 0));
            green = new Character("Mr. Green", TRace.Green);
            green.Pivot.Translate(new Vector3(700, 400, 0));
            king = new King(TRace.Yellow);
            king.Pivot.Translate(new Vector3(500, 100, 0));
            tentacle1 = new Tentacle("Tentacle 1", 4, 70);
            tentacle1.Pivot.Translate(new Vector3(300, 400, 0));
            tentacle2 = new Tentacle("Tentacle 2", 4, 70);
            tentacle2.Pivot.Translate(new Vector3(600, 400, 0));
            tentacle3 = new Tentacle("Tentacle 3", 4, 70);
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
            drawer = new Drawer(worldCanvas);
            Thread thr = new Thread(new ThreadStart(this.Render));
            thr.Start();

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

            TextBox tb = new TextBox();
            tb.DataContext = this.tentacle1.Pivot.Quaternion;
            var binding = new Binding();
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //binding.Source = Test;
            binding.Path = new PropertyPath("X");
            tb.SetBinding(TextBox.TextProperty, binding);
            pnlMain.Children.Add(tb);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            Dispatcher.Invoke(() =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
            );
        }
        public void Render() {
            while (true) {
                Dispatcher.Invoke(new Action(() => {
                    worldCanvas.Children.Clear();
                    foreach (var obj in stageObjects) {
                        obj.Pivot.UpdatePos();
                        drawer.DrawEntity(obj);
                        if (typeof(Tentacle) == obj.GetType())
                            Kinematics.InverseKinematics(obj.Pivot, GetMouseVec3());
                    }
                }));
                Thread.Sleep(10);
            }
        }
        MenuItem ConstructTree(Object rootObj) {
            MenuItem rootNode = new MenuItem();
            rootNode.Data = rootObj;
            var type = rootObj.GetType();
            var props = type.GetProperties();
            foreach (var prop in props) {
                if (prop.PropertyType == typeof(String) ||
                    prop.PropertyType == typeof(int) ||
                    prop.PropertyType == typeof(Vector3) ||
                    prop.PropertyType == typeof(Quaternion) ||
                    prop.PropertyType == typeof(Boolean) ||
                    prop.PropertyType == typeof(TRace)) {
                    Console.WriteLine("\tProperty: " + prop.Name + " Type: " + prop.PropertyType);
                } else {
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
                if (type.Namespace == nameSpace)
                    typeSelector.Items.Add(type);
            }
        }
        private StackPanel ConstructEditor(MenuItem menuItem) {
            Object obj = menuItem.Data;
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            var type = obj.GetType();
            var props = type.GetProperties();
            foreach (var prop in props) {
                if (prop.GetSetMethod(false) == null)
                    continue;
                Label lb = new Label();
                lb.Content = prop.Name + ":";
                panel.Children.Add(lb);
                if (prop.PropertyType == typeof(String)) {
                    TextBox tb = new TextBox();
                    tb.DataContext = obj;
                    var binding = new Binding();
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Path = new PropertyPath(prop.Name);
                    tb.SetBinding(TextBox.TextProperty, binding);
                    panel.Children.Add(tb);
                }
                if (prop.PropertyType == typeof(int)) {
                    TextBox tb = new TextBox();
                    var binding = new Binding();
                    tb.DataContext = obj;
                    binding.Path = new PropertyPath(prop.Name);
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Mode = BindingMode.TwoWay;
                    tb.SetBinding(TextBox.TextProperty, binding);
                    panel.Children.Add(tb);
                }
                if (prop.PropertyType == typeof(Boolean)) {
                    TextBox tb = new TextBox();
                    tb.DataContext = obj;
                    var binding = new Binding();
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Path = new PropertyPath(prop.Name);
                    tb.SetBinding(TextBox.TextProperty, binding);
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
                    l0.Content = "W:";
                    Label l1 = new Label();
                    l1.Content = "X:";
                    Label l2 = new Label();
                    l2.Content = "Y:";
                    Label l3 = new Label();
                    l3.Content = "Z:";
                    // binding stuff
                    var binding = new Binding();
                    tb0.DataContext = prop.GetValue(obj);
                    binding.Path = new PropertyPath("W");
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Mode = BindingMode.TwoWay;
                    tb0.SetBinding(TextBox.TextProperty, binding);

                    binding = new Binding();
                    tb1.DataContext = prop.GetValue(obj);
                    binding.Path = new PropertyPath("X");
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    tb1.SetBinding(TextBox.TextProperty, binding);

                    binding = new Binding();
                    tb2.DataContext = prop.GetValue(obj);
                    binding.Path = new PropertyPath("Y");
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    tb2.SetBinding(TextBox.TextProperty, binding);

                    binding = new Binding();
                    tb3.DataContext = prop.GetValue(obj);
                    binding.Path = new PropertyPath("Z");
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    tb3.SetBinding(TextBox.TextProperty, binding);

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
                if (prop.PropertyType == typeof(TRace)) {
                    ComboBox cb = new ComboBox();
                    cb.Items.Add(Enum.GetValues(typeof(TRace)));
                    panel.Children.Add(cb);
                }
            }
            return panel;
        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e) {
            //DataContext dataContext = ((FrameworkElement)sender).DataContext;
            var baseObj = sender as FrameworkElement;
            var dataContext = baseObj.DataContext as MenuItem;
            pnlEdit.Children.Clear();
            pnlEdit.Children.Add(ConstructEditor(dataContext));
            Console.WriteLine();
        }

        private void KEK_Click(object sender, RoutedEventArgs e) {
            Test += "X";
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
