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
                return (int)appSettings.Values[AppSettingsKeys.ComputerStrengthKey];
            }
            set
            {
                appSettings.Values[AppSettingsKeys.ComputerStrengthKey] = value;
            }
        }

        public SettingsViewModel(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
        }
    }
}
