namespace ExtraFunctions.Extras
{
    /// <summary>
    /// Helps Manage Console Commands
    /// </summary>
    public class ExCMD
    {
        /// <summary>
        /// The Command
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// Using Discription
        /// </summary>
        public string Usige { get; set; }
        /// <summary>
        /// Help / Info Description
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// Helps Manage Console Commands
        /// </summary>
        /// <param name="command">The Command</param>
        /// <param name="usige">What It's Used For \ Discription</param>
        /// <param name="help">How To Use The Command</param>
        public ExCMD(string command, string usige, string help)
        {
            Command = command;
            Usige = usige;
            Help = help;
        }

        /// <summary>
        /// Returnds A Line With All The Properties
        /// </summary>
        /// <returns>A String With All The Properties</returns>
        public override string ToString() =>
            $"{Command} \t\t\t\t\t = {Usige} \t\t\t\t\t : {Help}";
    }
}
