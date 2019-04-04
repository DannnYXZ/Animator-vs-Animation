using Animator_vs_Animation;
using System;
using System.ComponentModel;

namespace Characters {
    class Character : Humanoid {
        public Character(TRace race) : base(race) {
            Name = "Mr. Nobody";
        }
        public Character(string name, TRace race) : base(race) {
            Name = name;
        }
        public virtual void SaySomething() {
            Console.WriteLine("I'm " + Name + " and Life's GooD ^---^!");
        }
        public override string ToString() {
            return "Character: ID=" + ID.ToString() + ", Name=" + Name;
        }
    }
}
