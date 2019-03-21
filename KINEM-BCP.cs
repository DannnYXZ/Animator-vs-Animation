using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Animator_vs_Animation {
    class Kinematics {
        public static Vector3 ForwardKinematics(Joint root, float[] angles) {
            Joint curJoint = root;
            Vector3 prevPoint = root.position;
            Quaternion rotation = Quaternion.Identity;
            for (int i = 0; i < angles.Length && curJoint.joints.Count > 0; i++) {
                curJoint = curJoint.joints[0];
                Quaternion passedRot = new Quaternion(curJoint.axis, angles[i]);
                rotation = rotation.Mul(passedRot);
                prevPoint = prevPoint + rotation.Rotate(curJoint.startOffset);
            }
            return prevPoint;
        }
        public static float DistanceFromTarget(Joint rootJoint, Vector3 target, float[] angles) {
            Vector3 point = ForwardKinematics(rootJoint, angles);
            return Vector3.Distance(point, target);
        }
        private const float SamplingDistance = 0.05f;
        public static float PartialGradient(Joint rootJoint, Vector3 target, float[] angles, int i) {
            float saveAngle = angles[i];
            float f_x = DistanceFromTarget(rootJoint, target, angles);
            angles[i] += SamplingDistance;
            float f_x_plus_d = DistanceFromTarget(rootJoint, target, angles);
            float gradient = (f_x_plus_d - f_x) / SamplingDistance;
            angles[i] = saveAngle;
            return gradient;
        }
        private const float LearningRate = 0.01f;
        private const float DistanceThreshold = 0.5f;
        // Call repeatedly
        public static void InverseKinematics(Joint rootJoint, Vector3 target) {
            if (DistanceFromTarget(rootJoint, target, angles) < DistanceThreshold)
                return;
            for (int i = 0; i < angles.Length; i++) {
                // Gradient descent
                float gradient = PartialGradient(rootJoint, target, angles, i);
                angles[i] -= LearningRate * gradient;
                // Early termination
                if (DistanceFromTarget(rootJoint, target, angles) < DistanceThreshold)
                    return;
            }
        }
    }
}
