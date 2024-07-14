using System;
using UnityEngine;

public class StackController : MonoBehaviour
{
    public static Action<GridCell> OnStackPlaced;

    [SerializeField] private LayerMask _hexLayerMask;
    [SerializeField] private LayerMask _gridLayerMask;
    [SerializeField] private LayerMask _groundLayerMask;
    private HexStack _currentStack;
    private Vector3 _currentStackInitialPos;
    private GridCell _targetCell;

    private void Update()
    {
        ManagerControl();
    }

    private void ManagerControl()
    {
        if (Input.GetMouseButtonDown(0)) ManageMouseDown();
        else if (Input.GetMouseButton(0) && _currentStack != null) ManageMouseDrag();
        else if (Input.GetMouseButtonUp(0) && _currentStack != null) ManageMouseUp();
    }

    private void ManageMouseDown()
    {
        Physics.Raycast(GetClickedRay(), out RaycastHit hit, Mathf.Infinity, _hexLayerMask);
        if (hit.collider == null)
        {
            Debug.Log("No hexagon clicked");
            return;
        }

        _currentStack = hit.collider.GetComponentInParent<HexStack>();
        _currentStackInitialPos = _currentStack.transform.position;
    }

    private void ManageMouseDrag()
    {
        Ray ray = GetClickedRay();
        Physics.Raycast(ray, out RaycastHit hit, 500, _gridLayerMask);

        if (hit.collider == null)
            DraggingAboveGround();
        else
            DraggingAboveGridCell(hit);
    }

    private void DraggingAboveGridCell(RaycastHit hit)
    {
        var gridCell = hit.collider.GetComponent<GridCell>();

        if (gridCell.IsOccupied)
            DraggingAboveGround();
        else
            DraggingAboveUnoccupiedGridCell(gridCell);
    }

    private void DraggingAboveUnoccupiedGridCell(GridCell gridCell)
    {
        var curStackTargetPos = gridCell.transform.position + (Vector3.up * 2f);
        _currentStack.transform.position = Vector3.MoveTowards(_currentStack.transform.position, curStackTargetPos, 30 * Time.deltaTime);

        _targetCell = gridCell;
    }

    private void DraggingAboveGround()
    {
        Physics.Raycast(GetClickedRay(), out RaycastHit hit, 500, _groundLayerMask);

        if (hit.collider == null)
        {
            Debug.LogWarning("No ground clicked");
            return;
        }

        var curStackTargetPos = hit.point + (Vector3.up * 2f);
        _currentStack.transform.position = Vector3.MoveTowards(_currentStack.transform.position, curStackTargetPos, 30 * Time.deltaTime);

        _targetCell = null;
    }

    private void ManageMouseUp()
    {
        if(_targetCell == null)
        {
            _currentStack.transform.position = _currentStackInitialPos;
            _currentStack = null;
            return;
        }

        _currentStack.transform.position = _targetCell.transform.position + (Vector3.up * .1f);
        _currentStack.transform.SetParent(_targetCell.transform);

        _targetCell.AssignStack(_currentStack);
        OnStackPlaced?.Invoke(_targetCell);

        _currentStack = null;
        _targetCell = null;
    }

    private Ray GetClickedRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}