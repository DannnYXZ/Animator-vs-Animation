using ExtendedMath;
using System;
using System.ComponentModel;
using System.Windows;

namespace ExtendedMath {
    class Quaternion : INotifyPropertyChanged {
        float w;
        float x;
        float y;
        float z;
        public float W {
            get { return w; }
            set {
                w = value;
                OnPropertyChanged("W");
            }
        }
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
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static Quaternion Identity { get { return new Quaternion(0, 0, 0, 1); } }
        public Quaternion(float X, float Y, float Z, float W) {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }
        public Quaternion(Vector3 axis, float ang) {
            float sinHalf = (float)Math.Sin(Math.PI * ang / 360);
            float cosHalf = (float)Math.Cos(Math.PI * ang / 360);
            X = sinHalf * axis.X;
            Y = sinHalf * axis.Y;
            Z = sinHalf * axis.Z;
            W = cosHalf;
        }
        public Quaternion Copy() {
            return new Quaternion(X, Y, Z, W);
        }
        public float Length() {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }
        public float Angle() {
            return (float)Math.Acos(MetaMath.Clamp(W, -1, 1)) * 2;
        }
        public Quaternion Normalize() {
            float length = this.Length();
            X /= length;
            Y /= length;
            Z /= length;
            W /= length;
            return this;
        }
        public Quaternion Conjugate() {
            return new Quaternion(-X, -Y, -Z, W);
        }
        public Quaternion Mul(Quaternion r) {
            float _W = W * r.W - X * r.X - Y * r.Y - Z * r.Z;
            float _X = X * r.W + W * r.X - Z * r.Y + Y * r.Z;
            float _Y = Y * r.W + Z * r.X + W * r.Y - X * r.Z;
            float _Z = Z * r.W - Y * r.X + X * r.Y + W * r.Z;
            W = _W; X = _X; Y = _Y; Z = _Z;
            return this;
        }
        public Quaternion Mul(Vector3 r) {
            float _W = -X * r.X - Y * r.Y - Z * r.Z;
            float _X = W * r.X - Z * r.Y + Y * r.Z;
            float _Y = Z * r.X + W * r.Y - X * r.Z;
            float _Z = -Y * r.X + X * r.Y + W * r.Z;
            W = _W; X = _X; Y = _Y; Z = _Z;
            return this;
        }
        public Vector3 Rotate(Vector3 v) {
            Quaternion w = Copy().Mul(v).Mul(Conjugate());
            Vector3 res = new Vector3(w.X, w.Y, w.Z);
            return res;
        }
    }
}
