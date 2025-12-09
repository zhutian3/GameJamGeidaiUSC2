using UnityEngine;
using UnityEngine.EventSystems;

public class TransitionHotspotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Links")]
    public PanelManager panelManager;
    public ZoomablePanel myPanel;
    public ZoomablePanel targetPanel;

    [Header("Zoom Requirement")]
    public float requiredZoom = 2f;

    [Header("Transition Style")]
    public bool useAutoZoom = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (panelManager == null || myPanel == null || targetPanel == null)
            return;

        // Must be on the active panel
        if (panelManager.currentPanel != myPanel)
            return;

        // Must be zoomed in enough
        if (myPanel.GetZoom() < requiredZoom)
            return;

        if (useAutoZoom)
        {
            panelManager.SwitchWithAutoZoom(myPanel, targetPanel);
        }
        else
        {
            panelManager.SwitchToPanel(targetPanel);
        }
    }
}
