using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace ChessEngineClient
{
    public static class AppSettingsKeys
    {
        public const string ComputerStrengthKey = "ComputerStrength";

        public static void InitializeDefaultSettings(ApplicationDataContainer settingsContainer)
        {
            if (!settingsContainer.Values.ContainsKey(ComputerStrengthKey))
                settingsContainer.Values[ComputerStrengthKey] = 5;
        }
    }
}
