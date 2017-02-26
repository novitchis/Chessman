using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace ChessEngineClient
{
    public static class AppPersistenceManager
    {
        public const string ShowBestMoveArrowKey = "ShowAnalysisArrow";
        public const string ComputerStrengthKey = "ComputerStrength";
        public const string SavePositionsBetweenSessionsKey = "SavePositionsBetweenSessions";

        public const string SavedAnalysisPositionPgnKey = "SavedAnalysisPositionPgnKey";
        public const string SavedPracticePositionPgnKey = "SavedPracticePositionPgnKey";

        public static void InitializeDefaultSettings(ApplicationDataContainer settingsContainer)
        {
            if (!settingsContainer.Values.ContainsKey(ComputerStrengthKey))
                settingsContainer.Values[ComputerStrengthKey] = 5;

            if (!settingsContainer.Values.ContainsKey(ShowBestMoveArrowKey))
                settingsContainer.Values[ShowBestMoveArrowKey] = true;

            if (!settingsContainer.Values.ContainsKey(SavePositionsBetweenSessionsKey))
                settingsContainer.Values[SavePositionsBetweenSessionsKey] = true;
        }

        public static void SaveApplicationState(ApplicationDataContainer settingsContainer)
        {
            bool savesSessions = true;
            if (settingsContainer.Values.ContainsKey(SavePositionsBetweenSessionsKey))
                savesSessions = (bool)settingsContainer.Values[SavePositionsBetweenSessionsKey];

            if (savesSessions)
            {
                PositionLoadOptions analysisPosition = ViewModelLocator.MainViewModel.GetPositionLoadOptions(BoardSerializationType.PGN);
                settingsContainer.Values[SavedAnalysisPositionPgnKey] = JsonConvert.SerializeObject(analysisPosition);

                PositionLoadOptions practicePosition = ViewModelLocator.PracticeViewModel.GetPositionLoadOptions(BoardSerializationType.PGN);
                settingsContainer.Values[SavedPracticePositionPgnKey] = JsonConvert.SerializeObject(practicePosition);
            }
            else
            {
                settingsContainer.Values.Remove(SavedAnalysisPositionPgnKey);
                settingsContainer.Values.Remove(SavedPracticePositionPgnKey);
            }
        }

        public static void RestoreApplicationState(ApplicationDataContainer settingsContainer)
        {
            bool savesSessions = true;
            if (settingsContainer.Values.ContainsKey(SavePositionsBetweenSessionsKey))
                savesSessions = (bool)settingsContainer.Values[SavePositionsBetweenSessionsKey];

            if (savesSessions)
            {
                PositionLoadOptions analysisPosition = GetSavedPosition(settingsContainer, SavedAnalysisPositionPgnKey);
                if (analysisPosition != null)
                    ViewModelLocator.MainViewModel.LoadPosition(analysisPosition);

                PositionLoadOptions practicePosition = GetSavedPosition(settingsContainer, SavedPracticePositionPgnKey);
                if (practicePosition != null)
                    ViewModelLocator.PracticeViewModel.LoadPosition(practicePosition);
            }
        }

        private static PositionLoadOptions GetSavedPosition(ApplicationDataContainer settingsContainer, string settingsKey)
        {
            if (settingsContainer.Values.ContainsKey(settingsKey))
            {
                string serializedBoard = (string)settingsContainer.Values[settingsKey];
                try
                {
                    return JsonConvert.DeserializeObject<PositionLoadOptions>(serializedBoard);
                }
                catch(Exception ex)
                {
                    //TODO: log the error 
                }
            }
            return null;
        }
    }
}
