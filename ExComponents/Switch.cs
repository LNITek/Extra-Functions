using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// A Toggle Swich, Just Like A CheckBox
    /// </summary>
    public class Switch : CheckBox, INotifyPropertyChanged
    {
        static Switch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Switch), new FrameworkPropertyMetadata(typeof(Switch)));
        }

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            var Border = (Border)Template.FindName("Border", this);
            var Dot = (Ellipse)Template.FindName("PART_Dot", this);

            Border.CornerRadius = new CornerRadius(Height / 2);
            Dot.Width = Dot.Height = Height - 5;
            State();

            void State()
            {
                switch (IsChecked)
                {
                    case false: Dot.HorizontalAlignment = HorizontalAlignment.Left; 
                        Border.Background = Background; Dot.Fill = Foreground; Border.BorderThickness = BorderThickness; break;
                    case true: Dot.HorizontalAlignment = HorizontalAlignment.Right;
                        Border.Background = BackgroundToggled; Dot.Fill = ForegroundToggled; Border.BorderThickness = new Thickness(0); break;
                    default: Dot.HorizontalAlignment = HorizontalAlignment.Center;
                        Border.Background = Brushes.White; Dot.Fill = Brushes.Black; Border.BorderThickness = BorderThickness; break;
                }
            }

            Checked += (s, e) => State();
            Unchecked += (s, e) => State();
            Indeterminate += (s, e) => State();

            PreviewMouseDown += (s, e) => 
            {
                if (e.LeftButton != MouseButtonState.Released) return;
                if (IsThreeState && (!IsChecked ?? true)) IsChecked = null;
                IsChecked = (!IsChecked ?? true);
                State();
            };

            base.OnApplyTemplate();
        }

        /// <summary>
        /// The Background Brush When Toggeled
        /// </summary>
        public static DependencyProperty BackgroundToggledProperty =
            DependencyProperty.Register(nameof(BackgroundToggled), typeof(Brush), typeof(Switch));

        /// <summary>
        /// The Foreground Brush When Toggeled
        /// </summary>
        public static DependencyProperty ForegroundToggledProperty =
            DependencyProperty.Register(nameof(ForegroundToggled), typeof(Brush), typeof(Switch));

        /// <summary>
        /// The Background Brush When Toggeled
        /// </summary>
        [Description("The Background Brush When Toggeled"), Category("Brush")]
        public Brush BackgroundToggled
        {
            get { return (Brush)GetValue(BackgroundToggledProperty); }
            set { SetValue(BackgroundToggledProperty, value); OnPropertyChanged(nameof(BackgroundToggled)); }
        }

        /// <summary>
        /// The Foreground Brush When Toggeled
        /// </summary>
        [Description("The Foreground Brush When Toggeled"), Category("Brush")]
        public Brush ForegroundToggled
        {
            get { return (Brush)GetValue(ForegroundToggledProperty); }
            set { SetValue(ForegroundToggledProperty, value); OnPropertyChanged(nameof(ForegroundToggled)); }
        }

        /// <summary>
        /// Event When A Property Changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Exicutes Property Change Event 
        /// </summary>
        protected void OnPropertyChanged(string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
