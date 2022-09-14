using System.ComponentModel;
using System.Windows.Forms;

namespace ExtraFunctions.Components
{
    /// <summary>
    /// A Bindable Version Of A System.Windows.Forms ToolStripMenuItem
    /// Acts The Same As Any Other System.Windows.Forms Component
    /// </summary>
    public partial class BindableToolStripMenuItem : ToolStripMenuItem, IBindableComponent
    {
        private BindingContext? bindingContext;
        private ControlBindingsCollection? dataBindings;

        /// <summary>
        /// The Binding Context
        /// </summary>
        [Browsable(false)]
        public BindingContext BindingContext
        {
            get
            {
                bindingContext ??= new BindingContext();

                return bindingContext;
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
                dataBindings ??= new ControlBindingsCollection(this);

                return dataBindings;
            }
        }
    }
}
