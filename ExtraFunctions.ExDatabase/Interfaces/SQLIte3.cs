using System.Data;
using ExtraFunctions.ExDatabase;
using Microsoft.Data.Sqlite;

namespace SQLite3
{
    /// <summary>
    /// V 2.0
    /// </summary>
    public class SQLite3 : IxDatabase
    {
        SqliteConnection DBConnection;
        public SqliteCommand DB;

        public string FilePath { get; internal set; }
        private readonly string ConnectionString = "";

        public bool Connection => DBConnection.State == ConnectionState.Open;

        public void Connect() { if (!Connection) DBConnection.Open(); }
        public void Disconnect() { if (Connection) DBConnection.Close(); }

        public string Spacer => ", ";
        public string ColumnFormat => $"[{ExDatabase.COLUMN_NAME}] {ExDatabase.COLUMN_TYPE} {ExDatabase.COLUMN_DEFAULT} {ExDatabase.COLUMN_NULLABLE} {ExDatabase.COLUMN_PRIMARY_KEY} {ExDatabase.COLUMN_AUTO}";
        public string AUTO => "AUTOINCREMENT"; 
        public string PK => $"PRIMARY KEY";
        public string FK => $"CONSTRAINT FK_{ExDatabase.COLUMN_NAME} REFERENCES {ExDatabase.COLUMN_DEFAULT}";

        public SQLite3(string FilePath, string Password = "")
        {
            this.FilePath = FilePath;
            ConnectionString = new SqliteConnectionStringBuilder()
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                Password = Password,
                DataSource = FilePath,
                ForeignKeys = true,
            }.ToString();
            if (!string.IsNullOrEmpty(FilePath))
            {
                DBConnection = new SqliteConnection(ConnectionString);
                DB = new SqliteCommand(null, DBConnection);
            }
            else throw new NullReferenceException(string.IsNullOrEmpty(FilePath) ? "No Database File Selected." : "Could Not Find Database File.");
        }

        public IEnumerable<ExRecord> Run(string SQL)
        {
            if (DB == null || DB.Connection == null) return null;
            var arrSQL = SQL.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (arrSQL.Length <= 0) return null;
            Connect();
            var lst = new List<ExRecord>();
            if (arrSQL.Length == 1)
            {
                DB.CommandText = SQL;
                if (SQL.Trim().StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase))
                { var res = Reader(); Disconnect(); return res; }
                lst = new List<ExRecord>() { new(new ExValue(SQL, DB.ExecuteNonQuery())) };
            }
            else foreach (var QRY in arrSQL)
                {
                    DB.CommandText = QRY;
                    lst.Add(new(new ExValue(QRY, DB.ExecuteNonQuery())));
                }

            Disconnect();
            return lst;

            IEnumerable<ExRecord> Reader()
            {
                Connect();
                using (SqliteDataReader Reader = DB.ExecuteReader())
                    while (Reader.Read()) yield return new ExRecord(Col(Reader));
                Disconnect();

                IEnumerable<ExValue> Col(SqliteDataReader Reader)
                {
                    for (int I = 0; I < Reader.FieldCount; I++)
                    {
                        var Val = Reader[I];
                        var Type = FormatType(Reader.GetDataTypeName(I));

                        if(Type.IsEnum || Type == typeof(Enum)) Type = typeof(int);

                        if (Val is DBNull || Val == null) Val = Type switch
                        {
                            Type T when T == typeof(uint) => 0,
                            Type T when T == typeof(int) => 0,
                            Type T when T == typeof(double) => 0d,
                            Type T when T == typeof(DateTime) => new DateTime(),
                            Type T when T == typeof(bool) => false,
                            Type T when T == typeof(string) => "",
                            Type T when T == typeof(Enum) || T.IsEnum => 0,
                            _ => null,
                        };
                        yield return new ExValue(Reader.GetName(I), Convert.ChangeType(Val, Type));
                    }
                }
            }
        }

        public IEnumerable<(string, IEnumerable<ExField>)> GetStructure()
        {
            Connect();
            List<string> Tables = [];

            DB.CommandText = "SELECT * FROM sqlite_master where type='table';";
            using var Reader = DB.ExecuteReader();
            while (Reader.Read()) Tables.Add(Reader["name"].ToString());

            Tables.RemoveAll(x => x == "sqlite_sequence");
            foreach (var Item in Tables)
                yield return new(Item, GetColumns(Item));
            Disconnect();

            IEnumerable<ExField> GetColumns(string TableName)
            {
                DB.CommandText = $"PRAGMA table_info({TableName});";
                Connect();
                using var Reader = DB.ExecuteReader();
                while (Reader.Read())
                {
                    var Name = Reader["name"].ToString();
                    var Type = TypeConv(Reader["type"].ToString());
                    var NotNull = Convert.ToBoolean(int.Parse(Reader["notnull"].ToString()));
                    var Dflt = Reader["dflt_value"];
                    var PK = Convert.ToBoolean(int.Parse(Reader["pk"].ToString()));
                    var AUTO = Type.Item1 == typeof(uint) || Dflt.ToString() == "AUTO";

                    yield return new(Type.Item1, Name, SetDefault(Type.Item1, Dflt), !NotNull, PK, AUTO, Type.Item2);
                }
                Disconnect();

                static object SetDefault(Type Type, object Value)
                {
                    if (Value is DBNull) return null;
                    if(Value.ToString() == "AUTO") return null;
                    var T = Convert.ChangeType(Value, Type);
                    return T;
                }

                static int LenghtFormater(string StringLength)
                {
                    if (int.TryParse(StringLength, out int Lenght))
                        return Lenght;
                    return 0;
                }

                static (Type, int) TypeConv(string Type)
                {
                    var Arr = Type.Replace(")", "").Split('(');
                    int Lenght = 0;
                    if (Arr.Length > 1) Lenght = LenghtFormater(Arr[1]);
                    if (Arr.Length <= 0) return (typeof(string), 0);
                    var FieldType = FormatType(Arr[0].Trim());
                    return (FieldType, Lenght);
                }
            }
        }

        public string FormatValue(Type Type, object Value)
        {
            if (Value == null) return "NULL";
            return Type switch
            {
                Type T when T == typeof(DateTime) => $"'{Convert.ToDateTime(Value).ToShortDateString()}'",
                Type T when T == typeof(string) => $"'{Value}'",
                Type T when T == typeof(Enum) || T.IsEnum => Convert.ToInt32(Value).ToString(),
                _ => Value.ToString(),
            };
        }

        public string FormatType(Type Type)
        {
            return Type switch
            {
                Type T when T == typeof(uint) => "INTEGER",
                Type T when T == typeof(int) => "INT",
                Type T when T == typeof(double) => "REAL",
                Type T when T == typeof(DateTime) => "DATETIME",
                Type T when T == typeof(bool) => "BOOLEAN",
                Type T when T == typeof(string) => "TEXT",
                Type T when T == typeof(Enum) || T.IsEnum => "ENUM",
                Type T when T == typeof(object) => "BLOB",
                _ => "TEXT",
            };
        }

        public static Type FormatType(string TypeName)
        {
            return TypeName switch
            {
                "INTEGER" => typeof(uint),
                "INT" => typeof(int),
                "REAL" => typeof(double),
                "DATETIME" => typeof(DateTime),
                "BOOLEAN" => typeof(bool),
                "TEXT" => typeof(string),
                "ENUM" => typeof(Enum),
                "BLOB" => typeof(object),
                _ => typeof(string),
            };
        }
    }
}
