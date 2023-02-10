using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// A Button To Help Filter Your Collections
    /// </summary>
    public class FilterButton : ItemsControl, INotifyPropertyChanged
    {
        static FilterButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterButton), new FrameworkPropertyMetadata(typeof(FilterButton)));
        }

        TreeView trvList;
        bool Start = true;

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            var Popup = (Popup)Template.FindName("PART_Popup", this);
            trvList = (TreeView)Template.FindName("PART_Tree", this);
            var btnFilter = (ImageButton)Template.FindName("PART_Filter", this);
            var btnSortAsc = (ImageButton)Template.FindName("PART_SortAsc", this);
            var btnSortDesc = (ImageButton)Template.FindName("PART_SortDesc", this);
            var btnFilterClear = (ImageButton)Template.FindName("PART_FilterClear", this);
            var edtSearch = (SearchBox)Template.FindName("PART_Search", this);
            var cbxSelectAll = (CheckBox)Template.FindName("PART_SelectAll", this);

            btnFilter.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.Filter.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));
            btnSortAsc.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.SortAsc.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));
            btnSortDesc.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.SortDesc.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));
            btnFilterClear.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.FilterClear.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));

            btnFilterClear.Click += (sender, e) => cbxSelectAll.IsChecked = true;
            cbxSelectAll.Checked += (sender, e) => GetItems(trvList).ToList().ForEach(x => x.Checked = true);
            cbxSelectAll.Unchecked += (sender, e) => GetItems(trvList).ToList().ForEach(x => x.Checked = false);
            btnFilter.Click += (sender, e) =>
            {
                Popup.IsOpen = !Popup.IsOpen;
                if (cbxSelectAll.IsChecked ?? true && Start) 
                { 
                    Start = false;
                    GetItems(trvList).ToList().ForEach(x =>
                    {
                        x.Checked = true;
                        x.IsCheckedChanged += (s, z) => {
                            bool? state = null;
                            int I = -1;
                            foreach (var Child in GetItems(trvList))
                            {
                                I++;
                                bool? current = Child.IsChecked;
                                if (I == 0)
                                    state = current;
                                else if (state != current)
                                {
                                    state = null;
                                    break;
                                }
                            }
                            cbxSelectAll.IsChecked = state;
                        };
                    });
                }
            };
            btnSortAsc.Click += (sender, e) =>
            { ItemsSource = Items.OfType<object>().OrderBy(x => x.ToString()); Refresh(); };
            btnSortDesc.Click += (sender, e) => 
            { ItemsSource = Items.OfType<object>().OrderByDescending(x => x.ToString()); Refresh(); };
            edtSearch.OnSearch += (sender, e) =>
                GetItems(trvList).ToList().ForEach(x => x.Filter(edtSearch.Text));

            base.OnApplyTemplate();

            void Refresh()
            {
                GetItems(trvList).ToList().ForEach(x =>
                {
                    bool? state = null;
                    int I = -1;
                    foreach (var Child in GetItems(trvList))
                    {
                        I++;
                        bool? current = Child.IsChecked;
                        if (I == 0)
                            state = current;
                        else if (state != current)
                        {
                            state = null;
                            break;
                        }
                    }
                    cbxSelectAll.IsChecked = state;
                });
            }
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
        /// Selected Items
        /// </summary>
        public IEnumerable<object> SelectedItems
        {
            get
            {
                foreach (var Item in GetItems(trvList)) 
                    foreach (var Sel in GetSelectedItems(Item))
                        yield return Sel;

                IEnumerable<object> GetSelectedItems(CheckBoxTreeViewItem ParentItem)
                {
                    if(ParentItem == null) yield break;
                    foreach (var Child in ParentItem.Children)
                        if (Child.Children.Count <= 0 && (Child.IsChecked ?? false))
                            yield return Child.Header;
                        else if (Child.IsChecked ?? true)
                            foreach (var Sel in GetSelectedItems(Child))
                                yield return Sel;
                }
            }
        }

        internal IEnumerable<CheckBoxTreeViewItem> GetItems(TreeView Tree)
        {
            foreach (var Item in Tree.Items)
            {
                var ContentChild = Tree.ItemContainerGenerator.ContainerFromItem(Item);
                if (ContentChild == null) yield break;
                var Child = ContentChild.GetType().GetProperty("TemplateChild", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(ContentChild);
                if (Child is CheckBoxTreeViewItem)
                    yield return Child as CheckBoxTreeViewItem;
            }
        }
    }
}
