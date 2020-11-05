﻿
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static Jvedio.StaticVariable;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Jvedio
{


    /// <summary>
    /// 除 Main 和 Detail 外的的窗口样式
    /// </summary>
    public  class Jvedio_BaseWindow : Window
    {
        public Point WindowPoint = new Point(100, 100);
        public Size WindowSize = new Size(800, 500);
        public JvedioWindowState WinState = JvedioWindowState.Normal;

        private HwndSource _hwndSource;

        public Jvedio_BaseWindow()
        {
            InitStyle();//窗体的 Style
            AdjustWindow();
            this.Loaded += delegate { InitEvent(); };//初始化载入事件



        }

        #region "改变窗体大小"
        private void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WinState == JvedioWindowState.Maximized || WinState == JvedioWindowState.FullScreen) return;
            Rectangle rectangle = sender as Rectangle;

            if (rectangle != null)
            {
                switch (rectangle.Name)
                {
                    case "TopRectangle":
                        Cursor = Cursors.SizeNS;
                        ResizeWindow(ResizeDirection.Top);
                        break;
                    case "Bottom":
                        Cursor = Cursors.SizeNS;
                        ResizeWindow(ResizeDirection.Bottom);
                        break;
                    case "LeftRectangle":
                        Cursor = Cursors.SizeWE;
                        ResizeWindow(ResizeDirection.Left);
                        break;
                    case "Right":
                        Cursor = Cursors.SizeWE;
                        ResizeWindow(ResizeDirection.Right);
                        break;
                    case "TopLeft":
                        Cursor = Cursors.SizeNWSE;
                        ResizeWindow(ResizeDirection.TopLeft);
                        break;
                    case "TopRight":
                        Cursor = Cursors.SizeNESW;
                        ResizeWindow(ResizeDirection.TopRight);
                        break;
                    case "BottomLeft":
                        Cursor = Cursors.SizeNESW;
                        ResizeWindow(ResizeDirection.BottomLeft);
                        break;
                    case "BottomRight":
                        Cursor = Cursors.SizeNWSE;
                        ResizeWindow(ResizeDirection.BottomRight);
                        break;
                    default:
                        break;
                }
            }
        }


        protected void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                Cursor = Cursors.Arrow;
        }

        private void ResizeRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (WinState == JvedioWindowState.Maximized || WinState == JvedioWindowState.FullScreen) return;
            Rectangle rectangle = sender as Rectangle;

            if (rectangle != null)
            {
                switch (rectangle.Name)
                {
                    case "TopRectangle":
                        Cursor = Cursors.SizeNS;
                        break;
                    case "Bottom":
                        Cursor = Cursors.SizeNS;
                        break;
                    case "LeftRectangle":
                        Cursor = Cursors.SizeWE;
                        break;
                    case "Right":
                        Cursor = Cursors.SizeWE;
                        break;
                    case "TopLeft":
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case "TopRight":
                        Cursor = Cursors.SizeNESW;
                        break;
                    case "BottomLeft":
                        Cursor = Cursors.SizeNESW;
                        break;
                    case "BottomRight":
                        Cursor = Cursors.SizeNWSE;
                        break;
                    default:
                        break;
                }
            }
        }

        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        protected override void OnInitialized(EventArgs e)
        {
            SourceInitialized += MainWindow_SourceInitialized;
            base.OnInitialized(e);
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        #endregion



        /// <summary>
        /// 保存窗口状态
        /// </summary>
        private void SaveWindow()
        {
            if (this.WindowState != WindowState.Minimized)
            {
                if (this.WindowState == WindowState.Normal) WinState =JvedioWindowState.Normal;
                else if (this.WindowState == WindowState.Maximized) WinState = JvedioWindowState.FullScreen;
                else if (this.Width == SystemParameters.WorkArea.Width & this.Height == SystemParameters.WorkArea.Height) WinState = JvedioWindowState.Maximized;

                WindowConfig cj = new WindowConfig(this.GetType().Name);
                Rect rect = new Rect(this.Left, this.Top, this.Width, this.Height);
                cj.Save(rect, WinState);
            }
        }

        /// <summary>
        /// 调整窗体状态
        /// </summary>
        private void AdjustWindow()
        {
            //读取窗体设置
            WindowConfig cj = new WindowConfig(this.GetType().Name);
            Rect rect;
            (rect, WinState) = cj.GetValue();

            if ( rect.X!=-1 && rect.Y!=-1)
            {
                //读到属性值
                if (WinState == JvedioWindowState.Maximized)
                {
                    MaxWindow(this, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, MouseButton.Left));
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    this.Left = rect.X > 0 ? rect.X : 0;
                    this.Top = rect.Y > 0 ? rect.Y : 0;
                    this.Height = rect.Height > 100 ? rect.Height : 100;
                    this.Width = rect.Width > 100 ? rect.Width : 100;
                    if (this.Width == SystemParameters.WorkArea.Width | this.Height == SystemParameters.WorkArea.Height) { WinState = JvedioWindowState.Maximized; }
                }
            }
            else
            {
                WinState = JvedioWindowState.Normal;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            HideMargin();
        }

        private void InitStyle()
        {
            this.Style = (Style)App.Current.Resources["Jvedio_BaseWindowStyle"];
        }

        private void InitEvent()
        {
            ControlTemplate baseWindowTemplate = (ControlTemplate)App.Current.Resources["BaseWindowControlTemplate"];
            Border minBtn = (Border)baseWindowTemplate.FindName("BorderMin", this);
            minBtn.MouseLeftButtonUp += delegate (object sender, MouseButtonEventArgs e)
            {
                MinWindow();
            };

            Border maxBtn = (Border)baseWindowTemplate.FindName("BorderMax", this);
            maxBtn.MouseLeftButtonUp += MaxWindow;

            Border closeBtn = (Border)baseWindowTemplate.FindName("BorderClose", this);
            closeBtn.MouseLeftButtonUp += delegate (object sender, MouseButtonEventArgs e)
            {
                FadeOut();
            };

            Border borderTitle = (Border)baseWindowTemplate.FindName("BorderTitle", this);
            borderTitle.MouseMove += MoveWindow;
            borderTitle.MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e)
            {
                if (e.ClickCount >= 2)
                {
                    MaxWindow(this, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, MouseButton.Left));
                }
            };

            this.Closing += delegate {
                SaveWindow();
            };

            this.SizeChanged += onSizeChanged;



            #region "改变窗体大小"
            //https://www.cnblogs.com/yang-fei/p/4737308.html
            Grid resizeGrid = (Grid)baseWindowTemplate.FindName("resizeGrid", this);

            if (resizeGrid != null)
            {
                foreach (UIElement element in resizeGrid.Children)
                {
                    Rectangle resizeRectangle = element as Rectangle;
                    if (resizeRectangle != null)
                    {
                        resizeRectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
                        resizeRectangle.MouseMove += ResizeRectangle_MouseMove;
                    }
                }
            }
            PreviewMouseMove += OnPreviewMouseMove;
            #endregion

            FadeIn();

        }

        private void onSizeChanged(object sender, SizeChangedEventArgs e)
        {

            ControlTemplate baseWindowTemplate = (ControlTemplate)App.Current.Resources["BaseWindowControlTemplate"];
            Grid MainGrid = (Grid)baseWindowTemplate.FindName("MainGrid", this);
            Grid ContentGrid = (Grid)baseWindowTemplate.FindName("ContentGrid", this);
            Border MainBorder = (Border)baseWindowTemplate.FindName("MainBorder", this);
            Border BorderTitle = (Border)baseWindowTemplate.FindName("BorderTitle", this);
            if (MainGrid == null) return;

            if (this.Width == SystemParameters.WorkArea.Width || this.Height == SystemParameters.WorkArea.Height)
            {
                MainGrid.Margin = new Thickness(0);
                ContentGrid.Margin = new Thickness(0);
                MainBorder.CornerRadius = new CornerRadius() { TopLeft = 0, TopRight = 0, BottomRight = 0, BottomLeft = 0 };
                BorderTitle.CornerRadius = new CornerRadius() { TopLeft = 0, TopRight = 0, BottomRight = 0, BottomLeft = 0 };
                this.ResizeMode = ResizeMode.NoResize;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                MainGrid.Margin = new Thickness(0);
                MainBorder.CornerRadius = new CornerRadius() { TopLeft = 0, TopRight = 0, BottomRight = 0, BottomLeft = 0 };
                BorderTitle.CornerRadius = new CornerRadius() { TopLeft = 0, TopRight = 0, BottomRight = 0, BottomLeft = 0 };
                this.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                MainGrid.Margin = new Thickness(10);
                ContentGrid.Margin = new Thickness(5);
                MainBorder.CornerRadius = new CornerRadius() { TopLeft = 5, TopRight = 5, BottomRight = 5, BottomLeft = 5 };
                BorderTitle.CornerRadius = new CornerRadius() { TopLeft = 5, TopRight = 5, BottomRight = 0, BottomLeft = 0 };
                this.ResizeMode = ResizeMode.CanResize;
            }
        }

        public async void FadeIn()
        {
            if (Properties.Settings.Default.EnableWindowFade)
            {
                this.Opacity = 0;
                double opacity = this.Opacity;
                await Task.Run(() => {
                    while (opacity < 0.5)
                    {
                        this.Dispatcher.Invoke((Action)delegate { this.Opacity += 0.05; opacity = this.Opacity; });
                        Task.Delay(1).Wait();
                    }
                });
            }
            this.Opacity = 1;
        }

        public async void FadeOut()
        {
            if (Properties.Settings.Default.EnableWindowFade)
            {
                double opacity = this.Opacity;
                await Task.Run(() => {
                    while (opacity > 0.1)
                    {
                        this.Dispatcher.Invoke((Action)delegate { this.Opacity -= 0.05; opacity = this.Opacity; });
                        Task.Delay(1).Wait();
                    }
                });
                this.Opacity = 0;
            }
           
            this.Close();
        }


        public async void MinWindow()
        {
            if (Properties.Settings.Default.EnableWindowFade)
            {
                double opacity = this.Opacity;
                await Task.Run(() =>
                {
                    while (opacity > 0.2)
                    {
                        this.Dispatcher.Invoke((Action)delegate { this.Opacity -= 0.1; opacity = this.Opacity; });
                        Task.Delay(20).Wait();
                    }
                });
            }
            this.WindowState = WindowState.Minimized;
            this.Opacity = 1;

        }



        public void MaxWindow(object sender, MouseButtonEventArgs e)
        {
            if (WinState == JvedioWindowState.Normal)
            {
                //最大化
                WinState = JvedioWindowState.Maximized;
                WindowPoint = new Point(this.Left, this.Top);
                WindowSize = new Size(this.Width, this.Height);
                this.Height = SystemParameters.WorkArea.Height;
                this.Width = SystemParameters.WorkArea.Width;
                this.Left = SystemParameters.WorkArea.Left;
                this.Top = SystemParameters.WorkArea.Top;

            }
            else 
            {
                WinState = JvedioWindowState.Normal;
                this.Left = WindowPoint.X;
                this.Top = WindowPoint.Y;
                this.Width = WindowSize.Width;
                this.Height = WindowSize.Height;
            }
            this.WindowState = WindowState.Normal;
            HideMargin();
        }

        private void MoveWindow(object sender, MouseEventArgs e)
        {
            //移动窗口
            if (e.LeftButton == MouseButtonState.Pressed && WinState == JvedioWindowState.Normal)
            {
                this.DragMove();
            }
        }


        private void HideMargin()
        {
            ControlTemplate baseWindowTemplate = (ControlTemplate)App.Current.Resources["BaseWindowControlTemplate"];
            Grid MainGrid = (Grid)baseWindowTemplate.FindName("MainGrid", this);
            Grid ContentGrid = (Grid)baseWindowTemplate.FindName("ContentGrid", this);
            Border MainBorder = (Border)baseWindowTemplate.FindName("MainBorder", this);
            Border BorderTitle = (Border)baseWindowTemplate.FindName("BorderTitle", this);

            if (MainGrid == null ) return;
            if (WinState == JvedioWindowState.Normal)
            {
                MainGrid.Margin = new Thickness(10);
                ContentGrid.Margin = new Thickness(5);
                MainBorder.CornerRadius = new CornerRadius() { TopLeft = 5, TopRight = 5, BottomRight = 5, BottomLeft = 5 };
                BorderTitle.CornerRadius = new CornerRadius() { TopLeft = 5, TopRight = 5, BottomRight = 0, BottomLeft = 0 };
                this.ResizeMode = ResizeMode.CanResize;
            }
            else if (WinState == JvedioWindowState.Maximized || this.WindowState==WindowState.Maximized)
            {
                MainGrid.Margin = new Thickness(0);
                ContentGrid.Margin = new Thickness(0);
                MainBorder.CornerRadius = new CornerRadius() { TopLeft = 0, TopRight = 0, BottomRight = 0, BottomLeft = 0 };
                BorderTitle.CornerRadius = new CornerRadius() { TopLeft = 0, TopRight = 0, BottomRight = 0, BottomLeft = 0 };
                this.ResizeMode = ResizeMode.NoResize;

            }
        }


    }




}