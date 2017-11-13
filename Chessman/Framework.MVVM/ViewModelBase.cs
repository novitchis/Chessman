using System;

namespace Framework.MVVM
{
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}
