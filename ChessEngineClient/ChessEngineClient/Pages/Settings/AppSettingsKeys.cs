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
        public const string ShowBestMoveArrowKey = "ShowAnalysisArrow";
        public const string ComputerStrengthKey = "ComputerStrength";
        public const string SavePositionsBetweenSessionsKey = "SavePositionsBetweenSessions";

        public static void InitializeDefaultSettings(ApplicationDataContainer settingsContainer)
        {
            if (!settingsContainer.Values.ContainsKey(ComputerStrengthKey))
                settingsContainer.Values[ComputerStrengthKey] = 5;

            if (!settingsContainer.Values.ContainsKey(ShowBestMoveArrowKey))
                settingsContainer.Values[ShowBestMoveArrowKey] = true;

            if (!settingsContainer.Values.ContainsKey(SavePositionsBetweenSessionsKey))
                settingsContainer.Values[SavePositionsBetweenSessionsKey] = true;
        }
    }
}
