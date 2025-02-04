using WigiDashWidgetFramework;
using WigiDashWidgetFramework.WidgetUtility;

using System;
using System.Drawing;
using System.Windows.Controls;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Drawing.Drawing2D;
using System.Diagnostics;

// TODO: investigate adding hotkey and action base class support

namespace CleverWidget
{
    public abstract class CleverWidgetBase : IWidgetInstance
    {
        // IWidgetInstance
        public IWidgetObject WidgetObject
        {
            get
            {
                return parent;
            }
            private set
            {
                parent = (CleverWidgetFactory)value;
            }
        }
        public Guid Guid { get; private set; }
        public WidgetSize WidgetSize { get; private set; }

        public event WidgetUpdatedEventHandler WidgetUpdated;

        // this is called from WigiDash HQ & we also call it from our inherited class
        public void RequestUpdate()
        {
            WidgetUpdatedEventArgs e = new WidgetUpdatedEventArgs();
            e.WidgetBitmap = WigdetBitmap; // gets a thread safe copy
            e.WaitMax = WaitMax;
            WidgetUpdated?.Invoke(this, e);
        }

        public virtual void ClickEvent(ClickType click_type, int x, int y)
        {
            throw new NotImplementedException("ClickEvent must be implemented by a derived class");
        }

        public virtual UserControl GetSettingsControl()
        {
            throw new NotImplementedException("GetSettingsControl must be implemented by a derived class");
        }

        public virtual void EnterSleep()
        {
            throw new NotImplementedException("EnterSleep must be implemented by a derived class");
        }

        public virtual void ExitSleep()
        {
            throw new NotImplementedException("ExitSleep must be implemented by a derived class");
        }

        // IWidgetInstance : IDisposable 
        public virtual void Dispose()
        {
        }

        // set this if you want alternate behavior based on your custom boolean widget condition
        public readonly bool Toggleable = true;

        protected CleverWidgetFactory parent;

        protected volatile bool Toggled = false;

        // for WidgetImageControls.xaml
        protected readonly object _imageOverlayPrimaryBitmapLock = new object();
        protected Bitmap _imageOverlayPrimaryBitmap;
        public Bitmap OverlayImagePrimary
        {
            get
            {
                lock (_imageOverlayPrimaryBitmapLock)
                {
                    if (_imageOverlayPrimaryBitmap == null)
                        return null;

                    // TODO: rarely running into object in use exception when clicking UI 
                    return _imageOverlayPrimaryBitmap.Clone(
                        new Rectangle(0, 0, _imageOverlayPrimaryBitmap.Width, _imageOverlayPrimaryBitmap.Height),
                        _imageOverlayPrimaryBitmap.PixelFormat
                    );
                }
            }

            private set
            {
                lock (_imageOverlayPrimaryBitmapLock)
                {
                    if (value == null)
                    {
                        _imageOverlayPrimaryBitmap = null;
                        return;
                    }

                    if (_imageOverlayPrimaryBitmap != null)
                    {
                        ImageAnimator.StopAnimate(_imageOverlayPrimaryBitmap, OnFrameChanged); // harmless if not animating
                        _imageOverlayPrimaryBitmap.Dispose();
                    }

                    _imageOverlayPrimaryBitmap = value.Clone(
                        new Rectangle(0, 0, value.Width, value.Height),
                        value.PixelFormat
                    );
                }
            }
        }

        public string OverlayImagePrimaryFilepath = string.Empty;
        public Point OverlayImagePrimaryOffset = new Point(0, 0);
        public int OverlayImagePrimaryRotationDegrees = 0;
        public double OverlayImagePrimaryZoom = 1.0;

        protected readonly object _imageOverlaySecondaryBitmapLock = new object();
        protected Bitmap _imageOverlaySecondaryBitmap;
        public Bitmap OverlayImageSecondary
        {
            get
            {
                lock (_imageOverlaySecondaryBitmapLock)
                {
                    if (_imageOverlaySecondaryBitmap == null)
                        return null;

                    return _imageOverlaySecondaryBitmap.Clone(
                        new Rectangle(0, 0, _imageOverlaySecondaryBitmap.Width, _imageOverlaySecondaryBitmap.Height),
                        _imageOverlaySecondaryBitmap.PixelFormat
                    );
                }
            }

            private set
            {
                lock (_imageOverlaySecondaryBitmapLock)
                {
                    if (value == null)
                    {
                        _imageOverlaySecondaryBitmap = null;
                        return;
                    }

                    if (_imageOverlaySecondaryBitmap != null)
                    {
                        ImageAnimator.StopAnimate(_imageOverlaySecondaryBitmap, OnFrameChanged); // harmless if not animating
                        _imageOverlaySecondaryBitmap.Dispose();
                    }

                    _imageOverlaySecondaryBitmap = value.Clone(
                        new Rectangle(0, 0, value.Width, value.Height),
                        value.PixelFormat
                    );
                }
            }
        }

        public string OverlayImageSecondaryFilepath = string.Empty;
        public Point OverlayImageSecondaryOffset = new Point(0, 0);
        public int OverlayImageSecondaryRotationDegrees = 0;
        public double OverlayImageSecondaryZoom = 1.0;

        // for WidgetTextControls.xaml
        public string OverlayTextPrimary = string.Empty;
        //public int OverlayTextPrimaryFontSize = 32;
        public Font OverlayTextPrimaryFont = new Font("Basic Square 7", 32);
        public System.Drawing.Color OverlayTextPrimaryFontColor = System.Drawing.Color.GreenYellow;
        public StringAlignment OverlayTextPrimaryAlignment = StringAlignment.Center;
        public Point OverlayTextPrimaryOffset = new Point(0, 0);
        public System.Drawing.Color OverlayTextPrimaryStrokeColor = System.Drawing.Color.White;
        public float OverlayTextPrimaryStrokeWidth = 2.0f;

        public string OverlayTextSecondary = string.Empty;
        //public int OverlayTextSecondaryFontSize = 26;
        public Font OverlayTextSecondaryFont = new Font("Basic Square 7 Solid", 26);
        public System.Drawing.Color OverlayTextSecondaryFontColor = System.Drawing.Color.Crimson;
        public StringAlignment OverlayTextSecondaryAlignment = StringAlignment.Center;
        public Point OverlayTextSecondaryOffset = new Point(0, 0);
        public System.Drawing.Color OverlayTextSecondaryStrokeColor = System.Drawing.Color.White;
        public float OverlayTextSecondaryStrokeWidth = 2.0f;

        // for WidgetThemeControls.xaml
        public volatile bool UseGlobalTheme = true; //  TODO: bool & checkbox for each global setting
        public System.Drawing.Color UserForeColorPrimary = System.Drawing.Color.White;
        public System.Drawing.Color UserForeColorSecondary = System.Drawing.Color.Red;
        public System.Drawing.Color UserBackColorPrimary = System.Drawing.Color.FromArgb(48, 48, 48);
        public System.Drawing.Color UserBackColorSecondary = System.Drawing.Color.Gray;
        public Font UserFontPrimary = new Font("Basic Square 7", 32);
        public Font UserFontSecondary = new Font("Basic Square 7 Solid", 26);
        public int UserCornerRadius = 15;

        protected System.Drawing.Color GlobalForeColorPrimary = System.Drawing.Color.White;
        protected System.Drawing.Color GlobalForeColorSecondary = System.Drawing.Color.Red;
        protected System.Drawing.Color GlobalBackColorPrimary = System.Drawing.Color.FromArgb(48, 48, 48);
        protected System.Drawing.Color GlobalBackColorSecondary = System.Drawing.Color.Gray;
        protected Font GlobalFontPrimary = new Font("Basic Square 7", 32);
        protected Font GlobalFontSecondary = new Font("Basic Square 7 Solid", 26);
        protected int GlobalCornerRadius = 15;

        protected readonly int WaitMax = 40; // ms, >= 24 fps
        protected readonly object _widgetBitmapLock = new object();
        protected Bitmap _widgetBitmap;
        protected Bitmap WigdetBitmap
        {
            get
            {
                lock (_widgetBitmapLock)
                {
                    if (_widgetBitmap == null)
                        return null;

                    return _widgetBitmap.Clone(
                        new Rectangle(0, 0, _widgetBitmap.Width, _widgetBitmap.Height),
                        _widgetBitmap.PixelFormat
                    );
                }
            }

            set
            {
                lock (_widgetBitmapLock)
                {
                    if (value == null)
                        return;

                    if (_widgetBitmap != null)
                        _widgetBitmap.Dispose();

                    _widgetBitmap = value.Clone(
                        new Rectangle(0, 0, value.Width, value.Height),
                        value.PixelFormat
                    );
                }
            }
        }

        public CleverWidgetBase(CleverWidgetFactory parent, WidgetSize widget_size, Guid instance_guid)
        {
            WidgetObject = parent;
            WidgetSize = widget_size;
            Guid = instance_guid;

            WigdetBitmap = new Bitmap(widget_size.ToSize().Width, widget_size.ToSize().Height);

            try
            {
                LoadOverlayImageSettings();
            }
            catch (Exception ex)
            {

            }
            try
            {
                LoadOverlayTextSettings();
            }
            catch (Exception ex)
            {

            }

            try
            {
                LoadThemeSettings();
            }
            catch (Exception ex)
            {

            }

            WidgetObject.WidgetManager.GlobalThemeUpdated += WidgetManager_GlobalThemeUpdated;
        }

        private void LoadOverlayImageSettings()
        {
            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayImagePrimaryFilepath), out string overlayImagePrimaryFilepath))
                LoadOverlayImage(overlayImagePrimaryFilepath, false);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayImagePrimaryOffset), out string primaryOffsetStr))
            {
                var parts = primaryOffsetStr.Split(',');
                if (parts.Length == 2)
                {
                    OverlayImagePrimaryOffset.X = int.Parse(parts[0]);
                    OverlayImagePrimaryOffset.Y = int.Parse(parts[1]);
                }
            }

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayImagePrimaryRotationDegrees), out string primaryRotationStr))
                OverlayImagePrimaryRotationDegrees = int.Parse(primaryRotationStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayImagePrimaryZoom), out string primaryZoomStr))
                OverlayImagePrimaryZoom = double.Parse(primaryZoomStr);

            if (!Toggleable)
                return;

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayImageSecondaryFilepath), out string overlayImageSecondaryFilepath))
                LoadOverlayImage(overlayImageSecondaryFilepath, true);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayImageSecondaryOffset), out string secondaryOffsetStr))
            {
                var parts = secondaryOffsetStr.Split(',');
                if (parts.Length == 2)
                {
                    OverlayImageSecondaryOffset.X = int.Parse(parts[0]);
                    OverlayImageSecondaryOffset.Y = int.Parse(parts[1]);
                }
            }

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayImageSecondaryRotationDegrees), out string secondaryRotationStr))
                OverlayImageSecondaryRotationDegrees = int.Parse(secondaryRotationStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayImageSecondaryZoom), out string secondaryZoomStr))
                OverlayImageSecondaryZoom = double.Parse(secondaryZoomStr);
        }

        private void LoadOverlayTextSettings()
        {
            // initialize uninitialized members
            Random random = new Random();
            Array colors = Enum.GetValues(typeof(KnownColor));

            OverlayTextPrimaryFont = new Font(
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont.FontFamily,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont.Size,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont.Style);
            OverlayTextSecondaryFont = new Font(
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont.FontFamily,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont.Size,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont.Style);

            OverlayTextPrimaryFontColor = WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFgColor;
            OverlayTextSecondaryFontColor = WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFgColor;

            OverlayTextPrimaryStrokeColor = System.Drawing.Color.White;
            OverlayTextSecondaryStrokeColor = System.Drawing.Color.White;

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextPrimary), out string overlayTextPrimaryStr))
                OverlayTextPrimary = overlayTextPrimaryStr;

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextPrimaryFont), out var fontPrimaryStr))
                OverlayTextPrimaryFont = new FontConverter().ConvertFromInvariantString(fontPrimaryStr) as Font;

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextPrimaryFontColor), out string fontPrimaryColorStr))
                OverlayTextPrimaryFontColor = ColorTranslator.FromHtml(fontPrimaryColorStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextPrimaryAlignment), out string textPrimaryAlignment))
                OverlayTextPrimaryAlignment = (StringAlignment)int.Parse(textPrimaryAlignment);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextPrimaryOffset), out string primaryOffsetStr))
            {
                var parts = primaryOffsetStr.Split(',');
                if (parts.Length == 2)
                {
                    OverlayTextPrimaryOffset.X = int.Parse(parts[0]);
                    OverlayTextPrimaryOffset.Y = int.Parse(parts[1]);
                }
            }

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextPrimaryStrokeColor), out string strokePrimaryColorStr))
                OverlayTextPrimaryStrokeColor = ColorTranslator.FromHtml(strokePrimaryColorStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextPrimaryStrokeWidth), out string strokePrimaryWidthStr))
                OverlayTextPrimaryStrokeWidth = int.Parse(strokePrimaryWidthStr);


            if (!Toggleable)
                return;


            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextSecondary), out string overlayTextSecondaryStr))
                OverlayTextSecondary = overlayTextSecondaryStr;

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextSecondaryFont), out var fontSecondaryStr))
                OverlayTextSecondaryFont = new FontConverter().ConvertFromInvariantString(fontSecondaryStr) as Font;

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextSecondaryFontColor), out string fontSecondaryColorStr))
                OverlayTextSecondaryFontColor = ColorTranslator.FromHtml(fontSecondaryColorStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextSecondaryAlignment), out string textSecondaryAlignment))
                OverlayTextSecondaryAlignment = (StringAlignment)int.Parse(textSecondaryAlignment);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextSecondaryOffset), out string secondaryOffsetStr))
            {
                var parts = secondaryOffsetStr.Split(',');
                if (parts.Length == 2)
                {
                    OverlayTextSecondaryOffset.X = int.Parse(parts[0]);
                    OverlayTextSecondaryOffset.Y = int.Parse(parts[1]);
                }
            }

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextSecondaryStrokeColor), out string strokeSecondaryColorStr))
                OverlayTextSecondaryStrokeColor = ColorTranslator.FromHtml(strokeSecondaryColorStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(OverlayTextSecondaryStrokeWidth), out string strokeSecondaryWidthStr))
                OverlayTextSecondaryStrokeWidth = int.Parse(strokeSecondaryWidthStr);
        }

        private void LoadThemeSettings()
        {
            UseGlobalTheme = WidgetObject.WidgetManager.PreferGlobalTheme;

            // default to global theme
            UserForeColorPrimary = WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFgColor;
            UserForeColorSecondary = WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFgColor;
            UserBackColorPrimary = WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryBgColor;
            UserBackColorSecondary = WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryBgColor;
            UserFontPrimary = new Font(
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont.FontFamily,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont.Size,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont.Style);
            UserFontSecondary = new Font(
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont.FontFamily,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont.Size,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont.Style);

            UserCornerRadius = WidgetObject.WidgetManager.GlobalWidgetTheme.CornerRadius;

            GlobalForeColorPrimary = UserForeColorPrimary;
            GlobalForeColorSecondary = UserForeColorSecondary;
            GlobalBackColorPrimary = UserBackColorPrimary;
            GlobalBackColorSecondary = UserBackColorSecondary;
            GlobalFontPrimary = UserFontPrimary;
            GlobalFontSecondary = UserFontSecondary;
            GlobalCornerRadius = UserCornerRadius;

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(UseGlobalTheme), out string useGlobalStr))
                UseGlobalTheme = bool.Parse(useGlobalStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(UserForeColorPrimary), out string forePrimStr))
                UserForeColorPrimary = ColorTranslator.FromHtml(forePrimStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(UserForeColorSecondary), out string foreSecStr))
                UserForeColorSecondary = ColorTranslator.FromHtml(foreSecStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(UserBackColorPrimary), out string bgPrimStr))
                UserBackColorPrimary = ColorTranslator.FromHtml(bgPrimStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(UserBackColorSecondary), out string bgSecStr))
                UserBackColorSecondary = ColorTranslator.FromHtml(bgSecStr);

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(UserFontPrimary), out var fontPrimStr))
                UserFontPrimary = new FontConverter().ConvertFromInvariantString(fontPrimStr) as Font;

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(UserFontSecondary), out var fontSecStr))
                UserFontSecondary = new FontConverter().ConvertFromInvariantString(fontSecStr) as Font;

            if (WidgetObject.WidgetManager.LoadSetting(this, nameof(UserCornerRadius), out var cnrRadiusStr))
                UserCornerRadius = int.Parse(cnrRadiusStr);
        }

        public void SaveThemeSettings()
        {
            // TODO: this doesn't seem effecient

            WidgetObject.WidgetManager.StoreSetting(this,
                nameof(UseGlobalTheme), UseGlobalTheme.ToString());
            WidgetObject.WidgetManager.StoreSetting(this,
                nameof(UserForeColorPrimary), ColorTranslator.ToHtml(UserForeColorPrimary));
            WidgetObject.WidgetManager.StoreSetting(this,
                nameof(UserBackColorPrimary), ColorTranslator.ToHtml(UserBackColorPrimary));
            WidgetObject.WidgetManager.StoreSetting(this,
                nameof(UserForeColorSecondary), ColorTranslator.ToHtml(UserForeColorSecondary));
            WidgetObject.WidgetManager.StoreSetting(this,
                nameof(UserBackColorSecondary), ColorTranslator.ToHtml(UserBackColorSecondary));
            WidgetObject.WidgetManager.StoreSetting(this,
                nameof(UserFontPrimary), new FontConverter().ConvertToInvariantString(UserFontPrimary));
            WidgetObject.WidgetManager.StoreSetting(this,
                nameof(UserFontSecondary), new FontConverter().ConvertToInvariantString(UserFontSecondary));
            WidgetObject.WidgetManager.StoreSetting(this,
                nameof(UserCornerRadius), UserCornerRadius.ToString());
        }

        private void WidgetManager_GlobalThemeUpdated()
        {
            GlobalForeColorPrimary = WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFgColor;
            GlobalForeColorSecondary = WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFgColor;
            GlobalBackColorPrimary = WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryBgColor;
            GlobalBackColorSecondary = WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryBgColor;
            GlobalFontPrimary = WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont;
            GlobalFontSecondary = WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont;
            GlobalCornerRadius = WidgetObject.WidgetManager.GlobalWidgetTheme.CornerRadius;
        }

        public bool LoadOverlayImage(string filePath, bool toggledImage)
        {
            if (filePath == string.Empty)
                return false;

            if (!toggledImage)
            {
                try
                {
                    // the order of these two is important for imforming user
                    // when the file isn't found at that saved path
                    OverlayImagePrimaryFilepath = filePath;
                    OverlayImagePrimary = new Bitmap(filePath);

                    if (OverlayImagePrimary.RawFormat.Equals(ImageFormat.Gif)
                                            && ImageAnimator.CanAnimate(OverlayImagePrimary))
                    {
                        lock (_imageOverlayPrimaryBitmapLock)
                        {
                            ImageAnimator.Animate(_imageOverlayPrimaryBitmap, OnFrameChanged);
                        }
                    }

                    WidgetObject.WidgetManager.StoreSetting(this,
                        nameof(OverlayImagePrimaryFilepath), OverlayImagePrimaryFilepath);

                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    OverlayImageSecondaryFilepath = filePath;
                    OverlayImageSecondary = new Bitmap(filePath);

                    if (OverlayImageSecondary.RawFormat.Equals(ImageFormat.Gif)
                                            && ImageAnimator.CanAnimate(OverlayImageSecondary))
                    {
                        lock (_imageOverlaySecondaryBitmapLock)
                        {
                            ImageAnimator.Animate(_imageOverlaySecondaryBitmap, OnFrameChanged);
                        }
                    }

                    WidgetObject.WidgetManager.StoreSetting(this,
                        nameof(OverlayImageSecondaryFilepath), OverlayImageSecondaryFilepath);

                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }
        public void ClearOverlayImage(bool toggledImage)
        {
            if (!toggledImage)
            {
                OverlayImagePrimary = null;
                OverlayImagePrimaryFilepath = string.Empty;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImagePrimaryFilepath), OverlayImagePrimaryFilepath);
            }
            else
            {
                OverlayImageSecondary = null;
                OverlayImageSecondaryFilepath = string.Empty;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImageSecondaryFilepath), OverlayImageSecondaryFilepath);
            }

            ResetOverlayImageOptions(toggledImage);
        }

        public void ResetOverlayImageOptions(bool toggledImage)
        {
            if (!toggledImage)
            {
                OverlayImagePrimaryOffset.X = 0;
                OverlayImagePrimaryOffset.Y = 0;
                OverlayImagePrimaryZoom = 1.0;
                OverlayImagePrimaryRotationDegrees = 0;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImagePrimaryOffset), $"{OverlayImagePrimaryOffset.X},{OverlayImagePrimaryOffset.Y}");
                WidgetObject.WidgetManager.StoreSetting(this, nameof(OverlayImagePrimaryZoom), OverlayImagePrimaryZoom.ToString());
                WidgetObject.WidgetManager.StoreSetting(this, nameof(OverlayImagePrimaryRotationDegrees), OverlayImagePrimaryRotationDegrees.ToString());
            }
            else
            {
                OverlayImageSecondaryOffset.X = 0;
                OverlayImageSecondaryOffset.Y = 0;
                OverlayImageSecondaryZoom = 1.0;
                OverlayImageSecondaryRotationDegrees = 0;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImageSecondaryOffset), $"{OverlayImageSecondaryOffset.X},{OverlayImageSecondaryOffset.Y}");
                WidgetObject.WidgetManager.StoreSetting(this, nameof(OverlayImageSecondaryZoom), OverlayImageSecondaryZoom.ToString());
                WidgetObject.WidgetManager.StoreSetting(this, nameof(OverlayImageSecondaryRotationDegrees), OverlayImageSecondaryRotationDegrees.ToString());
            }
        }

        public void SetOverlayImageOffset(int value, bool XCoordinate, bool toggledImage)
        {
            if (!toggledImage)
            {
                if (XCoordinate)
                    OverlayImagePrimaryOffset.X = value;
                else
                    OverlayImagePrimaryOffset.Y = value;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImagePrimaryOffset), $"{OverlayImagePrimaryOffset.X},{OverlayImagePrimaryOffset.Y}");
            }
            else
            {
                if (XCoordinate)
                    OverlayImageSecondaryOffset.X = value;
                else
                    OverlayImageSecondaryOffset.Y = value;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImageSecondaryOffset), $"{OverlayImageSecondaryOffset.X},{OverlayImageSecondaryOffset.Y}");
            }
        }

        public void SetOverlayImageZoom(double value, bool toggledImage)
        {
            if (!toggledImage)
            {
                OverlayImagePrimaryZoom = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImagePrimaryZoom), OverlayImagePrimaryZoom.ToString());
            }
            else
            {
                OverlayImageSecondaryZoom = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImageSecondaryZoom), OverlayImageSecondaryZoom.ToString());
            }
        }

        public void SetOverlayImageRotation(int value, bool toggledImage)
        {
            if (!toggledImage)
            {
                OverlayImagePrimaryRotationDegrees = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImagePrimaryRotationDegrees), OverlayImagePrimaryRotationDegrees.ToString());
            }
            else
            {
                OverlayImageSecondaryRotationDegrees = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayImageSecondaryRotationDegrees), OverlayImageSecondaryRotationDegrees.ToString());
            }
        }

        public void SetOverlayText(string value, bool toggledText)
        {
            if (!toggledText)
            {
                OverlayTextPrimary = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextPrimary), OverlayTextPrimary);
            }
            else
            {
                OverlayTextSecondary = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextSecondary), OverlayTextSecondary);
            }
        }

        public void SetOverlayTextFont(Font value, bool toggledText)
        {
            if (!toggledText)
            {
                OverlayTextPrimaryFont = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextPrimaryFont), new FontConverter().ConvertToInvariantString(OverlayTextPrimaryFont));
            }
            else
            {
                OverlayTextSecondaryFont = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextSecondaryFont), new FontConverter().ConvertToInvariantString(OverlayTextSecondaryFont));
            }
        }

        public void SetOverlayTextFontColor(System.Drawing.Color value, bool toggledText)
        {
            if (!toggledText)
            {
                OverlayTextPrimaryFontColor = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextPrimaryFontColor), ColorTranslator.ToHtml(OverlayTextPrimaryFontColor));
            }
            else
            {
                OverlayTextSecondaryFontColor = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextSecondaryFontColor), ColorTranslator.ToHtml(OverlayTextSecondaryFontColor));
            }
        }

        public void SetOverlayTextAlignment(StringAlignment value, bool toggledText)
        {
            if (!toggledText)
            {
                OverlayTextPrimaryAlignment = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextPrimaryAlignment), ((int)OverlayTextPrimaryAlignment).ToString());
            }
            else
            {
                OverlayTextSecondaryAlignment = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextSecondaryAlignment), ((int)OverlayTextSecondaryAlignment).ToString());
            }
        }

        public void SetOverlayTextOffset(int value, bool XCoordinate, bool toggledText)
        {
            if (!toggledText)
            {
                if (XCoordinate)
                    OverlayTextPrimaryOffset.X = value;
                else
                    OverlayTextPrimaryOffset.Y = value;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextPrimaryOffset), $"{OverlayTextPrimaryOffset.X},{OverlayTextPrimaryOffset.Y}");
            }
            else
            {
                if (XCoordinate)
                    OverlayTextSecondaryOffset.X = value;
                else
                    OverlayTextSecondaryOffset.Y = value;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextSecondaryOffset), $"{OverlayTextSecondaryOffset.X},{OverlayTextSecondaryOffset.Y}");
            }
        }

        public void SetOverlayTextStrokeColor(System.Drawing.Color value, bool toggledText)
        {
            if (!toggledText)
            {
                OverlayTextPrimaryStrokeColor = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextPrimaryStrokeColor), ColorTranslator.ToHtml(OverlayTextPrimaryStrokeColor));
            }
            else
            {
                OverlayTextSecondaryStrokeColor = value;
                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextSecondaryStrokeColor), ColorTranslator.ToHtml(OverlayTextSecondaryStrokeColor));
            }
        }


        public void SetOverlayTextStrokeWidth(int value, bool toggledText)
        {
            if (!toggledText)
            {
                OverlayTextPrimaryStrokeWidth = (float)value;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextPrimaryStrokeWidth), OverlayTextPrimaryStrokeWidth.ToString());
            }
            else
            {
                OverlayTextSecondaryStrokeWidth = (float)value;

                WidgetObject.WidgetManager.StoreSetting(this,
                    nameof(OverlayTextSecondaryStrokeWidth), OverlayTextSecondaryStrokeWidth.ToString());
            }
        }

        protected void OnFrameChanged(object sender, EventArgs e)
        {
            // do nothing, ImageAnimator.UpdateFrames() called in DrawOverlays
        }

        protected void DrawOverlays(Graphics graphics)
        {
            try
            {
                // even though we have these locks, this occasionally still
                // hits an object in use execption. TODO: figure out why - should be threadsafe.
                // no matter though, it gets updated next time around
                ImageAnimator.UpdateFrames();
                // this updates frames for child widget gifs aswell if implemented
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ImageAnimator.UpdateFrames() failed, no matter");
            }

            try
            {
                DrawImageOverlay(graphics);
            }
            catch (Exception ex)
            {
            }

            try
            {
                DrawTextOverlay(graphics);
            }
            catch (Exception ex)
            {
            }
        }

        private void DrawImageOverlay(Graphics graphics)
        {
            lock (_imageOverlayPrimaryBitmapLock) // faster than cloning
            {
                lock (_imageOverlaySecondaryBitmapLock)
                {
                    if (!Toggled && _imageOverlayPrimaryBitmap != null) // TODO: is this actually thread-safe ?
                    {
                        int zoomedOffsetWidth = (int)(_imageOverlayPrimaryBitmap.Width * OverlayImagePrimaryZoom);
                        int zoomedOffsetHeight = (int)(_imageOverlayPrimaryBitmap.Height * OverlayImagePrimaryZoom);

                        int xOffset = OverlayImagePrimaryOffset.X
                            + WidgetSize.ToSize().Width / 2 - zoomedOffsetWidth / 2;
                        int yOffset = OverlayImagePrimaryOffset.Y
                            + WidgetSize.ToSize().Height / 2 - zoomedOffsetHeight / 2;

                        GraphicsState graphicsState = graphics.Save();

                        // set rotation point
                        graphics.TranslateTransform(
                            xOffset + zoomedOffsetWidth / 2,
                            yOffset + zoomedOffsetHeight / 2);

                        graphics.RotateTransform(OverlayImagePrimaryRotationDegrees);

                        graphics.DrawImage(_imageOverlayPrimaryBitmap,
                            new Rectangle(-zoomedOffsetWidth / 2, -zoomedOffsetHeight / 2,  // TODO: member value RedrawImageDestRect
                                zoomedOffsetWidth, zoomedOffsetHeight),
                            new Rectangle(0, 0, _imageOverlayPrimaryBitmap.Width, _imageOverlayPrimaryBitmap.Height), // TODO: member value
                                GraphicsUnit.Pixel);

                        graphics.Restore(graphicsState);
                    }
                    else if (Toggled && _imageOverlaySecondaryBitmap != null)
                    {
                        int zoomedOffsetWidth = (int)(_imageOverlaySecondaryBitmap.Width * OverlayImageSecondaryZoom);
                        int zoomedOffsetHeight = (int)(_imageOverlaySecondaryBitmap.Height * OverlayImageSecondaryZoom);

                        int xOffset = OverlayImageSecondaryOffset.X
                            + WidgetSize.ToSize().Width / 2 - zoomedOffsetWidth / 2;
                        int yOffset = OverlayImageSecondaryOffset.Y
                            + WidgetSize.ToSize().Height / 2 - zoomedOffsetHeight / 2;

                        GraphicsState graphicsState = graphics.Save();

                        // set rotation point
                        graphics.TranslateTransform(
                            xOffset + zoomedOffsetWidth / 2,
                            yOffset + zoomedOffsetHeight / 2);

                        graphics.RotateTransform(OverlayImageSecondaryRotationDegrees);

                        graphics.DrawImage(_imageOverlaySecondaryBitmap,
                            new Rectangle(-zoomedOffsetWidth / 2, -zoomedOffsetHeight / 2, // TODO: member value RedrawImageDestRect
                                zoomedOffsetWidth, zoomedOffsetHeight),
                            new Rectangle(0, 0, _imageOverlaySecondaryBitmap.Width, _imageOverlaySecondaryBitmap.Height), // TODO: member value
                                GraphicsUnit.Pixel);

                        graphics.Restore(graphicsState);
                    }
                }
            }
        }

        private void DrawTextOverlay(Graphics graphics)
        {

            if (!Toggled)
            {
                var rect = new Rectangle(
                    OverlayTextPrimaryOffset.X, OverlayTextPrimaryOffset.Y,
                    WidgetSize.ToSize().Width, WidgetSize.ToSize().Height);

                var format = new StringFormat
                {
                    Alignment = OverlayTextPrimaryAlignment,
                    LineAlignment = StringAlignment.Center
                };

                if (OverlayTextPrimaryStrokeWidth > 0.0f)
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddString(OverlayTextPrimary,
                                       OverlayTextPrimaryFont.FontFamily,
                                       (int)OverlayTextPrimaryFont.Style,
                                       OverlayTextPrimaryFont.Size, rect, format);

                        System.Drawing.Pen pen = new System.Drawing.Pen(
                            OverlayTextPrimaryStrokeColor, OverlayTextPrimaryStrokeWidth); // TODO: member value RefreshOverlayPen
                        graphics.DrawPath(pen, path);

                        SolidBrush brush = new SolidBrush(OverlayTextPrimaryFontColor); // TODO: backing value
                        graphics.FillPath(brush, path);
                    }
                }
                else
                {
                    graphics.DrawString(OverlayTextPrimary, OverlayTextPrimaryFont,
                        new SolidBrush(OverlayTextPrimaryFontColor),  // TODO: backing value
                        rect, format);
                }
            }
            else
            {
                var rect = new Rectangle(
                    OverlayTextSecondaryOffset.X, OverlayTextSecondaryOffset.Y,
                    WidgetSize.ToSize().Width, WidgetSize.ToSize().Height);

                var format = new StringFormat
                {
                    Alignment = OverlayTextSecondaryAlignment,
                    LineAlignment = StringAlignment.Center
                };

                if (OverlayTextSecondaryStrokeWidth > 0.0f)
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddString(OverlayTextSecondary,
                                       OverlayTextSecondaryFont.FontFamily,
                                       (int)OverlayTextSecondaryFont.Style,
                                       OverlayTextSecondaryFont.Size, rect, format);

                        System.Drawing.Pen pen = new System.Drawing.Pen(
                            OverlayTextSecondaryStrokeColor, OverlayTextSecondaryStrokeWidth); // TODO: member value RefreshOverlayPen
                        graphics.DrawPath(pen, path);

                        SolidBrush brush = new SolidBrush(OverlayTextSecondaryFontColor); // TODO: backing value
                        graphics.FillPath(brush, path);
                    }
                }
                else
                {
                    graphics.DrawString(OverlayTextSecondary, OverlayTextSecondaryFont,
                        new SolidBrush(OverlayTextSecondaryFontColor), // TODO: backing value
                        rect, format);
                }
            }
        }

        public static System.Windows.Media.Color ConvertDrawingColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Windows.Media.Color GetTextShadeForBgColor(System.Windows.Media.Brush brush)
        {
            SolidColorBrush solidBrush = brush as SolidColorBrush;
            if (solidBrush == null)
            {
                throw new ArgumentException("the provided brush is not a SolidColorBrush");
            }

            int r = solidBrush.Color.R;
            int g = solidBrush.Color.G;
            int b = solidBrush.Color.B;

            // ITU - R BT.709 (human eye brightness co-effecients)
            double brightness = (0.2126 * r + 0.7152 * g + 0.0722 * b) / 255.0;

            if (brightness > 0.5)
                return Colors.Black;
            else
                return Colors.White;
        }
    }
}

