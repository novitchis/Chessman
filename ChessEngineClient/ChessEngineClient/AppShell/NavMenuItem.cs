using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public class NavMenuItem
    {
        public string Name { get; set; }

        public string SymbolIconName { get; set; }

        public string PageNavigationName { get; set; }

        public NavMenuItem(string name, string symbolIconName, string pageNavigationName)
        {
            Name = name;
            SymbolIconName = symbolIconName;
            PageNavigationName = pageNavigationName;
        }
    }
}
