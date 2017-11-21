// ****************************************************************************
// <copyright file="NavigationService.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2014
// </copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>02.10.2014</date>
// <project>GalaSoft.MvvmLight</project>
// <web>http://www.mvvmlight.net</web>
// <license>
// See license.txt in this solution or http://www.galasoft.ch/license_MIT.txt
// </license>
// ****************************************************************************
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Framework.Platform.UWP
{
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();
        private Frame frame = null;
        private string currentPageKey = null;

        /// <summary>
        /// The key corresponding to the currently displayed page.
        /// </summary>
        public string CurrentPageKey
        {
            get { return currentPageKey; }
            private set
            {
                currentPageKey = value;
                OnCurrentPageChanged(EventArgs.Empty);
            }
        }

        public event EventHandler CurrentPageChanged;

        public NavigationService(Frame frame)
        {
            this.frame = frame;
        }

        /// <summary>
        /// If possible, discards the current page and displays the previous page
        /// on the navigation stack.
        /// </summary>
        public void GoBack()
        {
            if (frame.CanGoBack)
            {
                NotifyOnNavigatingFrom();
                
                frame.GoBack();
                lock (_pagesByKey)
                    CurrentPageKey = _pagesByKey.FirstOrDefault(k => k.Value == frame.Content.GetType()).Key;

                NotifyOnNavigatedTo(null);
            }
        }

        private void NotifyOnNavigatingFrom()
        {
            FrameworkElement contentElement = frame.Content as FrameworkElement;
            if (contentElement == null)
                return;

            INavigationAware navigationAwareContext = contentElement.DataContext as INavigationAware;
            if (navigationAwareContext != null)
                navigationAwareContext.OnNavigatingFrom();
        }

        private void NotifyOnNavigatedTo(object parameter)
        {
            FrameworkElement contentElement = frame.Content as FrameworkElement;
            if (contentElement == null)
                return;

            INavigationAware navigationAwareContext = contentElement.DataContext as INavigationAware;
            if (navigationAwareContext != null)
                navigationAwareContext.OnNavigatedTo(parameter);
        }

        /// <summary>
        /// Displays a new page corresponding to the given key. 
        /// Make sure to call the <see cref="Configure"/>
        /// method first.
        /// </summary>
        /// <param name="pageKey">The key corresponding to the page
        /// that should be displayed.</param>
        /// <exception cref="ArgumentException">When this method is called for 
        /// a key that has not been configured earlier.</exception>
        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        /// <summary>
        /// Displays a new page corresponding to the given key,
        /// and passes a parameter to the new page.
        /// Make sure to call the <see cref="Configure"/>
        /// method first.
        /// </summary>
        /// <param name="pageKey">The key corresponding to the page
        /// that should be displayed.</param>
        /// <param name="parameter">The parameter that should be passed
        /// to the new page.</param>
        /// <exception cref="ArgumentException">When this method is called for 
        /// a key that has not been configured earlier.</exception>
        public virtual void NavigateTo(string pageKey, object parameter)
        {
            lock (_pagesByKey)
            {
                if (!_pagesByKey.ContainsKey(pageKey))
                {
                    throw new ArgumentException(
                        string.Format(
                            "No such page: {0}. Did you forget to call NavigationService.Configure?",
                            pageKey),
                        "pageKey");
                }

                NotifyOnNavigatingFrom();

                frame.Navigate(_pagesByKey[pageKey], parameter);
                CurrentPageKey = pageKey;

                NotifyOnNavigatedTo(parameter);
            }
        }

        /// <summary>
        /// Adds a key/page pair to the navigation service.
        /// </summary>
        /// <param name="key">The key that will be used later
        /// in the <see cref="NavigateTo(string)"/> or <see cref="NavigateTo(string, object)"/> methods.</param>
        /// <param name="pageType">The type of the page corresponding to the key.</param>
        public void Configure(string key, Type pageType)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(key))
                {
                    _pagesByKey[key] = pageType;
                }
                else
                {
                    _pagesByKey.Add(key, pageType);
                }
            }
        }

        protected virtual void OnCurrentPageChanged(EventArgs e)
        {
            CurrentPageChanged?.Invoke(this, e);
        }
    }
}
