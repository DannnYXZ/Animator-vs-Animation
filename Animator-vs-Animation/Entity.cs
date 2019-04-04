using System.ComponentModel;

namespace Animator_vs_Animation {
    class Entity {
        public Joint Pivot { get; }
        static int idCounter = 1;
        int weight;
        private string name;
        public string Name {
            get { return name; }
            set {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public int ID { get; }
        public int Weight {
            get { return weight; }
            set { weight = value > 0 ? value : 0; }
        }
        public Entity() {
            Pivot = new Joint("Pivot");
            ID = idCounter++;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
