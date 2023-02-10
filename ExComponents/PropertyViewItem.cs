using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ExtraFunctions.Extras;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// WIP :
    /// A Small Object To Display Properties
    /// </summary>
    [ToolboxItem(false)]
    public class PropertyViewItem : HeaderedContentControl, INotifyPropertyChanged
    {
        static PropertyViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyViewItem), new FrameworkPropertyMetadata(typeof(PropertyViewItem)));
        }

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// IsReadOnly Property
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(PropertyViewItem), new PropertyMetadata(false));

        /// <summary>
        /// Category Property
        /// </summary>
        public static DependencyProperty CategoryProperty =
            DependencyProperty.Register(nameof(Category), typeof(string), typeof(PropertyViewItem), new PropertyMetadata("Default"));

        /// <summary>
        /// Headers Bruch Property
        /// </summary>
        public static DependencyProperty HeaderBrushProperty =
            DependencyProperty.Register(nameof(HeaderBrush), typeof(Brush), typeof(PropertyViewItem),
                new PropertyMetadata(Brushes.SkyBlue));

        /// <summary>
        /// Whether Its Editable Or Not
        /// </summary>
        [Description("Value Is Enabled"), Category("Common")]
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); OnPropertyChanged(nameof(IsReadOnly)); }
        }

        /// <summary>
        /// The Category To Categorized Its Self With
        /// </summary>
        [Description("How To Categorized This Object In PropertyView"), Category("Apearance")]
        public string Category
        {
            get { return (string)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); OnPropertyChanged(nameof(Category)); }
        }

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
