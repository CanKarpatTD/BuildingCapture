using System.Collections;
using System.Collections.Generic;
using Game.GlobalVariables;
using Game.Managers;
using TriflesGames.ManagerFramework;
using UnityEngine;

public class PlayerSoldierActor : Actor<SoldierManager>
{
    public enum SoldierLevel
    {
        Level1,
        Level2
    }

    public SoldierLevel soldierLevel;
    public int damage;
    
    protected override void MB_Start()
    {
        if (soldierLevel == SoldierLevel.Level1)
        {
            damage = 25;
        }
        if (soldierLevel == SoldierLevel.Level2)
        {
            damage = 50;
        }
        
        Push(CustomManagerEvents.DamageValue, damage);
    }
    
    protected override void MB_Listen(bool status)
    {
        if (status)
        {
            
        }
        else
        {
            
        }
    }
}
