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
        public void AddChild(Joint joint) {
            joint.parent = this;
            joints.Add(joint);
            joint.Translate(this.position - joint.position);
            joint.position = this.position + joint.startOffset;
        }
        public Joint(string name, Vector3 shift) {
            this.name = name;
            this.startOffset = shift;
        }
        public void Translate(Vector3 shift) {
            position += shift;
            foreach (Joint joint in joints)
                joint.Translate(shift);
        }
    }
}
