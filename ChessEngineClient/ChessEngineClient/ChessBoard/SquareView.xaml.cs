using ChessEngineClient.ViewModel;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
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
        public static readonly DependencyProperty IsPieceDraggedProperty = DependencyProperty.Register("IsPieceDragged", typeof(bool), typeof(SquareView), new PropertyMetadata(false, OnVisualStatePropertyChanged));
        public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.Register("IsDropTarget", typeof(bool), typeof(SquareView), new PropertyMetadata(false, OnVisualStatePropertyChanged));

        public bool IsDropTarget
        {
            get { return (bool)GetValue(IsDropTargetProperty); }
            set { SetValue(IsDropTargetProperty, value); }
        }

        public bool IsPieceDragged
        {
            get { return (bool)GetValue(IsPieceDraggedProperty); }
            set { SetValue(IsPieceDraggedProperty, value); }
        }

        public SquareView()
        {
            this.InitializeComponent();
            this.PointerPressed += OnSquareViewPointerPressed;
        }

        private void OnSquareViewPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Messenger.Default.Send((SquareViewModel)DataContext, NotificationMessages.SquarePressed);
        }

        private void RefreshVisualState()
        {
            if (IsPieceDragged)
            {
                VisualStateManager.GoToState(this, "IsDragSourceState", true);
            }
            else if (IsDropTarget)
            {
                VisualStateManager.GoToState(this, "IsDropTargetState", true);
                highlightBorder.Width = this.ActualWidth;
                highlightBorder.Height = this.ActualHeight;

                //PointerPoint pointerPoint = PointerPoint.GetCurrentPoint()

                //IReadOnlyList<PointerDevice> pointerDevices = PointerDevice.GetPointerDevices();
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultState", true);
            }
        }

        private static void OnVisualStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SquareView)d).RefreshVisualState();
        }
    }
}
