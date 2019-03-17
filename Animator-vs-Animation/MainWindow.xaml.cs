using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Animator_vs_Animation {
    public partial class MainWindow : Window {
        Character character;
        Drawer drawer;
        public MainWindow() {
            InitializeComponent();
            character = new Character();
            character.body.Translate(new Vector3(100, 100, 0));
            drawer = new Drawer(worldCanvas);
            drawer.DrawCharacter(character);
        }
    }
}
