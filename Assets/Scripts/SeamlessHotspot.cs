using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class SeamlessHotspot : MonoBehaviour, IPointerClickHandler
{
    [Header("Panel References")]
    [Tooltip("The panel this hotspot is on (e.g. Desk)")]
    public ZoomablePanel myPanel;

    [Tooltip("The panel to transition to - MUST BE A CHILD of this hotspot!")]
    public ZoomablePanel targetPanel;

    [Header("Settings")]
    [Tooltip("Minimum zoom required to activate (0 = always active)")]
    public float requiredZoom = 0f;

    [Tooltip("Time window for double-click detection")]
    public float doubleClickThreshold = 0.3f;

    RectTransform rect;
    Mask mask;
    Image image;
    float lastClickTime = 0f;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        mask = GetComponent<Mask>();
        if (mask == null)
        {
            mask = gameObject.AddComponent<Mask>();
        }

        mask.showMaskGraphic = true;
    }

    void Start()
    {
        ValidateSetup();
    }

    void ValidateSetup()
    {
        if (targetPanel == null)
        {
            Debug.LogError("SeamlessHotspot '" + gameObject.name + "': targetPanel is NULL!");
            return;
        }

        if (myPanel == null)
        {
            Debug.LogError("SeamlessHotspot '" + gameObject.name + "': myPanel is NULL!");
            return;
        }

        if (targetPanel.transform.parent != transform)
        {
            Debug.LogError("SeamlessHotspot '" + gameObject.name + "': targetPanel '" + targetPanel.name + "' is NOT a child of this hotspot!");
        }

        if (!targetPanel.gameObject.activeSelf)
        {
            targetPanel.gameObject.SetActive(true);
        }

        if (image != null && !image.raycastTarget)
        {
            image.raycastTarget = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("=== CLICK RECEIVED on " + gameObject.name + " ===");

        if (eventData.button != PointerEventData.InputButton.Left)
        {
            Debug.Log("Not left click, ignoring");
            return;
        }

        if (PanelManager.Instance == null)
        {
            Debug.LogError("PanelManager.Instance is NULL!");
            return;
        }

        if (PanelManager.Instance.currentPanel != myPanel)
        {
            Debug.Log("Current panel is " + PanelManager.Instance.currentPanel.name + ", but myPanel is " + myPanel.name + " - ignoring click");
            return;
        }

        if (requiredZoom > 0f && myPanel.GetZoom() < requiredZoom)
        {
            Debug.Log("Need to zoom in more to use " + gameObject.name);
            return;
        }

        float timeSinceLastClick = Time.time - lastClickTime;
        Debug.Log("Time since last click: " + timeSinceLastClick);

        bool isDoubleClick = (timeSinceLastClick <= doubleClickThreshold) && (timeSinceLastClick > 0f);
        lastClickTime = Time.time;

        if (isDoubleClick)
        {
            Debug.Log("DOUBLE CLICK DETECTED! Starting seamless transition!");
            myPanel.ClaimDoubleClick();

            if (targetPanel != null)
            {
                PanelManager.Instance.PlaySeamlessTransition(myPanel, targetPanel, rect);
            }
        }
        else
        {
            Debug.Log("Single click - waiting for second click...");
        }
    }

    public void TriggerTransition()
    {
        if (PanelManager.Instance == null || targetPanel == null || myPanel == null)
            return;

        PanelManager.Instance.PlaySeamlessTransition(myPanel, targetPanel, rect);
    }
}