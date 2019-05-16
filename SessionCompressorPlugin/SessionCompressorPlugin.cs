using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Rig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Plugin.SessionCompressor {
    public class SessionCompressor : IPlugin {
        IHost host;
        List<Entity> entities;
        public string Name => "Session Comressor Plugin";

        public string Description => "Serialize/Deserialize List<Entity>";

        public object Execute(object obj) {
            if (obj.GetType() == typeof(List<Entity>))
                entities = (List<Entity>)obj;
            return null;
        }

        Expander expander = null;
        StackPanel pluginBlock;
        Button btnSerialize, btnDeserialize;
        CheckBox modeBox;
        public FrameworkElement GetUI() {
            if (expander == null) {
                expander = new Expander();
                expander.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3CAEA3"));
                expander.Header = Name;
                pluginBlock = new StackPanel();
                // check for cryprto plugin
                if (host.Plugins.Contains("Scrambler Plugin")) {
                    modeBox = new CheckBox();
                    var caption = new TextBlock();
                    caption.Text = "Scramble";
                    modeBox.Content = caption;
                    pluginBlock.Children.Add(modeBox);
                }
                btnSerialize = new Button();
                btnSerialize.Height = 30;
                btnSerialize.Content = "SERIALIZE";
                btnSerialize.Click += BtnSerialize_Click;
                btnDeserialize = new Button();
                btnDeserialize.Click += BtnDeserialize_Click;
                btnDeserialize.Height = 30;
                btnDeserialize.Content = "DESERIALIZE";
                pluginBlock.Children.Add(btnSerialize);
                pluginBlock.Children.Add(btnDeserialize);
                expander.Content = pluginBlock;
            }
            return expander;
        }

        public class CustomContractResolver : DefaultContractResolver {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
                string[] propsToIgnore = { "Joints" };
                properties = properties.Where(p => !propsToIgnore.Contains(p.PropertyName)).ToList();
                return properties;
            }
        }

        private void BtnDeserialize_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON | *.json";
            openFileDialog.Title = "Open Session";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "") {
                List<Entity> objects = new List<Entity>();
                var jsonSettings = new JsonSerializerSettings {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All
                };
                jsonSettings.ContractResolver = new CustomContractResolver();
                jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                try {
                    objects = JsonConvert.DeserializeObject<List<Entity>>(File.ReadAllText(openFileDialog.FileName), jsonSettings);
                    entities.Clear();
                    entities.AddRange(objects);
                    // plugin interaction
                    var hierarchyPlugin = host.getPlugin("Hierarchy Plugin");
                    if (hierarchyPlugin != null)
                        hierarchyPlugin.Execute(entities);
                } catch {
                    try {
                        var scramblerPlugin = host.getPlugin("Scrambler Plugin");
                        if (scramblerPlugin != null) {
                            scramblerPlugin.SetArgument("mode", "decode");
                            string json = Encoding.UTF8.GetString(scramblerPlugin.Execute(File.ReadAllBytes(openFileDialog.FileName)) as byte[]);
                            objects = JsonConvert.DeserializeObject<List<Entity>>(json, jsonSettings);
                            entities.Clear();
                            entities.AddRange(objects);
                        }
                    } catch (Exception exp) {
                        MessageBox.Show(exp.Message);
                        Console.WriteLine(exp.Message);
                    }
                }
            }
        }

        private void BtnSerialize_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON | *.json";
            saveFileDialog.Title = "Save Session";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "") {
                try {
                    string json = JsonConvert.SerializeObject(
                        entities,
                        Formatting.Indented,
                        new JsonSerializerSettings {
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            TypeNameHandling = TypeNameHandling.All
                        });
                    // using scrambler
                    var scramblerPlugin = host.getPlugin("Scrambler Plugin");
                    if (scramblerPlugin != null && (modeBox.IsChecked ?? false)) {
                        scramblerPlugin.SetArgument("mode", "encode");
                        File.WriteAllBytes(saveFileDialog.FileName, scramblerPlugin.Execute(Encoding.UTF8.GetBytes(json)) as byte[]);
                    } else {
                        using (StreamWriter fs = new StreamWriter(saveFileDialog.FileName)) {
                            fs.Write(json);
                        }
                    }
                } catch (Exception exeption) {
                    Console.WriteLine(exeption.Message);
                }
            }
            return;
        }

        public void Initialize(IHost host) {
            this.host = host;
        }

        public void SetArgument(string key, object value) {
        }
    }
}
