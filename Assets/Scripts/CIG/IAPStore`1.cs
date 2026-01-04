using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace CIG
{
    public class IAPStore<T>  where T : StoreProduct, IStorable
    {
        public delegate void InitializedEventHandler();

        public delegate void PurchaseSuccessEventHandler(Purchase<T> purchase);

        public delegate void PurchaseFailedEventHandler(T product, IAPStorePurchaseFailedError error);

        public delegate void PurchaseDeferredEventHandler(T product);

        private static readonly Dictionary<PurchaseFailureReason, IAPStorePurchaseFailedError> _purchaseFailureReasonMapping = new Dictionary<PurchaseFailureReason, IAPStorePurchaseFailedError>
        {
            {
                PurchaseFailureReason.PurchasingUnavailable,
                IAPStorePurchaseFailedError.PurchasingUnavailable
            },
            {
                PurchaseFailureReason.ExistingPurchasePending,
                IAPStorePurchaseFailedError.ExistingPurchasePending
            },
            {
                PurchaseFailureReason.ProductUnavailable,
                IAPStorePurchaseFailedError.ProductUnavailable
            },
            {
                PurchaseFailureReason.SignatureInvalid,
                IAPStorePurchaseFailedError.SignatureInvalid
            },
            {
                PurchaseFailureReason.UserCancelled,
                IAPStorePurchaseFailedError.UserCancelled
            },
            {
                PurchaseFailureReason.PaymentDeclined,
                IAPStorePurchaseFailedError.PaymentDeclined
            },
            {
                PurchaseFailureReason.DuplicateTransaction,
                IAPStorePurchaseFailedError.DuplicateTransaction
            },
            {
                PurchaseFailureReason.Unknown,
                IAPStorePurchaseFailedError.Unknown
            }
        };

        private readonly WebService _webService;

        private readonly IAPValidationManager _iapValidator;

        private readonly IAPCatalog<T> _iapCatalog;

        private IStoreController _storeController;

        private readonly List<Purchase<T>> _unconsumedPurchases = new List<Purchase<T>>();

        private readonly List<Product> _unconsumedProducts = new List<Product>();

        public Purchase<T>[] UnconsumedPurchases => _unconsumedPurchases.ToArray();

        public T[] Products => _iapCatalog.AvailableProducts;

        public IAPStoreError LoadingError
        {
            get;
            private set;
        }

        public event InitializedEventHandler InitializedEvent;

        public event PurchaseSuccessEventHandler PurchaseSuccessEvent;

        public event PurchaseFailedEventHandler PurchaseFailedEvent;

        public event PurchaseDeferredEventHandler PurchaseDeferredEvent;

        private void FireInitializedEvent()
        {
            if (this.InitializedEvent != null)
            {
                this.InitializedEvent();
            }
        }

        private void FirePurchaseSuccessEvent(Purchase<T> purchase)
        {
            if (this.PurchaseSuccessEvent != null)
            {
                this.PurchaseSuccessEvent(purchase);
            }
        }

        private void FirePurchaseFailedEvent(T product, IAPStorePurchaseFailedError error)
        {
            if (this.PurchaseFailedEvent != null)
            {
                this.PurchaseFailedEvent(product, error);
            }
        }

        private void FirePurchaseDeferredEvent(T product)
        {
            if (this.PurchaseDeferredEvent != null)
            {
                this.PurchaseDeferredEvent(product);
            }
        }

        public IAPStore(User user, WebService webService, RoutineRunner routineRunner, Func<StorageDictionary, T> storeProductFactory)
        {
            _webService = webService;
            _iapValidator = new IAPValidationManager(user, webService, routineRunner);
            _iapCatalog = new IAPCatalog<T>(_webService, storeProductFactory);
            LoadingError = IAPStoreError.ProductsNotLoaded;
            routineRunner.StartCoroutine(LoadProducts());
        }

        public IEnumerator LoadProducts(bool forceReload = false)
        {
            if (LoadingError == IAPStoreError.ProductsNotLoaded || forceReload)
            {
                LoadingError = IAPStoreError.ProductsNotLoaded;
                yield return _iapCatalog.UpdatePricePoints();
                LoadingError = IAPStoreError.StoreInitializing;
                InitializeStoreController();
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                extensions.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(OnPurchaseDeferred);
            }
            _iapCatalog.UpdateProducts(_storeController.products.all);
            LoadingError = IAPStoreError.None;
            UnityEngine.Debug.Log("IAPStore Initialized successfully.");
            FireInitializedEvent();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            switch (error)
            {
                case InitializationFailureReason.AppNotKnown:
                    UnityEngine.Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                    LoadingError = IAPStoreError.AppNotKnown;
                    break;
                case InitializationFailureReason.PurchasingUnavailable:
                    UnityEngine.Debug.LogError("Billing disabled!");
                    LoadingError = IAPStoreError.PurchasingUnavailable;
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    UnityEngine.Debug.LogError("No products available for purchase!");
                    LoadingError = IAPStoreError.NoProductsAvailable;
                    break;
                default:
                    UnityEngine.Debug.LogErrorFormat("Unknown Store Initialization Failure Reason: {0}", error.ToString());
                    LoadingError = IAPStoreError.StoreInitializeFailed;
                    break;
            }
            FireInitializedEvent();
        }

        public void InitiatePurchase(T product)
        {
            //if (product == null)
            //{
            //    UnityEngine.Debug.LogError("Cannot purchase a product which is null.");
            //    FirePurchaseFailedEvent(product, IAPStorePurchaseFailedError.ProductNull);
            //    return;
            //}
            //Analytics.IAPPaymentInitiated(product.Identifier);
            //if (LoadingError != 0)
            //{
            //    UnityEngine.Debug.LogErrorFormat("Store wasn't loaded correctly. Loading Error: {0}", LoadingError.ToString());
            //    FirePurchaseFailedEvent(product, IAPStorePurchaseFailedError.StoreLoadingError);
            //    return;
            //}
            //if (_storeController == null)
            //{
            //    UnityEngine.Debug.LogError("Store Controller unavailable.");
            //    FirePurchaseFailedEvent(product, IAPStorePurchaseFailedError.StoreNull);
            //    return;
            //}
            //if (_webService != null)
            //{
            //    _webService.PurchaseIntent(product.Identifier);
            //}
            //UnityEngine.Debug.LogFormat("Going to start purchase of '{0}'.", product.Identifier);
            //_storeController.InitiatePurchase(product.Identifier);


            T val = FindStoreProduct(product.Identifier);
            Purchase<T> purchase = new Purchase<T>(val, "", true);
            {
                _unconsumedPurchases.Add(purchase);
                _iapValidator.AddPurchase(val.Identifier, "", "");
                FirePurchaseSuccessEvent(purchase);
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            bool validated = true;
            if (_webService != null && _webService.IAPSignValidationEnabled)
            {
            }
            T val = FindStoreProduct(e.purchasedProduct);
            if (val == null)
            {
                UnityEngine.Debug.LogErrorFormat("Couldn't find StoreProduct for '{0}' after purchasing was success!", e.purchasedProduct.definition.id);
                return PurchaseProcessingResult.Pending;
            }
            Purchase<T> purchase = new Purchase<T>(val, e.purchasedProduct.transactionID, validated);
            if (!_unconsumedPurchases.Exists((Purchase<T> p) => p.TransactionID == purchase.TransactionID))
            {
                _unconsumedPurchases.Add(purchase);
                _unconsumedProducts.Add(e.purchasedProduct);
                _iapValidator.AddPurchase(val.Identifier, e.purchasedProduct.transactionID, e.purchasedProduct.receipt);
                UnityEngine.Debug.LogFormat("Purchase of {0} successfull!", e.purchasedProduct.definition.id);
                FirePurchaseSuccessEvent(purchase);
            }
            return PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason error)
        {
            if (error != PurchaseFailureReason.UserCancelled)
            {
                UnityEngine.Debug.LogErrorFormat("Failed to purchase {0}: {1}", product.definition.id, error.ToString());
            }
            if (!_purchaseFailureReasonMapping.TryGetValue(error, out IAPStorePurchaseFailedError value))
            {
                value = IAPStorePurchaseFailedError.Unknown;
                UnityEngine.Debug.LogErrorFormat("Missing Purchase Failure Reason '{0}' mapping", error.ToString());
            }
            FirePurchaseFailedEvent(FindStoreProduct(product), value);
        }

        public void ValidatePurchases()
        {
            _iapValidator.ValidatePurchases();
        }

        public void MaskPurchaseAsConsumed(Purchase<T> purchase)
        {
            if (!_unconsumedPurchases.Contains(purchase))
            {
                UnityEngine.Debug.LogErrorFormat("Cannot mark Purchase of {0} with TransactionID {1} as consumed: purchase not found in the unconsumed list!", purchase.Product.Identifier, purchase.TransactionID);
                return;
            }
            Product product = _unconsumedProducts.Find((Product x) => x.transactionID == purchase.TransactionID);
            if (product == null)
            {
                return;
            }
            _unconsumedPurchases.Remove(purchase);
            _unconsumedProducts.Remove(product);
            _storeController.ConfirmPendingPurchase(product);
            if (purchase.Validated)
            {
                Analytics.IAPPaymentConfirmed(purchase.Product.Identifier);
            }
            else
            {
                Analytics.IAPPaymentInvalid(purchase.Product.Identifier, cheaterLocalInvalidIap: true);
            }
        }

        public T FindProduct(string identifier)
        {
            return _iapCatalog.FindProduct(identifier);
        }

        public T FindProduct(Predicate<T> predicate, bool includeUnavailable = false)
        {
            return _iapCatalog.FindProduct(predicate, includeUnavailable);
        }

        public T[] GetProducts(Predicate<T> predicate, bool includeUnavailable = false)
        {
            return _iapCatalog.GetProducts(predicate, includeUnavailable);
        }

        private void OnPurchaseDeferred(Product product)
        {
            UnityEngine.Debug.LogFormat("Purchase Deferred {0}", product.definition.id);
            FirePurchaseDeferredEvent(FindStoreProduct(product));
        }

        private void InitializeStoreController()
        {
            ConfigurationBuilder standardConfigurationBuilder = GetStandardConfigurationBuilder();
            T[] availableProducts = _iapCatalog.AvailableProducts;
            int num = availableProducts.Length;
            for (int i = 0; i < num; i++)
            {
                string identifier = availableProducts[i].Identifier;
                standardConfigurationBuilder.AddProduct(identifier, ProductType.Consumable);
            }
           // UnityPurchasing.Initialize(this, standardConfigurationBuilder);
        }

        private ConfigurationBuilder GetStandardConfigurationBuilder()
        {
            return ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        }

        private T FindStoreProduct(Product product)
        {
            return _iapCatalog.FindProduct(product.definition.id);
        }
        private T FindStoreProduct(string id)
        {
            return _iapCatalog.FindProduct(id);
        }
    }
}
