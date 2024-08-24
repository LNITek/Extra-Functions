using System.ComponentModel;
using System.Reflection;

namespace ExtraFunctions.ExDatabase
{
    /// <summary>
    /// A Field Or Column In The Table
    /// </summary>
    public class ExField
    {
        /// <summary>
        /// The name of the field
        /// </summary>
        public string Name { get; }
        //public string Description { get; set; }
        /// <summary>
        /// The default value of the field if its Null
        /// </summary>
        public object? DefaultValue { get; } = null;
        /// <summary>
        /// The lenght or max value of the field
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// The field type
        /// </summary>
        public Type? DataType { get; internal set; } = null;
        internal bool DefaultNull { get; set; } = true;

        /// <summary>
        /// Is Auto Incroment
        /// </summary>
        public bool AUTO { get; }
        /// <summary>
        /// Is Primary Key
        /// </summary>
        public bool PK { get; }
        /// <summary>
        /// Is Nullable
        /// </summary>
        public bool Nullable { get; } = true;
        /// <summary>
        /// Any or all referances to previuse names of the field. To retain or transfur the data.
        /// </summary>
        public IEnumerable<string> PreNames { get; set; } = [];

        /// <summary>
        /// Create A Field To Be Used In The Table
        /// </summary>
        /// <param name="DataType">The data type that this field uses</param>
        /// <param name="Name">The name of the field</param>
        /// <param name="DefaultValue">The default value to use if its value is null</param>
        /// <param name="Nullable">Is Nullabill</param>
        /// <param name="PK">Is Primary Key</param>
        /// <param name="AUTO">Is Field Auto Incriment</param>
        /// <param name="Length">Lenght Or Max Value Of The Field</param>
        public ExField(Type DataType, string Name, object? DefaultValue = null, bool Nullable = false, bool PK = false, bool AUTO = false, 
            int Length = 0)
        {
            this.AUTO = AUTO;
            this.DataType = DataType;
            if(DataType == typeof(uint)) this.AUTO = true;

            this.Name = Name;
            this.DefaultValue = DefaultValue;
            if (this.AUTO) this.DefaultValue = "AUTO";

            this.PK = PK;
            this.Nullable = Nullable;
            this.Length = Length;
        }

        internal ExField(PropertyInfo Property)
        {
            foreach (var item in Property.GetCustomAttributes<Rename>().Select(x => x.PreNames))
                PreNames = PreNames.Concat(item);

            Name = Property.Name;
            DataType = Property.PropertyType;

            DefaultValue = Property.GetCustomAttribute<Default>()?.Value;
            DefaultValue ??= Property.GetCustomAttribute<DefaultValueAttribute>()?.Value;
            Length = Property.GetCustomAttribute<FieldLenght>()?.Lenght ?? 0;

            PK = Property.GetCustomAttribute<PrimeryKey>() != null;

            if (DefaultNull)
            {
                Nullable = Property.GetCustomAttribute<Nullable>() != null;
                var NullT = System.Nullable.GetUnderlyingType(DataType);
                if (NullT != null)
                {
                    Nullable = true;
                    DataType = NullT;
                }
            }
            else
                Nullable = Property.GetCustomAttribute<Nullable>() == null;

            if (PK) Nullable = false;

            if (DataType == typeof(uint) || Property.GetCustomAttribute<AutoNumber>() != null)
            {
                AUTO = true;
                DefaultValue = "AUTO";
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name} {DataType} ({Length}) : {DefaultValue ?? ""}";
        }

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();
        /// <inheritdoc/>
        public override bool Equals(object? obj) => base.Equals(obj);
        /// <inheritdoc/>
        public static bool operator ==(ExField? left, ExField? right)
        {
            if ((left?.Equals(null) ?? true) && (right?.Equals(null) ?? true)) return true;
            if ((left?.Equals(null) ?? true) ^ (right?.Equals(null) ?? true)) return false;

            if (left?.Nullable == right?.Nullable &&
                left?.AUTO == right?.AUTO &&
                left?.PK == right?.PK &&
                //left?.FK == right?.FK && 
                left?.Name == right?.Name &&
                left?.DefaultValue?.ToString() == right?.DefaultValue?.ToString() &&
                left?.Length == right?.Length)
            {
                if (left?.DataType == right?.DataType) return true;
                if (left?.DataType == typeof(Enum) && right?.DataType == typeof(Enum)) return true;
                if (left?.DataType == typeof(Enum) && (right?.DataType?.IsEnum ?? false)) return true;
                if (right?.DataType == typeof(Enum) && (left?.DataType?.IsEnum ?? false)) return true;
                if (left?.DataType == typeof(Stream) && right?.DataType == typeof(Stream)) return true;
            }
            return false;
        }
        /// <inheritdoc/>
        public static bool operator !=(ExField? left, ExField? right) => !(left == right);
    }
}
