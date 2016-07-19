﻿using ChessEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        // Sample usage the Chess Engine //

        //void foo()
        //{
        //    // Sample engine usage //
        //    try
        //    {
        //        EngineNotifications notificationHandler = new EngineNotifications();
        //        ChessBoard board = new ChessBoard();
        //        board.Initialize();
        //        ChessEngine.Engine engine = new ChessEngine.Engine(notificationHandler);
        //        engine.Start();
        //        //engine.Analyze(board);
        //    }
        //    catch( Exception e)
        //    {

        //    }
        //}

        public MainPage()
        {
            // Sample engine usage //
            try
            {
                EngineNotifications notificationHandler = new EngineNotifications();
                ChessBoard board = new ChessBoard();
                board.Initialize();
                ChessEngine.Engine engine = new ChessEngine.Engine(notificationHandler);
                engine.Start();
                engine.Analyze(board);
            }
            catch( Exception e)
            {

            var result = await dialog.ShowAsync();

            if ((int)result.Id == 0)
                Application.Current.Exit();
        }
    }
}
    