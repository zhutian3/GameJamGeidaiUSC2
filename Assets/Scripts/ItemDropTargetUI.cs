using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ItemDropTargetUI : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("Expected Item")]
    public string requiredItemId;          // 例如 "rocket"

    [Header("Transition")]
    public PanelManager panelManager;
    public ZoomablePanel myPanel;          // 当前 Panel（比如火星）
    public ZoomablePanel targetPanel;      // 下一个 Panel（比如黑洞内部）
    public bool useAutoZoom = true;        // true = 用 PanelManager 的自动放大切换

    [Header("Visual After Place")]
    public Image targetImage;              // 放置成功后显示的图像（发射架上的火箭等）

    [Header("Click After Solved")]
    public bool transitionOnDrop = false;  // 如果为 true：放置后立刻切场景（默认建议 false）
    public bool transitionOnClick = true;  // 默认为 true：放置成功后点击来触发动画+切场景

    [Header("Optional Animation On Solved Click")]
    public Animator solvedClickAnimator;   // 点击后播放的动画（比如火箭起飞）
    public string solvedClickTrigger = "Play"; // Animator 中的 Trigger 名
    public float solvedClickDelay = 1f;    // 触发动画后等待多久再切场景（秒）

    private bool solved = false;
    private bool isTransitionPlaying = false;

    // 拖拽物品松手时触发
    public void OnDrop(PointerEventData eventData)
    {
        if (solved) return;
        if (panelManager == null || myPanel == null)
            return;

        // 必须当前 Panel 才能放置
        if (panelManager.currentPanel != myPanel)
            return;

        GameObject draggedObj = eventData.pointerDrag;
        if (draggedObj == null) return;

        InventoryItemUI invItem = draggedObj.GetComponent<InventoryItemUI>();
        if (invItem == null) return;

        // 判断是否正确物品
        if (invItem.ItemId != requiredItemId)
        {
            // 错误物品：InventoryItemUI 自己会回到原位置
            return;
        }

        // 正确物品：解谜成功
        solved = true;

        Sprite usedSprite = invItem.iconImage != null ? invItem.iconImage.sprite : null;

        // 在目标位置显示图像（画面补全：比如发射架上出现火箭）
        if (targetImage != null && usedSprite != null)
        {
            targetImage.sprite = usedSprite;
            targetImage.enabled = true;
        }

        // 清空背包（隐藏背包图标）
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearItem();
        }

        // 如果你希望一放置就切场景，可以打开这个开关
        if (transitionOnDrop)
        {
            StartTransitionSequence();
        }
        // 默认是等待玩家再点击一次这个位置（transitionOnClick）
    }

    // 放置成功后的「点击」：类似普通的热点 zoom 行为
    public void OnPointerClick(PointerEventData eventData)
    {
        // 只有解谜成功之后点击才有效
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

        // 1. 播放点击后的动画（比如火箭飞向黑洞）
        if (solvedClickAnimator != null && !string.IsNullOrEmpty(solvedClickTrigger))
        {
            solvedClickAnimator.SetTrigger(solvedClickTrigger);
        }

        // 2. 等一小段时间，让动画飞一会儿 / 播完
        if (solvedClickDelay > 0f)
        {
            yield return new WaitForSeconds(solvedClickDelay);
        }

        // 3. 然后通过 PanelManager 切到下一张 Panel
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
