using UnityEngine;

public class ZoomablePanel : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float minZoom = 1f;
    public float maxZoom = 3f;
    public float zoomStep = 1.5f;

    [Header("Links")]
    public ZoomablePanel previousPanel;   // where to go on double-right
    public PanelManager panelManager;     // drag your PanelManager here in Inspector

    RectTransform rect;
    float currentZoom = 1f;

    float lastLeftClickTime = 0f;
    float lastRightClickTime = 0f;
    float doubleClickThreshold = 0.25f; // seconds

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        ResetZoom();
    }

    void Update()
    {
        HandleMouse();
    }

    void HandleMouse()
    {
        // LEFT mouse → zoom in on double-click
        if (Input.GetMouseButtonDown(0))
        {
            float t = Time.time - lastLeftClickTime;
            if (t <= doubleClickThreshold)
            {
                ZoomIn();
                lastLeftClickTime = 0f;
            }
            else
            {
                lastLeftClickTime = Time.time;
            }
        }

        // RIGHT mouse → zoom out or go back on double-right
        if (Input.GetMouseButtonDown(1))
        {
            float t = Time.time - lastRightClickTime;
            if (t <= doubleClickThreshold)
            {
                TryGoToPreviousPanel();
                lastRightClickTime = 0f;
            }
            else
            {
                ZoomOut();
                lastRightClickTime = Time.time;
            }
        }
    }

    void ZoomIn()
    {
        if (currentZoom >= maxZoom) return;

        currentZoom = Mathf.Min(currentZoom * zoomStep, maxZoom);
        rect.localScale = Vector3.one * currentZoom;
    }

    void ZoomOut()
    {
        if (currentZoom <= minZoom) return;

        currentZoom = Mathf.Max(currentZoom / zoomStep, minZoom);
        rect.localScale = Vector3.one * currentZoom;
    }

    void TryGoToPreviousPanel()
    {
        // Only go back if fully zoomed out
        if (currentZoom > minZoom) return;
        if (previousPanel == null) return;
        if (panelManager == null) return;

        panelManager.SwitchToPanel(previousPanel);
    }

    public float GetZoom()
    {
        return currentZoom;
    }

    public void ResetZoom()
    {
        currentZoom = 1f;
        if (rect == null) rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
    }
}
