using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Characters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ExtendedMath;
using Rig;
using Plugin;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;

namespace Animator_vs_Animation {
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        List<Entity> stageObjects;
        Drawer drawer;
        PluginManager pluginManager;
        IPlugin compressor, hierarchy;
        public MainWindow() {
            InitializeComponent();
            drawer = new Drawer(worldCanvas);
            stageObjects = new List<Entity>();

            pluginManager = new PluginManager(Settings.PLUGIN_PATHS);
            pluginManager.LoadPlugins();
            pluginManager.InstallUI(pnlPlugins);

            compressor = pluginManager.getPlugin("Session Comressor Plugin");
            hierarchy = pluginManager.getPlugin("Hierarchy Plugin");
            if (compressor != null) compressor.Execute(stageObjects);

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
            if (hierarchy != null) hierarchy.Execute(stageObjects);
        }

        public static Point GetMousePoint() {
            return Mouse.GetPosition(Application.Current.MainWindow);
        }

        public static Vector3 GetMouseVec3() {
            Point pt = Mouse.GetPosition(Application.Current.MainWindow);
            return new Vector3((float)pt.X, (float)pt.Y, 0);
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) {
            if (hierarchy != null) hierarchy.Execute(stageObjects);
        }
    }
}
