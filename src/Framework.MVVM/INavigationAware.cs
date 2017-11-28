namespace Framework.MVVM
{
    public interface INavigationAware
    {
        void OnNavigatingFrom();

        void OnNavigatedTo(object parameter);
    }
}
