using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ChessEngineClient
{
    public class MoveAudioFeedbackService : IMoveAudioFeedbackService
    {
        private MediaElement mediaElement = null;

        public MoveAudioFeedbackService()
        {
            InitMediaElement();
        }

        private async void InitMediaElement()
        {
            mediaElement = new MediaElement();
            mediaElement.AutoPlay = false;
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("chess-move-on-alabaster.wav");
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mediaElement.SetSource(stream, file.ContentType);
        }

        public void PlayMoveExecuted()
        {
            mediaElement.Play();
        }
    }
}
