using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public class PositionLoadOptions
    {
        public string SerializedBoard { get; set; }

        public BoardSerializationType SerializationType { get; set; }

        public SideColor Perspective { get; set; }
    }
}
