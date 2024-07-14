using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    private List<GridCell> _updatedCells = new List<GridCell>();

    private void Awake()
    {
        StackController.OnStackPlaced += StackPlacedCallback;
    }

    private void OnDestroy()
    {
        StackController.OnStackPlaced -= StackPlacedCallback;
    }

    private void StackPlacedCallback(GridCell gridCell)
    {
        StartCoroutine(StackPlacedCo(gridCell));
    }

    private IEnumerator StackPlacedCo(GridCell gridCell)
    {
        _updatedCells.Add(gridCell);
        while (_updatedCells.Count > 0)
            yield return CheckForMerge(_updatedCells[0]);
    }

    private IEnumerator CheckForMerge(GridCell gridCell)
    {
        _updatedCells.Remove(gridCell);

        if (!gridCell.IsOccupied) yield break;

        List<GridCell> neighborCells = GetNeighborGridCells(gridCell);

        if (neighborCells.Count == 0) yield break;

        var cellTopHexColor = gridCell.Stack.GetTopHexColor();
        List<GridCell> similarNeighborCells = GetSimilarNeighborCells(neighborCells, cellTopHexColor);

        if (similarNeighborCells.Count == 0) yield break;

        _updatedCells.AddRange(similarNeighborCells);

        List<Hexagon> hexToAdd = GetHexToAdd(cellTopHexColor, similarNeighborCells);

        RemoveHexFromStacks(similarNeighborCells, hexToAdd);

        MoveHex(gridCell, hexToAdd);

        yield return new WaitForSeconds(0.2f + (hexToAdd.Count + 1) * .1f);

        yield return CheckCompleteStack(gridCell, cellTopHexColor);
    }

    private IEnumerator CheckCompleteStack(GridCell gridCell, Color cellTopHexColor)
    {
        if (gridCell.Stack.Hexagons.Count < 10) yield break;
        var similarHexagons = new List<Hexagon>();
        for (int i = gridCell.Stack.Hexagons.Count - 1; i >= 0; i--)
        {
            var hex = gridCell.Stack.Hexagons[i];
            if (hex.Color != cellTopHexColor) break;
            similarHexagons.Add(hex);
        }

        int similarHexCount = similarHexagons.Count;
        if (similarHexagons.Count < 10) yield break;

        float delay = 0f;
        while (similarHexagons.Count > 0)
        {
            gridCell.Stack.Remove(similarHexagons[0]);
            //Destroy(similarHexagons[0].gameObject);
            similarHexagons[0].Vanish(delay);
            delay += 0.1f;
            similarHexagons.RemoveAt(0);
        }

        _updatedCells.Add(gridCell);
        yield return new WaitForSeconds(0.2f + (similarHexCount + 1) * .1f);
    }

    private static void MoveHex(GridCell gridCell, List<Hexagon> hexToAdd)
    {
        var initialY = gridCell.Stack.Hexagons.Count * 0.1f;
        for (int i = 0; i < hexToAdd.Count; i++)
        {
            var hex = hexToAdd[i];
            var targetY = initialY + (i * 0.1f);
            //var targetPos = gridCell.transform.position + (Vector3.up * targetY);
            var targetPos = Vector3.up * targetY;
            gridCell.Stack.Add(hex);
            hex.MoveToLocal(targetPos);
        }
    }

    private static void RemoveHexFromStacks(List<GridCell> similarNeighborCells, List<Hexagon> hexToAdd)
    {
        foreach (var cell in similarNeighborCells)
        {
            var stack = cell.Stack;
            foreach (var hex in hexToAdd)
            {
                if (stack.Hexagons.Contains(hex))
                    stack.Remove(hex);
            }
        }
    }

    private static List<Hexagon> GetHexToAdd(Color cellTopHexColor, List<GridCell> similarNeighborCells)
    {
        var hexToAdd = new List<Hexagon>();
        foreach (var cell in similarNeighborCells)
        {
            var stack = cell.Stack;
            for (int i = stack.Hexagons.Count - 1; i >= 0; i--)
            {
                var hex = stack.Hexagons[i];
                if (hex.Color != cellTopHexColor) break;
                hexToAdd.Add(hex);
                hex.SetParent(null);
            }
        }

        return hexToAdd;
    }

    private static List<GridCell> GetSimilarNeighborCells(List<GridCell> neighborCells, Color cellTopHexColor)
    {
        var similarNeighborCells = new List<GridCell>();
        foreach (var neighborCell in neighborCells)
        {
            var neighborTopHexColor = neighborCell.Stack.GetTopHexColor();
            if (cellTopHexColor == neighborTopHexColor)
                similarNeighborCells.Add(neighborCell);
        }

        return similarNeighborCells;
    }

    private static List<GridCell> GetNeighborGridCells(GridCell gridCell)
    {
        var gridCellMask = 1 << gridCell.gameObject.layer;

        var neighborCells = new List<GridCell>();
        var neighborCellColliders = Physics.OverlapSphere(gridCell.transform.position, 1f, gridCellMask);
        foreach (var collider in neighborCellColliders)
        {
            var neighborCell = collider.GetComponent<GridCell>();
            if (!neighborCell.IsOccupied) continue;
            if (neighborCell == gridCell) continue;
            neighborCells.Add(neighborCell);
        }

        return neighborCells;
    }
}