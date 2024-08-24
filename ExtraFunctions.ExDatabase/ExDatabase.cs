namespace ExtraFunctions.ExDatabase
{
    /// <summary>
    /// An unaversal database management utility. For comunication between database and Automaticly keeping the database structure uptodate.
    /// </summary>
    public class ExDatabase
    {
        /// <summary>
        /// SQL Template Formats.
        /// </summary>
        /// <remarks>
        /// Will Be Replased With Its Corasponding Value.
        /// </remarks>
        public const string COLUMN_NAME = "{NAME}", COLUMN_TYPE = "{TYPE}", COLUMN_DEFAULT = "{DEFAULT}", COLUMN_NULLABLE = "{NULL}",
            COLUMN_KEY = "{KEY}", COLUMN_PRIMARY_KEY = "{PKEY}", COLUMN_FOREIGN_KEY = "{FKEY}", COLUMN_AUTO = "{AUTO}";

        /// <summary>
        /// The name of the database
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// List of all the tables in the database
        /// </summary>
        public IEnumerable<IxTable> Tables { get; }
        /// <summary>
        /// The interface to be used for comunication
        /// </summary>
        public IxDatabase Interface { get; }

        /// <summary>
        /// True to run maintenance on this database, otherwise false
        /// </summary>
        /// <remarks>
        /// Will Skip tables with Maintenance = false
        /// </remarks>
        public bool Maintenance { get; set; } = true;

        /// <summary>
        /// Init Database Manager System.
        /// </summary>
        /// <param name="Name">Name Of The Database</param>
        /// <param name="Tables">All Tables In The Database</param>
        /// <param name="Interface">The Interface For Comunicateing Between Database And Manager</param>
        /// <param name="Maintain">Start Maintenance On Createtion</param>
        public ExDatabase(string Name, IEnumerable<IxTable> Tables, IxDatabase Interface, bool Maintain = false)
        {
            this.Name = Name;
            this.Tables = Tables;
            this.Interface = Interface;
            if (Maintain) this.Maintain();
        }

        //Creates The SQL Query
        internal static string BuildSQL(ExQuery query)
        {
            var Separator = query.Database.Interface.Spacer;

            return query.Statement switch
            {
                Statement.Insert => GenInsert(),
                Statement.Update => GenUpdate(),
                Statement.Delete => GenDelete(),
                Statement.Count => GenCount(),
                _ => GenSelect(),
            };

            string AddWhere()
            {
                var sql = "";
                if (query.Condisions != null)
                    if (query.Condisions.Count() > 0 && query.Table.GetPK() != null)
                        sql += $"WHERE [{query.Table.GetPK()?.Name}] IN ({string.Join(Separator, query.Condisions)})";
                    else
                        sql += $"WHERE false";
                return sql;
            }

            string GenDelete() => $"DELETE FROM [{query.Table.Name}] {AddWhere()};";

            string GenSelect() => 
                $"SELECT {string.Join(Separator, query.Table.Fields.Select(x => $"[{x.Name}]"))} FROM [{query.Table.Name}] {AddWhere()};";

            string GenCount() => $"SELECT Count(*) FROM [{query.Table.Name}] {AddWhere()};";

            string GenInsert()
            {
                List<ExField> fields = query.Table.Fields.Where(x => !x.AUTO).ToList();
                List<string> Val = [];
                if (query.Value == null) throw new Exception("No Valid Records To Add.");

                foreach (var item in fields)
                {
                    if (item == null) continue;
                    var i = (query.Value as ExRecord)?.Find(x => x.Name == item.Name)?.Value;
                    if (i == null && !item.Nullable) i = item.DefaultValue;
                    var V = query.Database.Interface.FormatValue(item.DataType ?? typeof(string), i);
                    Val.Add(V);
                }

                return $"INSERT INTO [{query.Table.Name}] ({string.Join(Separator, fields.Select(x => $"[{x.Name}]"))}) " +
                    $"VALUES ({string.Join(Separator,Val)});";
            }

            string GenUpdate()
            {
                List<ExField> fields = query.Table.Fields.Where(x => !(x.AUTO || x.PK)).ToList();
                List<(string Name, string Val)> Val = [];
                if (query.Value == null || query.Value is not ExRecord rec) 
                    throw new Exception("No Valid Records To Update.");

                foreach (var item in rec)
                {
                    var i = fields.Find(x => x.Name == item.Name);
                    if (i == null || item == null) continue;
                    if (item.Value == null && !(i.Nullable && i.DefaultValue == null)) item.Value = i.DefaultValue;
                    var V = query.Database.Interface.FormatValue(i.DataType ?? typeof(string), item.Value);
                    Val.Add((item.Name, V));
                }

                return $"UPDATE [{query.Table.Name}] SET {string.Join(Separator, Val.Select(x => $"[{x.Name}] = {x.Val}"))} {AddWhere()};";
            }
        }

        /// <summary>
        /// Builds the SQL Statement Based On The query and run the statment.
        /// </summary>
        /// <param name="query">The Query To Build And Run</param>
        /// <returns>The result of the run SQL Stament thru the interface</returns>
        public static IEnumerable<ExRecord> Execute(ExQuery query) => query.Database.Interface.Run(BuildSQL(query));
        /// <summary>
        /// Run the given SQL Statment
        /// </summary>
        /// <param name="SQL">The stament to run</param>
        /// <returns>The result of the run SQL Stament thru the interface</returns>
        public IEnumerable<ExRecord> Execute(string SQL) => Interface.Run(SQL);

        /// <summary>
        /// Runs Maintenance On All Tables Condisions Maintenance = true
        /// </summary>
        /// <remarks>Will Not Do Anything If Maintenance = false Or If Ther Interface Does Not Support Maintenance</remarks>
        public void Maintain()
        {
            if (!Maintenance) return;

            var Tables = this.Tables.Where(x => x.Maintenance);
            var Struct = Interface.GetStructure();
            if (Struct == null) return;

            foreach (var (TableName, Fields) in Struct)
            {
                if (Tables.All(x => TableName != x.Name && !x.PreNames.Contains(TableName)))
                {
                    Execute(ExQuaryExtentions.GenDrop(TableName));
                    continue;
                }

                var rename = Tables.FirstOrDefault(x => TableName != x.Name && x.PreNames.Contains(TableName));
                if(rename != null)
                {
                    Execute(Interface.GenAlter(rename, new ExTable(TableName, Fields)));
                    continue;
                }
            }

            foreach(var item in Tables)
            {
                var old = Struct.FirstOrDefault(x => x.TableName == item.Name);

                if (old.Fields == null)
                {
                    if (old.TableName != null)
                        Execute(ExQuaryExtentions.GenDrop(old.TableName));
                    Execute(Interface.GenCreate(item));
                    continue;
                }

                if(old.Fields.Count() != item.Fields.Count())
                {
                    Execute(Interface.GenAlter(item, new ExTable(old.TableName, old.Fields)));
                    continue;
                }

                foreach (var field in item.Fields)
                {
                    var oldF = old.Fields.FirstOrDefault(x => x.Name == field.Name);
                    if (oldF != null && field == oldF) continue;

                    Execute(Interface.GenAlter(item, new ExTable(old.TableName, old.Fields)));
                    break;
                }
            }
        }
    }
}
