using static CleverWidget.MyWidget;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;

using Microsoft.Win32;

namespace CleverWidget
{
    /// <summary>
    /// Interaction logic for MyWidgetControls.xaml
    /// </summary>
    /// 

    public partial class MyWidgetControls : UserControl
    {
        private MyWidget ParentWidget;

        public MyWidgetControls(MyWidget parent)
        {
            ParentWidget = parent;
            InitializeComponent();

            hideCountCheckBox.IsChecked = ParentWidget.HideCount;

            optionCombo.ItemsSource = new List<int> { 1, 2, 3, 4, 5 }; // increment values
            optionCombo.SelectedIndex = parent.DemoIncrementValue - 1;


            {
                // CleverWidgetBase inhereited* controls
                var imageControls = new WidgetImageControls(parent);
                ImageControlsPlaceholder.Content = imageControls;

                var textControls = new WidgetTextControls(parent);
                TextControlsPlaceholder.Content = textControls;

                var themeControls = new WidgetThemeControls(parent);
                ThemeControlsPlaceholder.Content = themeControls;

                // you can override visibility and label contents here to make them contextual

                // e.g.
                //themeControls.primaryFontUI.Visibility = Visibility.Collapsed;
                //themeControls.primaryFGColorLabel.Content = "Vol. Bar Color";
            }
        }

        private void hideCountCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ParentWidget.HideCount = hideCountCheckBox.IsChecked ?? false;
        }

        private void HandleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ParentWidget.DemoIncrementValue = (int)e.AddedItems[0];
            }
        }
    }

}
