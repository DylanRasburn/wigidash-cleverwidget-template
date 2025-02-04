using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace CleverWidget
{
    /// <summary>
    /// Interaction logic for WidgetImageControls.xaml
    /// </summary>
    /// 
    public partial class WidgetImageControls : UserControl
    {
        public CleverWidgetBase ParentWidget;

        public WidgetImageControls(MyWidget parent)
        {
            InitializeComponent();
            ParentWidget = parent;
            DataContext = this;

            imagePrimaryXOffsetUpDown.Minimum = -ParentWidget.WidgetSize.ToSize().Width / 2;
            imagePrimaryXOffsetUpDown.Maximum = ParentWidget.WidgetSize.ToSize().Width / 2;

            imagePrimaryYOffsetUpDown.Minimum = -ParentWidget.WidgetSize.ToSize().Height / 2;
            imagePrimaryYOffsetUpDown.Maximum = ParentWidget.WidgetSize.ToSize().Height / 2;

            bool haveSavedImagePath = ParentWidget.OverlayImagePrimaryFilepath != string.Empty;
            if (haveSavedImagePath) // overwrites 'No image loaded...' default text
                imagePrimaryFilepathTextBox.Text = ParentWidget.OverlayImagePrimaryFilepath;

            bool haveImage = ParentWidget.OverlayImagePrimary != null;

            if (!haveImage && haveSavedImagePath) // error file not found
                imagePrimaryFilepathTextBox.Foreground = new SolidColorBrush(Colors.Maroon);

            EnableImageOptions(haveImage, false);

            if (!ParentWidget.Toggleable)
            {
                return;
            }

            // expand & show toggled image UI
            toggledImageOptions.Visibility = Visibility.Visible;

            imageSecondaryXOffsetUpDown.Minimum = -ParentWidget.WidgetSize.ToSize().Width / 2;
            imageSecondaryXOffsetUpDown.Maximum = ParentWidget.WidgetSize.ToSize().Width / 2;

            imageSecondaryYOffsetUpDown.Minimum = -ParentWidget.WidgetSize.ToSize().Height / 2;
            imageSecondaryYOffsetUpDown.Maximum = ParentWidget.WidgetSize.ToSize().Height / 2;

            haveSavedImagePath = ParentWidget.OverlayImageSecondaryFilepath != string.Empty;
            if (haveSavedImagePath) // overwrites 'No image loaded...' default text
                imageSecondaryFilepathTextBox.Text = ParentWidget.OverlayImageSecondaryFilepath;

            haveImage = ParentWidget.OverlayImageSecondary != null;

            if (!haveImage && haveSavedImagePath) // error file not found
                imageSecondaryFilepathTextBox.Foreground = new SolidColorBrush(Colors.Maroon);

            EnableImageOptions(haveImage, true);
        }

        private void EnableImageOptions(bool enable, bool toggledImage)
        {
            // TODO: improve IsEnabled logic and Value binding, couldn't get it going properly...
            // this functions solves the issue
            if (!toggledImage)
            {
                imagePrimaryXOffsetUpDown.IsEnabled = enable;
                imagePrimaryYOffsetUpDown.IsEnabled = enable;
                imagePrimaryZoomUpDown.IsEnabled = enable;
                imagePrimaryRotationUpDown.IsEnabled = enable;

                // remove the error red
                if (enable || ParentWidget.OverlayImagePrimaryFilepath == string.Empty)
                    imagePrimaryFilepathTextBox.ClearValue(TextBox.ForegroundProperty);

                // refresh values - this is called after ClearImage & LoadImage
                imagePrimaryXOffsetUpDown.Value = ParentWidget.OverlayImagePrimaryOffset.X;
                imagePrimaryYOffsetUpDown.Value = ParentWidget.OverlayImagePrimaryOffset.Y;
                imagePrimaryZoomUpDown.Value = ParentWidget.OverlayImagePrimaryZoom;
                imagePrimaryRotationUpDown.Value = ParentWidget.OverlayImagePrimaryRotationDegrees;
            }
            else
            {
                imageSecondaryXOffsetUpDown.IsEnabled = enable;
                imageSecondaryYOffsetUpDown.IsEnabled = enable;
                imageSecondaryZoomUpDown.IsEnabled = enable;
                imageSecondaryRotationUpDown.IsEnabled = enable;

                // remove the error red
                if (enable || ParentWidget.OverlayImageSecondaryFilepath == string.Empty)
                    imageSecondaryFilepathTextBox.ClearValue(TextBox.ForegroundProperty);

                // refresh values
                imageSecondaryXOffsetUpDown.Value = ParentWidget.OverlayImageSecondaryOffset.X;
                imageSecondaryYOffsetUpDown.Value = ParentWidget.OverlayImageSecondaryOffset.Y;
                imageSecondaryZoomUpDown.Value = ParentWidget.OverlayImageSecondaryZoom;
                imageSecondaryRotationUpDown.Value = ParentWidget.OverlayImageSecondaryRotationDegrees;
            }
        }

        private void imageFilePath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool showBrowseDialog = false;

            if (sender is FrameworkElement control)
            {
                showBrowseDialog |= ParentWidget.OverlayImagePrimaryFilepath == string.Empty
                    && control.Name == nameof(imagePrimaryFilepathTextBox);
                showBrowseDialog |= ParentWidget.OverlayImageSecondaryFilepath == string.Empty
                    && control.Name == nameof(imageSecondaryFilepathTextBox);
            }

            if (!showBrowseDialog)
                return;

            RoutedEventArgs dummyArgs = new RoutedEventArgs();
            imageBrowse_Click(sender, dummyArgs);
        }

        private void imageBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.gif;*.png)|*.bmp;*.jpg;*.jpeg;*.gif;*.png";

            if (openFileDialog.ShowDialog() == true && sender is FrameworkElement control)
            {
                bool toggledImage = !(control.Name == nameof(imagePrimaryBrowseButton)
                                    || control.Name == nameof(imagePrimaryFilepathTextBox));

                if (ParentWidget.LoadOverlayImage(openFileDialog.FileName, toggledImage))
                {
                    if (!toggledImage)
                    {
                        imagePrimaryFilepathTextBox.Text = ParentWidget.OverlayImagePrimaryFilepath;
                    }
                    else
                    {
                        imageSecondaryFilepathTextBox.Text = ParentWidget.OverlayImageSecondaryFilepath;
                    }

                    ParentWidget.ResetOverlayImageOptions(toggledImage);
                    EnableImageOptions(true, toggledImage); // refreshes UI values
                }
            }
        }

        private void clearImage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement control)
            {
                bool toggledImage = control.Name != nameof(imagePrimaryClearButton);

                ParentWidget.ClearOverlayImage(toggledImage);

                if (!toggledImage)
                {
                    imagePrimaryFilepathTextBox.Text = "No image loaded...";
                }
                else
                {
                    imageSecondaryFilepathTextBox.Text = "No image loaded...";
                }

                EnableImageOptions(false, toggledImage);
            }
        }


        private void imageXOffSet_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is Xceed.Wpf.Toolkit.IntegerUpDown upDown && upDown.Value.HasValue)
            {
                bool toggledImage = upDown.Name != nameof(imagePrimaryXOffsetUpDown);
                ParentWidget.SetOverlayImageOffset(upDown.Value.Value, true, toggledImage);
            }
        }

        private void imageYOffSet_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is Xceed.Wpf.Toolkit.IntegerUpDown upDown && upDown.Value.HasValue)
            {
                bool toggledImage = upDown.Name != nameof(imagePrimaryYOffsetUpDown);
                ParentWidget.SetOverlayImageOffset(upDown.Value.Value, false, toggledImage);
            }
        }

        private void imageZoom_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is Xceed.Wpf.Toolkit.DoubleUpDown upDown && upDown.Value.HasValue)
            {
                bool toggledImage = upDown.Name != nameof(imagePrimaryZoomUpDown);
                ParentWidget.SetOverlayImageZoom(upDown.Value.Value, toggledImage);
            }
        }

        private void imageRotation_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is Xceed.Wpf.Toolkit.IntegerUpDown upDown && upDown.Value.HasValue)
            {
                bool toggledImage = upDown.Name != nameof(imagePrimaryRotationUpDown);
                ParentWidget.SetOverlayImageRotation(upDown.Value.Value, toggledImage);
            }
        }

    }
}
