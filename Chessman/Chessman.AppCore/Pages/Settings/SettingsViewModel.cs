using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.System.Profile;

namespace Chessman.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private IAppSettings appSettings = null;

        public NotationType[] AvailableNotationTypes { get; } = new[] { NotationType.Figurines, NotationType.English };

        public NotationType NotationType
        {
            get
            {
                return (NotationType)appSettings.Values[AppPersistenceManager.NotationTypeKey];
            }
            set
            {
                appSettings.Values[AppPersistenceManager.NotationTypeKey] = (int)value;
            }
        }

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

        public bool ShowLegalMoves
        {
            get
            {
                return (bool)appSettings.Values[AppPersistenceManager.ShowLegalMovesKey];
            }
            set
            {
                appSettings.Values[AppPersistenceManager.ShowLegalMovesKey] = value;
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

        public int MaximumLines
        {
            get
            {
                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                   return 3;
                return 5;
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
