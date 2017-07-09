using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace ChessEngineClient.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private IAppSettings appSettings = null;

        public int ComputerStrength
        {
            get
            {
                return (int)appSettings.Values[AppPersistenceManager.ComputerStrengthKey];
            }
            set
            {
                appSettings.Values[AppPersistenceManager.ComputerStrengthKey] = value;
            }
        }

        public bool EnableMoveSounds
        {
            get
            {
                return (bool)appSettings.Values[AppPersistenceManager.EnableMoveSoundsKey];
            }
            set
            {
                appSettings.Values[AppPersistenceManager.EnableMoveSoundsKey] = value;
            }
        }

        public int MultipleLines
        {
            get
            {
                return (int)appSettings.Values[AppPersistenceManager.MultipleLinesKey];
            }
            set
            {
                appSettings.Values[AppPersistenceManager.MultipleLinesKey] = value;
            }
        }

        public bool ShowBestMoveArrow
        {
            get
            {
                return (bool)appSettings.Values[AppPersistenceManager.ShowBestMoveArrowKey];
            }
            set
            {
                appSettings.Values[AppPersistenceManager.ShowBestMoveArrowKey] = value;
            }
        }

        public bool SavePositionsBetweenSessions
        {
            get
            {
                return (bool)appSettings.Values[AppPersistenceManager.SavePositionsBetweenSessionsKey];
            }
            set
            {
                appSettings.Values[AppPersistenceManager.SavePositionsBetweenSessionsKey] = value;
            }
        }

        public SettingsViewModel(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
        }
    }
}
