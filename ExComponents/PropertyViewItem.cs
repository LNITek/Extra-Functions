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
    public class PropertyViewItem : Control, INotifyPropertyChanged
    {
        static PropertyViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyViewItem), new FrameworkPropertyMetadata(typeof(PropertyViewItem)));
        }

        private DockPanel pnlValue;

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            pnlValue = (DockPanel)Template.FindName("PART_Control", this);

            pnlValue.Children.Clear();
            pnlValue.Children.Add(GetElement(this, Type));
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Right Side: Value Property
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(PropertyViewItem),
                new PropertyMetadata("", ValuePropertyChanged));

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyViewItem PropView)
            {
                PropView.RaiseEvent(new RoutedEventArgs(ValueChangedEvent, PropView));
                PropView.OnPropertyChanged(nameof(Value));
            }
        }

        /// <summary>
        /// Left Side: Titel Property
        /// </summary>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(PropertyViewItem), new PropertyMetadata("Property"));

        /// <summary>
        /// Editable Property
        /// </summary>
        public static DependencyProperty EditableProperty =
            DependencyProperty.Register(nameof(Editable), typeof(bool), typeof(PropertyViewItem), new PropertyMetadata(true));

        /// <summary>
        /// Type Property
        /// </summary>
        public static DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(PropertyType), typeof(PropertyViewItem),
                new PropertyMetadata(PropertyType.Text, TypePropertyChanged));

        private static void TypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyViewItem PropView)
            {
                if (PropView.pnlValue == null) return;
                PropView.pnlValue.Children.Clear();
                PropView.pnlValue.Children.Add(GetElement(PropView,(PropertyType)e.NewValue));
                PropView.OnPropertyChanged(nameof(Type));
            }
        }

        /// <summary>
        /// Title Bruch Property
        /// </summary>
        public static DependencyProperty TitleBrushProperty =
            DependencyProperty.Register(nameof(TitleBrush), typeof(Brush), typeof(PropertyViewItem),
                new PropertyMetadata(Brushes.SkyBlue));

        /// <summary>
        /// Items Source Property
        /// </summary>
        public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(PropertyViewItem));

        /// <summary>
        /// The Value Typed, Selected Or Displayed
        /// </summary>
        [Description("Display Value"), Category("Common")]
        public string Value
        {
            get { return GetValue(ValueProperty).ToString(); }
            set { SetValue(ValueProperty, value); }
        }
        /// <summary>
        /// The Titel On The Right Side
        /// </summary>
        [Description("Display Titel"), Category("Common")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); OnPropertyChanged(nameof(Title)); }
        }
        /// <summary>
        /// Whether Its Editable Or Not
        /// </summary>
        [Description("Value Is Enabled"), Category("Common")]
        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); OnPropertyChanged(nameof(Editable)); }
        }
        /// <summary>
        /// The Mode To Display Input
        /// </summary>
        [Description("Object Type"), Category("Common")]
        public PropertyType Type
        {
            get { return (PropertyType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// The Background Brush For The Titel
        /// </summary>
        [Description("Background Brush For The Title"), Category("Brush")]
        public Brush TitleBrush
        {
            get { return (Brush)GetValue(TitleBrushProperty); }
            set { SetValue(TitleBrushProperty, value); }
        }

        /// <summary>
        /// The Items
        /// </summary>
        public IEnumerable Items { get { return ItemsSource; } }

        /// <summary>
        /// The Items Source
        /// </summary>
        [Description("The Collection Used To Create Its Contens"), Category("Common")]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); OnPropertyChanged(nameof(ItemsSource)); OnPropertyChanged(nameof(Items)); }
        }

        /// <summary>
        /// Event For When The Value Changes
        /// </summary>
        public static RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChaged", RoutingStrategy.Bubble,
            typeof(EventHandler), typeof(PropertyViewItem));

        /// <summary>
        /// Event When The Value Changes
        /// </summary>
        public event EventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
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

        private static UIElement GetElement(PropertyViewItem This, PropertyType Type)
        {
            switch (Type)
            {
                case PropertyType.Text:
                    var edt = new TextBox()
                    {
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent,
                        BorderThickness = new Thickness(.0),
                    };
                    edt.SetBinding(IsEnabledProperty, new Binding(nameof(Editable)) 
                    { Source = This, Mode = BindingMode.TwoWay });
                    edt.SetBinding(TextBox.TextProperty, new Binding(nameof(Value))
                    { Source = This, Mode = BindingMode.TwoWay });
                    return edt;
                case PropertyType.Number:
                    var nbx = new DigitBox()
                    {
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent,
                        BorderThickness = new Thickness(.0),
                    };
                    nbx.SetBinding(IsEnabledProperty, new Binding(nameof(Editable))
                    { Source = This, Mode = BindingMode.TwoWay });
                    nbx.SetBinding(DigitBox.TextProperty, new Binding(nameof(Value))
                    { Source = This, Mode = BindingMode.TwoWay });
                    return nbx;
                case PropertyType.Combo:
                    var cmd = new ComboBox()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent,
                        BorderThickness = new Thickness(.0),
                    };
                    cmd.SetBinding(ComboBox.IsEditableProperty, new Binding(nameof(Editable))
                    { Source = This, Mode = BindingMode.TwoWay });
                    cmd.SetBinding(ComboBox.ItemsSourceProperty, new Binding(nameof(ItemsSource)) { Source = This });
                    cmd.SetBinding(ComboBox.SelectedValueProperty, new Binding(nameof(Value))
                    { Source = This, Mode = BindingMode.TwoWay });
                    return cmd;
                default:
                    var lbl = new TextBlock()
                    {
                        Background = Brushes.Transparent,
                    };
                    lbl.SetBinding(IsEnabledProperty, new Binding(nameof(Editable))
                    { Source = This, Mode = BindingMode.TwoWay });
                    lbl.SetBinding(TextBlock.TextProperty, new Binding(nameof(Value))
                    { Source = This, Mode = BindingMode.TwoWay });
                    return lbl;
            }
        }
    }
}
