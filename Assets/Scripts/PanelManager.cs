using UnityEngine;
using System.Collections;

public class PanelManager : MonoBehaviour
{
    public ZoomablePanel currentPanel;

    [Header("Auto Zoom Settings")]
    public float autoZoomDuration = 0.5f;
    public float autoZoomMultiplier = 2f;

    public void SwitchToPanel(ZoomablePanel newPanel)
    {
        if (newPanel == null) return;
        if (newPanel == currentPanel) return;

        if (currentPanel != null)
            currentPanel.gameObject.SetActive(false);

        currentPanel = newPanel;
        currentPanel.gameObject.SetActive(true);
        currentPanel.ResetZoom();
    }

    public void SwitchWithAutoZoom(ZoomablePanel fromPanel, ZoomablePanel toPanel)
    {
        if (fromPanel == null || toPanel == null) return;
        StartCoroutine(AutoZoomRoutine(fromPanel, toPanel));
    }

    IEnumerator AutoZoomRoutine(ZoomablePanel fromPanel, ZoomablePanel toPanel)
    {
        RectTransform rect = fromPanel.GetComponent<RectTransform>();
        Vector3 startScale = rect.localScale;
        Vector3 zoomScale = startScale * autoZoomMultiplier;

        float t = 0f;
        while (t < autoZoomDuration)
        {
            t += Time.deltaTime;
            rect.localScale = Vector3.Lerp(startScale, zoomScale, t / autoZoomDuration);
            yield return null;
        }

        fromPanel.gameObject.SetActive(false);
        currentPanel = toPanel;
        toPanel.gameObject.SetActive(true);
        toPanel.ResetZoom();
    }
}
