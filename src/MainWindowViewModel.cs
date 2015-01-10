/*******************************************************************************************************************************
 * AK.Scribe.MainWindowViewModel
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
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

#endregion

namespace AK.Scribe
{
    /// <summary>
    /// View model that supports the main screen.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly string filePath;
        private string textContent;
        private bool isDirty;
        private DateTime? lastModified;
        private Visibility visibleIfDirty;
        private Visibility hiddenIfDirty;
        private string lastModifiedAsString;

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(string filePath)
        {
            this.filePath = filePath;

            var fileTitle = Path.GetFileNameWithoutExtension(filePath);
            fileTitle = fileTitle != null ? fileTitle.Replace('_', ' ') : "Untitled";
            if (fileTitle.Length > 37) fileTitle = fileTitle.Substring(0, 34) + "...";
            this.FileTitle = fileTitle;

            this.WindowTitle = string.Format("Scribe - [{0}]", Path.GetFileName(filePath));
            this.TextContent = File.ReadAllText(filePath);
            this.IsDirty = false;
        }

        public string WindowTitle { get; private set; }
        public string FileTitle { get; private set; }

        public string TextContent
        {
            get { return this.textContent; }
            set
            {
                this.textContent = value;
                this.OnPropertyChanged();
                this.IsDirty = true;
            }
        }

        public bool IsDirty
        {
            get { return this.isDirty; }
            set
            {
                this.isDirty = value;
                this.OnPropertyChanged();

                if (!this.isDirty)
                    this.LastModified = File.GetLastWriteTime(this.filePath);

                this.VisibleIfDirty = this.isDirty ? Visibility.Visible : Visibility.Hidden;
                this.HiddenIfDirty = this.isDirty ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public string LastModifiedAsString
        {
            get { return this.lastModifiedAsString; }
            set
            {
                this.lastModifiedAsString = value;
                this.OnPropertyChanged();
            }
        }

        public DateTime? LastModified
        {
            get { return this.lastModified; }
            set
            {
                this.lastModified = value;
                this.OnPropertyChanged();

                this.LastModifiedAsString = "Last Saved: " + Environment.NewLine +
                                            (this.lastModified.HasValue ? this.lastModified.ToString() : "Never");
            }
        }

        public Visibility VisibleIfDirty
        {
            get { return this.visibleIfDirty; }
            set
            {
                this.visibleIfDirty = value;
                this.OnPropertyChanged();
            }
        }

        public Visibility HiddenIfDirty
        {
            get { return this.hiddenIfDirty; }
            set
            {
                this.hiddenIfDirty = value;
                this.OnPropertyChanged();
            }
        }

        public void Save()
        {
            File.WriteAllText(this.filePath, this.TextContent);
            this.IsDirty = false;
        }

        public void Reload()
        {
            this.TextContent = File.ReadAllText(this.filePath);
            this.IsDirty = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}