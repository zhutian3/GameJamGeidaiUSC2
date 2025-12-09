using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;

    [Header("Starting Panel")]
    public ZoomablePanel currentPanel;

    [Header("Seamless Transition Settings")]
    [Tooltip("Duration of the zoom-into-hotspot animation")]
    public float seamlessZoomDuration = 0.8f;

    [Header("Legacy Transition Settings")]
    public float autoZoomScale = 2f;
    public float autoZoomDuration = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (currentPanel != null)
        {
            currentPanel.gameObject.SetActive(true);
            currentPanel.ResetZoom();
        }
    }

    public void SwitchToPanel(ZoomablePanel newPanel)
    {
        if (newPanel == null || newPanel == currentPanel) return;

        if (currentPanel != null)
        {
            currentPanel.gameObject.SetActive(false);
        }

        currentPanel = newPanel;
        currentPanel.gameObject.SetActive(true);
        currentPanel.ResetZoom();
    }

    public void SwitchWithAutoZoom(ZoomablePanel fromPanel, ZoomablePanel toPanel)
    {
        if (fromPanel == null || toPanel == null) return;
        PlayAutoZoomTransition(fromPanel, toPanel);
    }

    public void SwitchWithAutoZoom(ZoomablePanel toPanel)
    {
        if (toPanel == null || currentPanel == null) return;
        PlayAutoZoomTransition(currentPanel, toPanel);
    }

    public void PlaySeamlessTransition(ZoomablePanel fromPanel, ZoomablePanel toPanel, RectTransform hotspotRect)
    {
        if (fromPanel == null || toPanel == null || hotspotRect == null)
        {
            Debug.LogError("PlaySeamlessTransition: Missing references!");
            return;
        }
        StartCoroutine(SeamlessTransitionCoroutine(fromPanel, toPanel, hotspotRect));
    }

    IEnumerator SeamlessTransitionCoroutine(ZoomablePanel fromPanel, ZoomablePanel toPanel, RectTransform hotspotRect)
    {
        fromPanel.SetTransitioning(true);

        RectTransform panelRect = fromPanel.GetRectTransform();

        Vector3[] hotspotCorners = new Vector3[4];
        hotspotRect.GetWorldCorners(hotspotCorners);
        Vector3 hotspotWorldCenter = (hotspotCorners[0] + hotspotCorners[2]) / 2f;

        Vector3 localPos3D = panelRect.InverseTransformPoint(hotspotWorldCenter);
        Vector2 hotspotLocalPos = new Vector2(localPos3D.x, localPos3D.y);

        float hotspotWidth = Vector3.Distance(hotspotCorners[0], hotspotCorners[3]);
        float hotspotHeight = Vector3.Distance(hotspotCorners[0], hotspotCorners[1]);
        Vector2 hotspotWorldSize = new Vector2(hotspotWidth, hotspotHeight);

        Canvas canvas = panelRect.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.rect.size;

        float scaleX = canvasSize.x / Mathf.Max(hotspotWorldSize.x, 1f);
        float scaleY = canvasSize.y / Mathf.Max(hotspotWorldSize.y, 1f);
        float scaleNeeded = Mathf.Max(scaleX, scaleY) * 1.15f;

        scaleNeeded = Mathf.Clamp(scaleNeeded, 3f, 15f);

        Vector3 startScale = panelRect.localScale;
        Vector2 startPos = panelRect.anchoredPosition;

        Vector3 targetScale = startScale * scaleNeeded;
        Vector2 targetPos = -hotspotLocalPos * targetScale.x;

        float timer = 0f;

        while (timer < seamlessZoomDuration)
        {
            timer += Time.deltaTime;
            float rawT = timer / seamlessZoomDuration;

            float t = rawT * rawT * (3f - 2f * rawT);

            panelRect.localScale = Vector3.Lerp(startScale, targetScale, t);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        panelRect.localScale = targetScale;
        panelRect.anchoredPosition = targetPos;

        yield return new WaitForSeconds(0.03f);

        Transform canvasTransform = fromPanel.transform.parent;
        toPanel.transform.SetParent(canvasTransform, false);

        fromPanel.gameObject.SetActive(false);
        currentPanel = toPanel;
        toPanel.gameObject.SetActive(true);
        toPanel.ResetZoom();
    }

    public void PlayAutoZoomTransition(ZoomablePanel fromPanel, ZoomablePanel toPanel)
    {
        if (fromPanel == null || toPanel == null) return;
        StartCoroutine(AutoZoomRoutine(fromPanel, toPanel));
    }

    IEnumerator AutoZoomRoutine(ZoomablePanel fromPanel, ZoomablePanel toPanel)
    {
        fromPanel.SetTransitioning(true);

        var rect = fromPanel.GetRectTransform();
        Vector3 startScale = rect.localScale;
        Vector3 targetScale = startScale * autoZoomScale;

        float timer = 0f;

        while (timer < autoZoomDuration)
        {
            timer += Time.deltaTime;
            float t = timer / autoZoomDuration;
            t = t * t * (3f - 2f * t);

            rect.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        fromPanel.gameObject.SetActive(false);
        currentPanel = toPanel;
        toPanel.gameObject.SetActive(true);
        toPanel.ResetZoom();
    }

    public void PlayAutoZoomTransitionToFinal(ZoomablePanel fromPanel, ZoomablePanel toPanel)
    {
        if (fromPanel == null || toPanel == null) return;
        StartCoroutine(AutoZoomToFinalRoutine(fromPanel, toPanel));
    }

    IEnumerator AutoZoomToFinalRoutine(ZoomablePanel fromPanel, ZoomablePanel toPanel)
    {
        fromPanel.SetTransitioning(true);

        var rect = fromPanel.GetRectTransform();
        Vector3 startScale = rect.localScale;
        Vector3 targetScale = startScale * (autoZoomScale * 1.2f);

        float duration = autoZoomDuration * 1.2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = t * t * (3f - 2f * t);

            rect.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        fromPanel.gameObject.SetActive(false);
        currentPanel = toPanel;
        toPanel.gameObject.SetActive(true);
        toPanel.ResetZoom();
    }
}