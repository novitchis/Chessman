using Chessman.ViewModel;
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

namespace Chessman.View
{
    public sealed partial class SquareView : UserControl
    {
        public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.Register("IsDropTarget", typeof(bool), typeof(SquareView), new PropertyMetadata(false, OnVisualStatePropertyChanged));
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(SquareView), new PropertyMetadata(false, OnVisualStatePropertyChanged));

        public bool IsDropTarget
        {
            get { return (bool)GetValue(IsDropTargetProperty); }
            set { SetValue(IsDropTargetProperty, value); }
        }

        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public SquareView()
        {
            this.InitializeComponent();
            this.PointerPressed += OnSquareViewPointerPressed;
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // this is done manually, a grid will degrade performance on layout/render 
            ellipseMark.Width = this.ActualWidth / 3;
            ellipseMark.Height = this.ActualHeight / 3;
        }

        private void OnSquareViewPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Messenger.Default.Send((SquareViewModel)DataContext, NotificationMessages.SquarePressed);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // I don't know why this is required
            base.MeasureOverride(availableSize);
            return availableSize;
        }

        private void RefreshVisualState()
        {
            if (IsDropTarget || IsHighlighted)
                VisualStateManager.GoToState(this, "Highlighted", true);
            else
                VisualStateManager.GoToState(this, "DefaultState", true);
        }

        private static void OnVisualStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SquareView)d).RefreshVisualState();
        }
    }
}
