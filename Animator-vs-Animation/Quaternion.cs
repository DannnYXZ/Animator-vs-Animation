using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Animator_vs_Animation {
    class Quaternion {
        float X;
        float Y;
        float Z;
        float W;
        public static Quaternion Identity = new Quaternion(0, 0, 0, 1);
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
        public float Length() {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
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
            return new Quaternion(_X, _Y, _Z, _W);
        }
        public Quaternion Mul(Vector3 r) {
            float _W = -X * r.X - Y * r.Y - Z * r.Z;
            float _X = W * r.X - Z * r.Y + Y * r.Z;
            float _Y = Z * r.X + W * r.Y - X * r.Z;
            float _Z = X * r.Y - Y * r.X + W * r.Z;
            return new Quaternion(_X, _Y, _Z, _W);
        }
        public Vector3 Rotate(Vector3 v) {
            Quaternion w = this.Mul(v).Mul(this.Conjugate());
            Vector3 res = new Vector3(w.X, w.Y, w.Z);
            return res;
        }
    }
}
