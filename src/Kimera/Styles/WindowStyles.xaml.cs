using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.Styles
{
    public partial class WindowStyles : ResourceDictionary
    {
        #region ::Unmanaged Components::

        [StructLayout(LayoutKind.Sequential)]
        protected struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        /// <summary>
        /// The MONITORINFOEX structure contains information about a display monitor.
        /// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
        /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
        /// for the display monitor.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        protected struct MONITORINFOEX
        {
            // size of a device name string
            private const int CCHDEVICENAME = 32;

            /// <summary>
            /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
            /// Doing so lets the function determine the type of structure you are passing to it.
            /// </summary>
            public int Size;

            /// <summary>
            /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
            /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            public RECT Monitor;

            /// <summary>
            /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
            /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
            /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
            /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            public RECT WorkArea;

            /// <summary>
            /// The attributes of the display monitor.
            ///
            /// This member can be the following value:
            ///   1 : MONITORINFOF_PRIMARY
            /// </summary>
            public uint Flags;

            /// <summary>
            /// A string that specifies the device name of the monitor being used. Most applications have no use for a display monitor name,
            /// and so can save some bytes by using a MONITORINFO structure.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string DeviceName;

            public void Init()
            {
                this.Size = 40 + 2 * CCHDEVICENAME;
                this.DeviceName = string.Empty;
            }
        }

        /// <summary>
        /// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/dd162897%28VS.85%29.aspx"/>
        /// <remarks>
        /// By convention, the right and bottom edges of the rectangle are normally considered exclusive.
        /// In other words, the pixel whose coordinates are ( right, bottom ) lies immediately outside of the the rectangle.
        /// For example, when RECT is passed to the FillRect function, the rectangle is filled up to, but not including,
        /// the right column and bottom row of pixels. This structure is identical to the RECTL structure.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        protected struct RECT
        {
            /// <summary>
            /// The x-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Left;

            /// <summary>
            /// The y-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Top;

            /// <summary>
            /// The x-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Right;

            /// <summary>
            /// The y-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Bottom;
        }

        protected enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        #endregion

        #region ::Unmanaged Functions::

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        #endregion

        #region ::Fields & Properties::

        Window _window;

        #endregion

        #region ::Methods::

        private void LimitWindowSize()
        {
            if (GetWindow())
            {
                POINT cursorPosition;

                if (GetCursorPos(out cursorPosition))
                {
                    IntPtr hMonitor = MonitorFromPoint(cursorPosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

                    MONITORINFOEX monitorInfo = new MONITORINFOEX();

                    if (GetMonitorInfo(hMonitor, ref monitorInfo))
                    {
                        int width = monitorInfo.WorkArea.Right - monitorInfo.WorkArea.Left;
                        int height = monitorInfo.WorkArea.Bottom - monitorInfo.WorkArea.Top;
                        _window.MaxWidth = width + 16;
                        _window.MaxHeight = height + 16;
                    }
                }
            }
        }

        private bool GetWindow()
        {
            if (_window == null || _window != null)
            {
                WindowCollection windows = System.Windows.Application.Current.Windows;

                foreach (Window window in windows)
                {
                    if (window.IsActive == true)
                    {
                        _window = window;
                    }
                }

                return true;
            }
            else
                return false;
        }

        #endregion

        #region ::Event Subscribers::

        private void OnWindowLoaded(Object sender, RoutedEventArgs e)
        {
            if (sender is Window window && window.SizeToContent != SizeToContent.Manual)
            {
                FixLayout(window);
            }
        }

        private void FixLayout(Window window)
        {
            bool arrangeRequired = false;
            double deltaWidth = 0;
            double deltaHeight = 0;

            void CalculateDeltaSize()
            {
                deltaWidth = window.ActualWidth - deltaWidth;
                deltaHeight = window.ActualHeight - deltaHeight;
            }

            void OnSourceInitialized(object sender, EventArgs e)
            {
                window.InvalidateMeasure();
                arrangeRequired = true;
                window.SourceInitialized -= OnSourceInitialized;
            }

            void OnLayoutUpdated(object sender, EventArgs e)
            {
                if (arrangeRequired)
                {
                    if (window.SizeToContent == SizeToContent.WidthAndHeight)
                    {
                        CalculateDeltaSize();
                    }
                    window.Left -= deltaWidth * 0.5;
                    window.Top -= deltaHeight * 0.5;
                    window.LayoutUpdated -= OnLayoutUpdated;
                }
                else
                {
                    CalculateDeltaSize();
                }
            }

            window.SourceInitialized += OnSourceInitialized;
            window.LayoutUpdated += OnLayoutUpdated;
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            if (GetWindow())
                _window.WindowState = WindowState.Minimized;
        }

        private void OnMaximize(object sender, RoutedEventArgs e)
        {
            if (GetWindow())
            {
                if (_window.WindowState == WindowState.Maximized)
                {
                    _window.WindowState = WindowState.Normal;
                }
                else
                {
                    LimitWindowSize();
                    _window.WindowState = WindowState.Maximized;
                }
            }
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            if (GetWindow())
                _window.Close();
        }

        #endregion
    }
}
