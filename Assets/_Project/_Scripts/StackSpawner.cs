using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StackSpawner : MonoBehaviour
{
    [SerializeField] private Transform _stackPositions;
    [SerializeField] private Hexagon _hexPrefab;
    [SerializeField] private HexStack _hexStackPrefab;
    [SerializeField] private Color[] _colors;
    [SerializeField] private Vector2Int _minMaxHexCount;

    private int _stackCount;

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
        _stackCount++;
        if (_stackCount >= 3)
        {
            _stackCount = 0;
            GenerateStacks();
        }
    }

    private void Start()
    {
        GenerateStacks();
    }

    private void GenerateStacks()
    {
        for (int i = 0; i < _stackPositions.childCount; i++)
        {
            GenerateStack(_stackPositions.GetChild(i));
        }
    }

    private void GenerateStack(Transform parent)
    {
        var hexStack = Instantiate(_hexStackPrefab, parent.position, Quaternion.identity, parent);
        hexStack.name = $"Stack {parent.GetSiblingIndex()}";

        var stackColor = _colors[Random.Range(0, _colors.Length)];

        int amount = Random.Range(_minMaxHexCount.x, _minMaxHexCount.y);

        int firstColorHexCount = Random.Range(0, amount); //??
        var colors = GetRandomColors();

        for (int i = 0; i < amount; i++)
        {
            Vector3 hexLocPos = Vector3.up * i * 0.1f;
            Vector3 spawnPos = hexStack.transform.TransformPoint(hexLocPos);
            var hexInstance = Instantiate(_hexPrefab, spawnPos, Quaternion.identity, hexStack.transform);
            hexInstance.Color = i < firstColorHexCount ? colors[0] : colors[1];
            hexInstance.Configure(hexStack);

            hexStack.Add(hexInstance);
        }
    }

    private Color[] GetRandomColors()
    {
        var colorList = new List<Color>();
        colorList.AddRange(_colors);

        if (colorList.Count == 0)
        {
            Debug.LogError("No colors available");
            return null;
        }

        Color firstColor = colorList.OrderBy(x => Random.value).First();
        colorList.Remove(firstColor);

        if (colorList.Count == 0)
        {
            Debug.LogError("No colors available");
            return null;
        }

        Color secondColor = colorList.OrderBy(x => Random.value).First();

        return new Color[] { firstColor, secondColor };
    }
}
