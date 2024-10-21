using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance { get; private set; }

    IStoreController m_StoreController;

    [SerializeField]
    private StoreItemData _storeItemData;

    public UnityAction endAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetupBuilder();
    }

    private void SetupBuilder()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        /* 구글 플레이 상품들 추가 */
        for (int i = 0; i < _storeItemData.specialStoreItems.Length; i++)
        {
            builder.AddProduct(_storeItemData.specialStoreItems[i]._storeItemID, ProductType.Consumable, new IDs() { { _storeItemData.specialStoreItems[i]._storeItemID, GooglePlay.Name } });
        }

        for (int i = 0; i < _storeItemData.specialHeartStoreItems.Length; i++)
        {
            builder.AddProduct(_storeItemData.specialHeartStoreItems[i]._storeItemID, ProductType.Consumable, new IDs() { { _storeItemData.specialHeartStoreItems[i]._storeItemID, GooglePlay.Name } });
        }

        for (int i = 0; i < _storeItemData.coinStoreItems.Length; i++)
        {
            builder.AddProduct(_storeItemData.coinStoreItems[i]._storeItemID, ProductType.Consumable, new IDs() { { _storeItemData.coinStoreItems[i]._storeItemID, GooglePlay.Name } });
        }

        UnityPurchasing.Initialize(this, builder);
    }


    /// <summary>
    /// Unity IAP가 모든 제품 메타 데이터를 검색하여 구매할 준비가되면 호출된다.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP Initialize Sucess!");
        m_StoreController = controller;

    }

    /// <summary>
    /// Unity IAP가 초기화에 실패할 경우 호출된다.
    /// </summary>
    /// <param name="error"></param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("Initilize Failed!: " + error);
    }

    
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("Initilize Failed!: " + error + "\n" + message);
    }

    /// <summary>
    /// 구매가 실패하면 호출된다.
    /// </summary>
    /// <param name="error"></param>
    /// <param name="message"></param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("Purchase Failed!: " + product.definition.id + "\n" + failureReason.ToString());
    }

    /// <summary>
    /// 구매가 성공하면 호출된다.
    /// </summary>
    /// <param name="product"></param>
    /// <param name="failureReason"></param>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        Debug.Log("Purchase Succeed!: " + product.definition.id);

        endAction.Invoke();

        return PurchaseProcessingResult.Complete;
    }

    public void PurchaseConsumableItem(string productID, UnityAction endAction)
    {
        m_StoreController.InitiatePurchase(productID);

        this.endAction = endAction;
    }

    public string GetPrice(string productID)
    {
        return m_StoreController.products.WithID(productID).metadata.localizedPrice.ToString();
    }

    public string GetPriceUnit(string productID)
    {
        return m_StoreController.products.WithID(productID).metadata.isoCurrencyCode;
    }
}