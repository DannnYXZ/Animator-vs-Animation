namespace Animator_vs_Animation {
    class Entity {
        public Joint Pivot { get; }
        static int idCounter = 1;
        int weight;
        public int ID { get; }
        public int Weight {
            get { return weight; }
            set { weight = value > 0 ? value : 0; }
        }
        public Entity() {
            Pivot = new Joint("Pivot");
            ID = idCounter++;
        }
    }
}
