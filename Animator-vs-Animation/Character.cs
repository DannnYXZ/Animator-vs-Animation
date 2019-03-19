using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Animator_vs_Animation {
    class Character {
        public Joint body;
#if false
        public Character() {
            body = new Joint();
            Joint stomach = new Joint("Stomach", new Vector3(0, 20, 0));
            Joint chest = new Joint("Chest", new Vector3(0, 20, 0));
            Joint head = new Joint("Head", new Vector3(0, 20, 0));
            chest.AddChild(head);
            stomach.AddChild(chest);
            body.AddChild(stomach);
            Joint upperArmL = new Joint("Upper Arm.L", new Vector3(-20, 40, 0));
            Joint lowerArmL = new Joint("Upper Arm.L", new Vector3(-20, -10, 0));
            upperArmL.AddChild(lowerArmL);
            body.AddChild(upperArmL);

            Joint upperArmR = new Joint("Upper Arm.R", new Vector3(20, 40, 0));
            Joint lowerArmR = new Joint("Upper Arm.R", new Vector3(20, -10, 0));
            upperArmR.AddChild(lowerArmR);
            body.AddChild(upperArmR);

            Joint upperLegR = new Joint("Upper Leg.R", new Vector3(-20, -30, 0));
            Joint lowerLegR = new Joint("Upper Leg.R", new Vector3(0, -30, 0));
            upperLegR.AddChild(lowerLegR);
            body.AddChild(upperLegR);

            Joint upperLegL = new Joint("Upper Leg.L", new Vector3(20, -30, 0));
            Joint lowerLegL = new Joint("Upper Leg.L", new Vector3(0, -30, 0));
            upperLegL.AddChild(lowerLegL);
            body.AddChild(upperLegL);
        }
#endif
#if true
        public Character() {
            body = new Joint();
            Joint ptr = body;
            for(int i = 0; i < 20; i++) {
                Joint newJoint = new Joint(new Vector3(70, 70, 0));
                ptr.AddChild(newJoint);
                ptr = newJoint;
            }
        }
#endif
    }
}
