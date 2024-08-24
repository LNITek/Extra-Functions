using System.Data;
using System.Reflection;

namespace ExtraFunctions.ExDatabase
{
    /// <summary>
    /// The SQL Statement That Is Uses To Manipulating The Database.
    /// </summary>
    public enum Statement
    {
        /// <summary>
        /// Gets Record Data.
        /// </summary>
        Select,
        /// <summary>
        /// Adds Records.
        /// </summary>
        Insert,
        /// <summary>
        /// Changes Records.
        /// </summary>
        Update,
        /// <summary>
        /// Removes Records
        /// </summary>
        Delete,
        /// <summary>
        /// Count All Records
        /// </summary>
        Count,
    }

    /// <summary>
    /// Create A General Query Thats Not Linked To An Object
    /// </summary>
    /// <param name="database">The database to run the query on</param>
    /// <param name="table">The table to  run the query with</param>
    public class ExQuery(ExDatabase database, IxTable table) : ExQuery<ExRecord>(database, table)
    { }

    /// <summary>
    /// Create A Query Thats Linked To An Object / Table
    /// </summary>
    /// <typeparam name="T">The Object To Link To</typeparam>
    /// <param name="database">The database to run the query on</param>
    /// <param name="table">The table to  run the query with</param>
    public class ExQuery<T>(ExDatabase database, IxTable table)
    {
        /// <summary>
        /// The Databse To Quary.
        /// </summary>
        public ExDatabase Database { get; private set; } = database;

        /// <summary>
        /// The table to  run the query with 
        /// </summary>
        public IxTable Table { get; internal set; } = table;

        /// <summary>
        /// The satement that the query will generate
        /// </summary>
        /// <remarks>Default: Select</remarks>
        public Statement Statement { get; internal set; } = Statement.Select;

        /// <summary>
        /// The where condision to add to the query
        /// </summary>
        public IEnumerable<string>? Condisions { get; internal set; } = null;
        /// <summary>
        /// The Value Uses For Manipulation.
        /// </summary>
        public T? Value { get; internal set; } = default;
    }

    /// <summary>
    /// The Quary Builder Functions.
    /// </summary>
    public static class ExQuaryExtentions
    {
        internal static ExQuery ToBase<T>(this ExQuery<T> query) where T : class
        {
            ExQuery qry =  new(query.Database, query.Table) { Statement = query.Statement, Condisions = query.Condisions };
            if (query.Value != null)
                qry.Value = ExRecord.FromObject(query.Value);
            return qry;
        }

        /// <summary>
        /// Selects A Table Thats Linked To The Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Database"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static ExQuery<T> Select<T>(this ExDatabase Database) where T : class
        {
            var tbl = Database.Tables.FirstOrDefault(x => x.DataType == typeof(T)) ?? 
                Database.Tables.FirstOrDefault(x => x.Name == typeof(T).Name) ?? 
                throw new ArgumentException("Could Not Find The Table In The Database.", nameof(T));

            return new ExQuery<T>(Database, tbl);
        }
        /// <summary>
        /// Selects A Table Based On Its Name And Links It To The Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Database"></param>
        /// <param name="Name">The Name Of The Table To Select</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static ExQuery<T> Select<T>(this ExDatabase Database, string Name) where T : class
        {
            var tbl = Database.Tables.FirstOrDefault(x => x.Name == Name) ??
                Database.Tables.FirstOrDefault(x => x.DataType == typeof(T)) ??
                Database.Tables.FirstOrDefault(x => x.Name == typeof(T).Name) ??
                throw new ArgumentException("Could Not Find The Table In The Database.", nameof(T));

            return new ExQuery<T>(Database, tbl);
        }
        /// <summary>
        /// Selects A Table Based On Its Name.
        /// </summary>
        /// <param name="Database"></param>
        /// <param name="Name">The Name Of The Table To Select</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static ExQuery Select(this ExDatabase Database, string Name)
        {
            var tbl = Database.Tables.FirstOrDefault(x => x.Name == Name) ??
                throw new ArgumentException("Could Not Find The Table In The Database.", nameof(Name));
            return new ExQuery(Database, tbl);
        }

        /// <summary>
        /// Add Condisions To The Statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="Condision">Gets All Records In Table And Compars It To This Function</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ExQuery<T> Where<T>(this ExQuery<T> query, Func<T, bool> Condision) where T : class
        {
            var Items = query.GetRecords();
            query.Condisions ??= [];
            foreach (var Item in Items)
                if (Item != null && Condision.Invoke(Item))
                {
                    var Prop = query.Table.GetPK() ??
                        throw new Exception("No Primary Key To Mach Condisions With.");
                    query.Condisions = query.Condisions.Append(query.Database.Interface.FormatValue(Prop.DataType ?? typeof(string), Item.GetType().GetProperty(Prop.Name)?.GetValue(Item)));
                }
            return query;
        }
        /// <summary>
        /// Add Condisions To The Statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="Condision">Gets All Records In Table And Compars It To This Function</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ExQuery<T> Where<T>(this ExQuery<T> query, Func<T, int, bool> Condision) where T : class
        {
            var Items = query.GetRecords();
            query.Condisions ??= [];
            int I = -1;
            foreach (var Item in Items)
            {
                I++;
                if (Item != null && Condision.Invoke(Item, I))
                {
                    var Prop = query.Table.GetPK() ??
                        throw new Exception("No Primary Key To Mach Condisions With.");
                    query.Condisions = query.Condisions.Append(query.Database.Interface.FormatValue(Prop.DataType ?? typeof(string), Item.GetType().GetProperty(Prop.Name)?.GetValue(Item)));
                }
            }
            return query;
        }
        /// <summary>
        /// Add Condisions To The Statement.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="Condision">A List Of Values That Will Be Compared Agensed The Primery Key</param>
        /// <returns></returns>
        public static ExQuery<T> Where<T>(this ExQuery<T> query, params object[] Condision)
        {
            query.Condisions = Condision.Select(x =>
                query.Database.Interface.FormatValue(query.Table.GetPK()?.DataType ?? typeof(string), x));
            return query;
        }
        /// <summary>
        /// Add Condisions To The Statement.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="Condision">A List Of Values That Will Be Compared Agensed The Primery Key</param>
        /// <returns></returns>
        public static ExQuery<T> Where<T>(this ExQuery<T> query, IEnumerable<object> Condision)
        {
            query.Condisions = Condision.Select(x =>
                query.Database.Interface.FormatValue(query.Table.GetPK()?.DataType ?? typeof(string), x));
            return query;
        }
        /// <summary>
        /// Add Condisions To The Statement.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="Condision">Gets All Records In Table And Compars It To This Function</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ExQuery Where(this ExQuery query, Func<ExRecord, int, bool> Condision)
        {
            var Items = query.GetRecords();
            query.Condisions ??= [];
            int I = -1;
            foreach (var Item in Items)
            {
                I++;
                if (Item != null && Condision.Invoke(Item, I))
                {
                    var Prop = query.Table.GetPK() ??
                        throw new Exception("No Primary Key To Mach Condisions With.");
                    query.Condisions = query.Condisions.Append(query.Database.Interface.FormatValue(Prop.DataType ?? typeof(string), Item[Prop.Name]));
                }
            }
            return query;
        }
        /// <summary>
        /// Add Condisions To The Statement.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="Condision">A List Of Values That Will Be Compared Agensed The Primery Key</param>
        /// <returns></returns>
        public static ExQuery Where(this ExQuery query, params object[] Condision)
        {
            query.Condisions = Condision.Select(x => 
                query.Database.Interface.FormatValue(query.Table.GetPK()?.DataType ?? typeof(string), x));
            return query;
        }
        /// <summary>
        /// Add Condisions To The Statement.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="Condision">A List Of Values That Will Be Compared Agensed The Primery Key</param>
        /// <returns></returns>
        public static ExQuery Where(this ExQuery query, IEnumerable<object> Condision)
        {
            query.Condisions = Condision.Select(x => 
                query.Database.Interface.FormatValue(query.Table.GetPK()?.DataType ?? typeof(string), x));
            return query;
        }

        /// <summary>
        /// Generates A Select Statement, Runs The Statment And Converts The Result.
        /// </summary>
        /// <typeparam name="T">The Object To Return As</typeparam>
        /// <param name="query"></param>
        /// <returns>All The Records As The Object</returns>
        public static IEnumerable<T> GetRecords<T>(this ExQuery<T> query) where T: class => 
            ExDatabase.Execute(query.ToBase()).Select(x => x.ToObject<T>());
        /// <summary>
        /// Generates A Select Statement, Runs The Statment.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>All The Records</returns>
        public static IEnumerable<ExRecord> GetRecords(this ExQuery query) => ExDatabase.Execute(query);

        /// <summary>
        /// Added A Record To The Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="Value">The Object To Be Added</param>
        /// <returns>Total Amount Of Records Efected</returns>
        public static int Insert<T>(this ExQuery<T> query, T Value) where T : class
        {
            query.Value = Value;
            query.Statement = Statement.Insert;
            var res = ExDatabase.Execute(query.ToBase());
            if (int.TryParse(res?.FirstOrDefault()?.FirstOrDefault()?.Value?.ToString() ?? "0", out int iRes))
                return iRes;
            return 0;
        }
        /// <summary>
        /// Added A Record To The Table
        /// </summary>
        /// <param name="query"></param>
        /// <param name="Value">The Item To Be Added</param>
        /// <returns>Total Amount Of Records Efected</returns>
        public static int Insert(this ExQuery query, ExRecord Value)
        {
            query.Value = RecordsMacher(Value, query.Table);
            query.Statement = Statement.Insert;
            var res = ExDatabase.Execute(query);
            if (int.TryParse(res?.FirstOrDefault()?.FirstOrDefault()?.Value?.ToString() ?? "0", out int iRes))
                return iRes;
            return 0;
        }

        /// <summary>
        /// Updates A Records In The Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="Value">The Object To Update To</param>
        /// <param name="SkipWhere">True To Skip Exeption On Empty Condisions Condisions</param>
        /// <returns>Total Amount Of Records Efected</returns>
        public static int Update<T>(this ExQuery<T> query, T Value, bool SkipWhere = false) where T : class
        {
            if (!SkipWhere && (query.Condisions == null || !query.Condisions.Any())) throw new Exception("No Condisions Clouse Added To Query.");
            query.Value = Value;
            query.Statement = Statement.Update;
            var res = ExDatabase.Execute(query.ToBase());
            if (int.TryParse(res?.FirstOrDefault()?.FirstOrDefault()?.Value?.ToString() ?? "0", out int iRes))
                return iRes;
            return 0;
        }
        /// <summary>
        /// Updates A Records In The Table
        /// </summary>
        /// <param name="query"></param>
        /// <param name="Value">The Object To Update To</param>
        /// <param name="SkipWhere">True To Skip Exeption On Empty Condisions Condisions</param>
        /// <returns>Total Amount Of Records Efected</returns>
        public static int Update(this ExQuery query, ExRecord Value, bool SkipWhere = false)
        {
            if (!SkipWhere && (query.Condisions == null || !query.Condisions.Any())) throw new Exception("No Condisions Clouse Added To Query.");

            query.Value = RecordsMacher(Value, query.Table);
            query.Statement = Statement.Update;
            var res = ExDatabase.Execute(query);
            if (int.TryParse(res?.FirstOrDefault()?.FirstOrDefault()?.Value?.ToString() ?? "0", out int iRes))
                return iRes;
            return 0;
        }

        /// <summary>
        /// Remove A Records In The Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="SkipWhere">True To Skip Exeption On Empty Condisions Condisions</param>
        /// <returns>Total Amount Of Records Efected</returns>
        public static int Delete<T>(this ExQuery<T> query, bool SkipWhere = false) where T : class
        {
            if (!SkipWhere && (query.Condisions == null || !query.Condisions.Any())) throw new Exception("No Condisions Clouse Added To Query.");
            query.Statement = Statement.Delete;
            var res = ExDatabase.Execute(query.ToBase());
            if (int.TryParse(res?.FirstOrDefault()?.FirstOrDefault()?.Value?.ToString() ?? "0", out int iRes))
                return iRes;
            return 0;
        }
        /// <summary>
        /// Remove A Records In The Table
        /// </summary>
        /// <param name="query"></param>
        /// <param name="SkipWhere">True To Skip Exeption On Empty Condisions Condisions</param>
        /// <returns>Total Amount Of Records Efected</returns>
        public static int Delete(this ExQuery query, bool SkipWhere = false)
        {
            if (!SkipWhere && (query.Condisions == null || !query.Condisions.Any())) throw new Exception("No Condisions Clouse Added To Query.");
            query.Statement = Statement.Delete;
            var res = ExDatabase.Execute(query);
            if (int.TryParse(res?.FirstOrDefault()?.FirstOrDefault()?.Value?.ToString() ?? "0", out int iRes))
                return iRes;
            return 0;
        }

        /// <summary>
        /// Counts All Records In The Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns>Total Amount Of Records Efected</returns>
        public static int Count<T>(this ExQuery<T> query) where T : class
        {
            query.Statement = Statement.Count;
            var res = ExDatabase.Execute(query.ToBase());
            if (int.TryParse(res?.FirstOrDefault()?.FirstOrDefault()?.Value?.ToString() ?? "0", out int iRes))
                return iRes;
            return 0;
        }
        /// <summary>
        /// Counts All Records In The Table
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Total Amount Of Records Efected</returns>
        public static int Count(this ExQuery query)
        {
            query.Statement = Statement.Count;
            var res = ExDatabase.Execute(query.ToBase());
            if (int.TryParse(res?.FirstOrDefault()?.FirstOrDefault()?.Value?.ToString() ?? "0", out int iRes))
                return iRes;
            return 0;
        }

        static ExRecord RecordsMacher(ExRecord Values, IxTable Mach)
        {
            ExRecord Rec = [];
            foreach (var item in Values)
            {
                var fld = Mach.Fields.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase) || x.PreNames.Contains(item.Name));
                if (fld != null)
                    Rec.Add(new(fld.Name, item.Value));
            }
            return Rec;
        }

        internal static string FormatColumn(this IxDatabase Interface, ExField field)
        {
            var format = Interface.ColumnFormat;
            if (string.IsNullOrWhiteSpace(format)) throw new Exception("Invalid Format: Column Format Is Empty");

            var lng = field.Length > 0 ? $"({field.Length})" : "";
            var Def = field.DefaultValue != null ? $"DEFAULT {Interface.FormatValue(field.DataType ?? typeof(string), field.DefaultValue)}" : "";
            var Null = field.Nullable ? "" : "NOT NULL";
            var Auto = field.AUTO ? Interface.AUTO : "";

            if (format.Contains(ExDatabase.COLUMN_KEY) && field.PK) format = format.Replace(ExDatabase.COLUMN_KEY, Interface.PK);
            //TODO: FK

            if (format.Contains(ExDatabase.COLUMN_PRIMARY_KEY) && field.PK) format = format.Replace(ExDatabase.COLUMN_PRIMARY_KEY, Interface.PK);
            //if (format.Contains(ExDatabase.COLUMN_FOREIGN_KEY)) format = format.Replace(ExDatabase.COLUMN_FOREIGN_KEY, Interface.FK);

            return format.Replace(ExDatabase.COLUMN_TYPE, Interface.FormatType(field.DataType))
                .Replace(ExDatabase.COLUMN_NAME, field.Name + lng)
                .Replace(ExDatabase.COLUMN_NULLABLE, Null)
                .Replace(ExDatabase.COLUMN_DEFAULT, Def)
                .Replace(ExDatabase.COLUMN_AUTO, Auto)
                .Replace(ExDatabase.COLUMN_KEY, "")
                .Replace(ExDatabase.COLUMN_PRIMARY_KEY, "")
                .Replace(ExDatabase.COLUMN_FOREIGN_KEY, "");
        }

        internal static string GenDrop(string tableName) => $"DROP TABLE [{tableName}];";

        internal static string GenCreate(this IxDatabase Interface, IxTable table)
        {
            var Separator = Interface.Spacer;

            var Col = table.Fields.Select(Interface.FormatColumn);
            return $"CREATE TABLE [{table.Name}] ({string.Join(Separator, Col)});";
        }

        internal static string GenAlter(this IxDatabase Interface, IxTable newTable, IxTable oldTable)
        {
            var Separator = Interface.Spacer;
            var cols = new List<(ExField OLD, ExField NEW)>();

            foreach (var field in newTable.Fields)
            {
                var OLD = oldTable.Fields.FirstOrDefault(x => x.Name == field.Name || field.PreNames.Contains(x.Name));
                if (OLD != null) cols.Add((OLD, field));
            }

            var newCol = cols.Select(x => x.NEW.Name);
            var oldCol = cols.Select(x => x.OLD.Name);

            var SQL = $"CREATE TABLE [tblTEMP] ({string.Join(Separator, newTable.Fields.Select(Interface.FormatColumn))});";
            SQL += $"INSERT INTO [tblTEMP] ({string.Join(Separator, newCol)}) SELECT {string.Join(Separator, oldCol)} FROM [{oldTable.Name}];";
            SQL += GenDrop(oldTable.Name);
            SQL += GenCreate(Interface, newTable);
            SQL += $"INSERT INTO [{newTable.Name}] SELECT * FROM [tblTEMP];";
            return SQL + GenDrop("tblTEMP");
        }
    }
}
