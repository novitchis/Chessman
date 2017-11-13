using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public interface IMoveAudioFeedbackService
    {
        void PlayMoveExecuted(string pgnMove);
    }
}
