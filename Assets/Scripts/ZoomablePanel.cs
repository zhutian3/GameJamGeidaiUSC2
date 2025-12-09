using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ZoomablePanel : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float minZoom = 1f;
    public float maxZoom = 3f;
    public float zoomStep = 1.5f;
    public float zoomSmoothSpeed = 8f;

    [Header("Previous Panel (for double right-click)")]
    public ZoomablePanel previousPanel;

    RectTransform rect;
    float currentZoom = 1f;
    float targetZoom = 1f;
    Vector2 targetAnchoredPosition = Vector2.zero;
    Vector2 currentAnchoredPosition = Vector2.zero;

    float lastLeftClickTime = 0f;
    float lastRightClickTime = 0f;
    float doubleClickThreshold = 0.3f;

    bool hotspotClaimedDoubleClick = false;
    bool isTransitioning = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        currentZoom = 1f;
        targetZoom = 1f;
        currentAnchoredPosition = Vector2.zero;
        targetAnchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (!isTransitioning)
        {
            HandleMouse();
        }
        AnimateZoom();
    }

    void LateUpdate()
    {
        hotspotClaimedDoubleClick = false;
    }

    bool IsPointerOverUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            SeamlessHotspot hotspot = result.gameObject.GetComponent<SeamlessHotspot>();
            if (hotspot != null)
            {
                return true;
            }
        }
        return false;
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIElement())
            {
                return;
            }

            float t = Time.time - lastLeftClickTime;
            if (t <= doubleClickThreshold && t > 0f)
            {
                if (!hotspotClaimedDoubleClick)
                {
                    ZoomIn();
                }
                lastLeftClickTime = 0f;
            }
            else
            {
                lastLeftClickTime = Time.time;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            float t = Time.time - lastRightClickTime;
            if (t <= doubleClickThreshold && t > 0f)
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

    void AnimateZoom()
    {
        if (!Mathf.Approximately(currentZoom, targetZoom))
        {
            currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSmoothSpeed);

            if (Mathf.Abs(currentZoom - targetZoom) < 0.001f)
            {
                currentZoom = targetZoom;
            }

            rect.localScale = Vector3.one * currentZoom;
        }

        if (Vector2.Distance(currentAnchoredPosition, targetAnchoredPosition) > 0.01f)
        {
            currentAnchoredPosition = Vector2.Lerp(currentAnchoredPosition, targetAnchoredPosition, Time.deltaTime * zoomSmoothSpeed);

            if (Vector2.Distance(currentAnchoredPosition, targetAnchoredPosition) < 0.1f)
            {
                currentAnchoredPosition = targetAnchoredPosition;
            }

            rect.anchoredPosition = currentAnchoredPosition;
        }
    }

    void ZoomIn()
    {
        if (targetZoom >= maxZoom) return;
        targetZoom = Mathf.Min(targetZoom * zoomStep, maxZoom);
    }

    void ZoomOut()
    {
        if (targetZoom <= minZoom) return;
        targetZoom = Mathf.Max(targetZoom / zoomStep, minZoom);

        float zoomRatio = targetZoom / Mathf.Max(currentZoom, 0.001f);
        targetAnchoredPosition *= zoomRatio;
    }

    void TryGoToPreviousPanel()
    {
        if (targetZoom > minZoom + 0.01f) return;
        if (previousPanel == null) return;

        PanelManager.Instance.SwitchToPanel(previousPanel);
    }

    public float GetZoom()
    {
        return currentZoom;
    }

    public void ResetZoom()
    {
        currentZoom = 1f;
        targetZoom = 1f;
        currentAnchoredPosition = Vector2.zero;
        targetAnchoredPosition = Vector2.zero;
        if (rect == null) rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.anchoredPosition = Vector2.zero;
        isTransitioning = false;
    }

    public void ClaimDoubleClick()
    {
        hotspotClaimedDoubleClick = true;
    }

    public void SetTransitioning(bool value)
    {
        isTransitioning = value;
    }

    public RectTransform GetRectTransform()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        return rect;
    }
}