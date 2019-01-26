using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Services.Store;
using Windows.UI.Popups;

namespace Chessman.ViewModel
{
    public class BannerAdViewModel : ViewModelBase
    {
        public const string AnalysisAdUnitId = "1100038402";
        public const string PracticeAdUnitId = "1100038403";
        public const string EditBoardAdUnitId = "1100038404";

        private StoreContext context = null;

        // set to initially true so that when users buy an addon they dont see a flicker of the ads
        private bool removeAdsAddon = true;

        public const string RemoveAdsAddonStoreId = "9N8DMLL70VKF";


        public bool RemoveAdsAddon
        {
            get { return removeAdsAddon; }
            set
            {
                removeAdsAddon = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand RemoveAdsCommand { get { return new RelayCommand(OnRemoveAds); } }

        public BannerAdViewModel()
        {
            context = StoreContext.GetDefault();
            InitStoreContext();
        }

        private async void InitStoreContext()
        {
            // Specify the kinds of add-ons to retrieve.
            string[] productKinds = { "Durable" };
            List<String> filterList = new List<string>(productKinds);

            StoreProductQueryResult queryResult = await context.GetUserCollectionAsync(filterList);
            if (queryResult.ExtendedError != null)
            {
                RemoveAdsAddon = false;

                // TODO: implement a dialog service
                MessageDialog dialog = new MessageDialog("An error occured. Please check your internet connection.");
                await dialog.ShowAsync();
                return;
            }

            RemoveAdsAddon = queryResult.Products.ContainsKey(RemoveAdsAddonStoreId);
        }

        private async void OnRemoveAds(object obj)
        {
            var result = await context.RequestPurchaseAsync(RemoveAdsAddonStoreId);
            if (result.ExtendedError != null)
            {
                // TODO: implement a dialog service
                MessageDialog dialog = new MessageDialog("An error occured. Please check your internet connection.");
                await dialog.ShowAsync();
            }

            if (result.Status == StorePurchaseStatus.AlreadyPurchased || 
                result.Status == StorePurchaseStatus.Succeeded)
            {
                // TODO: implement a dialog service
                MessageDialog dialog = new MessageDialog("Thank you for supporting Chessman development. Enjoy your ad-free application.");
                await dialog.ShowAsync();

                RemoveAdsAddon = true;
            }
        }
    }
}
