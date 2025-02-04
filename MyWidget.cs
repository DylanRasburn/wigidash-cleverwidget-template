using WigiDashWidgetFramework;
using WigiDashWidgetFramework.WidgetUtility;

using System;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

// when cloning this project, create new GUIDs (Menu Bar -> Tools -> Create GUID) :
//
// -modify the Assembly name GUID in .csproj : AE0A7950-B3CD-454D-9ED7-6C402A87A718
// -the Assembly Information GUID: C7E7F5CF-7BE3-4A87-89CA-1DA000E6957C
// -modify .csproj & .sln GUIDs: 9EFF26F0-1050-4C98-9F63-F075D1DC3D05 & 0CE23E63-D784-4EA0-8C3B-43E1511408CB
// -the action & trigger guids if implementing them
// 
// and also rename the project & solution 

// TODO: move brushes into setters instead of newing up in draw threads

namespace CleverWidget
{
    public sealed class MyWidget : CleverWidgetBase
    {
        // IWidgetInstance
        public override void ClickEvent(ClickType click_type, int x, int y)
        {
            // on occasion I've had phantom finger presses come through when launching
            // this can cause issues depending on your implementation
            if (x == 0 && y == 0)
            {
                Debug.Assert(false, $"!!! phantom ClickEvent 0,0");
                return;
            }

            if (x < 0 || x > WidgetSize.ToSize().Width || y < 0 || y > WidgetSize.ToSize().Height)
            {
                Debug.Assert(false, $"!!! ClickEvent {x},{y} out of bounds of widget size {x},{y}");
                return;
            }

            // debounce the finger presses
            // in an old fw vers occasionally there will be second erroneous single click fired sequentially < 200ms
            // after the real one, this only occurs with the physical device & not using 'Emulate Device Touch'
            DateTime now = DateTime.Now;
            if ((DateTime.Now - _lastClickEventTime).TotalMilliseconds < 200) // a good tested value
                return;

            _lastClickEventTime = now;

            switch (click_type)
            {
                case ClickType.Single:

                    _singleClickCount += DemoIncrementValue;

                    if (Toggleable) Toggled = !Toggled;

                    WidgetObject.WidgetManager.OnTriggerOccurred(clicked_trigger_guid);

                    break;
                case ClickType.Double: // none of these appear to be called
                case ClickType.Long:
                case ClickType.SwipeLeft:
                case ClickType.SwipeRight:
                case ClickType.SwipeUp:
                case ClickType.SwipeDown:
                    break;
                default:
                    Debug.Assert(false, "!!! unsupported click_type: " + click_type.ToString());
                    break;
            }
        }

        public override System.Windows.Controls.UserControl GetSettingsControl()
        {
            return new MyWidgetControls(this);
        }

        public override void EnterSleep()
        {
            pause_task = true;
        }

        public override void ExitSleep()
        {
            pause_task = false;
        }

        // IWidgetInstance : IDisposable 
        public override void Dispose()
        {
            run_task = false;
        }

        public int DemoIncrementValue = 1;
        public bool HideCount = false;
        private volatile int _singleClickCount = 0;

        // create new GUIDs for if you're implmeneting this
        private Guid clicked_trigger_guid = new Guid("122F5BAA-5EFF-4D98-8391-D9DD8B14A541");
        // TODO: figure out actions

        private DateTime _lastClickEventTime = DateTime.MinValue;

        private Thread draw_task_thread;

        private volatile bool run_task = true;
        private volatile bool pause_task = false;

        public MyWidget(CleverWidgetFactory parent, WidgetSize widget_size, Guid instance_guid) :
                base(parent, widget_size, instance_guid)
        {
            WidgetObject.WidgetManager.RegisterTrigger(this, clicked_trigger_guid, "Counter Click");
            WidgetObject.WidgetManager.GlobalThemeUpdated += WidgetManager_GlobalThemeUpdated;

            _lastClickEventTime = DateTime.MinValue;
            run_task = true;
            pause_task = false;

            ThreadStart draw_thread_start = new ThreadStart(ThreadDrawTask);
            draw_task_thread = new Thread(draw_thread_start);
            draw_task_thread.IsBackground = true;
            draw_task_thread.Start();
        }

        private void ThreadDrawTask()
        {
            while (run_task)
            {
                while (pause_task)
                    Thread.Sleep(10);

                Bitmap bitmap_edit = WigdetBitmap; // creates a copy

                using (var graphics = Graphics.FromImage(bitmap_edit))
                {
                    // TODO: are these all necessary?
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    // replace all this code with your custom graphics drawing code

                    Font DrawFont = !Toggled ? UserFontPrimary : UserFontSecondary;
                    Color FontColor = !Toggled ? UserForeColorPrimary : UserForeColorSecondary;
                    Color BGColor = !Toggled ? UserBackColorPrimary : UserBackColorSecondary;

                    if (UseGlobalTheme)
                    {
                        DrawFont = !Toggled ?
                            new Font(
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont.FontFamily,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont.Size,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFont.Style)
                            :
                            new Font(
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont.FontFamily,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont.Size,
                                    WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFont.Style);

                        FontColor = !Toggled ? WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryFgColor
                            : WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryFgColor;

                        BGColor = !Toggled ? WidgetObject.WidgetManager.GlobalWidgetTheme.PrimaryBgColor
                            : WidgetObject.WidgetManager.GlobalWidgetTheme.SecondaryBgColor;
                    }

                    // example counter drawing
                    graphics.Clear(BGColor);

                    base.DrawOverlays(graphics); // draws text over images over

                    if (!HideCount)
                    {
                        var rect = new Rectangle(
                            0, 0,
                            WidgetSize.ToSize().Width, WidgetSize.ToSize().Height);

                        var format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        graphics.DrawString($"{_singleClickCount}", DrawFont,
                            new SolidBrush(FontColor), rect, format);
                    }
                }

                WigdetBitmap = bitmap_edit;

                RequestUpdate(); // note: the virtual wigidash panel is updated faster than the device

                Thread.Sleep(20);

                if (!run_task)
                    break;
            }
        }

        private void WidgetManager_GlobalThemeUpdated()
        {
            // any changes to global theme picked up automatically in next drawing operation anyhow
        }

        public static Bitmap GetWidgetThumbNail()
        {
            throw new NotImplementedException("MyWidget.GetWidgetThumbNail() not implemented");
        }

        public static Bitmap GetWidgetPreview(WidgetSize widget_size)
        {
            Bitmap widget_preview = new Bitmap(widget_size.ToSize().Width, widget_size.ToSize().Height);

            using (var graphics = Graphics.FromImage(widget_preview))
            {
                // TODO: are these all necessary?
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                Color BGColor = Color.FromArgb(48, 48, 48);
                Font DrawFont = new Font("Basic Square 7", 32); ;
                Color FontColor = Color.White;

                graphics.Clear(BGColor);
                var rect = new Rectangle(
                    0, 0,
                    widget_size.ToSize().Width, widget_size.ToSize().Height);

                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                graphics.DrawString($"{42}", DrawFont,
                    new SolidBrush(FontColor), rect, format);
            }

            return widget_preview;
        }
    }

    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, System.Drawing.Brush brush,
            float x, float y, float width, float height, float radius)
        {
            var path = GetRoundedRect(x, y, width, height, radius);
            graphics.FillPath(brush, path);
        }

        public static void DrawRoundedRectangle(this Graphics graphics, System.Drawing.Pen pen,
            float x, float y, float width, float height, float radius)
        {
            var path = GetRoundedRect(x, y, width, height, radius);
            graphics.DrawPath(pen, path);
        }

        public static GraphicsPath GetRoundedRect(float x, float y, float width, float height, float radius)
        {
            if (radius <= 0.0f) // for the case corner radius = 0, AddArc throws
                radius = 0.01f;

            var path = new GraphicsPath();
            var diameter = radius * 2;
            var rect = new RectangleF(x, y, width, height);

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
