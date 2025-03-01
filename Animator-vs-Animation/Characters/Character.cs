﻿using System;

namespace Animator_vs_Animation.Characters {
    class Character : Humanoid {
        string Name { get; }
        public Character(TRace race) : base(race) {
            Name = "Mr. Nobody";
        }
        public Character(string name, TRace race) : base(race) {
            Name = name;
        }
        public virtual void SaySomething() {
            Console.WriteLine("I'm "+ Name + " and Life's GooD ^---^!");
        }
        public override string ToString() {
            return "Character: ID=" + ID.ToString() + ", Name=" + Name;
        }
    }
}
