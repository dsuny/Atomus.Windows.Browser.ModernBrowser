﻿using Atomus;
using Atomus.Control;
using Atomus.Diagnostics;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Atomus.Windows.Browser
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModernBrowser : Window, IAction
    {
        private ICore style;
        private IAction loginControl;
        private IAction joinControl;
        private IAction toolbarControl;
        private IAction homeControl;

        private DockPanel dockPanel;

        private Grid gridCenter;
        private GridSplitter gridSplitter;

        private IAction menuControl;

        private StatusBar statusBar;

        private AtomusControlEventHandler beforeActionEventHandler;
        private AtomusControlEventHandler afterActionEventHandler;

        private System.Windows.Style DefaultWindowStyle;

        #region Init
        public ModernBrowser()
        {
            ViewModel.ModernBrowserViewModel defaultBrowserViewModel;

            this.SetStyle();

            defaultBrowserViewModel = new ViewModel.ModernBrowserViewModel(this);

            InitializeComponent();

            this.DataContext = defaultBrowserViewModel;

            //this.AllowsTransparency = true;

            //this.WindowStyle = WindowStyle.None;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.Closing += DefaultBrowser_Closing;
            this.KeyDown += DefaultBrowser_KeyDown1;

            this.DefaultWindowStyle = this.Style;

            this.WindowState = WindowState.Maximized;

            //this.Style = null;
            //this.StateChanged += MainWindowStateChangeRaised;
        }

        private void SetStyle()
        {
            try
            {
                //this.style = new Atomus.Windows.Style.ModernStyle(Application.Current.Resources);
                this.style = (ICore)this.CreateInstance("Style", true, true, new object[] { Application.Current.Resources });
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
            }
        }
        #endregion

        #region Dictionary
        #endregion

        #region Spread
        #endregion

        #region IO
        object IAction.ControlAction(ICore sender, AtomusControlArgs e)
        {
            try
            {
                this.beforeActionEventHandler?.Invoke(this, e);

                switch (e.Action)
                {
                    default:
                        throw new AtomusException((this).GetMessage("Common", "00047", "'{0}'은(는) 처리할 수 없는 {1} 입니다.").Message.Translate(e.Action, "Action"));
                        //throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            finally
            {
                this.afterActionEventHandler?.Invoke(this, e);
            }
        }
        private void LoginControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case "DragMove":
                        this.DragMove();
                        break;

                    case "Opacity.Set":
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate
                        {
                            (this.DataContext as ViewModel.ModernBrowserViewModel).Opacity = (double)e.Value;
                        }));
                        break;

                    case "Opacity.Get":
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate
                        {
                            e.Value = (this.DataContext as ViewModel.ModernBrowserViewModel).Opacity;
                        }));
                        break;

                    case "Form.Size":
                        this.Width = ((Size)e.Value).Width;
                        this.Height = ((Size)e.Value).Height;

                        ((UserControl)this.loginControl).Width = this.Width;
                        ((UserControl)this.loginControl).Height = this.Height;
                        break;

                    case "Login.Ok":
                        this.Style = this.DefaultWindowStyle;

                        (this.DataContext as ViewModel.ModernBrowserViewModel).Opacity = 0;

                        this.gridLayout.Children.Remove((UserControl)this.loginControl);

                        //this.ControlBox = true;
                        this.ShowInTaskbar = true;

                        this.WindowStyle = WindowStyle.SingleBorderWindow;
                        
                        //this.AllowsTransparency = false;
                        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        this.Topmost = false;

                        //this.WindowState = WindowState.Maximized;
                        //this.WindowState = WindowState.Normal;
                        //this.WindowState = WindowState.Maximized;

                        //try
                        //{
                        //    this.Icon = await this.GetAttributeMediaWebImage("Icon");
                        //}
                        //catch (Exception _Exception)
                        //{
                        //    DiagnosticsTool.MyTrace(_Exception);
                        //}

                        this.ResizeMode = ResizeMode.CanResizeWithGrip;

                        this.Style = null;
                        this.Style = this.DefaultWindowStyle;

                        //(this.DataContext as ViewModel.ModernBrowserViewModel).UserID = null;

                        this.SetBrowserViewer();

                        this.SetToolbar();

                        //this.SetStatusBar();

                        this.SetCenter();

                        this.SetMenu();

                        if (this.toolbarControl != null)
                        {
                            this.toolbarControl.ControlAction(this, "Menu.TopMenu", this.menuControl?.ControlAction(this, "Menu.TopMenu", null));
                        }

                        this.gridSplitter = new GridSplitter()
                        {
                            Width = 5,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            ResizeBehavior = GridResizeBehavior.CurrentAndNext,
                            Background = new SolidColorBrush("#2a3344".ToMediaColor())
                        };
                        this.gridCenter.Children.Add(this.gridSplitter);

                        this.SetTabControl();

                        this.AddHomeControl();

                        //this.SetStatusStrip();

                        //(this.DataContext as ViewModel.ModernBrowserViewModel).Opacity = 1;

                        //this.FormClosing -= new FormClosingEventHandler(this.DefaultBrowser_FormClosing);
                        break;

                    case "Login.Cancel":
                        //this.Style = this.DefaultWindowStyle;
                        this.Close();
                        break;

                    case "Login.JoinOrAccessNumberChange":
                        this.SetJoinControl();
                        break;

                    default:
                        throw new AtomusException((this).GetMessage("Common", "00047", "'{0}'은(는) 처리할 수 없는 {1} 입니다.").Message.Translate(e.Action, "Action"));
                        //throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }
        private void JoinControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case "DragMove":
                        this.DragMove();
                        break;

                    case "Form.Size":
                        this.Width = ((Size)e.Value).Width;
                        this.Height = ((Size)e.Value).Height;

                        ((UserControl)this.loginControl).Width = this.Width;
                        ((UserControl)this.loginControl).Height = this.Height;
                        break;

                    case "Join.Cancel":
                        this.SetLoginControl();
                        break;

                    default:
                        throw new AtomusException((this).GetMessage("Common", "00047", "'{0}'은(는) 처리할 수 없는 {1} 입니다.").Message.Translate(e.Action, "Action"));
                        //throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        private double oldGridSplitterWidth = 0;
        private void ToolbarControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            IAction action;
            TabItem tabItem;
            AtomusControlEventArgs atomusControlEventArgs;

            try
            {
                action = (IAction)this.Tag;

                switch (e.Action)
                {
                    case "Menu.TopMenuSelected":
                        this.menuControl.ControlAction(sender, e.Action, e.Value);
                        break;


                    case "Close":
                        if (!action.Equals(this.homeControl) && this.tabControl.SelectedItem != null && this.tabControl.SelectedItem is TabItem)
                        {
                            tabItem = (TabItem)this.tabControl.SelectedItem;
                            //this.tabControl.DeselectTab(tabPage);

                            if (action.GetAttribute("AllowCloseAction") != null && action.GetAttribute("AllowCloseAction") == "Y")
                                try
                                {
                                    if (!(bool)(action).ControlAction(this, new AtomusControlArgs(e.Action, null)))
                                    {
                                        e.Value = false;
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
                                    e.Value = false;
                                    return;
                                }

                            this.tabControl.Items.Remove(tabItem);

                            if (action is System.Windows.Forms.UserControl)
                                (action as System.Windows.Forms.UserControl).Dispose();

                            e.Value = true;
                        }
                        else
                            this.Close();

                        break;

                    case "FullWindowsMode":
                        if (this.gridCenter.ColumnDefinitions[0].Width.Value <= this.gridSplitter.Width)
                            this.gridCenter.ColumnDefinitions[0].Width = new GridLength(oldGridSplitterWidth);
                        else
                        {
                            this.oldGridSplitterWidth = this.gridCenter.ColumnDefinitions[0].Width.Value;
                            this.gridCenter.ColumnDefinitions[0].Width = new GridLength(this.gridSplitter.Width);
                        }

                        if (this.gridCenter.ColumnDefinitions[0].Width.Value <= this.gridSplitter.Width)
                            this.gridCenter.ColumnDefinitions[0].Width = new GridLength(this.gridSplitter.Width);

                        break;

                    case "Form.Size":
                        this.Width = ((Size)e.Value).Width;
                        this.Height = ((Size)e.Value).Height;

                        ((UserControl)this.loginControl).Width = this.Width;
                        ((UserControl)this.loginControl).Height = this.Height;
                        break;

                    case "Join.Cancel":
                        this.SetLoginControl();
                        break;

                    default:
                        if (!e.Action.StartsWith("Action."))
                        {
                            if (!sender.Equals(this.toolbarControl))//toolbarControl 아니면?(기본 툴바 버튼이면)
                                action = (IAction)sender;

                            if (sender.GetAttribute("ASSEMBLY_ID") != null)
                            {
                                atomusControlEventArgs = new AtomusControlEventArgs("UserControl.AssemblyVersionCheck", null);
                                this.UserControl_AfterActionEventHandler(action, atomusControlEventArgs);

                                if ((bool)atomusControlEventArgs.Value)
                                {
                                    action.ControlAction(sender, e.Action, e.Value);
                                    return;
                                }
                            }
                            else
                            {
                                action.ControlAction(sender, e.Action, e.Value);
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        private async void MenuControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            object[] objects;
            IAction core;
            string MENU_ID;

            try
            {
                switch (e.Action)
                {
                    case "Menu.MenuFolding":
                        objects = (object[])e.Value;// true:Fold, else:Expand
                        switch((bool?)objects[0])
                        {
                            case true:
                                this.gridCenter.ColumnDefinitions[0].Width = new GridLength(62);
                                break;                            
                            default:
                                this.gridCenter.ColumnDefinitions[0].Width = new GridLength(290);
                                break;
                        }                        
                        break;

                    case "Menu.OpenControl":
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, _VisibleOne

                        if ((bool)objects[2])//_VisibleOne
                        {
                            foreach (TabItem tabItem in this.tabControl.Items)
                            {
                                if (tabItem.Tag == null)
                                    continue;

                                core = (IAction)tabItem.Tag;


                                MENU_ID = core.GetAttribute("MENU_ID");

                                if (MENU_ID != null)
                                    if (MENU_ID.Equals(objects[0].ToString()))
                                    {
                                        this.Tag = tabItem.Tag;
                                        this.tabControl.SelectedItem = tabItem;

                                        return;//기존 화면이 있으니 바로 빠져 나감
                                    }
                            }
                        }

                        core = await (this.DataContext as ViewModel.ModernBrowserViewModel).OpenControlAsyncProcess((decimal)objects[0], (decimal)objects[1]);

                        e.Value = core;

                        core.BeforeActionEventHandler += UserControl_BeforeActionEventHandler;
                        core.AfterActionEventHandler += UserControl_AfterActionEventHandler;

                        this.AddTabItem(core.GetAttribute("NAME"), core.GetAttribute("DESCRIPTION"), core);

                        break;

                    default:
                        throw new AtomusException((this).GetMessage("Common", "00047", "'{0}'은(는) 처리할 수 없는 {1} 입니다.").Message.Translate(e.Action, "Action"));
                        //throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        private void HomeControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            //throw new NotImplementedException();
        }


        private void UserControl_BeforeActionEventHandler(ICore sender, AtomusControlEventArgs e) { }
        private void UserControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            object[] objects;
            IAction core;
            AtomusControlEventArgs atomusControlEventArgs;

            try
            {
                switch (e.Action)
                {
                    case "UserControl.OpenControl":
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, AtomusControlArgs

                        core = (this.DataContext as ViewModel.ModernBrowserViewModel).OpenControlProcess((decimal)objects[0], (decimal)objects[1]);

                        e.Value = core;

                        core.BeforeActionEventHandler += UserControl_BeforeActionEventHandler;
                        core.AfterActionEventHandler += UserControl_AfterActionEventHandler;

                        this.AddTabItem(core.GetAttribute("NAME"), core.GetAttribute("DESCRIPTION"), core);

                        if (objects.Length >= 3 && objects[2] != null && objects[2] is AtomusControlEventArgs)
                        {
                            atomusControlEventArgs = objects[2] as AtomusControlEventArgs;
                            core.ControlAction(sender, atomusControlEventArgs.Action, atomusControlEventArgs.Value);
                        }

                        break;
                    case "UserControl.GetControl":
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, AtomusControlArgs

                        core = (this.DataContext as ViewModel.ModernBrowserViewModel).OpenControlProcess((decimal)objects[0], (decimal)objects[1]);

                        e.Value = core;

                        if (objects.Length >= 3 && objects[2] != null && objects[2] is AtomusControlEventArgs)
                        {
                            atomusControlEventArgs = objects[2] as AtomusControlEventArgs;
                            core.ControlAction(sender, atomusControlEventArgs.Action, atomusControlEventArgs.Value);
                        }

                        break;

                    //case "UserControl.AssemblyVersionCheck":
                    //    tmp = this.GetAttribute("ProcedureAssemblyVersionCheck");

                    //    if (tmp != null && tmp.Trim() != "")
                    //    {
                    //        response = this.AssemblyVersionCheck(sender);

                    //        if (response.Status != Service.Status.OK)
                    //        {
                    //            this.MessageBoxShow(this, response.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //            e.Value = false;
                    //        }
                    //        else
                    //            e.Value = true;
                    //    }
                    //    else
                    //        e.Value = true;

                    //    break;


                    //case "UserControl.Status":
                    //    objects = (object[])e.Value;//StatusBarInfomation1  Text

                    //    this.ribbon.Items[string.Format("RibbonStatusBar_{0}", objects[0])].Caption = (string)objects[1];

                    //    break;

                    //    //default:
                    //    //    throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception exception)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], exception);
            }
        }

        #endregion

        #region Event
        event AtomusControlEventHandler IAction.BeforeActionEventHandler
        {
            add
            {
                this.beforeActionEventHandler += value;
            }
            remove
            {
                this.beforeActionEventHandler -= value;
            }
        }
        event AtomusControlEventHandler IAction.AfterActionEventHandler
        {
            add
            {
                this.afterActionEventHandler += value;
            }
            remove
            {
                this.afterActionEventHandler -= value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
#if DEBUG
                DiagnosticsTool.MyDebug(string.Format("DefaultBrowser_Load(object sender = {0}, EventArgs e = {1})", (sender != null) ? sender.ToString() : "null", (e != null) ? e.ToString() : "null"));
#endif
                //this.Width = 0;
                //this.Height = 0;
                (this.DataContext as ViewModel.ModernBrowserViewModel).Title = Factory.FactoryConfig.GetAttribute("Atomus", "ServiceName");

            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
                Application.Current.Shutdown();
            }

            try
            {
                this.SetLoginControl();

                //System.Threading.Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
                Application.Current.Shutdown();
            }
        }


        private void DefaultBrowser_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(this.DataContext as ViewModel.ModernBrowserViewModel).IsShutdown)
            {
                e.Cancel = true;
                (this.DataContext as ViewModel.ModernBrowserViewModel).ShutdownCommand.Execute(null);
            }
        }

        private void DefaultBrowser_KeyDown1(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F4)
            {
                try
                {
                    this.ToolbarControl_AfterActionEventHandler(toolbarControl, new AtomusControlArgs("Close", null));
                    //((IAction)this).ControlAction(_ToolbarControl, ));
                }
                catch (Exception ex)
                {
                    (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
                }
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Tab && FocusManager.GetFocusedElement(this) != this.tabControl)
            {
                if (this.tabControl.SelectedIndex + 1 == this.tabControl.Items.Count)
                    this.tabControl.SelectedIndex = 0;
                else
                    this.tabControl.SelectedIndex += 1;
            }

#if DEBUG
            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D)
            {
                DiagnosticsTool.ShowForm();
            }
#endif

            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.T)
            {
                DiagnosticsTool.ShowForm();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tabControl;
            ICore core;
            object value;

            tabControl = (TabControl)sender;

            if (tabControl.SelectedItem == null)
                return;

            if (this.Tag != null && this.Tag.Equals((tabControl.SelectedItem as TabItem).Tag))
                return;

            if (this.Tag != null)
                this.toolbarControl.ControlAction((ICore)this.Tag, "UserToolbarButton.Remove", null);
            else
                this.toolbarControl.ControlAction((ICore)(tabControl.SelectedItem as TabItem).Tag, "UserToolbarButton.Remove", null);

            this.Tag = (tabControl.SelectedItem as TabItem).Tag;

            if (this.Tag != null && this.Tag is ICore)
            {
                core = (this.Tag as ICore);

                value = core.GetAttribute("Action.New");
                this.toolbarControl.ControlAction(core, "Action.New", value ?? "N");

                if (core is System.Windows.Forms.UserControl)
                {
                    value = core.GetAttribute("Action.Search");
                    this.toolbarControl.ControlAction(core, "Action.Search", value ?? "N");
                    value = core.GetAttribute("Action.Save");
                    this.toolbarControl.ControlAction(core, "Action.Save", value ?? "N");
                    value = core.GetAttribute("Action.Delete");
                    this.toolbarControl.ControlAction(core, "Action.Delete", value ?? "N");
                    value = core.GetAttribute("Action.Print");
                    this.toolbarControl.ControlAction(core, "Action.Print", value ?? "N");
                }
                else
                {
                    this.toolbarControl.ControlAction(core, "Action.Search", "N");
                    this.toolbarControl.ControlAction(core, "Action.Save", "N");
                    this.toolbarControl.ControlAction(core, "Action.Delete", "N");
                    this.toolbarControl.ControlAction(core, "Action.Print", "N");
                }
            }

            this.toolbarControl.ControlAction((ICore)this.Tag, "UserToolbarButton.Add", null);
        }
        #endregion

        #region ETC
        private void SetLoginControl()
        {
            UserControl userControl;

            try
            {
                if (this.loginControl == null)
                {
                    //this.loginControl = new Atomus.Windows.Controls.Login.ModernLogin();
                    this.loginControl = (IAction)(this).CreateInstance("LoginControl", false);

                    loginControl.AfterActionEventHandler += LoginControl_AfterActionEventHandler;

                    userControl = (UserControl)this.loginControl;

                    userControl.VerticalAlignment = VerticalAlignment.Stretch;
                    userControl.HorizontalAlignment = HorizontalAlignment.Stretch;

                    userControl.VerticalContentAlignment = VerticalAlignment.Stretch;
                    userControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                }
                else
                    userControl = (UserControl)this.loginControl;

                userControl.Visibility = Visibility.Visible;

                if (this.joinControl != null)
                    this.gridLayout.Children.Remove((UserControl)this.joinControl);
                this.gridLayout.Children.Remove(userControl);
                this.gridLayout.Children.Add(userControl);
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }
        private void SetJoinControl()
        {
            UserControl userControl;

            try
            {
                if (this.joinControl == null)
                {
                    //this.joinControl = new Atomus.Windows.Controls.Join.DefaultJoin();
                    this.joinControl = (IAction)(this).CreateInstance("JoinControl", false);

                    this.joinControl.AfterActionEventHandler += JoinControl_AfterActionEventHandler;

                    userControl = (UserControl)this.joinControl;

                    userControl.VerticalAlignment = VerticalAlignment.Stretch;
                    userControl.HorizontalAlignment = HorizontalAlignment.Stretch;

                    userControl.VerticalContentAlignment = VerticalAlignment.Stretch;
                    userControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                }
                else
                    userControl = (UserControl)this.joinControl;

                userControl.Visibility = Visibility.Visible;

                if (this.loginControl != null)
                    this.gridLayout.Children.Remove((UserControl)this.loginControl);
                this.gridLayout.Children.Remove(userControl);
                this.gridLayout.Children.Add(userControl);
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        private void SetBrowserViewer()
        {
            this.gridLayout.Children.Clear();

            this.dockPanel = new DockPanel()
            {
                LastChildFill = true,
                //Background = new SolidColorBrush(Color.FromRgb(0, 255, 0))
            };

            //this.RemoveLogicalChild(this.gridLayout);

            this.gridLayout.Children.Add(this.dockPanel);

            //try
            //{
            //    this.browserViewer = new UserControl
            //    {
            //        Dock = DockStyle.Fill,
            //        BackColor = Color.Transparent
            //    };
            //    this.Controls.Add(this.browserViewer);
            //}
            //catch (Exception exception)
            //{
            //    this.MessageBoxShow(this, exception);
            //}
        }

        private void SetToolbar()
        {
            UserControl userControl;

            try
            {
                if (this.toolbarControl == null)
                {
                    //this.toolbarControl = new Atomus.Windows.Controls.Toolbar.DefaultToolbar();
                    //this.toolbarControl = new Atomus.Windows.Controls.Toolbar.ModernToolbar();
                    this.toolbarControl = (IAction)(this).CreateInstance("ToolbarControl", false);

                    this.toolbarControl.AfterActionEventHandler += ToolbarControl_AfterActionEventHandler; ;

                    userControl = (UserControl)this.toolbarControl;

                    userControl.VerticalAlignment = VerticalAlignment.Top;
                    userControl.HorizontalAlignment = HorizontalAlignment.Stretch;

                    userControl.VerticalContentAlignment = VerticalAlignment.Stretch;
                    userControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                }
                else
                    userControl = (UserControl)this.toolbarControl;

                userControl.Visibility = Visibility.Visible;

                DockPanel.SetDock(userControl, Dock.Top);

                this.dockPanel.Children.Add(userControl);

                //this.Background = (Brush)this.toolbarControl.ControlAction(this, "Toolbar.Background");
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        private void SetCenter()
        {
            try
            {
                if (this.gridCenter == null)
                {
                    this.gridCenter = new Grid();
                    this.gridCenter.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(this.Width * 0.15)
                    }
                    );
                    this.gridCenter.ColumnDefinitions.Add(new ColumnDefinition());
                }

                this.gridCenter.Visibility = Visibility.Visible;

                DockPanel.SetDock(this.gridCenter, Dock.Left);

                this.dockPanel.Children.Add(this.gridCenter);
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }


        private void SetMenu()
        {
            UserControl userControl;

            try
            {
                if (this.menuControl == null)
                {
                    //this.menuControl = new Atomus.Windows.Controls.Menu.DefaultMenu();
                    //this.menuControl = new Atomus.Windows.Controls.Menu.ModernMenu();
                    this.menuControl = (IAction)(this).CreateInstance("MenuControl", false);

                    this.menuControl.AfterActionEventHandler += MenuControl_AfterActionEventHandler;

                    userControl = (UserControl)this.menuControl;

                    userControl.VerticalAlignment = VerticalAlignment.Stretch;
                    userControl.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
                else
                    userControl = (UserControl)this.menuControl;

                userControl.Visibility = Visibility.Visible;

                //DockPanel.SetDock(userControl, Dock.Left);

                this.gridCenter.Children.Add(userControl);

                Grid.SetColumn(userControl, 0);
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        private void SetTabControl()
        {
            try
            {
                if (this.tabControl == null)
                {
                    //this.tabControl = new TabControl()
                    //{
                    //    Padding = new Thickness(0),
                    //    Margin = new Thickness(0),
                    //    BorderThickness = new Thickness(0, 0, 0, 0)
                    //};

                    //this.tabControl.SelectionChanged += TabControl_SelectionChanged;
                }

                this.tabControl.SelectionChanged += TabControl_SelectionChanged;

                this.tabControl.Visibility = Visibility.Visible;

                //DockPanel.SetDock(this.tabControl, Dock.Right);

                this.gridCenter.Children.Add(this.tabControl);

                Grid.SetColumn(this.tabControl, 1);
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        private void AddHomeControl()
        {
            UserControl userControl;

            try
            {
                if (this.homeControl == null)
                {
                    //this.homeControl = new Atomus.Windows.Controls.Home.DefaultHome();
                    this.homeControl = (IAction)(this).CreateInstance("HomeControl", false);

                    this.homeControl.AfterActionEventHandler += HomeControl_AfterActionEventHandler;

                    userControl = (UserControl)this.homeControl;

                    userControl.VerticalAlignment = VerticalAlignment.Stretch;
                    userControl.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
                else
                    userControl = (UserControl)this.homeControl;

                userControl.Visibility = Visibility.Visible;

                this.AddTabItem("Home", null, userControl);
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }
        private void AddTabItem(string name, string description, object userControl)
        {
            TabItem tabItem;
            System.Windows.Forms.Integration.WindowsFormsHost host;

            try
            {
                tabItem = new TabItem()
                {
                    Header = new Label()
                    {
                        Content = name,
                        ToolTip = description ?? name
                    }
                };

                if (userControl == null)
                    tabItem.Content = new TextBox();
                else
                {
                    if (userControl is System.Windows.Forms.UserControl)
                    {
                        host = new System.Windows.Forms.Integration.WindowsFormsHost()
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        };

                        host.Child = (System.Windows.Forms.UserControl)userControl;

                        tabItem.Content = host;
                    }
                    else
                        tabItem.Content = userControl;
                }

                tabItem.Tag = userControl;

                this.tabControl.Items.Add(tabItem);

                this.tabControl.SelectedItem = tabItem;

            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        private void SetStatusBar()
        {
            StatusBarItem statusBarItem;

            try
            {
                if (this.statusBar == null)
                {
                    this.statusBar = new StatusBar();
                    //statusBar.BorderBrush = (Brush)this.toolbarControl.ControlAction(this, "Toolbar.BorderBrush");
                    //statusBar.BorderThickness = (Thickness)this.toolbarControl.ControlAction(this, "Toolbar.BorderThickness");

                    statusBarItem = new StatusBarItem();

                    statusBarItem.Content = new TextBlock()
                    {
                        Text = "Text1",
                        Background = this.Background
                    };

                    this.statusBar.Items.Add(statusBarItem);
                }


                DockPanel.SetDock(statusBar, Dock.Bottom);

                this.dockPanel.Children.Add(statusBar);
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem1;
            TabItem tabItem2;

            try
            {
                tabItem1 = sender as TabItem;
                tabItem2 = null;

                foreach (Button button in tabItem1.FindVisualChildren<Button>())
                {
                    if (e.OriginalSource.Equals(button))
                    {

                        if (!tabItem1.IsSelected)
                            tabItem2 = this.tabControl.SelectedItem as TabItem;

                        tabItem1.IsSelected = true;
                        e.Handled = true;

                        this.ToolbarControl_AfterActionEventHandler(toolbarControl, new AtomusControlArgs("Close", null));

                        if (tabItem2 != null)
                            tabItem2.IsSelected = true;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                (this).WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }
    }
}
