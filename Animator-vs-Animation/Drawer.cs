using System;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;
using Characters;
using System.Windows.Shapes;

namespace Animator_vs_Animation {
    class Drawer {
        Canvas canvas;
        public Drawer(Canvas canvas) {
            this.canvas = canvas;
        }
        private void DrawJoints(Joint joint, int color) {
            Vector3 startPoint = joint.Position;
            foreach (Joint child in joint.Joints) {
                if (joint.ShowDependencies) {
                    Line line = new Line();
                    line.Stroke = Brushes.Orange;
                    line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString('#' + Convert.ToString(color, 16)));
                    line.StrokeThickness = 5;
                    line.X1 = joint.Position.X;
                    line.Y1 = joint.Position.Y;
                    line.X2 = child.Position.X;
                    line.Y2 = child.Position.Y;
                    canvas.Children.Add(line);
                }
                DrawJoints(child, color);
            }
            Ellipse circle = new Ellipse();
            circle.Width = 7;
            circle.Height = 7;
            circle.SetValue(Canvas.LeftProperty, (double)(joint.Position.X - circle.Width / 2f));
            circle.SetValue(Canvas.TopProperty, (double)(joint.Position.Y - circle.Height / 2f));
            circle.Fill = new SolidColorBrush(Color.FromArgb(255, 240, 87, 7));
            canvas.Children.Add(circle);
        }
        public void DrawEntity(Entity entity) {
            if (entity.GetType() == typeof(Tentacle))
                DrawJoints(entity.Pivot, 0xAC3BAF);
            else if (entity.GetType() == typeof(Character)) {
                DrawJoints(entity.Pivot, (int)((Character)entity).Race);
            }
        }
    }
}
