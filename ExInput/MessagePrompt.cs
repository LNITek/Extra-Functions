using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using ExtraFunctions.Extras;

namespace ExtraFunctions.ExInput
{
    /// <summary>
    /// Create A Popup With An Icon And Custom Buttons
    /// </summary>
    public class MessagePrompt : ExInputBase, IExInput
    {
        readonly Image imgIcon = new Image()
        {
            Height = 75,
            Width = 75,
            Margin = new Thickness(5),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        /// <summary>
        /// Create A Popup With An Icon And Custom Buttons
        /// </summary>
        /// <param name="Parent">The Parent In Wich The Prompt Must Center In (Nullable)</param>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Icon">The Icon To Display</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public MessagePrompt(IWin32Window Parent, string Title, string PromptText, Icons Icon = Icons.None, 
            BasicButton[] Buttons = default)
        {
            Main.Owner = (Window)Parent;
            this.Title = Title;
            Message = PromptText;
            Value = "";
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);


            var vIcon = LoadIcon(Icon);
            if (Icon == Icons.None)
                imgIcon.Width = 0;
            else
                imgIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(
                    vIcon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(100, 100));
            ExComponent = imgIcon;
            SetSlotA();
        }

        /// <summary>
        /// Create A Popup With An Icon And Custom Buttons
        /// </summary>
        /// <param name="Title">The Title / Heading Of The Prompt</param>
        /// <param name="PromptText">The Message Within The Prompt</param>
        /// <param name="Icon">The Icon To Display</param>
        /// <param name="Buttons">Add Custom Buttons Or Use default Set</param>
        public MessagePrompt(string Title, string PromptText, Icons Icon = Icons.None, BasicButton[] Buttons = default)
        {
            this.Title = Title;
            Message = PromptText;
            Value = "";
            if (Buttons == default)
            {
                lstButtons.Add(new BasicButton(ButtonResult.Accept));
                lstButtons.Add(new BasicButton(ButtonResult.Cancel));
            }
            else
                lstButtons.AddRange(Buttons);


            var vIcon = LoadIcon(Icon);
            if (Icon == Icons.None)
                imgIcon.Width = 0;
            else
                imgIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(
                    vIcon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(100, 100));
            ExComponent = imgIcon;
            SetSlotA();
        }

        private Bitmap LoadIcon(Icons Icon)
        {
            switch (Icon)
            {
                case Icons.Info: return ExInputRes.Info_Icon;
                case Icons.Notify: return ExInputRes.Notifi_Icon;
                case Icons.Question: return ExInputRes.Question_Icon;
                case Icons.Warning: return ExInputRes.Warning_Icon;
                case Icons.Error: return ExInputRes.Error_Icon;
                case Icons.None: return null;
                default: return ExInputRes.None_Icon;
            }
        }
    }
}
