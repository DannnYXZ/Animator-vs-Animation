using System;

namespace Characters {
    class King : Character {
        public King() : base() { }
        public King(TRace race) : base(race) { }
        public override void SaySomething() {
            Console.WriteLine(@"
                             .
                            / \
                           _\ /_
                 .     .  (,'v`.)  .     .
                 \)   ( )  ,' `.  ( )   (/
                  \`. / `-'     `-' \ ,'/
                   : '    _______    ' :
                   |  _,-'  ,-.  `-._  |
                   |,' ( )__`-'__( ) `.|
                   (|,-,'-._   _.-`.-.|)
                   /  /<( o)> <( o)>\  \
                   :  :     | |     :  :
                   |  |     ; :     |  |
                   |  |    (.-.)    |  |
                   |  |  ,' ___ `.  |  |
                   ;  |)/ ,'---'. \(|  :
               _,-/   |/\(       )/\|   \-._
         _..--'.-(    |   `-'''-'   |    )-.`--.._
                  `.  ;`._________,':  ,'
                 ,' `/               \'`.
                      `------.------'");
        }
        public override string ToString() {
            return "King: ID=" + ID.ToString() + ", Name=" + Name;
        }
    }
}
