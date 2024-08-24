namespace ExtraFunctions.ExDatabase
{
    /// <summary>
    /// Spesifys That This Constructor Should Me Used First When Converting
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class DatabaseContructor : Attribute { }

    /// <summary>
    /// Ignore The Property In The Auto Field Creater.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Ignore : Attribute { }

    /// <summary>
    /// Added Previous Names To Tables And Fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class Rename : Attribute
    {
        /// <summary>
        /// The Previous Names.
        /// </summary>
        public IEnumerable<string> PreNames { get; private set; } = [];

        /// <summary>
        /// Add An Old Name.
        /// </summary>
        public Rename(string PreName) { PreNames = PreNames.Append(PreName); }
        /// <summary>
        /// Add A List Of Old Names
        /// </summary>
        /// <param name="PreName"></param>
        public Rename(IEnumerable<string> PreName) { PreNames = PreNames.Concat(PreName); }
        /// <summary>
        /// Add A List Of Old Names
        /// </summary>
        /// <param name="PreName"></param>
        public Rename(params string[] PreName) { PreNames = PreNames.Concat(PreName); }
    }

    /// <summary>
    /// The Max Lenght Of A Field.
    /// </summary>
    /// <remarks>
    /// Set The Max Lenght Of A Field.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldLenght(int Lenght) : Attribute
    {
        /// <summary>
        /// The Max Lenght Of A Field.
        /// </summary>
        public int Lenght { get; private set; } = Lenght;
    }

    /// <summary>
    /// The Default Value Of A Field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Default : Attribute
    {
        /// <summary>
        /// The Default Value Of A Field.
        /// </summary>
        public object Value { get; private set; }
        /// <summary>
        /// Set The Default Value Of A Field.
        /// </summary>
        public Default(object Value) { this.Value = Value; }
        /// <summary>
        /// Set The Default Value Of A Field.
        /// </summary>
        public Default(object Value, Type Type) { this.Value = Convert.ChangeType(Value, Type); }
    }

    /// <summary>
    /// Sets The Field To Be Primary Key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimeryKey : Attribute { }

    /// <summary>
    /// Sets The Field To Have An Foreign Key.
    /// </summary>
    /// <remarks>
    /// WIP: Not Implemented
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKey : Attribute
    {
        /// <summary>
        /// Refrences To The Table.
        /// </summary>
        public string Table { get; private set; }
        /// <summary>
        /// Refrences To The Field.
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// Test If Foreign Condisions Mach.
        /// </summary>
        /// <returns>True If Mach, Otherwise False</returns>
        public override bool Equals(object? obj)
        {
            //if (obj is ForeignKey FK) return this == FK;
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns Tha Hash Code Of This Insance.
        /// </summary>
        /// <returns>A 32-bit Signed Int Hash Code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Test If Foreign Condisions Mach.
        /// </summary>
        /// <returns>True If Mach And Not Null, Otherwise False</returns>
        public static bool operator ==(ForeignKey? left, ForeignKey? right)
        {
            if (left?.Equals(right) ?? false) return true;
            if ((left?.Equals(null) ?? true) || (right?.Equals(null) ?? true)) return false;
            if (left.Table == right.Table && left.Field == right.Field) return true;
            return false;
        }

        /// <summary>
        /// Test If Foreign Condisions Does Not Mach.
        /// </summary>
        /// <returns>True If Not Mach, Otherwise False</returns>
        public static bool operator !=(ForeignKey? left, ForeignKey? right)
        {
            if (left == null && right == null) return false;
            if (left?.Table != right?.Table && left?.Field != right?.Field) return true;
            return false;
        }

        /// <summary>
        /// Set Foreign Key.
        /// </summary>
        /// <param name="RefTable">The Refrence Table</param>
        /// <param name="RefField">The Refrence Field</param>
        public ForeignKey(string RefTable, string RefField)
        {
            Table = RefTable;
            Field = RefField;
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Sets Whether The Field Accepts Null Values. 
    /// Or If DefualtNull = false. Sets If Field Is Not Null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Nullable : Attribute { }

    /// <summary>
    /// Sets The Field To Autoincroment And Defualt Value To 'AUTO'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoNumber : Attribute { }

    /// <summary>
    /// States That The Table Does Not Allow Maintenace
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NOMaintenance : Attribute { }
}
