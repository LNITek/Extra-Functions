using System.Reflection;

namespace ExtraFunctions.ExDatabase
{
    /// <summary>
    /// A Database Table
    /// </summary>
    public interface IxTable
    {
        /// <summary>
        /// The name of the table
        /// </summary>
        string Name { get; }
        /// <summary>
        /// True to run maintenance on this table, otherwise false
        /// </summary>
        bool Maintenance { get; set; }

        /// <summary>
        /// the data type of object that the table is based on
        /// </summary>
        Type DataType { get; }
        /// <summary>
        /// All the fields within the table
        /// </summary>
        IEnumerable<ExField> Fields { get; }
        /// <summary>
        /// Any or all referances to previuse names of the table. To retain or transfur the data.
        /// </summary>
        IEnumerable<string> PreNames { get; }

        /// <summary>
        /// Gets the Primary Key field
        /// </summary>
        /// <returns>The field that has PK = true</returns>
        ExField? GetPK();
    }

    /// <summary>
    /// A Database Table
    /// </summary>
    /// <remarks>
    /// Creates A Custom Table Refrance.
    /// </remarks>
    /// <param name="Name">The name of the table</param>
    /// <param name="Fields">All the fields in the table</param>
    public class ExTable(string Name, IEnumerable<ExField> Fields) : IxTable
    {
        /// <summary>
        /// The name of the table
        /// </summary>
        public string Name { get; } = Name;
        /// <summary>
        /// True to run maintenance on this table, otherwise false
        /// </summary>
        public bool Maintenance { get; set; } = true;

        /// <summary>
        /// the data type of object that the table is based on
        /// </summary>
        public Type DataType => typeof(ExTable);
        /// <summary>
        /// All the fields within the table
        /// </summary>
        public IEnumerable<ExField> Fields { get; } = Fields;
        /// <summary>
        /// Any or all referances to previuse names of the table. To retain or transfur the data.
        /// </summary>
        public IEnumerable<string> PreNames { get; set; } = [];

        /// <summary>
        /// Gets the Primary Key field
        /// </summary>
        /// <returns>The field that has PK = true</returns>
        public ExField? GetPK() => Fields.FirstOrDefault(x => x.PK);
    }

    /// <summary>
    /// A Auto Generated Database Table
    /// </summary>
    public class ExTable<T> : IxTable where T : class
    {
        const int MAXPK = 1;

        /// <summary>
        /// The name of the table
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// True to run maintenance on this table, otherwise false
        /// </summary>
        public bool Maintenance { get; set; } = true;
        /// <summary>
        /// The default state wheter the Nullable Atrobute is for 'Nullable' Or 'Not NUll'. 
        /// True For Nullable, Otherwise false
        /// </summary>
        public bool DefaultNull { get; set; } = true;

        /// <summary>
        /// the data type of object that the table is based on
        /// </summary>
        public Type DataType => typeof(T);
        /// <summary>
        /// All the fields within the table
        /// </summary>
        public IEnumerable<ExField> Fields { get; private set; } = [];
        /// <summary>
        /// Any or all referances to previuse names of the table. To retain or transfur the data.
        /// </summary>
        public IEnumerable<string> PreNames { get; set; } = [];

        /// <summary>
        /// Auto Creates A Table Refrance With Fields.
        /// </summary>
        public ExTable()
        {
            Name = DataType.Name;
            GenTable();
        }

        /// <summary>
        /// Auto Creates A Table Refrance With Fields.
        /// </summary>
        /// <param name="Name">The table name</param>
        public ExTable(string Name)
        {
            this.Name = Name;
            GenTable();
        }

        /// <summary>
        /// Gets the Primary Key field
        /// </summary>
        /// <returns>The field that has PK = true</returns>
        public ExField? GetPK() => Fields.FirstOrDefault(x => x.PK);

        internal void GenTable()
        {
            var Type = DataType;
            Maintenance = Type.GetCustomAttribute<NOMaintenance>() == null;

            foreach (var item in Type.GetCustomAttributes<Rename>().Select(x => x.PreNames))
                PreNames = PreNames.Concat(item);

            GenFields();
        }

        internal void GenFields()
        {
            var Type = DataType;

            var I = Type.GetProperties().Where(x => x.GetCustomAttribute(typeof(PrimeryKey)) != null).Count();

            if (I > MAXPK && MAXPK > 0)
                throw new Exception($"You Can Only Have A Maximum Of {MAXPK} Primery Key!");
            if (!Type.IsClass) throw new InvalidCastException("ObjectType Must Be Of Type Class");

            foreach (var x in Type.GetProperties().Where(x => x.GetCustomAttribute(typeof(Ignore)) == null && x.CanWrite && x.CanRead))
                Fields = Fields.Append(new ExField(x) { DefaultNull = DefaultNull });
        }
    }
}
