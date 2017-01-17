using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.MVVM
{
    public interface INavigationAware
    {
        void OnNavigatingFrom();

        void OnNavigatedTo(object parameter);
    }
}
