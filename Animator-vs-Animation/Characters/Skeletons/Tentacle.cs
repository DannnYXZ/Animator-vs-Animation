using Animator_vs_Animation;
using ExtendedMath;
using Rig;

namespace Characters {
    class Tentacle : Entity {
        private int segments = 5;
        private int segLength = 50;
        public Tentacle() : base() {
            Joint ptr = Pivot;
            for (int i = 0; i < segments; i++) {
                Joint newJoint = new Joint("Segment " + (i + 1).ToString(), new Vector3(segLength, segLength, 0));
                ptr.AddChild(newJoint);
                ptr = newJoint;
            }
        }
        public Tentacle(string name, int segments, float segLength) : base(name) {
            Name = name;
            this.segments = segments;
            Joint ptr = Pivot;
            for (int i = 0; i < segments; i++) {
                Joint newJoint = new Joint("Segment " + (i + 1).ToString(), new Vector3(segLength, segLength, 0));
                ptr.AddChild(newJoint);
                ptr = newJoint;
            }
        }
        public override string ToString() {
            return "Tentacle: ID=" + ID.ToString();
        }
    }
}
