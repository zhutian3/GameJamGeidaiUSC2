using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CollectibleItem : MonoBehaviour, IPointerClickHandler
{
    [Header("Item Info")]
    public string itemId;
    public Sprite itemIcon;

    [Header("Links")]
    public PanelManager panelManager;
    public ZoomablePanel myPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.Instance == null)
            return;

        if (panelManager != null && panelManager.currentPanel != myPanel)
            return;

        if (InventoryManager.Instance.HasItem)
            return;

        if (itemIcon == null)
        {
            Image img = GetComponent<Image>();
            if (img != null)
                itemIcon = img.sprite;
        }

        InventoryManager.Instance.AddItem(itemId, itemIcon);

        gameObject.SetActive(false);
    }
}