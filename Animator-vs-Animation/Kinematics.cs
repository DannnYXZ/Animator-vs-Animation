using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Animator_vs_Animation {
    class Kinematics {
#if false
        public Vector3 ForwardKinematics(float[] angles) {
            Vector3 prevPoint = joints[0].position;
            Quaternion rotation = Quaternion.Identity;
            for (int i = 1; i < joints.Count; i++) {
                rotation *= new Quaternion(joints[i - 1].axis, angles[i - 1]);
                Vector3 nextPoint = prevPoint + Vector3.Transform(joints[i].startOffset, rotation);
                prevPoint = nextPoint;
            }
            return prevPoint;
        }
#endif
        public static Vector3 ForwardKinematics(Joint root, float[] angles) {
            Joint jointPtr = root;
            Vector3 prevPoint = root.position;
            Quaternion rotation = Quaternion.Identity;
            for (int i = 0; i < angles.Length && jointPtr.joints.Count > 0; i++) {
                jointPtr = jointPtr.joints[0];
                Quaternion q = new Quaternion(jointPtr.axis, angles[i]);
                //rotation = rotation q;
                //Vector3 nextPoint = prevPoint + Vector3.Transform(jointPtr.startOffset, rotation);
                //prevPoint = nextPoint;
            }
            return prevPoint;
        }
        /*
        public float DistanceFromTarget(Vector3 target, float[] angles) {
            Vector3 point = ForwardKinematics(angles);
            return Vector3.Distance(point, target);
        }
        */

        /*
        private const float SamplingDistance = 0.001f;
        public float PartialGradient(Vector3 target, float[] angles, int i) {
            // Saves the angle,
            // it will be restored later
            float angle = angles[i];
            // Gradient : [F(x+SamplingDistance) - F(x)] / h
            float f_x = DistanceFromTarget(target, angles);
            angles[i] += SamplingDistance;
            float f_x_plus_d = DistanceFromTarget(target, angles);
            float gradient = (f_x_plus_d - f_x) / SamplingDistance;
            // Restores
            angles[i] = angle;
            return gradient;
        }
        private const float LearningRate = 0.001f;
        private const float DistanceThreshold = 0.5f;
        //call repeatedly
        public void InverseKinematics(Vector3 target, float[] angles) {
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
                return;
            for (int i = 0; i < joints.Count; i++) {
                // Gradient descent
                float gradient = PartialGradient(target, angles, i);
                angles[i] -= LearningRate * gradient;
                // Early termination
                if (DistanceFromTarget(target, angles) < DistanceThreshold)
                    return;
            }
        }
        */
    }
}
