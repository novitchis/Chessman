using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public interface IExerciseBoardService : IBoardService
    {
        SideColor UserPerspective { get; set; }
    }
}
