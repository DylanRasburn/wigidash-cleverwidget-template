using WigiDashWidgetFramework;
using WigiDashWidgetFramework.WidgetUtility;

using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


namespace CleverWidget
{
    public enum WidgetSizeEnum
    {
        _1x1 = 0, _2x1, _3x1, _4x1, _5x1,
        _1x2, _2x2, _3x2, _4x2, _5x2,
        _1x3, _2x3, _3x3, _4x3, _5x3,
        _1x4, _2x4, _3x4, _4x4, _5x4,
        kNumWidgetSizes
    }

    // TODO: is this supposed to be a singleton?
    public sealed class CleverWidgetFactory : IWidgetObject
    {
        // IWidgetBase
        public Guid Guid => new Guid(GetType().Assembly.GetName().Name);
        public string Name => Properties.Resources.CleverWidget_Name;
        public string Author => "Dilbo Baggins";
        public string Website => "https://github.com/DylanRasburn/wigidash-cleverwidget-template";
        public string Description => CleverWidget.Properties.Resources.CleverWidget_Description;
        public Version Version => new Version(1, 0, 0);
        public SdkVersion TargetSdk => WidgetUtility.CurrentSdkVersion;
        public List<WidgetSize> SupportedSizes =>
            new List<WidgetSize>() { 
                // comment out unsupported sizes
                WidgetSizes.GetByFirst(WidgetSizeEnum._1x1),
                WidgetSizes.GetByFirst(WidgetSizeEnum._2x1),
                WidgetSizes.GetByFirst(WidgetSizeEnum._3x1),
                WidgetSizes.GetByFirst(WidgetSizeEnum._4x1),
                WidgetSizes.GetByFirst(WidgetSizeEnum._5x1),
                WidgetSizes.GetByFirst(WidgetSizeEnum._1x2),
                WidgetSizes.GetByFirst(WidgetSizeEnum._2x2),
                WidgetSizes.GetByFirst(WidgetSizeEnum._3x2),
                WidgetSizes.GetByFirst(WidgetSizeEnum._4x2),
                WidgetSizes.GetByFirst(WidgetSizeEnum._5x2),
                WidgetSizes.GetByFirst(WidgetSizeEnum._1x3),
                WidgetSizes.GetByFirst(WidgetSizeEnum._2x3),
                WidgetSizes.GetByFirst(WidgetSizeEnum._3x3),
                WidgetSizes.GetByFirst(WidgetSizeEnum._4x3),
                WidgetSizes.GetByFirst(WidgetSizeEnum._5x3),
                WidgetSizes.GetByFirst(WidgetSizeEnum._1x4),
                WidgetSizes.GetByFirst(WidgetSizeEnum._2x4),
                WidgetSizes.GetByFirst(WidgetSizeEnum._3x4),
                WidgetSizes.GetByFirst(WidgetSizeEnum._4x4),
                WidgetSizes.GetByFirst(WidgetSizeEnum._5x4),
            };
        private Bitmap _previewImage;
        public Bitmap PreviewImage // TODO: not sure where this is called from? marketplace?
        {
            get
            {
                if (_previewImage == null)
                {
                    try
                    {
                        _previewImage = new Bitmap(ResourcePath + "preview_5x4.png");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error loading preview image: {ex.Message}");

                        // rather than return an error, we supply a thumb with a useful message
                        _previewImage = new Bitmap(907, 523); // same dimensions as a 5x4
                        string thumbStr = "PreviewImage\nnot found.";
                        int numLines = 2;

                        Font drawFont = new Font(WidgetManager.GlobalWidgetTheme.PrimaryFont.FontFamily,
                                                WidgetManager.GlobalWidgetTheme.PrimaryFont.Size,
                                                WidgetManager.GlobalWidgetTheme.PrimaryFont.Style);

                        var format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        format.Trimming = StringTrimming.Word;

                        drawFont = FindMaxFontSize(_previewImage, thumbStr, drawFont, format, numLines);

                        using (var graphics = Graphics.FromImage(_previewImage))
                        {
                            graphics.Clear(WidgetManager.GlobalWidgetTheme.PrimaryBgColor);

                            var rect = new Rectangle(0, 0, _previewImage.Width, _previewImage.Height);

                            graphics.DrawString(thumbStr, drawFont, new SolidBrush(WidgetManager.GlobalWidgetTheme.PrimaryFgColor), rect, format);
                        }
                    }
                }
                return _previewImage;
            }
            private set { _previewImage = value; }
        }

        // IWidgetObject : IWidgetBase
        public IWidgetManager WidgetManager { get; set; }
        private Bitmap _widgetThumbnail;

        public Bitmap WidgetThumbnail
        {
            get
            {
                if (_widgetThumbnail == null)
                {
                    // rather than return an error, we supply a thumb with a useful message
                    _widgetThumbnail = new Bitmap(200, 150);
                    string thumbStr = "thumb.png\nnot found";
                    int numLines = 2;

                    // it appears that this is called before the user profile
                    // overrides are loaded, but not a big deal...
                    Font drawFont = new Font(WidgetManager.GlobalWidgetTheme.PrimaryFont.FontFamily,
                                            WidgetManager.GlobalWidgetTheme.PrimaryFont.Size,
                                            WidgetManager.GlobalWidgetTheme.PrimaryFont.Style);

                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    format.Trimming = StringTrimming.Word;

                    drawFont = FindMaxFontSize(_widgetThumbnail, thumbStr, drawFont, format, numLines);

                    using (var graphics = Graphics.FromImage(_widgetThumbnail))
                    {
                        graphics.Clear(ColorTranslator.FromHtml("#2D2D30")); // WigiDash grey

                        var rect = new Rectangle(0, 0, _widgetThumbnail.Width, _widgetThumbnail.Height);

                        graphics.DrawString(thumbStr, drawFont, new SolidBrush(WidgetManager.GlobalWidgetTheme.PrimaryFgColor), rect, format);
                    }
                }

                return _widgetThumbnail;
            }
            private set { _widgetThumbnail = value; }
        }
        public Bitmap GetWidgetPreview(WidgetSize widget_size)
        {
            try
            {
                // will throw if unimplemented, expected
                return MyWidget.GetWidgetPreview(widget_size);
            }
            catch (Exception ex)
            {
            }

            try
            {
                // will throw if widget_size is invalid or no image
                Bitmap loadedPreview = WidgetPreviews[(int)WidgetSizes.GetBySecond(widget_size)];
                if (loadedPreview != null)
                    return loadedPreview;
            }
            catch (Exception ex)
            {
            }

            Bitmap bitmap = new Bitmap(widget_size.ToSize().Width, widget_size.ToSize().Height);

            string previewStr = $"preview_{widget_size.Width}x{widget_size.Height}.png\nnot found\nnot impl.";
            int numLines = 3;

            Font drawFont = new Font(WidgetManager.GlobalWidgetTheme.PrimaryFont.FontFamily,
                                    WidgetManager.GlobalWidgetTheme.PrimaryFont.Size,
                                    WidgetManager.GlobalWidgetTheme.PrimaryFont.Style);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            format.Trimming = StringTrimming.Word;

            drawFont = FindMaxFontSize(bitmap, previewStr, drawFont, format, numLines);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(WidgetManager.GlobalWidgetTheme.PrimaryBgColor);

                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                graphics.DrawString(previewStr, drawFont, new SolidBrush(WidgetManager.GlobalWidgetTheme.PrimaryFgColor), rect, format);

                return bitmap;
            }
        }
        public IWidgetInstance CreateWidgetInstance(WidgetSize widget_size, Guid instance_guid)
        {
            return new MyWidget(this, widget_size, instance_guid);
        }
        public bool RemoveWidgetInstance(Guid instance_guid)
        {
            throw new NotImplementedException("RemoveWidgetInstance not implemented");
        }

        public WidgetError Load(string resource_path)
        {
            ResourcePath = resource_path;

            // load or initialize previews
            WidgetPreviews = WidgetSizes.Select(kvp =>
            {
                Bitmap bitmap = null;
                try
                {
                    bitmap = new Bitmap(ResourcePath + $"preview_{kvp.Value.Width}x{kvp.Value.Height}.png");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"preview_{kvp.Value.Width}x{kvp.Value.Height}.png not found");
                }
                return bitmap;
            }).ToArray();


            // load or intialize thumb
            try
            {
                WidgetThumbnail = new Bitmap(Path.Combine(ResourcePath, "thumb.png"));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"!!! thumb.png not found");

                try
                {
                    WidgetThumbnail = MyWidget.GetWidgetThumbNail();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"!!! GetWidgetThumbNail not implemented and no thumb.png found");
                }
            }

            return WidgetError.NO_ERROR;
        }
        public WidgetError Unload()
        {
            return WidgetError.NO_ERROR;
        }
        public string LastErrorMessage { get; set; }


        public string ResourcePath;

        private Bitmap[] WidgetPreviews;

        // ideally this would be apart of the WidgetSize class
        public static readonly BiDictionary<WidgetSizeEnum, WidgetSize> WidgetSizes =
            new BiDictionary<WidgetSizeEnum, WidgetSize>
            {
                { WidgetSizeEnum._1x1,  new WidgetSize(1, 1) },
                { WidgetSizeEnum._2x1,  new WidgetSize(2, 1) },
                { WidgetSizeEnum._3x1,  new WidgetSize(3, 1) },
                { WidgetSizeEnum._4x1,  new WidgetSize(4, 1) },
                { WidgetSizeEnum._5x1,  new WidgetSize(5, 1) },
                { WidgetSizeEnum._1x2,  new WidgetSize(1, 2) },
                { WidgetSizeEnum._2x2,  new WidgetSize(2, 2) },
                { WidgetSizeEnum._3x2,  new WidgetSize(3, 2) },
                { WidgetSizeEnum._4x2,  new WidgetSize(4, 2) },
                { WidgetSizeEnum._5x2,  new WidgetSize(5, 2) },
                { WidgetSizeEnum._1x3,  new WidgetSize(1, 3) },
                { WidgetSizeEnum._2x3,  new WidgetSize(2, 3) },
                { WidgetSizeEnum._3x3,  new WidgetSize(3, 3) },
                { WidgetSizeEnum._4x3,  new WidgetSize(4, 3) },
                { WidgetSizeEnum._5x3,  new WidgetSize(5, 3) },
                { WidgetSizeEnum._1x4,  new WidgetSize(1, 4) },
                { WidgetSizeEnum._2x4,  new WidgetSize(2, 4) },
                { WidgetSizeEnum._3x4,  new WidgetSize(3, 4) },
                { WidgetSizeEnum._4x4,  new WidgetSize(4, 4) },
                { WidgetSizeEnum._5x4,  new WidgetSize(5, 4) },
            };

        // TODO: move to utils? font extensions?
        public static Font FindMaxFontSize(Bitmap bitmap, string text, Font font, StringFormat stringFormat, int numLines)
        {
            SizeF layoutArea = new SizeF(bitmap.Width, bitmap.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                SizeF textSize;
                float fontSize = 1f;
                Font testFont;

                bool goodFit = false;
                int lineCount = 0;

                do
                {
                    fontSize += 0.5f;
                    testFont = new Font(font.FontFamily, fontSize, font.Style);
                    textSize = g.MeasureString(text, testFont, layoutArea, stringFormat, out int charactersFitted, out int linesFilled);

                    goodFit = charactersFitted == text.Length && linesFilled == numLines;
                    lineCount = linesFilled;

                    if (fontSize > 1000.0f)
                        break; // jic
                }
                while ((goodFit || lineCount <= numLines)
                    && textSize.Width < layoutArea.Width && textSize.Height < layoutArea.Height);

                return new Font(font.FontFamily, Math.Max(fontSize - 1f, 1f), font.Style);
            }

        }
    }
}
