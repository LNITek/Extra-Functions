using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace ExtraFunctions.ExInput
{
    /// <summary>
    /// Interface For Custom Inputs
    /// </summary>
    public interface IExInput
    {
        /// <summary>
        /// Title Of The Prompt
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// Prompt Message
        /// </summary>
        string Message { get; set; }
        /// <summary>
        /// List Of Custom Buttons
        /// </summary>
        List<BasicButton> lstButtons { get; set; }

        /// <summary>
        /// A Returning Value
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Main Window
        /// </summary>
        Window Main { get; set; }
        /// <summary>
        /// Stack Panel That Contains The Buttons
        /// </summary>
        DockPanel pnlBTN { get; set; }
        /// <summary>
        /// The Message Component
        /// </summary>
        TextBlock lblMessage { get; set; }
        /// <summary>
        /// Extra Component
        /// </summary>
        UIElement ExComponent { get; set; }
        /// <summary>
        /// A Grid For Structure
        /// </summary>
        Grid GridX { get; set; }

        /// <summary>
        /// Shows The Prompt
        /// </summary>
        /// <returns>DialogResult Of The Button</returns>
        bool Show();
    }
}
