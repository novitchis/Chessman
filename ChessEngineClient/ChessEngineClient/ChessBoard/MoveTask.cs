using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class MoveTask
    {
        public Action OnTransitionCompleted
        {
            get; set;
        }

        public List<Tuple<Coordinate, Coordinate>> MovedPiecesCoordinates
        {
            get; set;
        }

        public Coordinate CapturedPieceCoordinate { get; set; }

        public MoveData MoveData { get; set; }

        public MoveTask(MoveData moveData)
        {
            MoveData = moveData;
            MovedPiecesCoordinates = GetChangedPieceCoordinates(moveData);

            if (moveData.CapturedPiece != null)
            {
                if (moveData.EnPassantCapture)
                    CapturedPieceCoordinate = new Coordinate(moveData.Move.GetTo().X, moveData.Move.GetFrom().Y);
                else
                    CapturedPieceCoordinate = moveData.Move.GetTo();
            }
        }

        private List<Tuple<Coordinate, Coordinate>> GetChangedPieceCoordinates(MoveData moveData)
        {
            List<Tuple<Coordinate, Coordinate>> coordinateChanges = new List<Tuple<Coordinate, Coordinate>>();
            Coordinate from = moveData.Move.GetFrom();
            Coordinate to = moveData.Move.GetTo();

            coordinateChanges.Add(Tuple.Create(from, to));

            // rook position change
            if (moveData.IsCastle)
            {
                bool isShortCastle = from.X < to.X;
                if (isShortCastle)
                    coordinateChanges.Add(Tuple.Create(new Coordinate(7, from.Y), new Coordinate(5, to.Y)));
                else
                    coordinateChanges.Add(Tuple.Create(new Coordinate(0, from.Y), new Coordinate(3, to.Y)));
            }

            return coordinateChanges;
        }

        public void ReverseMovedPieceCoordinates()
        {
            MovedPiecesCoordinates = MovedPiecesCoordinates.Select(t => Tuple.Create(t.Item2, t.Item1)).ToList();
        }

        public void CompleteTask()
        {
            OnTransitionCompleted?.Invoke();
        }
    }
}
