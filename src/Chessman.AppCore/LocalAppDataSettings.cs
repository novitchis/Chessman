using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Chessman
{
    public class LocalAppDataSettings : IAppSettings
    {
        public IPropertySet Values
        {
            get { return ApplicationData.Current.LocalSettings.Values; }
        }
    }
}
