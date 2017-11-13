using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Chessman
{
    public class MoveAudioFeedbackService : IMoveAudioFeedbackService
    {
        private Dictionary<MoveType, MediaElement> mediaElements = new Dictionary<MoveType, MediaElement>();

        public MoveAudioFeedbackService()
        {
            InitMediaElementsAsync();
        }

        private async void InitMediaElementsAsync()
        {
            await Task.WhenAll(
                InitMediaElementAsync(MoveType.NormalMove, "move.wav"),
                InitMediaElementAsync(MoveType.Castle, "castle.wav"),
                InitMediaElementAsync(MoveType.Check, "check.wav"),
                InitMediaElementAsync(MoveType.Capture, "capture.wav"));
        }

        private async Task InitMediaElementAsync(MoveType moveType, string audioFileName)
        {
            MediaElement moveMediaElement = new MediaElement();
            moveMediaElement.AutoPlay = false;

            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync(audioFileName);
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            moveMediaElement.SetSource(stream, file.ContentType);
            mediaElements[moveType] = moveMediaElement;
        }
        
        public void PlayMoveExecuted(string pgnMove)
        {
            MoveType moveType = GetMoveType(pgnMove);

            if (mediaElements.ContainsKey(moveType))
                mediaElements[moveType].Play();
        }

        private MoveType GetMoveType(string pgnMove)
        {
            MoveType moveType = MoveType.NormalMove;
            if (pgnMove.Contains('+') || pgnMove.Contains('#'))
                moveType = MoveType.Check;
            else if (pgnMove.Contains("x"))
                moveType = MoveType.Capture;
            else if (pgnMove.Contains("O-O"))
                moveType = MoveType.Castle;

            return moveType;
        }

        private enum MoveType
        {
            NormalMove,
            Capture,
            Castle,
            Check
        }

    }
}
