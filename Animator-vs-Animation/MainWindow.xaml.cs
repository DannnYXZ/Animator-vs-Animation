using Animator_vs_Animation.Characters;
using System;
using System.Numerics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Animator_vs_Animation {
    public partial class MainWindow : Window {
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
            green = new Character("Mr. Green",TRace.Green);
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
        public static Point GetMousePoint() {
            return Mouse.GetPosition(Application.Current.MainWindow);
        }
        public static Vector3 GetMouseVec3() {
            Point pt = Mouse.GetPosition(Application.Current.MainWindow);
            return new Vector3((float)pt.X, (float)pt.Y, 0);
        }
    }
}
