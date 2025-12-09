using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ItemDropTargetUI : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("Expected Item")]
    public string requiredItemId;

    [Header("Transition")]
    public PanelManager panelManager;
    public ZoomablePanel myPanel;
    public ZoomablePanel targetPanel;
    public bool useAutoZoom = true;

    [Header("Visual After Place")]
    public Image targetImage;

    [Header("Click After Solved")]
    public bool transitionOnDrop = false;
    public bool transitionOnClick = true;

    [Header("Optional Animation On Solved Click")]
    public Animator solvedClickAnimator;
    public string solvedClickTrigger = "Play";
    public float solvedClickDelay = 1f;

    private bool solved = false;
    private bool isTransitionPlaying = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (solved) return;
        if (panelManager == null || myPanel == null)
            return;

        if (panelManager.currentPanel != myPanel)
            return;

        GameObject draggedObj = eventData.pointerDrag;
        if (draggedObj == null) return;

        InventoryItemUI invItem = draggedObj.GetComponent<InventoryItemUI>();
        if (invItem == null) return;

        if (invItem.ItemId != requiredItemId)
        {
            return;
        }

        solved = true;

        Sprite usedSprite = invItem.iconImage != null ? invItem.iconImage.sprite : null;

        if (targetImage != null && usedSprite != null)
        {
            targetImage.sprite = usedSprite;
            targetImage.enabled = true;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearItem();
        }

        if (transitionOnDrop)
        {
            StartTransitionSequence();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!solved)
            return;

        if (!transitionOnClick)
            return;

        if (panelManager == null || myPanel == null || targetPanel == null)
            return;

        StartTransitionSequence();
    }

    void StartTransitionSequence()
    {
        if (isTransitionPlaying)
            return;

        if (panelManager == null || targetPanel == null || myPanel == null)
            return;

        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        isTransitionPlaying = true;

        if (solvedClickAnimator != null && !string.IsNullOrEmpty(solvedClickTrigger))
        {
            solvedClickAnimator.SetTrigger(solvedClickTrigger);
        }

        if (solvedClickDelay > 0f)
        {
            yield return new WaitForSeconds(solvedClickDelay);
        }

        if (useAutoZoom)
        {
            panelManager.SwitchWithAutoZoom(myPanel, targetPanel);
        }
        else
        {
            panelManager.SwitchToPanel(targetPanel);
        }

        isTransitionPlaying = false;
    }
}