namespace ExtraFunctions.ExDatabase
{
    /// <summary>
    /// The interface for comunication between the database and the manager
    /// </summary>
    public interface IxDatabase
    {
        /// <summary>
        /// The Spacer To Use Between Values Or Columns.
        /// </summary>
        string Spacer { get; }
        /// <summary>
        /// The Template For Creating A Culomn.
        /// </summary>
        string ColumnFormat { get; }
        /// <summary>
        /// Auto Incoment Key Words.
        /// </summary>
        string AUTO { get; }
        /// <summary>
        /// Primery Key, Key Words.
        /// </summary>
        string PK { get; }
        /// <summary>
        /// Foreign key, Key Words.
        /// </summary>
        /// <remarks>
        /// WIP: Not Implemented
        /// </remarks>
        string FK { get; }

        /// <summary>
        /// Run A SQL Quary.
        /// </summary>
        /// <param name="SQL">The Quary As A SQL Statement In A String.</param>
        /// <returns>The result as an record</returns>
        IEnumerable<ExRecord> Run(string SQL);

        /// <summary>
        /// Gets The Structure Of The Database.
        /// </summary>
        /// <returns>The Database Structure As ExTables, Otherwise Null When No Maintenance Will Be Applied.</returns>
        IEnumerable<(string TableName, IEnumerable<ExField> Fields)>? GetStructure();

        /// <summary>
        /// Converts The Value To Its Formated Value By A Type.
        /// </summary>
        /// <remarks>
        /// WIP: Can Be Simplyfied
        /// </remarks>
        /// <param name="Type">The Fileds Type Of The Value For Converting.</param>
        /// <param name="Value">The Value To Convert.</param>
        /// <returns>A Converted Value With The Format Of The Type.</returns>
        string FormatValue(Type Type, object? Value);

        /// <summary>
        /// Converts The Data Type To Its Corasponding Colulmn Type For The Database.
        /// </summary>
        /// <param name="Type">The Data Type To Convert</param>
        /// <returns>The Colulmn Type As String Of The Type.</returns>
        string FormatType(Type? Type);
    }
}
