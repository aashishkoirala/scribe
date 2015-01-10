/*******************************************************************************************************************************
 * AK.Scribe.MainWindow
 * Copyright © 2015 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of SCRIBE.
 *  
 * SCRIBE is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SCRIBE is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with SCRIBE.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using System;
using System.Linq;
using System.Windows;

#endregion

namespace AK.Scribe
{
    /// <summary>
    /// Code-behind for the main window, as well as the main entry point for the application.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public partial class MainWindow
    {
        private readonly MainWindowViewModel viewModel;
        private bool lastSaveHadError;

        [STAThread]
        public static void Main(string[] args)
        {
            new Application {StartupUri = new Uri("MainWindow.xaml", UriKind.Relative)}.Run();
        }

        public MainWindow()
        {
            try
            {
                this.InitializeComponent();

                var filePath = string.Empty;
                var args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                    filePath = string.Join(" ", args.Skip(1).ToArray()).Trim();

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    MessageBox.Show(this, "Please specify a file to load in the command line.", "No File Specified",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    Application.Current.Shutdown();
                    return;
                }

                this.viewModel = new MainWindowViewModel(filePath);
                this.DataContext = this.viewModel;
            }
            catch (Exception ex)
            {
                var message = string.Format("Oops, something went wrong while loading. The system says: {0}.",
                                            ex.Message);
                MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void OnMainWindowActivated(object sender, EventArgs e)
        {
            this.MainTextBox.Focus();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            this.lastSaveHadError = true;
            try
            {
                this.viewModel.Save();
                this.lastSaveHadError = false;
            }
            catch (Exception ex)
            {
                var message = string.Format("Oops, something went wrong while saving. The system says: {0}.", ex.Message);
                MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnRevert(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(this, "You will lose all unsaved changes. Are you sure?", "Revert",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No) return;

            try
            {
                this.viewModel.Reload();
            }
            catch (Exception ex)
            {
                var message = string.Format("Oops, something went wrong while reverting. The system says: {0}.",
                                            ex.Message);
                MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnQuit(object sender, RoutedEventArgs e)
        {
            if (this.viewModel.IsDirty)
            {
                var result = MessageBox.Show(this, "You have unsaved changes. Do you want to save them before you quit?",
                                             "Quit", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes)
                {
                    this.OnSave(sender, e);
                    if (this.lastSaveHadError) return;
                }
            }

            Application.Current.Shutdown();
        }
    }
}