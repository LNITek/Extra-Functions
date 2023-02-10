using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// A TextBox With A Search Style
    /// </summary>
    public class SearchBox : TextBox
    {
        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));
        }

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            var edtFind = (TextBox)Template.FindName("PART_Search", this);
            var btnSearch = (ImageButton)Template.FindName("PART_Accept", this);

            edtFind.SetBinding(TextProperty, new Binding(nameof(Text)) { Source = this, Mode = BindingMode.TwoWay });
            btnSearch.Source = Imaging.CreateBitmapSourceFromHBitmap(ExComponentsRes.Search.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));

            edtFind.PreviewKeyDown += (sender, e) => 
            {
                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    Text = edtFind.Text;
                    RaiseEvent(new RoutedEventArgs(OnSearchEvent, this));
                }
            };
            btnSearch.Click += (sender, e) => RaiseEvent(new RoutedEventArgs(OnSearchEvent, this));

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Event For When The Popup Opens
        /// </summary>
        public static RoutedEvent OnSearchEvent = EventManager.RegisterRoutedEvent(nameof(OnSearch),
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchBox));

        /// <summary>
        /// Event When The Popup Opens
        /// </summary>
        public event RoutedEventHandler OnSearch
        {
            add { AddHandler(OnSearchEvent, value); }
            remove { RemoveHandler(OnSearchEvent, value); }
        }

    }
}
