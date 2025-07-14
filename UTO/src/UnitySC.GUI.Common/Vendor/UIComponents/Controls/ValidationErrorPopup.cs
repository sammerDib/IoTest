using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// This custom popup can be used by validation error templates or something else.
    /// It provides some additional nice features:
    /// - repositioning if host-window size or location changed
    /// - repositioning if host-window gets maximized and vice versa
    /// - it's only topmost if the host-window is activated
    /// </summary>
    public class ValidationErrorPopup : Popup
    {
        /// <summary>
        /// Identifies the <see cref="CloseOnMouseLeftButtonDown" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty CloseOnMouseLeftButtonDownProperty
            = DependencyProperty.Register(nameof(CloseOnMouseLeftButtonDown),
                typeof(bool),
                typeof(ValidationErrorPopup),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets/sets if the popup can be closed by left mouse button down.
        /// </summary>
        public bool CloseOnMouseLeftButtonDown
        {
            get { return (bool)GetValue(CloseOnMouseLeftButtonDownProperty); }
            set { SetValue(CloseOnMouseLeftButtonDownProperty, value); }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initialize a new instance of <see cref="T:Agileo.GUI.Wpf.Controls.ValidationErrorPopup" />
        /// </summary>
        public ValidationErrorPopup()
        {
            Loaded += PopupEx_Loaded;
            Opened += PopupEx_Opened;
        }

        /// <summary>
        /// Causes the popup to update it's position according to it's current settings.
        /// </summary>
        public void RefreshPosition()
        {
            var offset = HorizontalOffset;
            // "bump" the offset to cause the popup to reposition itself on its own
            SetCurrentValue(HorizontalOffsetProperty, offset + 1);
            SetCurrentValue(HorizontalOffsetProperty, offset);
        }

        private void PopupEx_Loaded(object sender, RoutedEventArgs e)
        {
            var target = PlacementTarget as FrameworkElement;
            if (target == null) return;

            _hostWindow = Window.GetWindow(target);
            if (_hostWindow == null) return;

            _hostWindow.LocationChanged -= hostWindow_SizeOrLocationChanged;
            _hostWindow.LocationChanged += hostWindow_SizeOrLocationChanged;
            _hostWindow.SizeChanged -= hostWindow_SizeOrLocationChanged;
            _hostWindow.SizeChanged += hostWindow_SizeOrLocationChanged;
            target.SizeChanged -= hostWindow_SizeOrLocationChanged;
            target.SizeChanged += hostWindow_SizeOrLocationChanged;
            _hostWindow.StateChanged -= hostWindow_StateChanged;
            _hostWindow.StateChanged += hostWindow_StateChanged;
            _hostWindow.Activated -= hostWindow_Activated;
            _hostWindow.Activated += hostWindow_Activated;
            _hostWindow.Deactivated -= hostWindow_Deactivated;
            _hostWindow.Deactivated += hostWindow_Deactivated;

            Unloaded -= PopupEx_Unloaded;
            Unloaded += PopupEx_Unloaded;
        }

        private void PopupEx_Opened(object sender, EventArgs e)
        {
            SetTopmostState(true);
        }

        private void hostWindow_Activated(object sender, EventArgs e)
        {
            SetTopmostState(true);
        }

        private void hostWindow_Deactivated(object sender, EventArgs e)
        {
            SetTopmostState(false);
        }

        private void PopupEx_Unloaded(object sender, RoutedEventArgs e)
        {
            if (PlacementTarget is FrameworkElement target)
            {
                target.SizeChanged -= hostWindow_SizeOrLocationChanged;
            }

            if (_hostWindow != null)
            {
                _hostWindow.LocationChanged -= hostWindow_SizeOrLocationChanged;
                _hostWindow.SizeChanged -= hostWindow_SizeOrLocationChanged;
                _hostWindow.StateChanged -= hostWindow_StateChanged;
                _hostWindow.Activated -= hostWindow_Activated;
                _hostWindow.Deactivated -= hostWindow_Deactivated;
            }

            Unloaded -= PopupEx_Unloaded;
            Opened -= PopupEx_Opened;
            _hostWindow = null;
        }

        private void hostWindow_StateChanged(object sender, EventArgs e)
        {
            if (_hostWindow != null && _hostWindow.WindowState != WindowState.Minimized)
            {
                // special handling for validation popup
                var target = PlacementTarget as FrameworkElement;
                var holder = target?.DataContext as AdornedElementPlaceholder;
                if (holder?.AdornedElement != null)
                {
                    PopupAnimation = PopupAnimation.None;
                    IsOpen = false;
                    var errorTemplate = holder.AdornedElement.GetValue(Validation.ErrorTemplateProperty);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, null);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, errorTemplate);
                }
            }
        }

        private void hostWindow_SizeOrLocationChanged(object sender, EventArgs e)
        {
            RefreshPosition();
        }

        private void SetTopmostState(bool isTop)
        {
            // Donât apply state if itâs the same as incoming state
            if (_appliedTopMost.HasValue && _appliedTopMost == isTop) return;

            if (Child == null) return;

            if (PresentationSource.FromVisual(Child) is not HwndSource hwndSource) return;
            var hwnd = hwndSource.Handle;

            if (!GetWindowRect(hwnd, out var rect)) return;
            //Debug.WriteLine("setting z-order " + isTop)

            var left = rect.Left;
            var top = rect.Top;
            var width = rect.Width;
            var height = rect.Height;
            if (isTop)
            {
                SetWindowPos(hwnd, HwndTopmost, left, top, width, height, Swp.Topmost);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                SetWindowPos(hwnd, HwndBottom, left, top, width, height, Swp.Topmost);
                SetWindowPos(hwnd, HwndTop, left, top, width, height, Swp.Topmost);
                SetWindowPos(hwnd, HwndNotopmost, left, top, width, height, Swp.Topmost);
            }

            _appliedTopMost = isTop;
        }

        /// <inheritdoc />
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CloseOnMouseLeftButtonDown) IsOpen = false;
        }

        private Window _hostWindow;
        private bool? _appliedTopMost;
        private static readonly IntPtr HwndTopmost = new IntPtr(-1);
        private static readonly IntPtr HwndNotopmost = new IntPtr(-2);
        private static readonly IntPtr HwndTop = new IntPtr(0);
        private static readonly IntPtr HwndBottom = new IntPtr(1);

        /// <summary>
        /// SetWindowPos options
        /// </summary>
        [Flags]
        internal enum Swp
        {
            Asyncwindowpos = 0x4000,
            Defererase = 0x2000,
            Drawframe = 0x0020,
            Framechanged = 0x0020,
            Hidewindow = 0x0080,
            Noactivate = 0x0010,
            Nocopybits = 0x0100,
            Nomove = 0x0002,
            Noownerzorder = 0x0200,
            Noredraw = 0x0008,
            Noreposition = 0x0200,
            Nosendchanging = 0x0400,
            Nosize = 0x0001,
            Nozorder = 0x0004,
            Showwindow = 0x0040,
            Topmost = Noactivate | Noownerzorder | Nosize | Nomove | Noredraw | Nosendchanging
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static int Loword(int i)
        {
            return (short)(i & 0xFFFF);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Size
        {
            public int cx;
            public int cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public void Offset(int dx, int dy)
            {
                Left += dx;
                Top += dy;
                Right += dx;
                Bottom += dy;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Left { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Right { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Top { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Bottom { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Width => Right - Left;

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public int Height => Bottom - Top;

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public Point Position => new Point { x = Left, y = Top };

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public Size Size => new Size { cx = Width, cy = Height };

            public static Rect Union(Rect rect1, Rect rect2)
            {
                return new Rect
                {
                    Left = Math.Min(rect1.Left, rect2.Left),
                    Top = Math.Min(rect1.Top, rect2.Top),
                    Right = Math.Max(rect1.Right, rect2.Right),
                    Bottom = Math.Max(rect1.Bottom, rect2.Bottom)
                };
            }

            public override bool Equals(object obj)
            {
                try
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var rc = (Rect)obj;
                    return rc.Bottom == Bottom
                           && rc.Left == Left
                           && rc.Right == Right
                           && rc.Top == Top;
                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }

            [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
            public override int GetHashCode()
            {
                return ((Left << 16) | Loword(Right)) ^ ((Top << 16) | Loword(Bottom));
            }
        }

        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
            Swp uFlags);

        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, Swp uFlags)
        {
            if (!_SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags)) return false;

            return true;
        }
    }
}
