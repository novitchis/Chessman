﻿using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessEngineClient.ViewModel
{
    public class ChessBoardViewModel : ViewModelBase
    {
        public static int[] RankNumbersAsWhite = new[] { 8, 7, 6, 5, 4, 3, 2, 1 };
        public static char[] FieldLettersAsWhite = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

        private IBasicBoardService basicBoardService = null;
        private List<SquareViewModel> squares = null;
        private SquareViewModel selectedSquare = null;
        private SideColor perspective = SideColor.White;
        private int[] rankNumbers = RankNumbersAsWhite;
        private char[] fieldLetters = FieldLettersAsWhite;

        #region "Properties"

        public List<SquareViewModel> Squares
        {
            get { return squares; }
            set
            {
                if (squares != value)
                {
                    squares = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public SquareViewModel SelectedSquare
        {
            get { return selectedSquare; }
            set
            {
                if (selectedSquare != value)
                {
                    OnSelectionChanged(selectedSquare, value);
                    selectedSquare = value;

                    NotifyPropertyChanged();
                }
            }
        }       

        public int[] RankNumbers
        {
            get { return rankNumbers; }
            set
            {
                rankNumbers = value;
                NotifyPropertyChanged();
            }
        }

        public char[] FieldLetters
        {
            get { return fieldLetters; }
            set
            {
                fieldLetters = value;
                NotifyPropertyChanged();
            }
        }

        public SideColor Perspective
        {
            get { return perspective; }
        }

        public ICommand TogglePerspectiveCommand
        {
            get { return new RelayCommand(TogglePerspectiveExecuted); }
        }

        #endregion

        public ChessBoardViewModel(IBasicBoardService basicBoardService)
        {
            this.basicBoardService = basicBoardService;
        }

        protected void InitBoard()
        {
            var newSquares = new List<SquareViewModel>();

            if (perspective == SideColor.White)
            {
                RankNumbers = RankNumbersAsWhite;
                FieldLetters = FieldLettersAsWhite;

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                        newSquares.Add(new SquareViewModel(new Coordinate(x, 7 - y)));
                }
            }
            else
            {
                RankNumbers = RankNumbersAsWhite.Reverse().ToArray();
                FieldLetters = FieldLettersAsWhite.Reverse().ToArray();

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                        newSquares.Add(new SquareViewModel(new Coordinate(7 - x, y)));
                }
            }

            Squares = newSquares;

            RefreshSquares();
        }

        public void RefreshBoard(SideColor changedPerspective)
        {
            if (perspective == changedPerspective)
            {
                RefreshSquares();
            }
            else
            {
                perspective = changedPerspective;
                InitBoard();
            }
        }

        public virtual void RefreshSquares()
        {
            foreach (SquareViewModel square in Squares)
            {
                square.Piece = basicBoardService.GetPiece(square.Coordinate);
                square.IsLastMoveSquare = false;
            }

            SelectedSquare = null;
        }

        protected virtual void OnSelectionChanged(SquareViewModel selectedSquare, SquareViewModel value)
        {
        }

        private void TogglePerspectiveExecuted(object obj)
        {
            perspective = perspective == SideColor.Black ? SideColor.White : SideColor.Black;
            InitBoard();
        }
    }
}
