using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    [DataContract]
    public class PositionLoadOptions
    {
        [DataMember]
        public string SerializedBoard { get; set; }

        [DataMember]
        public BoardSerializationType SerializationType { get; set; }

        [DataMember]
        public SideColor Perspective { get; set; }

        [DataMember]
        public int CurrentMoveIndex { get; set; }

        public PositionLoadOptions()
        {
            CurrentMoveIndex = -1;
        }
    }
}
