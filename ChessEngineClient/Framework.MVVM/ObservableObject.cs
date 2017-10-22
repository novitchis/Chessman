using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Framework.MVVM
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //TODO: Not supported by .net standard 1.4
//#if DEBUG
//            if (GetType().GetProperty(propertyName) == null)
//                throw new ArgumentException(String.Format("This class does not have a \"{0}\" property.", propertyName));
//#endif
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
