using ED262C;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Catalogo de mejoras")]
    [SerializeField] private List<ShopUpgradeEntry> shopUpgrades;

    private AVL<ShopUpgradeEntry> upgradeTree = new AVL<ShopUpgradeEntry>();

    private Graph<int> upgradeGraph = new Graph<int>();
    private DijkstraSolver<int> dijkstra = new DijkstraSolver<int>();

    private PlayerStats player;
    private InventoryManager inventory;

    private HashSet<int> purchasedUpgrades = new HashSet<int>();


    private void Awake()
    {
        inventory = FindAnyObjectByType<InventoryManager>();
        player = FindAnyObjectByType<PlayerStats>();

        BuildUpgradeTree();
        BuildUpgradeGraph();
       

        
    }
    private void Start()
    {
        foreach (PassiveItem item in inventory.passiveItemSlots)
        {
            if (item == null || item.passiveItemData == null)
                continue;

            Debug.Log("Mejora en inventario: " +
                item.passiveItemData.Name +
                " | ID: " + item.passiveItemData.UpgradeID);
        }
    }

    public int GetUpgradeCost(ShopUpgradeEntry upgrade, bool useCheapestPath)
    {
        if (upgrade == null)
            return -1;

        if (!useCheapestPath)
            return GetUpgradePrice(upgrade.UpgradeID);

        int rootID = GetRootUpgradeID(upgrade.UpgradeID);

        return GetRemainingCost(rootID, upgrade.UpgradeID);
    }

    private int GetRootUpgradeID(int upgradeID)
    {
        int currentID = upgradeID;
        ISimpleList<int> vertices = upgradeGraph.GetVertices();

        bool foundPrevious = true;
        int attempts = 0;

        while (foundPrevious && attempts < vertices.Count)
        {
            foundPrevious = false;
            attempts++;

            for (int i = 0; i < vertices.Count; i++)
            {
                int possiblePrevious = vertices[i];

                if (upgradeGraph.HasEdge(possiblePrevious, currentID))
                {
                    currentID = possiblePrevious;
                    foundPrevious = true;
                    break;
                }
            }
        }

        return currentID;
    }


    private int GetAccumulatedCost(int startID, int destinationID)
    {
        PathResult<int> result = dijkstra.FindPath(
            upgradeGraph,
            startID,
            destinationID
        );

        if (!result.Found)
            return -1;

        int totalCost = GetUpgradePrice(startID);

        for (int i = 1; i < result.Path.Count; i++)
        {
            totalCost += GetUpgradePrice(result.Path[i]);
        }

        return totalCost;
    }

    private int GetRemainingCost(int startID, int destinationID)
    {
        PathResult<int> result = dijkstra.FindPath(
            upgradeGraph,
            startID,
            destinationID
        );

        if (!result.Found)
            return -1;

        int totalCost = 0;

        // Empezamos en 1 porque el nodo inicial
        // representa la mejora que el jugador ya posee.
        for (int i = 1; i < result.Path.Count; i++)
        {
            int upgradeID = result.Path[i];

            if (!purchasedUpgrades.Contains(upgradeID))
            {
                totalCost += GetUpgradePrice(upgradeID);
            }
        }

        return totalCost;
    }



    private void BuildUpgradeTree()
    {
        upgradeTree = new AVL<ShopUpgradeEntry>();

        foreach (ShopUpgradeEntry upgrade in shopUpgrades)
        {
            if (upgrade != null && upgrade.prefab != null)
            {
                upgradeTree.Insert(upgrade);
            }
        }
    }

    private void BuildUpgradeGraph()
    {
        upgradeGraph = new Graph<int>();

        foreach (ShopUpgradeEntry upgrade in shopUpgrades)
        {
            if (upgrade == null || upgrade.prefab == null)
                continue;

            PassiveItem item = upgrade.prefab.GetComponent<PassiveItem>();

            if (item == null || item.passiveItemData == null)
                continue;

            int id = item.passiveItemData.UpgradeID;

            if (upgradeGraph.ContainsVertex(id))
            {
                Debug.LogWarning("ID duplicado en la tienda: " + id);
                continue;
            }

            upgradeGraph.AddVertex(id);

           
        }
        upgradeGraph.AddEdge(1, 2, GetUpgradePrice(2));
        upgradeGraph.AddEdge(3, 4, GetUpgradePrice(4));
    }

    public List<ShopUpgradeEntry> GetAvailableUpgrades()
    {
        List<ShopUpgradeEntry> available = new List<ShopUpgradeEntry>();

        if (player == null)
            return available;

        List<ShopUpgradeEntry> allUpgrades = upgradeTree.GetInOrder();

        foreach (ShopUpgradeEntry upgrade in allUpgrades)
        {
            if (player.level >= upgrade.requiredPlayerLevel)
            {
                available.Add(upgrade);
            }
        }

        return available;
    }
    public bool BuyUpgrade(ShopUpgradeEntry upgrade, bool useCheapestPath = false)
    {
        if (upgrade == null || player == null)
            return false;

        if (player.level < upgrade.requiredPlayerLevel)
        {
            return false;
        }
        if (!HasRequiredUpgrade(upgrade.UpgradeID))
        {
            Debug.Log("Primero tenés que comprar la mejora anterior.");
            return false;
        }


        if (!upgradeTree.Contains(upgrade)) 
        
            return false;
        
           

   PassiveItemScriptableObject itemData =upgrade.prefab.GetComponent<PassiveItem>().passiveItemData;

        if (itemData == null)
            return false;

        if (!itemData.IsShopItem)
            return false;

        int price;

        if (useCheapestPath)
        {
            int rootID = GetRootUpgradeID(upgrade.UpgradeID);

            price = GetRemainingCost(
                rootID,
                upgrade.UpgradeID
            );
        }
        else
        {
            price = itemData.TokenPrice;
        }

        if (price < 0)
            return false;

        if (inventory == null)
            return false;

        if (!inventory.CanReceivePurchasedItem(upgrade.prefab))
        {
            Debug.Log("No tenés espacio en el inventario o ya tenés esa mejora.");
            return false;
        }

        if (!player.SpendTokens(price))
            return false;


        if (!inventory.TryAddPurchasedItem(upgrade.prefab))
            return false;

        upgradeTree.Remove(upgrade);
        purchasedUpgrades.Add(itemData.UpgradeID);

        Debug.Log("Mejora comprada: " + itemData.Name);

        return true;
    }

    private bool HasRequiredUpgrade(int upgradeID)
    {
        // Primero: comprobar si ya tenemos esta mejora.
        foreach (PassiveItem item in inventory.passiveItemSlots)
        {
            if (item == null || item.passiveItemData == null)
                continue;

            if (item.passiveItemData.UpgradeID == upgradeID)
            {
                Debug.Log("Ya tenés esta mejora: " +
                    item.passiveItemData.Name);

                return false;
            }
        }
        ISimpleList<int> vertices = upgradeGraph.GetVertices();

        for (int i = 0; i < vertices.Count; i++)
        {
            int previousID = vertices[i];

            if (upgradeGraph.HasEdge(previousID, upgradeID))
            {
                foreach (PassiveItem item in inventory.passiveItemSlots)
                {
                    if (item == null || item.passiveItemData == null)
                        continue;

                    if (item.passiveItemData.UpgradeID == previousID)
                        return true;
                }

                return false;
            }
        }

        // No tiene mejora anterior.
        return true;
    }
    public void CheckUpgradePath(int destinationID)
    {
        int rootID = GetRootUpgradeID(destinationID);

        PathResult<int> result = dijkstra.FindPath(
            upgradeGraph,
            rootID,
            destinationID
        );

        if (!result.Found)
        {
            Debug.Log("No existe un camino entre esas mejoras.");
            return;
        }

        Debug.Log("Costo mínimo: " + result.TotalCost);
        Debug.Log("Costo acumulado: " +
            GetAccumulatedCost(rootID, destinationID));
        Debug.Log("Costo restante: " +
            GetRemainingCost(rootID, destinationID));

        for (int i = 0; i < result.Path.Count; i++)
        {
            Debug.Log("ID de mejora: " + result.Path[i]);
        }
    }

    private int GetUpgradePrice(int upgradeID)
    {
        foreach (ShopUpgradeEntry upgrade in shopUpgrades)
        {
            if (upgrade == null || upgrade.prefab == null)
                continue;

            PassiveItem item = upgrade.prefab.GetComponent<PassiveItem>();

            if (item == null || item.passiveItemData == null)
                continue;

            if (item.passiveItemData.UpgradeID == upgradeID)
                return item.passiveItemData.TokenPrice;
        }

        return 0;
    }

    private void TestUpgradePaths()
    {
        CheckUpgradePath(2);
        CheckUpgradePath(4);
    }
}