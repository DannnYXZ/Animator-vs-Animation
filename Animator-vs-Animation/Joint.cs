using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Animator_vs_Animation {
    class Joint {
        Joint parent;
        public string name;
        public Vector3 startOffset = Vector3.Zero;
        public Vector3 position = new Vector3(0, 0, 0);
        public Quaternion rot = Quaternion.Identity;
        // rotation limitation
        public Vector3 axis = new Vector3(0, 0, 1);
        public float MinAngle;
        public float MaxAngle;


        public List<Joint> joints = new List<Joint>();
        public bool showDependencies = true;
        public Joint() { }
        public Joint(Vector3 shift) {
            this.startOffset = shift;
        }
        public Joint(string name, Vector3 shift) {
            this.name = name;
            this.startOffset = shift;
        }
        private void UpdatePos() {
            if (parent == null)
                position = Vector3.Zero + startOffset;
            else
                this.position = parent.position + this.startOffset;
            foreach (Joint child in joints)
                child.UpdatePos();
        }
        public void Rotate(float ang) {
            Quaternion angRot = new Quaternion(this.axis, ang);
            foreach (Joint child in joints) {
                child.startOffset = angRot.Rotate(child.startOffset);
                child.UpdatePos();
                child.Rotate(ang);
            }
            this.rot = this.rot.Mul(angRot);
        }
        public void AddChild(Joint joint) {
            joint.parent = this;
            joints.Add(joint);
            joint.UpdatePos();
        }
        public void Translate(Vector3 shift) {
            this.startOffset += shift;
            UpdatePos();
            foreach (Joint joint in joints)
                joint.UpdatePos();
        }
    }
}
