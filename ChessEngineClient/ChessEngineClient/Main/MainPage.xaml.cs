using ChessEngine;
using ChessEngineClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

namespace ChessEngineClient.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Sample engine usage //
        async void foo()
        {
            try
            {
                EngineNotifications notificationHandler = new EngineNotifications();
                ChessBoard board = new ChessBoard();
                board.Initialize();

                String testFEN = "rnbqkbnr/pppp1ppp/8/8/3pP3/2P5/PP3PPP/RNBQKBNR b KQkq - 0 3";
                testFEN = "rnbqkbnr/pp1ppppp/2p5/8/3P4/2P5/PP2PPPP/RNBQKBNR b KQkq d3";
                //board.LoadFrom(testFEN, 0);

                ChessEngine.Engine engine = new ChessEngine.Engine(notificationHandler);
                engine.Start();
                engine.Analyze(board);
                await Task.Delay(TimeSpan.FromSeconds(1000));
            }
            catch (Exception e)
            {

            }
        }
        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager navigation = SystemNavigationManager.GetForCurrentView();
            navigation.BackRequested += OnBackExecuted;
            foo();
        }

        private async void OnBackExecuted(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            var dialog = new MessageDialog("Are you sure you want to exit?");
            dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });

            var result = await dialog.ShowAsync();

            if ((int)result.Id == 0)
                Application.Current.Exit();
        }
    }
}
