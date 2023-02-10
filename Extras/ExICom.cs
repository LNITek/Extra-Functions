using System;
using System.Windows.Input;

namespace ExtraFunctions.Extras
{
    /// <summary>
    /// Some Routed Commands. 
    /// Note - These Command Don't Do Anything On There Own. 
    /// Thay Are Use In `CommandBinding` Like : Save, SaveAs, Open, New Ect.
    /// </summary>
    public static class ExCom
    {
        /// <summary>
        /// Exit Command
        /// </summary>
        public static RoutedCommand Exit { get; set; } = new RoutedCommand();
        /// <summary>
        /// Refrech Command
        /// </summary>
        public static RoutedCommand Refresh { get; set; } = new RoutedCommand();
    }

    /// <summary>
    /// Create A Costom Command
    /// </summary>
    public class ExICom : ICommand
    {
        /// <summary>
        /// Used For The CanExecuteFun Parameter To Skip Validation
        /// </summary>
        public static Func<object, bool> NoVal = (sender) => true;

        Action<object> Method;
        Func<object, bool> CanExecuteFun;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="Method">Action to be executed by the command</param>
        /// <param name="CanExecuteFun">A bolean property to containing current permissions to execute the command</param>
        public ExICom(Action<object> Method, Func<object, bool> CanExecuteFun)
        {
            this.Method = Method;
            this.CanExecuteFun = CanExecuteFun;
        }

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="Method">Action to be executed by the command</param>
        public ExICom(Action<object> Method)
        {
            this.Method = Method;
            this.CanExecuteFun = NoVal;
        }

        /// <summary>
        /// Wires CanExecuteChanged event 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="Parameter">CanExecute Functions Parameters</param>
        /// <returns>Result Of CanExecute Function</returns>
        public bool CanExecute(object Parameter) =>
            CanExecuteFun.Invoke(Parameter);

        /// <summary>
        /// Executes The Action
        /// </summary>
        /// <param name="Parameter">Actions Parameters</param>
        public void Execute(object Parameter = null) =>
            Method.Invoke(Parameter);
    }
}
