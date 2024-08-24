using System;

namespace ExtraFunctions.ExGenerators
{
    /// <summary>
    /// Generate A ProperyChanged Propery For This Field Or Property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class NotifyChanged : Attribute
    {
        /// <summary>
        /// The PropertyName Of Your Property.
        /// </summary>
        public string PropertyName { get; internal set; }
        /// <summary>
        /// Oher Properties To Be Notified Of Change Along With This Property.
        /// </summary>
        public string[] Properties { get; internal set; } = new string[0];
        /// <summary>
        /// Set true to include other attributes to be used on the generated property.
        /// </summary>
        public bool Attributes { get; set; } = false;

        /// <summary>
        /// Generate A ProperyChanged Propery For This Field.
        /// </summary>
        public NotifyChanged() => PropertyName = string.Empty;
        /// <summary>
        /// Generate A ProperyChanged Propery For This Field.
        /// </summary>
        /// <param name="PropertyName">The PropertyName Of Your Property.</param>
        public NotifyChanged(string PropertyName) => this.PropertyName = PropertyName;
        /// <summary>
        /// Generate A ProperyChanged Propery For This Field.
        /// </summary>
        /// <param name="Properties">Oher Properties To Be Notified Of Change Along With This Property.</param>
        public NotifyChanged(string[] Properties)
        {
            PropertyName = string.Empty;
            this.Properties = Properties;
        }
        /// <summary>
        /// Generate A ProperyChanged Propery For This Field.
        /// </summary>
        /// <param name="PropertyName">The PropertyName Of Your Property.</param>
        /// <param name="Properties">Oher Properties To Be Notified Of Change Along With This Property.</param>
        public NotifyChanged(string PropertyName, string[] Properties)
        {
            this.PropertyName = PropertyName;
            this.Properties = Properties;
        }
    }
}