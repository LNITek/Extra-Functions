using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExtraFunctions.ExGenerators
{
    /// <summary>
    /// WIP. 
    /// Generates Partial Class With INotifyPropertyChanged For Your Properties And Fields.
    /// </summary>
    [Generator]
    public class PropertyChangedGenerator : ISourceGenerator
    {
        /// <summary>
        /// Init Generator
        /// </summary>
        public void Initialize(GeneratorInitializationContext context)
        {
            //if(!Debugger.IsAttached) Debugger.Launch();
            string Attributes = $@"using System;

namespace ExtraFunctions.ExGenerators
{{
    /// <summary>
    /// Generate A ProperyChanged Propery For This Field Or Property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class NotifyChanged : Attribute
    {{
        /// <summary>
        /// The PropertyName Of Your Property.
        /// </summary>
        public string PropertyName {{ get; internal set; }}
        /// <summary>
        /// Oher Properties To Be Notified Of Change Along With This Property.
        /// </summary>
        public string[] Properties {{ get; internal set; }} = new string[0];
        /// <summary>
        /// Set true to include other attributes to be used on the generated property.
        /// </summary>
        public bool Attributes {{ get; set; }} = false;

        /// <summary>
        /// Generate A ProperyChanged Propery For This Field.
        /// </summary>
        public NotifyChanged() => PropertyName = string.Empty;
        /// <summary>
        /// Generate A ProperyChanged Propery For This Field.
        /// </summary>
        /// <param name=""PropertyName"">The PropertyName Of Your Property.</param>
        public NotifyChanged(string PropertyName) => this.PropertyName = PropertyName;
        /// <summary>
        /// Generate A ProperyChanged Propery For This Field.
        /// </summary>
        /// <param name=""Properties"">Oher Properties To Be Notified Of Change Along With This Property.</param>
        public NotifyChanged(string[] Properties)
        {{
            PropertyName = string.Empty;
            this.Properties = Properties;
        }}
        /// <summary>
        /// Generate A ProperyChanged Propery For This Field.
        /// </summary>
        /// <param name=""PropertyName"">The PropertyName Of Your Property.</param>
        /// <param name=""Properties"">Oher Properties To Be Notified Of Change Along With This Property.</param>
        public NotifyChanged(string PropertyName, string[] Properties)
        {{
            this.PropertyName = PropertyName;
            this.Properties = Properties;
        }}
    }}
}}";
            context.RegisterForPostInitialization(ctx => ctx.AddSource($"Attributes.g.cs", SourceText.From(Attributes, Encoding.UTF8)));
            context.RegisterForSyntaxNotifications(() => new FieldSyntaxReciever());
        }

        /// <summary>
        /// Build Source Code.
        /// </summary>
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not FieldSyntaxReciever syntaxReciever) return;
            var sourceBuilder = new StringBuilder();
            var notifySymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");
            foreach (var containingClassGroup in syntaxReciever.Identified.GroupBy(x => x.ContainingType ?? x.OriginalDefinition as INamedTypeSymbol))
            {
                if (containingClassGroup == null) continue;
                var containingClass = containingClassGroup.Key;
                var namespc = containingClass?.ContainingNamespace;
                var hasNotifyImplementtion = containingClass?.Interfaces.Contains(notifySymbol);
                if (!(hasNotifyImplementtion ?? false)) continue;
                var source = GenerateClass(context, containingClass, namespc, containingClassGroup.ToList());
                context.AddSource($"{containingClass.Name}_AutoNotify.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private string GenerateClass(GeneratorExecutionContext context, INamedTypeSymbol @class,
            INamespaceSymbol @namespace, List<ISymbol> fields)
        {
            List<string> Props = new();
            var classBuilder = new StringBuilder();
            classBuilder.AppendLine("using System;");
            var notifyPropertyChangedSymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");
            var callerMemberSymbol = context.Compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.CallerMemberNameAttribute");
            classBuilder.AppendLine($"using {notifyPropertyChangedSymbol?.ContainingNamespace};");
            classBuilder.AppendLine($"using {callerMemberSymbol?.ContainingNamespace};");
            classBuilder.AppendLine($@"
namespace {@namespace.ToDisplayString()}
{{
    public partial class {@class.ToDisplayString().Replace(@namespace.ToDisplayString()+".","")} : {notifyPropertyChangedSymbol?.Name}
    {{{ImplementInterface()}");

            foreach (var members in fields.Where(x => x is INamedTypeSymbol prop).Cast<INamedTypeSymbol>()
                .Select(x => x.GetMembers().Where(x => (x is IPropertySymbol p && p.DeclaredAccessibility == Accessibility.Public && !p.IsWriteOnly) 
                || (x is IFieldSymbol f && f.DeclaredAccessibility == Accessibility.Public))))
            {
                if(members == null) continue;
                foreach (var field in members)
                    Props.Add(field.Name);
            }

            foreach (var field in fields.Where(x => x is IFieldSymbol prop).Cast<IFieldSymbol>())
            {
                if (field == null) continue;
                classBuilder.Append(AddProp(field, field.Type.ToDisplayString(), field.IsReadOnly));
            }

            foreach (var field in fields.Where(x => x is IPropertySymbol prop).Cast<IPropertySymbol>())
            {
                if (field == null) continue;
                classBuilder.Append(AddProp(field, field.Type.ToDisplayString(), field.IsReadOnly));
            }

            classBuilder.AppendLine($@"
        public void UpdateAll()
        {{");

            Props.Distinct().ToList().ForEach(x => classBuilder.AppendLine($"            NotifyPropertyChanged(nameof({x}));"));

            classBuilder.AppendLine("        }");
            classBuilder.AppendLine("    }");
            classBuilder.AppendLine("}");
            return classBuilder.ToString();

            string AddProp(ISymbol field, string fieldType, bool ReadOnly)
            {
                var at = field.GetAttributes().First(x => x.AttributeClass?.Name == nameof(NotifyChanged)
                                || x.AttributeClass?.Name == typeof(NotifyChanged).FullName);
                var Attribute = at.NamedArguments.ToDictionary(x => x.Key, x => {
                    if (x.Value.Kind == TypedConstantKind.Array)
                        return x.Value.Values;
                    else
                        return x.Value.Value;
                });
                for (int I = 0; I < at.AttributeConstructor?.Parameters.Count(); I++)
                {
                    Attribute.Add(at.AttributeConstructor.Parameters[I].Name,
                        at.ConstructorArguments[I].Kind == TypedConstantKind.Array ? 
                        at.ConstructorArguments[I].Values.Select(x => x.Value?.ToString()) : at.ConstructorArguments[I].Value);
                }

                var fieldName = field.Name;
                IEnumerable<string?> fieldAtts = [];
                if(Attribute.FirstOrDefault(x => x.Key == nameof(NotifyChanged.Attributes)).Value as bool? ?? false)
                    fieldAtts = field.GetAttributes().Where(x => x.AttributeClass?.Name != nameof(NotifyChanged)
                                && x.AttributeClass?.Name != typeof(NotifyChanged).FullName)
                                .Select(x => x.ToString().Replace("Attribute", ""));
                string newFieldName = Attribute.FirstOrDefault(x => x.Key == nameof(NotifyChanged.PropertyName)).Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(newFieldName)) newFieldName = NormalizePropertyName(fieldName);
                var arrAtt = Attribute.FirstOrDefault(x => x.Key == nameof(NotifyChanged.Properties));
                List<string?> Addons = new();
                if(arrAtt.Value is IEnumerable<string?> add) Addons.AddRange(add);
                if (ReadOnly) { Props.Add(fieldName); return ""; }

                var classBuilder = new StringBuilder();
                Props.Add(newFieldName);
                if (fieldAtts.Any())
                    classBuilder.Append($"\r\n\t\t[{string.Join(", ", fieldAtts)}]");
                classBuilder.Append($@"
        public {fieldType} {newFieldName}
        {{
            get => {fieldName};
            set
            {{
                if((object){fieldName} == (object)value) return;
                {fieldName} = value;
                NotifyPropertyChanged();
");
                Addons?.ForEach(x => classBuilder.AppendLine($"                NotifyPropertyChanged(nameof({x}));"));
                classBuilder.Append($@"            }}
        }}
");
                return classBuilder.ToString();
            }
        }

        private string NormalizePropertyName(string fieldName, bool Upper = true)
        {
            var FieldName = fieldName.Replace("_", "");
            if (string.IsNullOrWhiteSpace(FieldName)) return "Field_" + new Random().Next(100);
            var sb = new StringBuilder();
            if (Upper) sb.Append(FieldName.First().ToString().ToUpper());
            else sb.Append(FieldName.First().ToString().ToLower());
            if (FieldName.Length > 1 ) sb.Append(FieldName.Substring(1));
            if(sb.ToString() == fieldName) return NormalizePropertyName(fieldName, !Upper);
            return sb.ToString();
        }

        private string ImplementInterface()
        {
            return @"
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = """") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));";
        }

        private class FieldSyntaxReciever : ISyntaxContextReceiver
        {
            public List<ISymbol> Identified { get; } = new();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax classDeclaration && classDeclaration.AttributeLists.Any())
                {
                    var Class = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

                    if (Class is INamedTypeSymbol classInfo && classInfo.GetAttributes()
                        .Any(x => x.AttributeClass?.ToDisplayString() == nameof(NotifyChanged)
                                || x.AttributeClass?.ToDisplayString() == typeof(NotifyChanged).FullName))
                        Identified.Add(classInfo);
                }
                if (context.Node is FieldDeclarationSyntax fieldDeclaration && fieldDeclaration.AttributeLists.Any())
                {
                    var variableDeclaration = fieldDeclaration.Declaration.Variables;
                    foreach (var variable in variableDeclaration)
                    {
                        var field = context.SemanticModel.GetDeclaredSymbol(variable);
                        if (field is IFieldSymbol fieldInfo && fieldInfo.GetAttributes()
                                .Any(x => x.AttributeClass?.ToDisplayString() == nameof(NotifyChanged)
                                || x.AttributeClass?.ToDisplayString() == typeof(NotifyChanged).FullName))
                            Identified.Add(fieldInfo);
                    }
                }
                if (context.Node is PropertyDeclarationSyntax propertyDeclaration && propertyDeclaration.AttributeLists.Any())
                {
                    var property = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
                    if (property is IPropertySymbol propertyInfo && propertyInfo.GetAttributes()
                            .Any(x => x.AttributeClass?.ToDisplayString() == nameof(NotifyChanged)
                                || x.AttributeClass?.ToDisplayString() == typeof(NotifyChanged).FullName))
                        Identified.Add(propertyInfo);
                }
            }
        }
    }
}