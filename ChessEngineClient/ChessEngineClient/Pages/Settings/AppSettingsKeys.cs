using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace ChessEngineClient
{
    public static class AppSettingsKeys
    {
        public const string ComputerStrengthKey = "ComputerStrength";

        public static void InitializeDefaultSettings(IPropertySet settingsSet)
        {
            if (!settingsSet.ContainsKey(ComputerStrengthKey))
                settingsSet[ComputerStrengthKey] = 5;
        }
    }
}
