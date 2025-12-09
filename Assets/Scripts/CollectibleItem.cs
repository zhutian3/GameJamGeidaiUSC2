using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CollectibleItem : MonoBehaviour, IPointerClickHandler
{
    [Header("Item Info")]
    public string itemId;      // 比如 "rocket"
    public Sprite itemIcon;    // 背包里显示的图标（可以用和自己相同的 sprite）

    [Header("Links")]
    public PanelManager panelManager;
    public ZoomablePanel myPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.Instance == null)
            return;

        // 必须当前 Panel 才能拾取
        if (panelManager != null && panelManager.currentPanel != myPanel)
            return;

        // 单物品背包：如果已经有物品，则不再拾取
        if (InventoryManager.Instance.HasItem)
            return;

        if (itemIcon == null)
        {
            Image img = GetComponent<Image>();
            if (img != null)
                itemIcon = img.sprite;
        }

        InventoryManager.Instance.AddItem(itemId, itemIcon);

        // 拾取后把画面里的这个图隐藏（或 Destroy）
        gameObject.SetActive(false);
    }
}
