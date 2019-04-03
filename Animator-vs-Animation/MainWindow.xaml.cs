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

namespace Animator_vs_Animation {
    public class MenuItem {
        public MenuItem() {
            this.Items = new ObservableCollection<MenuItem>();
        }
        public string Title { get; set; }
        public Object Target { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
    }
    public partial class MainWindow : Window {
        List<Object> stageObjects;
        Character orange, theOne, green;
        King king;
        Tentacle tentacle;
        Drawer drawer;
        public MainWindow() {
            InitializeComponent();
            orange = new Character("OrangY", TRace.Orange);
            orange.Pivot.Translate(new Vector3(700, 200, 0));
            theOne = new Character(TRace.White);
            theOne.Pivot.Translate(new Vector3(400, 200, 0));
            green = new Character("Mr. Green", TRace.Green);
            green.Pivot.Translate(new Vector3(700, 400, 0));
            king = new King(TRace.Yellow);
            king.Pivot.Translate(new Vector3(500, 100, 0));
            tentacle = new Tentacle(4, 70);
            tentacle.Pivot.Translate(new Vector3(500, 400, 0));
            Console.WriteLine(orange.ToString());
            Console.WriteLine(theOne.ToString());
            Console.WriteLine(green.ToString());
            Console.WriteLine(king.ToString());
            Console.WriteLine(tentacle.ToString());
            Console.WriteLine("Green Says: ");
            green.SaySomething();
            Console.WriteLine("King Says: ");
            king.SaySomething();
            drawer = new Drawer(worldCanvas);
            Thread thr = new Thread(new ThreadStart(this.Render));
            thr.Start();

            stageObjects = new List<object>();
            stageObjects.Add(orange);
            stageObjects.Add(theOne);
            stageObjects.Add(tentacle);
            stageObjects.Add(green);
            stageObjects.Add(king);

            foreach(var x in stageObjects)
                trvMenu.Items.Add(ConstructTree(x));

            //ReflectionTools.ListTypesInfo();


            MenuItem root = new MenuItem() { Title = "Menu" };
            MenuItem childItem1 = new MenuItem() { Title = "Child item #1" };
            childItem1.Items.Add(new MenuItem() { Title = "Child item #1.1" });
            childItem1.Items.Add(new MenuItem() { Title = "Child item #1.2" });
            root.Items.Add(childItem1);
            root.Items.Add(new MenuItem() { Title = "Child item #2" });
            trvMenu.Items.Add(root);
        }
        public void Render() {
            while (true) {
                Dispatcher.Invoke(new Action(() => {
                    worldCanvas.Children.Clear();
                    drawer.DrawEntity(tentacle);
                    drawer.DrawEntity(orange);
                    drawer.DrawEntity(theOne);
                    drawer.DrawEntity(green);
                    drawer.DrawEntity(king);
                    Kinematics.InverseKinematics(tentacle.Pivot, GetMouseVec3());
                }));
                Thread.Sleep(10);
            }
        }
        MenuItem ConstructTree(Object rootObj) {
            MenuItem rootNode = new MenuItem();
            var type = rootObj.GetType();
            if (type.GetProperty("Name") != null)
                rootNode.Title = (string)type.GetProperty("Name").GetValue(rootObj);
            else
                rootNode.Title = type.Name;
            var props = type.GetProperties();
            foreach (var prop in props) {
                if (prop.CanWrite &&
                    prop.PropertyType == typeof(String) ||
                    prop.PropertyType == typeof(int) ||
                    prop.PropertyType == typeof(Vector3) ||
                    prop.PropertyType == typeof(Quaternion) ||
                    prop.PropertyType == typeof(Boolean) ||
                    prop.PropertyType == typeof(TRace)){
                    Console.WriteLine("\tProperty: " + prop.Name + " Type: " + prop.PropertyType);
                } else {
                    if(prop.GetValue(rootObj) is List<Joint>) {
                        foreach(var item in (List<Joint>)prop.GetValue(rootObj)) {
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

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e) {
            //DataContext dataContext = ((FrameworkElement)sender).DataContext;
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
