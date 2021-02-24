using Atomus.Control;
using Atomus.Diagnostics;
using Atomus.Windows.Browser.Controllers;
using Atomus.Windows.Browser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Atomus.Windows.Browser.ViewModel
{
    public class ModernBrowserViewModel : Atomus.MVVM.ViewModel
    {
        #region Declare
        private bool showInTaskbar;
        //private string backColor;
        //private string foreColor;
        private ResizeMode resizeMode;
        private bool topmost;

        private double opacity;

        private string title;

        private bool isShutdown;
        private bool isEnabledControl;
        #endregion

        #region Property
        public ICore Core { get; set; }

        public bool ShowInTaskbar
        {
            get
            {
                return this.showInTaskbar;
            }
            set
            {
                if (this.showInTaskbar != value)
                {
                    this.showInTaskbar = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public ResizeMode ResizeMode
        {
            get
            {
                return this.resizeMode;
            }
            set
            {
                if (this.resizeMode != value)
                {
                    this.resizeMode = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public bool Topmost
        {
            get
            {
                return this.topmost;
            }
            set
            {
                if (this.topmost != value)
                {
                    this.topmost = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public double Opacity
        {
            get
            {
                return this.opacity;
            }
            set
            {
                if (this.opacity != value)
                {
                    this.opacity = value;
                    this.NotifyPropertyChanged();
                }
            }
        }


        public string UserID
        {
            get
            {
                return string.Format("ID : {0}", Config.Client.GetAttribute("Account.EMAIL"));
                //return (string)Config.Client.GetAttribute("Account.USER_ID");
            }
            set
            {
                this.NotifyPropertyChanged();
            }
        }
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsShutdown
        {
            get
            {
                return this.isShutdown;
            }
            set
            {
                if (this.isShutdown != value)
                {
                    this.isShutdown = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public bool IsEnabledControl
        {
            get
            {
                return this.isEnabledControl;
            }
            set
            {
                if (this.isEnabledControl != value)
                {
                    this.isEnabledControl = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ImageSource ICon
        {
            get
            {
                return (this.Core as Window).Icon;
            }
            set
            {
                if ((this.Core as Window).Icon != value)
                {
                    (this.Core as Window).Icon = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ICommand ShutdownCommand { get; set; }
        public ICommand OpenControlCommand { get; set; }
        #endregion

        #region INIT
        public ModernBrowserViewModel()
        {
            this.ShutdownCommand = new MVVM.DelegateCommand(async () => { await this.ShutdownProcess(); }
                                                            , () => { return !this.isShutdown; });

            this.OpenControlCommand = new MVVM.DelegateCommand(async () => { await this.OpenControlAsyncProcess(-1, -1); }
                                                            , () => { return this.IsEnabledControl; });
        }
        public ModernBrowserViewModel(ICore core) : this()
        {
            this.Core = core;

            this.showInTaskbar = true;

            this.resizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            this.topmost = false;

            this.opacity = 0D;

            this.GetActivationUri();
        }
        #endregion

        #region IO
        private async Task ShutdownProcess()
        {
            try
            {
                (this.ShutdownCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();

                this.isShutdown = false;

                if (this.Core.WindowsMessageBoxShow(Application.Current.Windows[0], "종료 하시겠습니까?", "종료", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    this.isShutdown = true;
                    Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
            finally
            {
                (this.ShutdownCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();
            }
        }

        internal async Task<IAction> OpenControlAsyncProcess(decimal MENU_ID, decimal ASSEMBLY_ID)
        {
            Service.IResponse result;
            IAction _Core;

            try
            {
                this.IsEnabledControl = false;
                (this.OpenControlCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();

                if (MENU_ID < 1) return null;
                if (ASSEMBLY_ID < 1) return null;

                result = await this.Core.SearchOpenControlAsync(new ModernBrowserSearchModel()
                {
                    MENU_ID = MENU_ID,
                    ASSEMBLY_ID = ASSEMBLY_ID
                });

                if (result.Status == Service.Status.OK)
                {
                    if (result.DataSet.Tables.Count == 2)
                        if (result.DataSet.Tables[0].Rows.Count == 1)
                        {
                            if (result.DataSet.Tables[0].Columns.Contains("FILE_TEXT") && result.DataSet.Tables[0].Rows[0]["FILE_TEXT"] != DBNull.Value)
                                _Core = (IAction)Factory.CreateInstance(Convert.FromBase64String((string)result.DataSet.Tables[0].Rows[0]["FILE_TEXT"]), result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);
                            else
                                _Core = (IAction)Factory.CreateInstance((byte[])result.DataSet.Tables[0].Rows[0]["FILE"], result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);

                            _Core.SetAttribute("MENU_ID", MENU_ID.ToString());
                            _Core.SetAttribute("ASSEMBLY_ID", ASSEMBLY_ID.ToString());

                            foreach (DataRow _DataRow in result.DataSet.Tables[1].Rows)
                            {
                                _Core.SetAttribute(_DataRow["ATTRIBUTE_NAME"].ToString(), _DataRow["ATTRIBUTE_VALUE"].ToString());
                            }

                            _Core.SetAttribute("NAME", result.DataSet.Tables[0].Rows[0]["NAME"].ToString().Translate());
                            _Core.SetAttribute("DESCRIPTION", string.Format("{0} {1}", (result.DataSet.Tables[0].Rows[0]["DESCRIPTION"] as string).Translate(), _Core.GetType().Assembly.GetName().Version.ToString()));

                            return _Core;
                        }

                    return null;
                }
                else
                {
                    this.WindowsMessageBoxShow(Application.Current.Windows[0], result.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

            }
            catch (Exception ex)
            {
                this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
                return null;
            }
            finally
            {
                this.IsEnabledControl = true;
                (this.OpenControlCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();
            }
        }
        internal IAction OpenControlProcess(decimal MENU_ID, decimal ASSEMBLY_ID)
        {
            Service.IResponse result;
            IAction _Core;

            try
            {
                this.IsEnabledControl = false;
                (this.OpenControlCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();

                if (MENU_ID < 1) return null;
                if (ASSEMBLY_ID < 1) return null;

                result = this.Core.SearchOpenControl(new ModernBrowserSearchModel()
                {
                    MENU_ID = MENU_ID,
                    ASSEMBLY_ID = ASSEMBLY_ID
                });

                if (result.Status == Service.Status.OK)
                {
                    if (result.DataSet.Tables.Count == 2)
                        if (result.DataSet.Tables[0].Rows.Count == 1)
                        {
                            if (result.DataSet.Tables[0].Columns.Contains("FILE_TEXT") && result.DataSet.Tables[0].Rows[0]["FILE_TEXT"] != DBNull.Value)
                                _Core = (IAction)Factory.CreateInstance(Convert.FromBase64String((string)result.DataSet.Tables[0].Rows[0]["FILE_TEXT"]), result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);
                            else
                                _Core = (IAction)Factory.CreateInstance((byte[])result.DataSet.Tables[0].Rows[0]["FILE"], result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);

                            _Core.SetAttribute("MENU_ID", MENU_ID.ToString());
                            _Core.SetAttribute("ASSEMBLY_ID", ASSEMBLY_ID.ToString());

                            foreach (DataRow _DataRow in result.DataSet.Tables[1].Rows)
                            {
                                _Core.SetAttribute(_DataRow["ATTRIBUTE_NAME"].ToString(), _DataRow["ATTRIBUTE_VALUE"].ToString());
                            }

                            _Core.SetAttribute("NAME", result.DataSet.Tables[0].Rows[0]["NAME"].ToString().Translate());
                            _Core.SetAttribute("DESCRIPTION", string.Format("{0} {1}", (result.DataSet.Tables[0].Rows[0]["DESCRIPTION"] as string).Translate(), _Core.GetType().Assembly.GetName().Version.ToString()));

                            return _Core;
                        }

                    return null;
                }
                else
                {
                    this.WindowsMessageBoxShow(Application.Current.Windows[0], result.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

            }
            catch (Exception ex)
            {
                this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
                return null;
            }
            finally
            {
                this.IsEnabledControl = true;
                (this.OpenControlCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region ETC
        private void GetActivationUri()
        {
            string tmp;
            string[] tmps;
            string[] tmps1;

            try
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    tmp = ApplicationDeployment.CurrentDeployment.ActivationUri.Query;

                    if (!tmp.IsNullOrEmpty() && tmp.Contains("?"))
                    {
                        tmps = tmp.Substring(tmp.IndexOf('?') + 1).Split('&');

                        foreach (string value in tmps)
                            if (value.Contains("="))
                            {
                                tmps1 = value.Split('=');

                                if (tmps1.Length > 1)
                                    Config.Client.SetAttribute(string.Format("UriParameter.{0}", tmps1[0]), tmps1[1]);
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
            }
        }
        #endregion
    }
}