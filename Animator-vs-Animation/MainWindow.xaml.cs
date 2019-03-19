using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Animator_vs_Animation {
    public partial class MainWindow : Window {
        Character character;
        Drawer drawer;
        public MainWindow() {
            InitializeComponent();
            character = new Character();
            character.body.Translate(new Vector3(400, 200, 0));
            drawer = new Drawer(worldCanvas);
            //Vector3 point = Kinematics.ForwardKinematics(character.body, new float[] { 0, 45, 0 });
            //await Render();
            var t = Task.Run(async () => {
                await Render();
            });
        }
        public Task Render() {
            while (true) {
                Dispatcher.Invoke(new Action(() => {
                    worldCanvas.Children.Clear();
                    drawer.DrawCharacter(character);

                    Joint jointPtr = character.body;
                    while(jointPtr.joints.Count > 0) {
                        jointPtr.Rotate(1);
                        jointPtr = jointPtr.joints[0];
                    }
                }));
                Thread.Sleep(100);
                Console.WriteLine("KEK");
            }
        }
    }
}
