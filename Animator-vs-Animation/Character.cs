using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Animator_vs_Animation {
    class Character {
        public Joint body;
        public Character() {
            body = new Joint();
            body.AddChild(new Joint("Stomach",new Vector3(0, 0, 0)));
            Joint UpperArmL = new Joint("Upper Arm.L", new Vector3(20, -80, 0));
            Joint LowerArmL = new Joint("Upper Arm.L", new Vector3(30, 0, 0));
            UpperArmL.AddChild(LowerArmL);
            Joint UpperArmR = new Joint("Upper Arm.R", new Vector3(-20, -80, 0));
            Joint LowerArmR = new Joint("Upper Arm.R", new Vector3(-20, -80, 0));
            UpperArmR.AddChild(LowerArmR);
            body.AddChild(UpperArmL);
            body.AddChild(UpperArmR);
            body.AddChild(new Joint("Upper Leg.L", new Vector3(20, 0, 0)));
            body.AddChild(new Joint("Upper Leg.R", new Vector3(-20, 0, 0)));
        }
        // OUTPUT - last joint position
        // non-modifying points???
        /*
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
        public float DistanceFromTarget(Vector3 target, float[] angles) {
            Vector3 point = ForwardKinematics(angles);
            return Vector3.Distance(point, target);
        }
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
