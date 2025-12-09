using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Single Slot")]
    public InventoryItemUI inventoryItemUI;

    private string currentItemId;

    public bool HasItem => !string.IsNullOrEmpty(currentItemId);
    public string CurrentItemId => currentItemId;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (inventoryItemUI != null)
        {
            inventoryItemUI.gameObject.SetActive(false);
            inventoryItemUI.Clear();
        }
    }

    public void AddItem(string itemId, Sprite icon)
    {
        if (HasItem)
        {
            return;
        }

        currentItemId = itemId;

        if (inventoryItemUI != null)
        {
            inventoryItemUI.Setup(itemId, icon);
            inventoryItemUI.gameObject.SetActive(true);
        }
    }

    public void ClearItem()
    {
        currentItemId = null;

        if (inventoryItemUI != null)
        {
            inventoryItemUI.Clear();
            inventoryItemUI.gameObject.SetActive(false);
        }
    }
}