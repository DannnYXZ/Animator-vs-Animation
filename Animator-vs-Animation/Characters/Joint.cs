using System.Collections.Generic;
using System.Numerics;

namespace Animator_vs_Animation {
    class Joint {
        Joint parent;
        public string Name { get; set; }
        public Vector3 StartOffset { get; set; } = Vector3.Zero;
        public Vector3 Position { get; private set; } = new Vector3(0, 0, 0);
        public Quaternion Quaternion { get; set; } = Quaternion.Identity;
        public Vector3 Axis { get; } = new Vector3(0, 0, 1);
        public List<Joint> Joints { get; private set; } = new List<Joint>();
        public bool ShowDependencies { get; set; } = true;
        public Joint() { }
        public Joint(string name) {
            Name = name;
        }
        public Joint(Vector3 shift) {
            StartOffset = shift;
        }
        public Joint(string name, Vector3 shift) {
            Name = name;
            StartOffset = shift;
        }
        public void UpdatePos() {
            if (parent == null)
                Position = Vector3.Zero + StartOffset;
            else
                Position = parent.Position + StartOffset;
            foreach (Joint child in Joints)
                child.UpdatePos();
        }
        public void Rotate(float ang) {
            Quaternion angRot = new Quaternion(Axis, ang);
            foreach (Joint child in Joints) {
                child.StartOffset = angRot.Rotate(child.StartOffset);
                child.UpdatePos();
                child.Rotate(ang);
            }
            Quaternion.Mul(angRot);
        }
        public void AddChild(Joint joint) {
            joint.parent = this;
            Joints.Add(joint);
            joint.UpdatePos();
        }
        public void Translate(Vector3 shift) {
            StartOffset += shift;
            UpdatePos();
            foreach (Joint joint in Joints)
                joint.UpdatePos();
        }
    }
}
