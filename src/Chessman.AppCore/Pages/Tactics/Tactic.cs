using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public class Tactic
    {
        public string id {get;set;}

        public string blunderMove {get;set;}

        public int elo { get;set;}

        public string fenBefore { get; set; }

        public string[] forcedLine { get; set; }

        public TacticInfo Info { get; set; }
    }

    public class TacticResponse<T>
    {
        public T data { get; set; }

        public string status { get; set; }

        public string message { get; set; }
    }

    [DataContract]
    public class TacticInfo
    {
        [DataMember(Name = "game-info")]
        public GameInfo GameInfo { get; set; }
    }

    public class GameInfo
    {
        public string Black { get; set; } = "Unknown";
        public string BlackElo { get; set; } = "?";
        public string White { get; set; } = "Unknown";
        public string WhiteElo { get; set; } = "?";
    }

    public class TokenResponse
    {
        public string token { get; set; }

        public string status { get; set; }

        public string message { get; set; }
    }
}
