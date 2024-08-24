using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ExtraFunctions.ExSettings
{
    /// <summary>
    /// WIP. 
    /// Generates Your Settings Into A CSharp File For Easy Use.
    /// </summary>
    [Generator]
    public class SettingsGenerator : IIncrementalGenerator
    {
        /// <summary>
        /// The Path To Your Settings File. Default Is null;
        /// </summary>
        public static string? SettingFilePath = null;

        /// <summary>
        /// Initialises The Generator.
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            Debug.WriteLine("Initalize code SettingsGenerator");
            // Using the context, get any additional files that end in .xmlsettings
            var settingsFiles = context.AdditionalTextsProvider.Where(static at => at.Path.EndsWith(".setx"))
                .Select((text, cancellationToken) => 
                (path: Path.GetFileName(text.Path), content: text.GetText(cancellationToken)!.ToString()));
            
            context.RegisterSourceOutput(settingsFiles, ProcessSettingsFile);
        }

        private void ProcessSettingsFile(SourceProductionContext context, (string path, string content) xmlFile)
        {
            Debug.WriteLine("Execute code SettingsGenerator");
            Debug.WriteLine(xmlFile.path + " | " + xmlFile.content);

            XmlDocument xmlDoc = new();
            try { xmlDoc.LoadXml(xmlFile.content); }
            catch { return; }

            string Name = xmlDoc.DocumentElement.GetAttribute("PropertyName");
            if (string.IsNullOrWhiteSpace(Name)) Name = "default";

            StringBuilder sb = new($@"
namespace ExtraFunctions.ExSettings
{{
    using System;
    using System.Xml;

    public static class {Name}Settings
    {{
        static XmlDocument xmlDoc = new XmlDocument();

        private static string path {{ get => ""{xmlFile.path}""; }}

        static {Name}Settings()
        {{
            xmlDoc.Load(path);
        }}
");

            for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes.Count; i++)
            {
                XmlElement setting = (XmlElement)xmlDoc.DocumentElement.ChildNodes[i];
                string settingName = setting.GetAttribute("PropertyName");
                string settingType = setting.GetAttribute("Type");

                if (string.IsNullOrEmpty(settingName)) settingName = "Setting_" + i.ToString();
                if (string.IsNullOrEmpty(settingType)) settingType = "object";

                sb.Append($@"

        public static {settingType} {settingName}
        {{ 
            get => ({settingType})Convert.ChangeType(((XmlElement)xmlDoc.DocumentElement.ChildNodes[{i}]).InnerText, typeof({settingType})); 
            set {{
                    ((XmlElement)xmlDoc.DocumentElement.ChildNodes[{i}]).InnerText = value.ToString(); 
                    xmlDoc.Save(path);
                }}
        }}");
            }

            sb.Append("} }");

            context.AddSource($"Settings_{Name}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
