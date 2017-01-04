using ChessEngine;
using Framework.MVVM;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessEngineClient.ViewModel
{
    public class MainViewModel : BoardPageViewModel, INavigationAware
    {
        public AnalysisViewModel AnalysisViewModel { get; set; }

        public MainViewModel(INavigationService navigationService, IEngineBoardService analysisBoardService)
            : base(navigationService, analysisBoardService)
        {
            AnalysisViewModel = ViewModelLocator.IOCContainer.Resolve<AnalysisViewModel>();
            BoardViewModel = new AnalysisChessBoardViewModel(analysisBoardService);
        }

        public override void OnNavigatedTo(object parameter)
        {
            AnalysisViewModel.SubscribeToAnalysis();
            base.OnNavigatedTo(parameter);
        }

        public override void OnNavigatingFrom()
        {
            base.OnNavigatingFrom();
            AnalysisViewModel.UnsubscribeToAnalysis();
        }
    }
}
