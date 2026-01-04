using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
    public sealed class PurchaseHandler : MonoBehaviour
    {
        public delegate void ProductConsumedEventHandler(TOCIStoreProduct product);

        private const float ConsumptionRetrytime = 60f;

        private IAPStore<TOCIStoreProduct> _store;

        private PopupManager _popupManager;

        private GameStats _gameStats;

        private CraneManager _craneManager;

        private CriticalProcesses _criticalProcesses;

        private IProductConsumer _shopIAPConsumer;

        private IProductConsumer _treasureChestIAPConsumer;

        private IProductConsumer _landmarkIAPConsumer;

        private IProductConsumer _craneOfferIAPConsumer;

        private IProductConsumer _flyingStartIAPConsumer;

        private PopupRequest _purchasePopupRequest;

        private Action _purchaseCollectedCallback;

        private Action _purchaseFailedCallback;

        private ILocalizedString StoreLoadingError
        {
            get
            {
                switch (_store.LoadingError)
                {
                    case IAPStoreError.PurchasingUnavailable:
                        return Localization.Key("purchase.failed_purchasing_unavailable");
                    case IAPStoreError.StoreInitializing:
                        return Localization.Key("purchase.failed_internet_connection");
                    default:
                        return Localization.EmptyLocalizedString;
                }
            }
        }

        public event ProductConsumedEventHandler ProductConsumedEvent;

        private void FireProductConsumedEvent(TOCIStoreProduct product)
        {
            if (this.ProductConsumedEvent != null)
            {
                this.ProductConsumedEvent(product);
            }
        }

        public void Initialize(IAPStore<TOCIStoreProduct> store, PopupManager popupManager, CriticalProcesses criticalProcesses, GameStats gameStats, CraneManager craneManager, IProductConsumer shopIAPConsumer, IProductConsumer treasureChestIAPConsumer, IProductConsumer landmarkIAPConsumer, IProductConsumer craneOfferIAPConsumer, IProductConsumer flyingStartIAPConsumer)
        {
            _store = store;
            _popupManager = popupManager;
            _criticalProcesses = criticalProcesses;
            _gameStats = gameStats;
            _craneManager = craneManager;
            _shopIAPConsumer = shopIAPConsumer;
            _treasureChestIAPConsumer = treasureChestIAPConsumer;
            _landmarkIAPConsumer = landmarkIAPConsumer;
            _craneOfferIAPConsumer = craneOfferIAPConsumer;
            _flyingStartIAPConsumer = flyingStartIAPConsumer;
            _store.PurchaseSuccessEvent += OnPurchaseSuccess;
            _store.PurchaseFailedEvent += OnPurchaseFailed;
            _store.PurchaseDeferredEvent += OnPurchaseDeferred;
            TryConsumingPurchases();
            this.InvokeRepeating(ValidatePurchases, 10f, 600f, realtime: true);
        }

        private void OnDestroy()
        {
            this.CancelInvoke(TryConsumingPurchases);
            this.CancelInvoke(ValidatePurchases);
            if (_store != null)
            {
                _store.PurchaseSuccessEvent -= OnPurchaseSuccess;
                _store.PurchaseFailedEvent -= OnPurchaseFailed;
                _store.PurchaseDeferredEvent -= OnPurchaseDeferred;
                _store = null;
            }
            if (_criticalProcesses != null)
            {
                _criticalProcesses.UnregisterCriticalProcess(this);
                _criticalProcesses = null;
            }
        }

        public void InitiatePurchase(TOCIStoreProduct product, Action purchaseCollectedCallback = null, Action purchaseFailedCallback = null)
        {
            //_purchaseFailedCallback = purchaseFailedCallback;
            //if (_store == null)
            //{
            //	UnityEngine.Debug.LogErrorFormat("Cannot initiate Purchase, PurchaseHandler was never initialized.");
            //}
            //else if (_store.LoadingError == IAPStoreError.None)
            //{
            //	if (_purchasePopupRequest == null)
            //{
            ILocalizedString body = Localization.Key("social_loading");
            _purchasePopupRequest = new GenericPopupRequest("purchase_loading").SetDismissable(dismissable: false).SetTexts(Localization.EmptyLocalizedString, body);
            _popupManager.RequestPopup(_purchasePopupRequest);
            _criticalProcesses.RegisterCriticalProcess(this);
            _purchaseCollectedCallback = purchaseCollectedCallback;
            _store.InitiatePurchase(product);
            //	}
            //	else
            //	{
            //		UnityEngine.Debug.LogError("Purchase failed, Another purchase is still pending.");
            //	}
            //}
            //else
            //{
            //	GenericPopupRequest request = new GenericPopupRequest("purchase_failed").SetDismissable(dismissable: false).SetTexts(Localization.Key("purchase.failed_title"), Localization.Format(Localization.Key("purchase.failed_desc"), StoreLoadingError)).SetGreenOkButton();
            //	_popupManager.RequestPopup(request);
            //}
        }

        private void ValidatePurchases()
        {
            _store.ValidatePurchases();
        }

        private void TryConsumingPurchases()
        {
            Purchase<TOCIStoreProduct>[] unconsumedPurchases = _store.UnconsumedPurchases;
            int num = unconsumedPurchases.Length;
            if (num == 0)
            {
                return;
            }
            if (_popupManager.IsShowingPopup)
            {
                this.Invoke(TryConsumingPurchases, 60f);
                return;
            }
            for (int i = 0; i < num; i++)
            {
                ShowPurchaseSuccessPopup(unconsumedPurchases[i]);
            }
        }

        private void ShowPurchaseSuccessPopup(Purchase<TOCIStoreProduct> purchase)
        {
            Action action = delegate
            {
                OnConsumeProduct(purchase, GetProductConsumer(purchase.Product));
            };
            GenericPopupRequest genericPopupRequest = new GenericPopupRequest("purchase_consumed").SetDismissable(dismissable: false, action).SetGreenOkButton(action);
            ILocalizedString title = Localization.Key("build_purchase_succeeded_title");
            switch (purchase.Product.Category)
            {
                case StoreProductCategory.Shop:
                case StoreProductCategory.ShopSale:
                    {
                        ILocalizedString body = Localization.Format(Localization.Key("build_purchase_succeeded_message"), purchase.Product.Currencies.LocalizedString());
                        genericPopupRequest.SetTexts(title, body);
                        _popupManager.RequestPopup(genericPopupRequest);
                        break;
                    }
                case StoreProductCategory.Chest:
                case StoreProductCategory.ChestOTO:
                    action();
                    break;
                case StoreProductCategory.Landmark:
                    {
                        ILocalizedString body = Localization.Key("tutorial.buildingwarehouse_new_building");
                        genericPopupRequest.SetTexts(title, body);
                        _popupManager.RequestPopup(genericPopupRequest);
                        break;
                    }
                case StoreProductCategory.CraneOffer:
                case StoreProductCategory.Cranes:
                    {
                        int craneCount = purchase.Product.CraneCount;
                        ILocalizedString body = (craneCount == 1) ? Localization.Format(Localization.Key("crane_purchased"), Localization.Integer(_craneManager.MaxBuildCount)) : Localization.Format(Localization.Key("cranes_purchased"), Localization.Integer(craneCount), Localization.Integer(_craneManager.MaxBuildCount + craneCount));
                        genericPopupRequest.SetTexts(title, body);
                        _popupManager.RequestPopup(genericPopupRequest);
                        break;
                    }
                case StoreProductCategory.FlyingStartDeal:
                    {
                        ILocalizedString body = Localization.Format(Localization.Key("build_purchase_succeeded_message"), purchase.Product.Currencies.LocalizedString());
                        genericPopupRequest.SetTexts(title, body);
                        _popupManager.RequestPopup(genericPopupRequest);
                        break;
                    }
                default:
                    UnityEngine.Debug.LogErrorFormat("Missing PurchaseSuccessPopup for category '{0}' of '{1}'", purchase.Product.Category, purchase.Product.Identifier);
                    break;
            }
            Clip clip = Clip.IAPCompleted;
            SingletonMonobehaviour<AudioManager>.Instance.PlayClip(clip);
            float clipLength = SingletonMonobehaviour<AudioManager>.Instance.GetClipLength(clip);
            SingletonMonobehaviour<AudioManager>.Instance.SetMusicVolume(0.2f, clipLength);
        }

        private void OnConsumeProduct(Purchase<TOCIStoreProduct> purchase, IProductConsumer productConsumer)
        {
            if (!productConsumer.ConsumeProduct(purchase.Product))
            {
                UnityEngine.Debug.LogErrorFormat("Failed to consume '{0}'", purchase.Product.Identifier);
                return;
            }
            _store.MaskPurchaseAsConsumed(purchase);
            if (purchase.Validated)
            {
                _gameStats.AddNumberOfIAPsPurchased(1);
            }
            FireProductConsumedEvent(purchase.Product);
            StorageController.Save();
            UnityEngine.Debug.Log("IAP handled, purchases marked as consumed.");
            if (_purchaseCollectedCallback != null)
            {
                _purchaseCollectedCallback();
                _purchaseCollectedCallback = null;
            }
        }

        private void OnPurchaseSuccess(Purchase<TOCIStoreProduct> purchase)
        {
            if (_purchasePopupRequest != null)
            {
                _popupManager.ClosePopup(_purchasePopupRequest, instant: false);
                _purchasePopupRequest = null;
            }
            ShowPurchaseSuccessPopup(purchase);
            _criticalProcesses.UnregisterCriticalProcess(this);
        }

        private void OnPurchaseFailed(TOCIStoreProduct product, IAPStorePurchaseFailedError error)
        {
            if (_purchasePopupRequest != null)
            {
                _popupManager.ClosePopup(_purchasePopupRequest, instant: false);
                _purchasePopupRequest = null;
            }
            ILocalizedString localizedString;
            switch (error)
            {
                case IAPStorePurchaseFailedError.StoreLoadingError:
                    localizedString = StoreLoadingError;
                    break;
                case IAPStorePurchaseFailedError.PurchasingUnavailable:
                    localizedString = Localization.Key("purchase.failed_purchasing_unavailable");
                    break;
                case IAPStorePurchaseFailedError.ExistingPurchasePending:
                    localizedString = Localization.Key("purchase.failed_pending_purchase");
                    break;
                case IAPStorePurchaseFailedError.ProductUnavailable:
                    localizedString = Localization.Key("purchase.failed_product_unavailable");
                    break;
                case IAPStorePurchaseFailedError.SignatureInvalid:
                    localizedString = Localization.Key("purchase.failed_signature_invalid");
                    break;
                case IAPStorePurchaseFailedError.UserCancelled:
                    localizedString = Localization.Key("purchase.failed_user_cancelled");
                    break;
                case IAPStorePurchaseFailedError.PaymentDeclined:
                    localizedString = Localization.Key("purchase.failed_payment_declined");
                    break;
                default:
                    localizedString = Localization.EmptyLocalizedString;
                    break;
            }
            if (product != null)
            {
                if (error == IAPStorePurchaseFailedError.StoreLoadingError)
                {
                    Analytics.IAPPaymentCancelled(product.Identifier, _store.LoadingError.ToString());
                }
                else
                {
                    Analytics.IAPPaymentCancelled(product.Identifier, error.ToString());
                }
            }
            GenericPopupRequest request = new GenericPopupRequest("purchase_failed").SetDismissable(dismissable: false).SetTexts(Localization.Key("purchase.failed_title"), Localization.Format(Localization.Key("purchase.failed_desc"), localizedString)).SetGreenOkButton();
            _popupManager.RequestPopup(request);
            _purchaseFailedCallback?.Invoke();
            _criticalProcesses.UnregisterCriticalProcess(this);
        }

        private void OnPurchaseDeferred(TOCIStoreProduct product)
        {
            if (_purchasePopupRequest != null)
            {
                _popupManager.ClosePopup(_purchasePopupRequest, instant: false);
                _purchasePopupRequest = null;
            }
            _criticalProcesses.UnregisterCriticalProcess(this);
        }

        private IProductConsumer GetProductConsumer(TOCIStoreProduct product)
        {
            switch (product.Category)
            {
                case StoreProductCategory.Chest:
                case StoreProductCategory.ChestOTO:
                    return _treasureChestIAPConsumer;
                case StoreProductCategory.Landmark:
                    return _landmarkIAPConsumer;
                case StoreProductCategory.CraneOffer:
                    return _craneOfferIAPConsumer;
                case StoreProductCategory.FlyingStartDeal:
                    return _flyingStartIAPConsumer;
                default:
                    return _shopIAPConsumer;
            }
        }
    }
}
