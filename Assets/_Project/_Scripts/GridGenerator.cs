using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _hexagon;

    [SerializeField] private int _gridSize;

    [ContextMenu("Generate Grid")]
    private void GenerateGrid()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        for (int x = -_gridSize; x <= _gridSize; x++)
        {
            for (int y = -_gridSize; y <= _gridSize; y++)
            {
                Vector3 spawnPos = _grid.CellToWorld(new Vector3Int(x, y, 0));

                if (spawnPos.magnitude > _grid.CellToWorld(new Vector3Int(1, 0, 0)).magnitude * _gridSize)
                    continue;

                GameObject hexagon = Instantiate(_hexagon, spawnPos, Quaternion.identity, transform);
            }
        }
    }
}