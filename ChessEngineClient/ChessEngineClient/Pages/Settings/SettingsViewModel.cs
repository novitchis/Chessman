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
        private IPropertySet settingsSet = null;

        public int ComputerStrength
        {
            get
            {
                return (int)settingsSet[AppSettingsKeys.ComputerStrengthKey];
            }
            set
            {
                settingsSet[AppSettingsKeys.ComputerStrengthKey] = value;
            }
        }

        public SettingsViewModel(IPropertySet settingsSet)
        {
            this.settingsSet = settingsSet;
        }
    }
}
