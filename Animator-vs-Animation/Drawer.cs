using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
                    line.StrokeThickness = 10;
                    line.X1 = joint.position.X;
                    line.Y1 = joint.position.Y;
                    line.X2 = child.position.X;
                    line.Y2 = child.position.Y;
                    canvas.Children.Add(line);
                }
                DrawJoints(child);
            }
        }
        public void DrawCharacter(Character character) {
            DrawJoints(character.body);
        }
    }
}
