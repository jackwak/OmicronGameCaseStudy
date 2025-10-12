using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Base class for hexagon stack groups with different behaviors
/// </summary>
public abstract class HexagonStackGroup : MonoBehaviour
{
    [Title("Group Settings")]
    [ReadOnly]
    public string GroupName = "Stack Group";
    
    [Title("Child Stacks")]
    [ReadOnly]
    [InfoBox("These are the spawned stack objects under this group.")]
    public List<GameObject> ChildStacks = new List<GameObject>();
    
    [Title("Group Behavior")]
    [InfoBox("Override behavior in derived classes.")]
    public bool IsActive = true;
    
    protected virtual void Start()
    {
        OnGroupInitialized();
    }
    
    protected virtual void Update()
    {
        if (IsActive)
        {
            UpdateGroupBehavior();
        }
    }
    
    /// <summary>
    /// Called once when the group is spawned
    /// </summary>
    protected virtual void OnGroupInitialized()
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Called every frame - implement group-specific behavior here
    /// </summary>
    protected abstract void UpdateGroupBehavior();
    
    /// <summary>
    /// Register a spawned stack as child
    /// </summary>
    public void RegisterChildStack(GameObject stack)
    {
        if (!ChildStacks.Contains(stack))
        {
            ChildStacks.Add(stack);
        }
    }
    
    /// <summary>
    /// Remove a destroyed stack from children
    /// </summary>
    public void UnregisterChildStack(GameObject stack)
    {
        ChildStacks.Remove(stack);
    }
    
    /// <summary>
    /// Get all alive child stacks
    /// </summary>
    public List<GameObject> GetAliveStacks()
    {
        ChildStacks.RemoveAll(s => s == null);
        return ChildStacks;
    }
}