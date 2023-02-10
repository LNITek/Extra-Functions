using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// A Fully Functional ResizeGrip Control
    /// </summary>
    public class ResizeGrip : Control, INotifyPropertyChanged
    {
        static ResizeGrip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeGrip), new FrameworkPropertyMetadata(typeof(ResizeGrip)));
        }

        /// <summary>
        /// The Resize Mode For ResizeGrip Control
        /// </summary>
        public enum ResizeGripType
        {
            /// <summary>
            /// Both Axes : X + Y
            /// </summary>
            All,
            /// <summary>
            /// Horizontal Axes : X
            /// </summary>
            Horizontal,
            /// <summary>
            /// Vertical Axes : Y
            /// </summary>
            Vertical,
        }

        System.Windows.Controls.Primitives.ResizeGrip Resizer;
        Point StartPosition, TargetSize;
        bool IsResizing = false;

        /// <summary>
        /// Init The Component
        /// </summary>
        public override void OnApplyTemplate()
        {
            Resizer = (System.Windows.Controls.Primitives.ResizeGrip)Template.FindName("PART_ResizeGrip", this);
            switch (ResizeGripMode)
            {
                default: Resizer.Cursor = Cursors.SizeAll; break;
                case ResizeGripType.All: Resizer.Cursor = Cursors.SizeNWSE; break;
                case ResizeGripType.Horizontal: Resizer.Cursor = Cursors.SizeWE; break;
                case ResizeGripType.Vertical: Resizer.Cursor = Cursors.SizeNS; break;
            }

            Resizer.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (Mouse.Capture(Resizer))
                {
                    StartPosition = Mouse.GetPosition(Resizer);
                    IsResizing = true;
                }
            };
            Resizer.PreviewMouseLeftButtonUp += (sender, e) =>
            {
                if (IsResizing == true)
                {
                    IsResizing = false;
                    Mouse.Capture(null);
                }

            };
            Resizer.PreviewMouseMove += (sender, e) =>
            {
                if (IsResizing && Target != null && IsEnabled)
                {
                    TargetSize.X = ((FrameworkElement)Target).Width;
                    TargetSize.Y = ((FrameworkElement)Target).Height;

                    var CurrentPosition = Mouse.GetPosition(Resizer);
                    if (ResizeGripMode != ResizeGripType.Vertical)
                        TargetSize.X += CurrentPosition.X - StartPosition.X;
                    if (ResizeGripMode != ResizeGripType.Horizontal)
                        TargetSize.Y += CurrentPosition.Y - StartPosition.Y;

                    StartPosition.X = CurrentPosition.X;

                    if (TargetSize.X > 0)
                        ((FrameworkElement)Target).Width = TargetSize.X;
                    if (TargetSize.Y > 0)
                        ((FrameworkElement)Target).Height = TargetSize.Y;
                }
            };
        }

        /// <summary>
        /// The Target To Resize
        /// </summary>
        public static DependencyProperty TargetProperty =
            DependencyProperty.Register(nameof(Target), typeof(UIElement), typeof(ResizeGrip));

        /// <summary>
        /// The GripMode
        /// </summary>
        public static DependencyProperty ResizeModeProperty =
            DependencyProperty.Register(nameof(ResizeGripMode), typeof(ResizeGripType), typeof(ResizeGrip),
                new PropertyMetadata(ResizeGripType.All));

        /// <summary>
        /// The Target To Resize
        /// </summary>
        [Description("Gets or sets the component that will be resized."), Category("Common")]
        public UIElement Target
        {
            get { return (UIElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); OnPropertyChanged(nameof(Target)); }
        }

        /// <summary>
        /// The Axes That Can Resize
        /// </summary>
        [Description("Gets or sets the resize mode."), Category("Common")]
        public ResizeGripType ResizeGripMode
        {
            get { return (ResizeGripType)GetValue(ResizeModeProperty); }
            set { SetValue(TargetProperty, value); OnPropertyChanged(nameof(ResizeGripType)); }
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
