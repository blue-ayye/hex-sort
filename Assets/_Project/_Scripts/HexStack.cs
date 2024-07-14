using System;
using System.Collections.Generic;
using UnityEngine;

public class HexStack : MonoBehaviour
{
    public List<Hexagon> Hexagons { get; private set; } = new List<Hexagon>();

    public void Add(Hexagon hexagon)
    {
        Hexagons.Add(hexagon);
        hexagon.SetParent(transform);
    }

    internal Color GetTopHexColor()
    {
        return Hexagons[^1].Color;
    }

    internal void Place()
    {
        foreach (var hexagon in Hexagons)
        {
            hexagon.DisableCollider();
        }
    }

    internal void Remove(Hexagon hex)
    {
        Hexagons.Remove(hex);
        if (Hexagons.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}
