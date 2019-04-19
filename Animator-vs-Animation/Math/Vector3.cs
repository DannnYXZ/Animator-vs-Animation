using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedMath {
    public class Vector3 : INotifyPropertyChanged {
        float x;
        float y;
        float z;
        public float X {
            get { return x; }
            set {
                x = value;
                OnPropertyChanged("X");
            }
        }
        public float Y {
            get { return y; }
            set {
                y = value;
                OnPropertyChanged("Y");
            }
        }
        public float Z {
            get { return z; }
            set {
                z = value;
                OnPropertyChanged("Z");
            }
        }
        public static Vector3 Zero { get; } = new Vector3(0, 0, 0);
        public Vector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static float Distance(Vector3 a, Vector3 b) {
            float dx = b.x - a.x;
            float dy = b.y - a.y;
            float dz = b.z - a.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static Vector3 operator +(Vector3 a, Vector3 b) {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
    }
}
