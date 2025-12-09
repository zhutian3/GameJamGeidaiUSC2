using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Image inventoryIcon;
    public Sprite emptySprite;

    string currentItemId = null;

    void Awake()
    {
        Instance = this;
    }

    public bool HasItem() => !string.IsNullOrEmpty(currentItemId);

    public bool TryPickup(string itemId, Sprite iconSprite)
    {
        if (HasItem()) return false;

        currentItemId = itemId;
        inventoryIcon.sprite = iconSprite;
        return true;
    }

    public bool UseItem(string requiredItemId)
    {
        if (!HasItem()) return false;
        if (currentItemId != requiredItemId) return false;

        Clear();
        return true;
    }

    public void Clear()
    {
        currentItemId = null;
        inventoryIcon.sprite = emptySprite;
    }

    public string GetCurrentItemId() => currentItemId;
}
