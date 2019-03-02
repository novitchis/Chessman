using Framework.MVVM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chessman.ViewModel
{
    public class TacticsViewModel : BoardPageViewModel, INavigationAware
    {
        private ITacticsService tacticsService = null;
        private string currentTactic = "";

        public string CurrentTactic
        {
            get { return currentTactic; }
            set
            {
                if (currentTactic != value)
                {
                    currentTactic = value;
                    NotifyPropertyChanged();
                } 
            }
        }

        public ICommand SkipCommand
        {
            get
            {
                return new RelayCommand(OnSkip);
            }
        }

        public TacticsViewModel(INavigationService navigationService, ITacticsBoardService boardService, ITacticsService tacticsService)
            :base(navigationService, boardService)
        {
            this.tacticsService = tacticsService;
            BoardViewModel = new AnalysisChessBoardViewModel(boardService);
        }

        private async void UpdateCurrentTactic()
        {
            Tactic tactic = await tacticsService.GetAsync();
            CurrentTactic = JsonConvert.SerializeObject(tactic, Formatting.Indented);

            PositionLoadOptions positionOptions = new PositionLoadOptions()
            {
                SerializationType = BoardSerializationType.FEN,
                SerializedBoard = tactic.fenBefore,
            };

            LoadPosition(positionOptions);
         }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            UpdateCurrentTactic();
        }

        public void OnNavigatingFrom()
        {
            //throw new NotImplementedException();
        }

        private async void OnSkip(object obj)
        {
            await tacticsService.Skip();
            UpdateCurrentTactic();
        }
    }
}
