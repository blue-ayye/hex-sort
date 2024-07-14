using System;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public HexStack Stack { get; private set; }

    public bool IsOccupied => Stack != null;


    public void AssignStack(HexStack currentStack)
    {
        Stack = currentStack;
        Stack.Place();
    }
}
