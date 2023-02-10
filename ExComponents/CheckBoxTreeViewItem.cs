using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// A Tree View Item, But With Check Boxes
    /// </summary>
    public class CheckBoxTreeViewItem : HeaderedItemsControl, INotifyPropertyChanged
    {
        static CheckBoxTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxTreeViewItem), new FrameworkPropertyMetadata(typeof(CheckBoxTreeViewItem)));
        }
        List<bool?> Updates = new List<bool?>();
        bool Start = true;

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            var cbx = (CheckBox)Template.FindName("PART_Box", this);

            cbx.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(Checked)) { Source = this, Mode = BindingMode.TwoWay });

            Loaded += (sender, e) => Initialize();

            base.OnApplyTemplate();
        }

        /// <summary>
        /// A String Repesenting This Objects Header.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Header.ToString();

        internal List<CheckBoxTreeViewItem> Children
        {
            get
            {
                var List = new List<CheckBoxTreeViewItem>();
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] is CheckBoxTreeViewItem Item)
                    {
                        List.Add(Item);
                        continue;
                    }
                    var ContentChild = ItemContainerGenerator.ContainerFromIndex(i);
                    if (ContentChild == null) throw new NullReferenceException("ContentChild Is Null");
                    var Child = ContentChild.GetType().GetProperty("TemplateChild", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(ContentChild) as CheckBoxTreeViewItem;
                    //Child.Parent = this;
                    List.Add(Child);
                }
                return List;
            }
        }

        /// <summary>
        /// Controlls Header
        /// </summary>
        public static DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(CheckBoxTreeViewItem),
                new PropertyMetadata(true));

        /// <summary>
        /// Is Item Checked
        /// </summary>
        public static DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool?), typeof(CheckBoxTreeViewItem),
                new PropertyMetadata(false, IsCheckedPropertyChanged));

        private static void IsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CheckBoxTreeViewItem cbxTVI)
            {
                if(cbxTVI.Start)
                    cbxTVI.Updates.Add((bool?)e.NewValue);
                else
                    cbxTVI.RaiseEvent(new RoutedEventArgs(IsCheckedChangedEvent, cbxTVI));
            }
        }

        /// <summary>
        /// Event For When The Value Changes
        /// </summary>
        public static RoutedEvent IsCheckedChangedEvent = EventManager.RegisterRoutedEvent(nameof(IsCheckedChanged),
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CheckBoxTreeViewItem));

        /// <summary>
        /// Event When The Value Changes
        /// </summary>
        public event RoutedEventHandler IsCheckedChanged
        {
            add { AddHandler(IsCheckedChangedEvent, value); }
            remove { RemoveHandler(IsCheckedChangedEvent, value); }
        }

        /// <summary>
        /// Whether The Items Is Expanded Or Collapsed
        /// </summary>
        [Description("Gets or sets whether items in this element is expanded or collapsed"), Category("Miscellaneous")]
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); OnPropertyChanged(nameof(IsExpanded)); }
        }

        /// <summary>
        /// Is Item Checked And All Its Children. 
        /// DO NOT Use This Property In Your Code! Use `Checked` Insted.
        /// </summary>
        [Description("Gets or sets whether the item is in a checked state."), Category("Common")]
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Get And Set The Checked State Of This Item And Its Children.
        /// </summary>
        public bool? Checked
        {
            get { return IsChecked; }
            set { SetIsChecked(value, true, true); }
        }

        internal void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == IsChecked)
                return;

            SetValue(IsCheckedProperty, value);

            if (updateChildren && IsChecked.HasValue)
                Children.ForEach(c => c.SetIsChecked(IsChecked, true, false));

            if (updateParent && Parent != null)
                Parent.VerifyCheckState();

            OnPropertyChanged(nameof(Checked));
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                    state = current;
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            SetIsChecked(state, false, true);
        }

        /// <summary>
        /// Parent Object
        /// </summary>
        public new CheckBoxTreeViewItem Parent { get; private set; }

        void Initialize()
        {
            foreach (CheckBoxTreeViewItem child in Children)
            {
                child.Parent = this;
                child.Initialize();
            }

            Start = false;
            foreach (var Update in Updates)
            {
                IsChecked = false;
                Checked = Update;
            }
            Updates.Clear();
        }

        /// <summary>
        /// Filters This Object And Its Children By The Header Property. 
        /// Not Case Sensetive.
        /// </summary>
        /// <param name="Filter">The Filter (As String) To Filter Objects Out.</param>
        public bool Filter(string Filter)
        {
            bool Filterd = !string.IsNullOrWhiteSpace(Filter) && Header.ToString().ToLower().Contains(Filter.ToLower());
            Children.ForEach(x => Filterd = x.Filter(Filter) || Filterd );
            Visibility = Filterd ? Visibility.Visible : Visibility = Visibility.Collapsed;
            return Filterd;
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
