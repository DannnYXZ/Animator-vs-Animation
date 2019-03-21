using System.Numerics;

namespace Animator_vs_Animation {
    class Tentacle: Entity {
        public Tentacle(int segments, float segLength) : base() {
            Joint ptr = Pivot;
            for (int i = 0; i < segments; i++) {
                Joint newJoint = new Joint(new Vector3(segLength, segLength, 0));
                ptr.AddChild(newJoint);
                ptr = newJoint;
            }
        }
        public override string ToString() {
            return "Tentacle: ID=" + ID.ToString();
        }
    }
}
