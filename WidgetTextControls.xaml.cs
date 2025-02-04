using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CleverWidget
{
    /// <summary>
    /// Interaction logic for WidgetTextControls.xaml
    /// </summary>
    public partial class WidgetTextControls : UserControl
    {
        protected CleverWidgetBase ParentWidget;

        public WidgetTextControls(MyWidget parent)
        {
            InitializeComponent();
            ParentWidget = parent;
            DataContext = this;

            textPrimary.Text = ParentWidget.OverlayTextPrimary;

            textPrimaryFontSelect.Tag = ParentWidget.OverlayTextPrimaryFont;
            textPrimaryFontSelect.Content =
                new FontConverter().ConvertToInvariantString(ParentWidget.OverlayTextPrimaryFont);

            textPrimaryColorSelect.Content = ColorTranslator.ToHtml(ParentWidget.OverlayTextPrimaryFontColor);
            textPrimaryColorSelect.Background = new SolidColorBrush(
                CleverWidgetBase.ConvertDrawingColor(ParentWidget.OverlayTextPrimaryFontColor));
            textPrimaryColorSelect.Foreground = new SolidColorBrush(
                CleverWidgetBase.GetTextShadeForBgColor(textPrimaryColorSelect.Background));

            textPrimaryAlignmentCombo.ItemsSource = new List<string> { "Left", "Centered", "Right" }; // increment values
            textPrimaryAlignmentCombo.SelectedIndex = (int)ParentWidget.OverlayTextPrimaryAlignment;

            textPrimaryXOffsetUpDown.Value = ParentWidget.OverlayTextPrimaryOffset.X;
            textPrimaryYOffsetUpDown.Value = ParentWidget.OverlayTextPrimaryOffset.Y;

            textPrimaryXOffsetUpDown.Minimum = -ParentWidget.WidgetSize.ToSize().Width / 2;
            textPrimaryXOffsetUpDown.Maximum = ParentWidget.WidgetSize.ToSize().Width / 2;

            textPrimaryYOffsetUpDown.Minimum = -ParentWidget.WidgetSize.ToSize().Height / 2;
            textPrimaryYOffsetUpDown.Maximum = ParentWidget.WidgetSize.ToSize().Height / 2;

            strokePrimaryColorSelect.Content = ColorTranslator.ToHtml(ParentWidget.OverlayTextPrimaryStrokeColor);
            strokePrimaryColorSelect.Background = new SolidColorBrush(
                CleverWidgetBase.ConvertDrawingColor(ParentWidget.OverlayTextPrimaryStrokeColor));
            strokePrimaryColorSelect.Foreground = new SolidColorBrush(
                CleverWidgetBase.GetTextShadeForBgColor(strokePrimaryColorSelect.Background));

            strokePrimaryWidthUpDown.Value = (int)ParentWidget.OverlayTextPrimaryStrokeWidth;

            if (!ParentWidget.Toggleable)
            {
                return;
            }

            // expand & show toggled text UI
            toggledTextOptions.Visibility = Visibility.Visible;

            textSecondary.Text = ParentWidget.OverlayTextSecondary;

            textSecondaryFontSelect.Tag = ParentWidget.OverlayTextSecondaryFont;
            textSecondaryFontSelect.Content =
                new FontConverter().ConvertToInvariantString(ParentWidget.OverlayTextSecondaryFont);

            textSecondaryColorSelect.Content = ColorTranslator.ToHtml(ParentWidget.OverlayTextSecondaryFontColor);
            textSecondaryColorSelect.Background = new SolidColorBrush(
                CleverWidgetBase.ConvertDrawingColor(ParentWidget.OverlayTextSecondaryFontColor));
            textSecondaryColorSelect.Foreground = new SolidColorBrush(
                CleverWidgetBase.GetTextShadeForBgColor(textSecondaryColorSelect.Background));

            textSecondaryAlignmentCombo.ItemsSource = new List<string> { "Left", "Centered", "Right" }; // increment values
            textSecondaryAlignmentCombo.SelectedIndex = (int)ParentWidget.OverlayTextSecondaryAlignment;

            textSecondaryXOffsetUpDown.Value = ParentWidget.OverlayTextSecondaryOffset.X;
            textSecondaryYOffsetUpDown.Value = ParentWidget.OverlayTextSecondaryOffset.Y;

            textSecondaryXOffsetUpDown.Minimum = -ParentWidget.WidgetSize.ToSize().Width / 2;
            textSecondaryXOffsetUpDown.Maximum = ParentWidget.WidgetSize.ToSize().Width / 2;

            textSecondaryYOffsetUpDown.Minimum = -ParentWidget.WidgetSize.ToSize().Height / 2;
            textSecondaryYOffsetUpDown.Maximum = ParentWidget.WidgetSize.ToSize().Height / 2;

            strokeSecondaryColorSelect.Content = ColorTranslator.ToHtml(ParentWidget.OverlayTextSecondaryStrokeColor);
            strokeSecondaryColorSelect.Background = new SolidColorBrush(
                CleverWidgetBase.ConvertDrawingColor(ParentWidget.OverlayTextSecondaryStrokeColor));
            strokeSecondaryColorSelect.Foreground = new SolidColorBrush(
                CleverWidgetBase.GetTextShadeForBgColor(strokeSecondaryColorSelect.Background));

            strokeSecondaryWidthUpDown.Value = (int)ParentWidget.OverlayTextSecondaryStrokeWidth;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                bool toggledText = textBox.Name != nameof(textPrimary);
                ParentWidget.SetOverlayText(textBox.Text, toggledText);
            }
        }

        private void textFontSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                bool toggledText = button.Name != nameof(textPrimaryFontSelect);

                Font selectedFont =
                    ParentWidget.WidgetObject.WidgetManager.RequestFontSelection(
                        !toggledText ? ParentWidget.OverlayTextPrimaryFont : ParentWidget.OverlayTextSecondaryFont);

                button.Content = new FontConverter().ConvertToInvariantString(selectedFont);
                button.Tag = selectedFont;

                ParentWidget.SetOverlayTextFont(selectedFont, toggledText);
            }
        }

        private void textColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                bool toggledText = button.Name != nameof(textPrimaryColorSelect);

                System.Drawing.Color defaultColor = ColorTranslator.FromHtml(button.Content.ToString());
                System.Drawing.Color selectedColor = ParentWidget.WidgetObject.WidgetManager.RequestColorSelection(defaultColor);
                button.Content = ColorTranslator.ToHtml(selectedColor);
                button.Background = new SolidColorBrush(CleverWidgetBase.ConvertDrawingColor(selectedColor));
                button.Foreground = new SolidColorBrush(
                    CleverWidgetBase.GetTextShadeForBgColor(button.Background));

                ParentWidget.SetOverlayTextFontColor(selectedColor, toggledText);
            }
        }

        private void textAlignment_OnValueChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && sender is ComboBox combo)
            {
                bool toggledText = combo.Name != nameof(textPrimaryAlignmentCombo);
                ParentWidget.SetOverlayTextAlignment((StringAlignment)combo.SelectedIndex, toggledText);
            }
        }

        private void textXOffSet_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is Xceed.Wpf.Toolkit.IntegerUpDown upDown && upDown.Value.HasValue)
            {
                bool toggledText = upDown.Name != nameof(textPrimaryXOffsetUpDown);
                ParentWidget.SetOverlayTextOffset(upDown.Value.Value, true, toggledText);
            }
        }

        private void textYOffSet_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is Xceed.Wpf.Toolkit.IntegerUpDown upDown && upDown.Value.HasValue)
            {
                bool toggledText = upDown.Name != nameof(textPrimaryYOffsetUpDown);
                ParentWidget.SetOverlayTextOffset(upDown.Value.Value, false, toggledText);
            }
        }

        private void strokeColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                bool toggledText = button.Name != nameof(strokePrimaryColorSelect);

                System.Drawing.Color defaultColor = ColorTranslator.FromHtml(button.Content.ToString());
                System.Drawing.Color selectedColor = ParentWidget.WidgetObject.WidgetManager.RequestColorSelection(defaultColor);
                button.Content = ColorTranslator.ToHtml(selectedColor);
                button.Background = new SolidColorBrush(CleverWidgetBase.ConvertDrawingColor(selectedColor));
                button.Foreground = new SolidColorBrush(
                    CleverWidgetBase.GetTextShadeForBgColor(button.Background));

                ParentWidget.SetOverlayTextStrokeColor(selectedColor, toggledText);
            }
        }

        private void strokeWidth_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is Xceed.Wpf.Toolkit.IntegerUpDown upDown && upDown.Value.HasValue)
            {
                bool toggledText = upDown.Name != nameof(strokePrimaryWidthUpDown);
                ParentWidget.SetOverlayTextStrokeWidth(upDown.Value.Value, toggledText);
            }
        }

    }
}
