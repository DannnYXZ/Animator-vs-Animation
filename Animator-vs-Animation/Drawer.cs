using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Animator_vs_Animation {
    class Drawer {
        Canvas canvas;
        public Drawer(Canvas canvas) {
            this.canvas = canvas;
        }
        private void DrawJoints(Joint joint) {
            Vector3 startPoint = joint.position;
            foreach(Joint child in joint.joints) {
                if (joint.showDependencies) {
                    Line line = new Line();
                    line.Stroke = System.Windows.Media.Brushes.Orange;
                    line.StrokeThickness = 5;
                    line.X1 = joint.position.X;
                    line.Y1 = joint.position.Y;
                    line.X2 = child.position.X;
                    line.Y2 = child.position.Y;
                    canvas.Children.Add(line);
                }
                DrawJoints(child);
            }

            Ellipse circle = new Ellipse();
            circle.Width = 7;
            circle.Height = 7;
            circle.SetValue(Canvas.LeftProperty, (double)(joint.position.X - circle.Width / 2f));
            circle.SetValue(Canvas.TopProperty, (double)(joint.position.Y - circle.Height / 2f));
            circle.Fill = new SolidColorBrush(Color.FromArgb(255, 240, 87, 7));
            canvas.Children.Add(circle);
        }
        public void DrawCharacter(Character character) {
            DrawJoints(character.body);
        }
    }
}
