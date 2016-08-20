﻿using ChessEngineClient.ViewModel;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ChessEngineClient.View
{
    public sealed partial class SquareView : UserControl
    {
        public SquareView()
        {
            this.InitializeComponent();

            this.PointerPressed += OnSquareViewPointerPressed;
        }

        private void OnSquareViewPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Messenger.Default.Send((SquareViewModel)DataContext, NotificationMessages.SquarePressed);
        }
    }
}
