using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerStateManager : EntityStateManager<Player>
{
    [ClassTypeName(typeof(PlayerState))]
    public string[] states;

    protected override List<EntityState<Player>> GetStateList()
    {
        return PlayerState.CreateListFromStringArray(states);
    }
}
