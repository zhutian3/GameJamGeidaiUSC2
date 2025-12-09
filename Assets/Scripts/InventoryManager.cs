using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Single Slot")]
    public InventoryItemUI inventoryItemUI;   // 场景里的 InventoryItemImage 上的脚本

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

    /// <summary>
    /// 往背包里放一个物品。当前已经有物品时直接忽略（单物品背包）。
    /// </summary>
    public void AddItem(string itemId, Sprite icon)
    {
        if (HasItem)
        {
            // 如果以后想改成覆盖旧物品，可以先 ClearItem() 再赋值
            return;
        }

        currentItemId = itemId;

        if (inventoryItemUI != null)
        {
            inventoryItemUI.Setup(itemId, icon);
            inventoryItemUI.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 使用完物品后清空背包。
    /// </summary>
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
