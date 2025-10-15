using UnityEngine;
/// <summary>
/// Static stack group - no movement, just holds stacks together
/// </summary>
public class StaticStackGroup : OctagonStackGroup
{
    protected override void OnGroupInitialized()
    {
        GroupName = "Static Group";
    }

    protected override void UpdateGroupBehavior()
    {
        // No behavior - just a container
    }
}

