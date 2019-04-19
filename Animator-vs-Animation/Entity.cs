using Rig;
using System.ComponentModel;

namespace Rig {
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
        public Entity(string name = "Unknown") {
            Name = name;
            ID = idCounter++;
            Pivot = new Joint("Pivot");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
