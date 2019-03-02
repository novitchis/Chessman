using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    public class TacticResponse
    {
        public Tactic data { get; set; }

        public string status { get; set; }

        public string message { get; set; }
    }

    public class TokenResponse
    {
        public string token { get; set; }

        public string status { get; set; }

        public string message { get; set; }
    }
}
