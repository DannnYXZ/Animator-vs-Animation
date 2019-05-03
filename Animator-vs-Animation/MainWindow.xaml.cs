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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ExtendedMath;
using Rig;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;
using Plugin;

namespace Animator_vs_Animation {
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        List<Entity> stageObjects;
        Character orange, theOne, green;
        King king;
        Tentacle tentacle1, tentacle2, tentacle3;
        Drawer drawer;


        PluginManager pluginManager;
        IPlugin hierarchyPlugin;

        public MainWindow() {
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

            pluginManager = new PluginManager(Settings.PLUGIN_PATH);
            pluginManager.ReloadPlugins();
            hierarchyPlugin = pluginManager.GetPlugin("Hierarchy Plugin");
            if (hierarchyPlugin != null) {

                //pnlMain.Children.Add((FrameworkElement)hierarchyPlugin.Execute(stageObjects));
                pnlPlugins.Children.Add((FrameworkElement)hierarchyPlugin.Execute(stageObjects));
            }

            Thread thr = new Thread(new ThreadStart(Render));
            thr.Start();
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

        private void TypeSelector_Loaded(object sender, RoutedEventArgs e) {
            string nameSpace = "Characters";
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types) {
                if (type.Namespace == nameSpace && !type.IsEnum)
                    typeSelector.Items.Add(type);
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e) {
            var type = (Type)typeSelector.SelectedItem;
            if (type == null)
                return;
            var instance = Activator.CreateInstance(type) as Entity;
            stageObjects.Add(instance);

            if (hierarchyPlugin != null)
                hierarchyPlugin.Execute(stageObjects);
        }

        private void BtnSerialize_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON | *.json";
            saveFileDialog.Title = "Save Session";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "") {
                try {
                    string json = JsonConvert.SerializeObject(
                        stageObjects,
                        Formatting.Indented,
                        new JsonSerializerSettings {
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            TypeNameHandling = TypeNameHandling.All
                        });
                    using (StreamWriter fs = new StreamWriter(saveFileDialog.FileName)) {
                        fs.Write(json);
                    }
                } catch (Exception exeption) {
                    Console.WriteLine(exeption.Message);
                }
            }
            return;
        }

        private void BtnDeserialize_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON | *.json";
            openFileDialog.Title = "Open Session";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "") {
                try {
                    List<Entity> objects = new List<Entity>();
                    objects = JsonConvert.DeserializeObject<List<Entity>>(File.ReadAllText(openFileDialog.FileName),
                    new JsonSerializerSettings {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.All
                    });
                    stageObjects.Clear();
                    stageObjects = objects;
                } catch (Exception exception) {
                    MessageBox.Show(exception.Message);
                    Console.WriteLine(exception.Message);
                }

                if (hierarchyPlugin != null)
                    hierarchyPlugin.Execute(stageObjects);
            }
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
