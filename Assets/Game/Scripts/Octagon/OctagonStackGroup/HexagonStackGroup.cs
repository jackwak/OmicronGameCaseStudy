using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Base class for hexagon stack groups with different behaviors
/// </summary>
public abstract class OctagonStackGroup : MonoBehaviour
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

    protected virtual void OnGroupInitialized()
    {
        // Override in derived classes
    }

    protected abstract void UpdateGroupBehavior();
    

    public void RegisterChildStack(GameObject stack)
    {
        if (!ChildStacks.Contains(stack))
        {
            ChildStacks.Add(stack);
        }
    }
    

    public void UnregisterChildStack(GameObject stack)
    {
        ChildStacks.Remove(stack);
    }
    
    public List<GameObject> GetAliveStacks()
    {
        ChildStacks.RemoveAll(s => s == null);
        return ChildStacks;
    }
}