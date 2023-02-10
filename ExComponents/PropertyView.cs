using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// Displays Properties Like In VS
    /// </summary>
    public class PropertyView : HeaderedItemsControl, INotifyPropertyChanged
    {
        static PropertyView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyView), new FrameworkPropertyMetadata(typeof(PropertyView)));
        }

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Headers Bruch Property
        /// </summary>
        public static DependencyProperty HeaderBrushProperty =
            DependencyProperty.Register(nameof(HeaderBrush), typeof(Brush), typeof(PropertyView),
                new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// The Background Brush For The Titel
        /// </summary>
        [Description("Background Brush For The Title"), Category("Brush")]
        public Brush HeaderBrush
        {
            get { return (Brush)GetValue(HeaderBrushProperty); }
            set { SetValue(HeaderBrushProperty, value); }
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
