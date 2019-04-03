using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Characters;

namespace Animator_vs_Animation.Reflection {
    class ReflectionTools {
        public static void ListTypesInfo() {
            var assembly = Assembly.GetExecutingAssembly();
            //assembly = Assembly.GetAssembly("Ani")
            Console.WriteLine(assembly.FullName);

            //ConstructorInfo[] ci = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var types = assembly.GetTypes();
            foreach(var type in types) {
                Console.WriteLine("Type: " + type.Name + " BaseType: " + type.BaseType);
                var props = type.GetProperties();
                foreach(var prop in props) {
                    Console.WriteLine("\tProperty: " + prop.Name + " Type: " + prop.PropertyType);
                }
                var fields = type.GetFields();
                foreach (var field in fields) {
                    Console.WriteLine("\tField: " + field.Name);
                }
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (var method in methods) {
                    Console.WriteLine("\tMethod: " + method.Name);
                }
            }

            var _type = typeof(King);
            Object obj = Activator.CreateInstance(_type, new Object[] { TRace.Black});
            var _prop = _type.GetProperty("Name");
            Console.WriteLine(_prop.GetValue(obj));
            var _method = _type.GetMethod("SaySomething");
            _method.Invoke(obj, null);
            //Type tt = typeof(Character);
            //Object obj = Activator.CreateInstance(tt, new Object[] { TRace.Black});
            //PropertyInfo prop = tt.GetProperty("Pivot");
            //prop.SetValue(obj, new);
            //string name = (string)prop.GetValue(obj, null);
            //name = (string)prop.GetValue(obj, null);
            
        }
    }
}
