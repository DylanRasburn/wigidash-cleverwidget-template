using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;


namespace CleverWidget
{
    /// <summary>
    /// Interaction logic for WidgetThemeControls.xaml
    /// </summary>
    public partial class WidgetThemeControls : UserControl
    {
        protected CleverWidgetBase ParentWidget;

        public WidgetThemeControls(MyWidget parent)
        {
            InitializeComponent();
            ParentWidget = parent;
            DataContext = this;

            useGlobalThemeCheckbox.IsChecked = ParentWidget.UseGlobalTheme;

            primaryFontSelect.Tag = ParentWidget.UserFontPrimary;
            primaryFontSelect.Content =
                new FontConverter().ConvertToInvariantString(ParentWidget.UserFontPrimary);
            secondaryFontSelect.Tag = ParentWidget.UserFontSecondary;
            secondaryFontSelect.Content =
                new FontConverter().ConvertToInvariantString(ParentWidget.UserFontSecondary);

            primaryFGColorSelect.Content = ColorTranslator.ToHtml(ParentWidget.UserForeColorPrimary);
            primaryFGColorSelect.Background = new SolidColorBrush(
                CleverWidgetBase.ConvertDrawingColor(ColorTranslator.FromHtml(primaryFGColorSelect.Content as string)));
            secondaryFGColorSelect.Content = ColorTranslator.ToHtml(ParentWidget.UserForeColorSecondary);
            secondaryFGColorSelect.Background = new SolidColorBrush(
                CleverWidgetBase.ConvertDrawingColor(ColorTranslator.FromHtml(secondaryFGColorSelect.Content as string)));
            primaryBGColorSelect.Content = ColorTranslator.ToHtml(ParentWidget.UserBackColorPrimary);
            primaryBGColorSelect.Background = new SolidColorBrush(
                CleverWidgetBase.ConvertDrawingColor(ColorTranslator.FromHtml(primaryBGColorSelect.Content as string)));
            secondaryBGColorSelect.Content = ColorTranslator.ToHtml(ParentWidget.UserBackColorSecondary);
            secondaryBGColorSelect.Background = new SolidColorBrush(
                CleverWidgetBase.ConvertDrawingColor(ColorTranslator.FromHtml(secondaryBGColorSelect.Content as string)));

            primaryFGColorSelect.Foreground = new SolidColorBrush(
                CleverWidgetBase.GetTextShadeForBgColor(primaryFGColorSelect.Background));
            secondaryFGColorSelect.Foreground = new SolidColorBrush(
                CleverWidgetBase.GetTextShadeForBgColor(secondaryFGColorSelect.Background));
            primaryBGColorSelect.Foreground = new SolidColorBrush(
                CleverWidgetBase.GetTextShadeForBgColor(primaryBGColorSelect.Background));
            secondaryBGColorSelect.Foreground = new SolidColorBrush(
                CleverWidgetBase.GetTextShadeForBgColor(secondaryBGColorSelect.Background));

            cornerRadiusUpDown.Value = ParentWidget.UserCornerRadius;
        }

        private void useGlobalThemeCheckbox_Click(object sender, RoutedEventArgs e)
        {
            ParentWidget.UseGlobalTheme = useGlobalThemeCheckbox.IsChecked ?? false;
            ParentWidget.SaveThemeSettings();
        }

        private void ColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                bool primaryFGColor = button.Name == nameof(primaryFGColorSelect);
                bool secondaryFGColor = button.Name == nameof(secondaryFGColorSelect);
                bool primaryBGColor = button.Name == nameof(primaryBGColorSelect);
                bool secondaryBGColor = button.Name == nameof(secondaryBGColorSelect);

                System.Drawing.Color defaultColor = ColorTranslator.FromHtml(button.Content.ToString());
                System.Drawing.Color selectedColor = ParentWidget.WidgetObject.WidgetManager.RequestColorSelection(defaultColor);
                button.Content = ColorTranslator.ToHtml(selectedColor);

                button.Background = new SolidColorBrush(
                    CleverWidgetBase.ConvertDrawingColor(selectedColor));
                button.Foreground = new SolidColorBrush(
                    CleverWidgetBase.GetTextShadeForBgColor(button.Background));

                if (primaryFGColor)
                    ParentWidget.UserForeColorPrimary = selectedColor;
                else if (secondaryFGColor)
                    ParentWidget.UserForeColorSecondary = selectedColor;
                else if (primaryBGColor)
                    ParentWidget.UserBackColorPrimary = selectedColor;
                else if (secondaryBGColor)
                    ParentWidget.UserBackColorSecondary = selectedColor;

                ParentWidget.SaveThemeSettings();
            }
        }

        private void FontSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                bool primaryFont = button.Name == nameof(primaryFontSelect);
                bool secondaryFont = button.Name == nameof(secondaryFontSelect);

                Font selectedFont =
                    ParentWidget.WidgetObject.WidgetManager.RequestFontSelection(
                        primaryFont ? ParentWidget.UserFontPrimary : ParentWidget.UserFontSecondary);

                button.Content = new FontConverter().ConvertToInvariantString(selectedFont);
                button.Tag = selectedFont;

                if (primaryFont)
                    ParentWidget.UserFontPrimary = primaryFontSelect.Tag as Font;
                else if (secondaryFont)
                    ParentWidget.UserFontSecondary = secondaryFontSelect.Tag as Font;

                ParentWidget.SaveThemeSettings();
            }
        }

        private void cornerRadiusUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is Xceed.Wpf.Toolkit.IntegerUpDown upDown && upDown.Value.HasValue)
            {
                ParentWidget.UserCornerRadius = upDown.Value.Value;
                ParentWidget.SaveThemeSettings();
            }
        }

    }
}
