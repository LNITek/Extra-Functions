using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace ExtraFunctions.ExStyles
{
    public partial class Win11DataGridStyle
    {
        int SelectedRow { get; set; } = -1;

        void CelSelect(object sender, RoutedEventArgs e)
        {
            var Cell = sender as DataGridCell;
            var GridDisplay = Cell.GetType().GetProperty("DataGridOwner", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Cell) as DataGrid;
            if (SelectedRow >= 0)
            {
                var PreRow = (DataGridRow)GridDisplay.ItemContainerGenerator.ContainerFromIndex(SelectedRow);
                PreRow.ClearValue(Control.BackgroundProperty);
                PreRow.ClearValue(Control.BorderBrushProperty);
            }
            SelectedRow = GridDisplay.Items.IndexOf(GridDisplay.SelectedCells[0].Item);
            var Row = (DataGridRow)GridDisplay.ItemContainerGenerator.ContainerFromIndex(SelectedRow);
            Row.Background = Brushes.LightGray;
            Row.BorderBrush = Brushes.Gray;
        }
    }
}
