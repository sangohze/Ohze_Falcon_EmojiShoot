using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIMoveLeftRight : UIElementAnim
{
    public enum Direction { Left, Right, Up, Down }

    [Header("Move Settings")]
    [SerializeField] private Direction moveDirection = Direction.Left;
    [SerializeField] private float moveDistance = 500f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private RectTransform rectTrf;
    private Vector3 showPos;
    private Vector3 hidePos;

    protected void Awake()
    {
        rectTrf = GetComponent<RectTransform>();
        showPos = rectTrf.anchoredPosition3D;
        hidePos = CalculateHidePos(showPos, moveDirection, moveDistance);
    }

    private Vector3 CalculateHidePos(Vector3 basePos, Direction dir, float dist)
    {
        switch (dir)
        {
            case Direction.Left: return basePos + Vector3.left * dist;
            case Direction.Right: return basePos + Vector3.right * dist;
            case Direction.Up: return basePos + Vector3.up * dist;
            case Direction.Down: return basePos + Vector3.down * dist;
            default: return basePos;
        }
    }

    public override void Show()
    {
        if (!gameObject.activeInHierarchy) return;

        rectTrf.anchoredPosition3D = hidePos;
        Tweener = rectTrf.DOAnchorPos3D(showPos, Duration).SetEase(ease);
    }

    public override void Hide()
    {
        if (!gameObject.activeInHierarchy) return;

        Tweener = rectTrf.DOAnchorPos3D(hidePos, Duration).SetEase(ease);
    }

    private void OnDisable()
    {
        Tweener?.Kill();
    }
}
