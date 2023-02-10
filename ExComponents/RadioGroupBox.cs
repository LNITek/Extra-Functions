﻿using ExtraFunctions.Extras;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// Group Box With Radio Buttons
    /// </summary>
    public class RadioGroupBox : Control
    {
        private bool EnableSelectionEvent = true;

        static RadioGroupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioGroupBox), new FrameworkPropertyMetadata(typeof(RadioGroupBox)));
        }

        Grid GridDisplay;

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            GridDisplay = (Grid)Template.FindName("PART_Grid", this);
            for (int I = 0; I < ColumnCount; I++)
                GridDisplay.ColumnDefinitions.Add(new ColumnDefinition());
            GridDisplay.RowDefinitions.Add(new RowDefinition());
            int Column = -1, Row = 0;
            if (ItemsSource != null)
                foreach (var Item in ItemsSource)
                {
                    Column++;
                    if (Column >= ColumnCount)
                    {
                        Column = 0;
                        Row++;
                        GridDisplay.RowDefinitions.Add(new RowDefinition());
                    }
                    var cbx = new RadioButton() { Content = Item.ToString() };
                    cbx.Checked += Select;
                    Children.Add(cbx);
                    Grid.SetColumn(cbx, Column);
                    Grid.SetRow(cbx, Row);
                    GridDisplay.Children.Add(cbx);
                }

            base.OnApplyTemplate();

            void Select(object sender, RoutedEventArgs e)
            {
                if (!EnableSelectionEvent) return;
                if (SelectionMode == SelectionType.One)
                {
                    EnableSelectionEvent = false;
                    var I = Children.IndexOf((RadioButton)sender);
                    foreach (var Item in Children.Where(x => x.IsChecked ?? false))
                        Item.IsChecked = false;
                    Children[I].IsChecked = true;
                    EnableSelectionEvent = true;
                }

                RaiseEvent(new RoutedEventArgs(SelectionChangedEvent, this));
                OnPropertyChanged(nameof(SelectedItems));
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        /// <summary>
        /// Items Source Property
        /// </summary>
        public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<object>), typeof(RadioGroupBox));

        /// <summary>
        /// Total Column Property
        /// </summary>
        public static DependencyProperty ColumnCountProperty =
            DependencyProperty.Register(nameof(ColumnCount), typeof(int), typeof(RadioGroupBox), new PropertyMetadata(1));

        /// <summary>
        /// The Selection Mode
        /// </summary>
        public static DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionType), typeof(RadioGroupBox), 
                new PropertyMetadata(SelectionType.One));

        /// <summary>
        /// Event For When The Selected Items Changes
        /// </summary>
        public static RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged),
            RoutingStrategy.Bubble, typeof(EventHandler), typeof(RadioGroupBox));

        /// <summary>
        /// Event When The Selected Items Changes
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
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

        /// <summary>
        /// Gets an object representing the collection of the items contained in the items control
        /// </summary>
        [Description("Gets an object representing the collection of the items contained in the items control"), Category("Common")]
        public IEnumerable<object> Items { get { return ItemsSource; } }

        /// <summary>
        /// Gets or sets a collection that is used to generate the content of the control
        /// </summary>
        [Description("Gets or sets a collection that is used to generate the content of the control"), Category("Common")]
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); OnPropertyChanged(nameof(ItemsSource)); OnPropertyChanged(nameof(Items)); }
        }

        /// <summary>
        /// Total Column
        /// </summary>
        [Description("Total Columns"), Category("Common")]
        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set
            {
                if (value <= 0) throw new IndexOutOfRangeException("Columns Can Not Be Less Than 0");
                SetValue(ColumnCountProperty, value); OnPropertyChanged(nameof(ColumnCount));
            }
        }

        /// <summary>
        /// The Selection Mode
        /// </summary>
        [Description("Selection Mode"), Category("Common")]
        public SelectionType SelectionMode
        {
            get { return (SelectionType)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); OnPropertyChanged(nameof(SelectionMode)); }
        }

        /// <summary>
        /// Selected Items
        /// </summary>
        public IEnumerable<object> SelectedItems
        {
            get
            {
                return Children.Where(x => x.IsChecked ?? false).Select(x => ItemsSource.ToArray()[Children.IndexOf(x)]);
            }
        }

        /// <summary>
        /// Selected Items Indexed
        /// </summary>
        public IEnumerable<int> SelectedIndex
        {
            get
            {
                return Children.Where(x => x.IsChecked ?? false).Select(x => Children.IndexOf(x));
            }
        }

        /// <summary>
        /// All The Children In The Object
        /// </summary>
        public List<RadioButton> Children { get; private set; } = new List<RadioButton>();
    }
}