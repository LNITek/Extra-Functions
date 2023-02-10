using System.ComponentModel;
using System.Windows.Forms;

namespace ExtraFunctions.ExComponents
{
    /// <summary>
    /// A Bindable Version Of A System.Windows.Forms ToolStripMenuItem
    /// Acts The Same As Any Other System.Windows.Forms Component
    /// </summary>
    public partial class BindableToolStripMenuItem : ToolStripMenuItem, IBindableComponent
    {
        private BindingContext bindingContext;
        private ControlBindingsCollection dataBindings;

        /// <summary>
        /// The Binding Context
        /// </summary>
        [Browsable(false)]
        public BindingContext BindingContext
        {
            get
            {
                return bindingContext ?? new BindingContext();
            }
            set
            {
                bindingContext = value;
            }
        }

        /// <summary>
        /// Binding Property
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBindingsCollection DataBindings
        {
            get
            {
                return dataBindings ?? new ControlBindingsCollection(this);
            }
        }
    }
}
