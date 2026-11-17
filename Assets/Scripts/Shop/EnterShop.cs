
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeCardUI
    {
        [Header("Upgrade")]
        public ShopUpgradeEntry upgrade;

        [Header("Visual")]
        public Image icon;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI price;
        public TextMeshProUGUI description;

        [Header("Buttons")]
        public Button normalButton;
        public Button dijkstraButton;
    }

    [Header("References")]
    [SerializeField] private ShopManager shopManager;

    [Header("Upgrade Cards")]
    [SerializeField] private List<UpgradeCardUI> upgradeCards = new List<UpgradeCardUI>();

    private void Start()
    {
        RefreshShop();
    }

    public void RefreshShop()
    {
        foreach (UpgradeCardUI card in upgradeCards)
        {
            if (card == null || card.upgrade == null)
                continue;

            ConfigureCard(card);
        }
    }


    private void ConfigureCard(UpgradeCardUI card)
    {
        if (card.upgrade == null || card.upgrade.prefab == null)
        {
            Debug.LogError("Falta asignar la mejora o su Prefab.");
            return;
        }

        PassiveItem item = card.upgrade.prefab.GetComponent<PassiveItem>();

        if (item == null || item.passiveItemData == null)
        {
            Debug.LogError("El Prefab no tiene PassiveItem o PassiveItemData.");
            return;
        }

        PassiveItemScriptableObject data = item.passiveItemData;
        

        // Mostrar información
        card.icon.sprite = data.Icon;
        card.itemName.text = data.Name;
        card.description.text = data.Description;

        // Mostrar precios
        int normalPrice = shopManager.GetUpgradeCost(card.upgrade, false);
        int dijkstraPrice = shopManager.GetUpgradeCost(card.upgrade, true);

        card.price.text ="Price:"+ normalPrice;

        // Limpiar listeners anteriores
        card.normalButton.onClick.RemoveAllListeners();
        card.dijkstraButton.onClick.RemoveAllListeners();

        // Conectar botones
        card.normalButton.onClick.AddListener(
            () => BuyUpgrade(card.upgrade, false)
        );

        card.dijkstraButton.onClick.AddListener(
            () => BuyUpgrade(card.upgrade, true)
        );
    }


    private void BuyUpgrade(ShopUpgradeEntry upgrade, bool useDijkstra)
    {
        bool purchased = shopManager.BuyUpgrade(upgrade, useDijkstra);


        if (purchased)
        {
            Debug.Log("Compra realizada correctamente.");

            RefreshShop();
        }

        else
        {
            Debug.Log("No se pudo realizar la compra.");
        }
    }
}
