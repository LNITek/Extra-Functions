﻿using System;
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
    /// <summary>
    /// </summary>
    public partial class Win10DataGridStyle
    {
        void CelSelect(object sender, RoutedEventArgs e)
        {
            var GridDisplay = ((DataGridCell)sender).GetType().GetProperty("DataGridOwner", 
                BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sender) as DataGrid;
            var SelectedRow = -1;
            if (!string.IsNullOrWhiteSpace((GridDisplay.Tag ?? "").ToString()))
                _ = int.TryParse(GridDisplay.Tag.ToString(), out SelectedRow);
            if (SelectedRow >= 0 && SelectedRow < GridDisplay.Items.Count)
            {
                var PreRow = (DataGridRow)GridDisplay.ItemContainerGenerator.ContainerFromIndex(SelectedRow);
                PreRow.ClearValue(Control.BackgroundProperty);
                PreRow.ClearValue(Control.BorderBrushProperty);
            }
            SelectedRow = GridDisplay.Items.IndexOf(GridDisplay.SelectedCells[0].Item);
            var Row = (DataGridRow)GridDisplay.ItemContainerGenerator.ContainerFromIndex(SelectedRow);
            Row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#b8cfde");
            Row.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#7c98ab");
            GridDisplay.Tag = SelectedRow;
        }
    }
}
