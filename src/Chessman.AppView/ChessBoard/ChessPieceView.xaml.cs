﻿using System;
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

namespace Chessman.View
{
    public sealed partial class ChessPieceView : UserControl
    {
        public static readonly DependencyProperty IsBeingDraggedProperty = DependencyProperty.Register("IsBeingDragged", typeof(bool), typeof(ChessPieceView), new PropertyMetadata(false));

        public bool IsBeingDragged
        {
            get { return (bool)GetValue(IsBeingDraggedProperty); }
            set { SetValue(IsBeingDraggedProperty, value); }
        }

        public ChessPieceView()
        {
            this.InitializeComponent();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // I don't know why this is required
            base.MeasureOverride(availableSize);
            return availableSize;
        }
    }
}
