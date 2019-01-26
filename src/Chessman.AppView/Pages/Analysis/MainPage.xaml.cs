using ChessEngine;
using Chessman;
using Chessman.ViewModel;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Chessman.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += OnMainPageLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Window.Current.CoreWindow.KeyDown += OnCoreWindowKeyDown;
            chessBoard.RegisterAnimationHandlers();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Window.Current.CoreWindow.KeyDown -= OnCoreWindowKeyDown;
            chessBoard.UnRegisterAnimationHandlers();
        }

        private void OnCoreWindowKeyDown(CoreWindow sender, KeyEventArgs e)
        {
            if (e.Handled)
                return;

            var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            if (ctrl.HasFlag(CoreVirtualKeyStates.Down) && e.VirtualKey == VirtualKey.V)
            {
                (this.DataContext as MainViewModel).LoadFromClipboardCommand.Execute(null);
            }
            else if (e.VirtualKey == VirtualKey.Left)
            {
                (this.DataContext as MainViewModel).GoBackCommand.Execute(null);
            }
            else if (e.VirtualKey == VirtualKey.Right)
            {
                (this.DataContext as MainViewModel).GoForwardCommand.Execute(null);
            }
        }

        private void OnMainPageLoaded(object sender, RoutedEventArgs e)
        {
            // When changing the current page the PageSizeChanged event
            // is raised before layout is performed
            // UpdateColumnsRestraints needs AdaptiveTrigger for windows size to be applied
            UpdateColumnsRestraints(RenderSize);

            GetUserCollection();
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnsRestraints(e.NewSize);
        }

        private void UpdateColumnsRestraints(Size newSize)
        {
            // for now there is no other way to enforce a min width on the right column
            // for pc layout that can be honored by the parent pannel
            if (analysisView.MinWidth > 0)
                chessBoardPanel.MaxWidth = newSize.Width - analysisView.MinWidth - 50 - adBanner.Width;
            else
                chessBoardPanel.MaxWidth = Double.PositiveInfinity;

            if (notationView.MinHeight > 0)
                chessBoardPanel.MaxHeight = newSize.Height - notationView.MinHeight - 130;
            else
                chessBoardPanel.MaxHeight = Double.PositiveInfinity;
        }

        private void OnCommandBarClosed(object sender, object e)
        {
            // get rid of the command bar being keyboard focused
            this.Focus(FocusState.Programmatic);
        }

        private void BoardSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // needs manual update since ActualHeight doesn't notifie changes for bonding
            evaluationBar.Height = chessBoard.ActualHeight;
        }

        private StoreContext context = null;

        public async void GetUserCollection()
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
                // If your app is a desktop app that uses the Desktop Bridge, you
                // may need additional code to configure the StoreContext object.
                // For more info, see https://aka.ms/storecontext-for-desktop.
            }

            // Specify the kinds of add-ons to retrieve.
            string[] productKinds = { "Durable" };
            List<String> filterList = new List<string>(productKinds);

            //workingProgressRing.IsActive = true;
            StoreProductQueryResult queryResult = await context.GetUserCollectionAsync(filterList);
            //workingProgressRing.IsActive = false;

            if (queryResult.ExtendedError != null)
            {

                //foreach (var product in queryResult.Products)
                //{
                //    System.Diagnostics.Debug.WriteLine(product.Value);
                //}


                // The user may be offline or there might be some other server failure.
                //textBlock.Text = $"ExtendedError: {queryResult.ExtendedError.Message}";
                return;
            }

            System.Diagnostics.Debug.WriteLine("product.Value");

            foreach (KeyValuePair<string, StoreProduct> item in queryResult.Products)
            {
                StoreProduct product = item.Value;
                System.Diagnostics.Debug.WriteLine(product);

                // Use members of the product object to access info for the product...
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string addOnStoreId = "9N8DMLL70VKF";

            var context2 = StoreContext.GetDefault();

            await context2.RequestPurchaseAsync(addOnStoreId);
        }
    }
}
