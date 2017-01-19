using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace ChessEngineClient
{
    public interface IAppSettings
    {
        IPropertySet Values { get; }
    }
}
