using System.Reflection;

namespace ExtraFunctions.ExDatabase
{
    /// <summary>
    /// A Basic Row With Columns That Is Not Based On A Object.
    /// </summary>
    public class ExRecord : List<ExValue>
    {
        /// <summary>
        /// Create A List Of Basic Columns.
        /// </summary>
        public ExRecord() : base() { }
        /// <summary>
        /// Create A List Of One Column.
        /// </summary>
        /// <param name="Column"></param>
        public ExRecord(ExValue Column) : base() { Add(Column); }
        /// <summary>
        /// Create A List Of Basic Columns.
        /// </summary>
        /// <param name="ListColumns">Basic Columns.</param>
        public ExRecord(IEnumerable<ExValue> ListColumns) : base(ListColumns) { }
        /// <summary>
        /// Create A List Of Basic Columns.
        /// </summary>
        /// <param name="ListColumns">Basic Columns.</param>
        public ExRecord(params ExValue[] ListColumns) : base(ListColumns) { }

        /// <summary>
        /// Gets The Value Of The Field
        /// </summary>
        /// <param name="Name">The Name Of The Field</param>
        /// <returns>The Fields Value</returns>
        public object? this[string Name] => this.FirstOrDefault(x => x.Name == Name)?.Value;

        /// <summary>
        /// Converts To An Object By Maching Its Constructors And Propertys
        /// </summary>
        /// <typeparam name="T">The Object To Convert To</typeparam>
        /// <returns>The Newly Created Object</returns>
        public T ToObject<T>() where T : class => (T)ToObject(typeof(T));
        /// <summary>
        /// Converts To An Object By Maching Its Constructors And Propertys
        /// </summary>
        /// <param name="Type">The typeof the object to wich to convert to</param>
        /// <returns>The Newly Created Object</returns>
        /// <exception cref="Exception"></exception>
        public object ToObject(Type Type)
        {
            var ctors = Type.GetConstructors().OrderBy(x => x.GetCustomAttribute<DatabaseContructor>() != null)
                .ThenByDescending(x => x.GetParameters().Length);

            var Props = new List<PropertyInfo>();
            var obj = CreateObject();
            foreach (var item in Props)
            {
                if (!item.CanWrite || !item.CanRead) continue;
                var val = this.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)
                            && x.Type == item.PropertyType)?.Value;
                item.SetValue(obj, val);
            }

            return obj;

            object CreateObject()
            {
                var UsedProps =  new List<string?>();
                foreach (var ctor in ctors)
                {
                    List<object?> list = [];
                    var parms = ctor.GetParameters();
                    foreach (var param in parms)
                    {
                        var val = this.FirstOrDefault(x => x.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase)
                            && (x.Type == param.ParameterType))?.Value;

                        // && System.Nullable.GetUnderlyingType(param.ParameterType) == null
                        if (val == null && !param.HasDefaultValue)
                        {
                            Props.Clear();
                            list.Clear();
                            break;
                        }
                        list.Add(val ?? param.DefaultValue);
                        UsedProps.Add(param.Name?.ToLower());
                    }
                    if (parms.Length <= 0 || list.Count > 0)
                    {
                        UsedProps.RemoveAll(string.IsNullOrWhiteSpace);
                        Props = Type.GetProperties().Where(x => !UsedProps.Contains(x.Name.ToLower())).ToList();
                        return ctor.Invoke(list.ToArray());
                    }
                }
                throw new Exception("Could Not Find A Maching Contructor");
            }
        }

        /// <summary>
        /// Converts An Object To An Basic Version
        /// </summary>
        /// <typeparam name="T">The Object To Convert From</typeparam>
        /// <param name="Obj">The Object To Convert</param>
        /// <returns>A Simplyfied Version Of The Object</returns>
        public static ExRecord FromObject<T>(T Obj) where T : class
        {
            var Type = typeof(T);
            var tbl = new ExTable<T>();
            var rec  = new ExRecord();

            foreach (var Field in tbl.Fields)
            {
                var Val = Type.GetProperty(Field.Name)?.GetValue(Obj);
                rec.Add(new ExValue(Field.Name, Val));
            }
            return rec;
        }
    }

    /// <summary>
    /// A Basic Column That Is Not Based On A Object Property.
    /// </summary>
    public class ExValue
    {
        /// <summary>
        /// The Column Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The Value In The Column.
        /// </summary>
        public object? Value { get; set; }
        /// <summary>
        /// The Data Type Of The Value.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Create A Column That's Not Based On A Object Field.
        /// </summary>
        /// <param name="Name">The Name Of The Column.</param>
        /// <param name="Value">The Value In The Column.</param>
        /// <param name="Type">The Data Type Of The Value.</param>
        public ExValue(string Name, object? Value, Type Type)
        {
            this.Name = Name;
            this.Value = Value;
            this.Type = Type;
        }

        /// <summary>
        /// Create A Column That's Not Based On A Object Field.
        /// </summary>
        /// <param name="Name">The Name Of The Column.</param>
        /// <param name="Value">The Value In The Column.</param>
        public ExValue(string Name, object? Value)
        {
            this.Name = Name;
            this.Value = Value;
            Type = Value?.GetType() ?? typeof(object);
        }
    }
}
