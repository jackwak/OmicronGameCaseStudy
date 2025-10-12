using UnityEngine;
using Sirenix.OdinInspector;

public class RotatingStackGroup : HexagonStackGroup
{
    [Title("Rotation Settings")]
    [Range(-360f, 360f)]
    public float RotationSpeed = 45f; // degrees per second
    
    public Vector3 RotationAxis = Vector3.up;
    
    [Title("Pivot Settings")]
    [InfoBox("Pivot determines the rotation center. Stacks will be positioned relative to this pivot.")]
    public PivotPosition PivotPosition = PivotPosition.Center;
    
    protected override void OnGroupInitialized()
    {
        GroupName = "Rotating Group";
    }
    
    protected override void UpdateGroupBehavior()
    {
        transform.Rotate(RotationAxis, RotationSpeed * Time.deltaTime, Space.World);
    }
}

public enum PivotPosition
{
    Center,
    Left,
    Right,
    Top,
    Bottom,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}
