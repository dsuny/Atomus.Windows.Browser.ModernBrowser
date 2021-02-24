using Atomus.Control;
using Atomus.Diagnostics;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Atomus.Windows.Browser
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModernApp : Application, IAction
    {
        public ModernApp()
        {
            this.InitializeComponent();
        }

        event AtomusControlEventHandler IAction.BeforeActionEventHandler
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
        event AtomusControlEventHandler IAction.AfterActionEventHandler
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        object IAction.ControlAction(ICore sender, AtomusControlArgs e)
        {
            //this.InitializeComponent();
            return true;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(sender as Button).WindowState = WindowState.Minimized;
        }
        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(sender as Button).WindowState = WindowState.Normal;
        }
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(sender as Button).WindowState = WindowState.Maximized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(sender as Button).Close();
        }
    }
}
