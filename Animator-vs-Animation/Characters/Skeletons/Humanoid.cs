using System.Numerics;
namespace Animator_vs_Animation {
    public enum TRace {
        Orange = 0xFF6D00,
        Red = 0xCC0001,
        White = 0xFFFFFF,
        Black = 0,
        Purple = 0xAC3BAF,
        Yellow = 0xFFCC00,
        Green = 0x66CC00
    }
    class Humanoid : Entity {
        public TRace Race { get; }
        public Humanoid(TRace race) {
            Race = race;
            Pivot.ShowDependencies = false;
            Joint stomach = new Joint("Stomach", new Vector3(0, -20, 0));
            Joint chest = new Joint("Chest", new Vector3(0, -20, 0));
            Joint head = new Joint("Head", new Vector3(0, -20, 0));
            chest.AddChild(head);
            stomach.AddChild(chest);
            Pivot.AddChild(stomach);
            Joint upperArmL = new Joint("Upper Arm.L", new Vector3(-20, -40, 0));
            Joint lowerArmL = new Joint("Lower Arm.L", new Vector3(-20, 10, 0));
            upperArmL.AddChild(lowerArmL);
            Pivot.AddChild(upperArmL);

            Joint upperArmR = new Joint("Upper Arm.R", new Vector3(20, -40, 0));
            Joint lowerArmR = new Joint("Lower Arm.R", new Vector3(20, 10, 0));
            upperArmR.AddChild(lowerArmR);
            Pivot.AddChild(upperArmR);

            Joint upperLegR = new Joint("Upper Leg.R", new Vector3(-20, 30, 0));
            Joint lowerLegR = new Joint("Lower Leg.R", new Vector3(0, 30, 0));
            upperLegR.AddChild(lowerLegR);
            Pivot.AddChild(upperLegR);

            Joint upperLegL = new Joint("Upper Leg.L", new Vector3(20, 30, 0));
            Joint lowerLegL = new Joint("Lower Leg.L", new Vector3(0, 30, 0));
            upperLegL.AddChild(lowerLegL);
            Pivot.AddChild(upperLegL);
        }
    }
}
