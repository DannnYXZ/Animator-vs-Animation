using ExtendedMath;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rig {
    class Joint {
        Joint parent = null;
        public string Name { get; set; }
        public Vector3 StartOffset { get; set; } = Vector3.Zero;
        public Vector3 Position { get; private set; } = Vector3.Zero;
        public Quaternion Quaternion { get; set; } = Quaternion.Identity;
        public Vector3 Axis { get; } = new Vector3(0, 0, 1);
        [JsonIgnore]
        public List<Joint> Joints { get; } = new List<Joint>();
        // public List<Joint> Joints { get; }
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
                Position = Vector3.Zero + Quaternion.Rotate(StartOffset);
            else
                Position = parent.Position + Quaternion.Rotate(StartOffset);
            foreach (Joint child in Joints)
                child.UpdatePos();
        }
        public void Rotate(float ang) {
            Quaternion angRot = new Quaternion(Axis, ang);
            Quaternion.Mul(angRot);
            foreach (Joint child in Joints)
                child.Rotate(ang);
        }
        public void AddChild(Joint joint) {
            joint.parent = this;
            Joints.Add(joint);
        }
        public void Translate(Vector3 shift) {
            StartOffset += shift;
        }
    }
}
