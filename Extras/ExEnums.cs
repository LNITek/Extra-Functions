namespace ExtraFunctions.Extras
{
    /// <summary>
    /// Icons For The Popup Prompt
    /// </summary>
    public enum Icons
    {
        /// <summary>
        /// No Icon
        /// </summary>
        None,
        /// <summary>
        /// Info / i Icon
        /// </summary>
        Info,
        /// <summary>
        /// Notification / Exclamation Mark Icon
        /// </summary>
        Notify,
        /// <summary>
        /// Question Mark Icon
        /// </summary>
        Question,
        /// <summary>
        /// Worning Icon
        /// </summary>
        Warning,
        /// <summary>
        /// Error Icon
        /// </summary>
        Error,
        /// <summary>
        /// Critical Icon
        /// </summary>
        Critical,
    }

    /// <summary>
    /// Buttons Purpose On The Promt
    /// </summary>
    public enum ButtonResult
    {
        /// <summary>
        /// No Special Efects
        /// </summary>
        None,
        /// <summary>
        /// Defualt / Accept
        /// </summary>
        Accept,
        /// <summary>
        /// Exit / Cancel
        /// </summary>
        Cancel,
        /// <summary>
        /// Defualt / Retry
        /// </summary>
        Retry,
    }

    /// <summary>
    /// File Types Include File Formats And Extentions
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// A Standerd Text File Format | Extention = .txt
        /// Support : ExSettings , ExLog
        /// </summary>
        txt,
        /// <summary>
        /// A Standerd Text File Format | Extention = .dat
        /// Support : ExSettings , ExLog
        /// </summary>
        dat,
        /// <summary>
        /// A Standerd Text File Format | Extention = .log
        /// Support : ExLog
        /// </summary>
        log,
    }

    /// <summary>
    /// Selection Type For Radio Group Box
    /// </summary>
    public enum SelectionType
    {
        /// <summary>
        /// Only One Item
        /// </summary>
        One,
        /// <summary>
        /// More Than One Item
        /// </summary>
        Many,
    }

    /// <summary>
    /// The Alignment Of The Image In A ImageButtton
    /// </summary>
    public enum ImageAlignment
    {
        /// <summary>
        /// Alignment Of Top Center
        /// </summary>
        Top,
        /// <summary>
        /// Alignment Of Bottom Center
        /// </summary>
        Bottom,
        /// <summary>
        /// Alignment Of Left Center
        /// </summary>
        Left,
        /// <summary>
        /// Alignment Of Right Center
        /// </summary>
        Right,
        /// <summary>
        /// Alignment Of Dead Center
        /// </summary>
        Center,
    }

}
