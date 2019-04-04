using System.Numerics;

namespace Animator_vs_Animation {
    static class Kinematics {
        public static Vector3 ForwardKinematics(Joint root, float angle) {
            Joint curJoint = root;
            Vector3 prevPoint = root.Position;
            Quaternion rotation = Quaternion.Identity;
            while (curJoint.Joints.Count > 0) {
                curJoint = curJoint.Joints[0];
                Quaternion passedRot = new Quaternion(curJoint.Axis, angle);
                rotation.Mul(passedRot);
                prevPoint = prevPoint + rotation.Rotate(curJoint.StartOffset);
            }
            return prevPoint;
        }
        public static float DistanceFromTarget(Joint rootJoint, Vector3 target, float angle) {
            Vector3 point = ForwardKinematics(rootJoint, angle);
            return Vector3.Distance(point, target);
        }
        private const float samplingDistance = 2f;
        public static float PartialGradient(Joint rootJoint, Vector3 target) {
            float angle = rootJoint.Quaternion.Angle();
            float f_x = DistanceFromTarget(rootJoint, target, angle);
            angle += samplingDistance;
            float f_x_plus_d = DistanceFromTarget(rootJoint, target, angle);
            float gradient = (f_x_plus_d - f_x) / samplingDistance;
            return gradient;
        }
        private const float LearningRate = 0.2f;
        private const float DistanceThreshold = 1f;
        public static void InverseKinematics(Joint rootJoint, Vector3 target) {
            float curAngle = rootJoint.Quaternion.Angle();
            if (DistanceFromTarget(rootJoint, target, curAngle) < DistanceThreshold)
                return;
            if (rootJoint.Joints.Count > 0) {
                float gradient = PartialGradient(rootJoint, target);
                float dAng = -LearningRate * gradient;
                rootJoint.Rotate(dAng);
                if (DistanceFromTarget(rootJoint, target, curAngle) < DistanceThreshold)
                    return;
                InverseKinematics(rootJoint.Joints[0], target);
            }
        }
    }
}
