using PrimeTween;
using System;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Collider _collider;

    public HexStack HexStack { get; private set; }
    public Color Color { get => _renderer.material.color; set => _renderer.material.color = value; }

    public void Configure(HexStack hexStack)
    {
        HexStack = hexStack;
    }

    internal void DisableCollider()
    {
        _collider.enabled = false;
    }

    internal void MoveToLocal(Vector3 targetPos)
    {
        var delay = transform.GetSiblingIndex() * 0.1f;
        Tween.LocalPosition(transform, targetPos, .2f, ease: Ease.OutBack, startDelay: delay);
    }

    internal void SetParent(Transform value)
    {
        transform.SetParent(value);
    }

    internal void Vanish(float delay)
    {
        Tween.Scale(transform, Vector3.zero, .2f, ease: Ease.InBack, startDelay: delay).OnComplete(() =>
        {
            if (gameObject != null)
                Destroy(gameObject);
        });
    }
}