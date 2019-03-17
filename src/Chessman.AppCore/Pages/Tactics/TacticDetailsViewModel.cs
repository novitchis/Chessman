using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman.ViewModel
{
    public class TacticDetailsViewModel : ViewModelBase
    {
        private TacticState state = TacticState.NotStarted;

        public Tactic Tactic
        {
            get;
            set;
        }

        public TacticState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string WhitePlayer
        {
            get { return String.Format("{0} ({1})", Tactic.Info.GameInfo.White, Tactic.Info.GameInfo.WhiteElo); }
        }

        public string BlackPlayer
        {
            get { return String.Format("{0} ({1})", Tactic.Info.GameInfo.Black, Tactic.Info.GameInfo.BlackElo); }
        }

        public TacticDetailsViewModel(Tactic tactic)
        {
            Tactic = tactic;
        }
    }
}
