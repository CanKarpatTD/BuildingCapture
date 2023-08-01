using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Actors;
using Game.GlobalVariables;
using Game.Helpers;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SoldierManager : Manager<SoldierManager>
{
    [Header("*Objects*")]
    public GameObject lvOneSoldier;
    public GameObject lvTwoSoldier;
    public GameObject captureSoldier;

    [Header("*Mats*")]
    public Material blueSoldier;
    public Material greenSoldier;
    public Material redSoldier;
    
    [Header("*Lists*")]
    public List<GameObject> playerSoldiers = new List<GameObject>();
    public List<GameObject> redSoldiers = new List<GameObject>();
    public List<GameObject> greenSoldiers = new List<GameObject>();

    [Header("*Particles*")] public GameObject deadParticle;
    public GameObject playerSoldierSpawn;
    public GameObject redSoldierSpawn,greenSoldierSpawn;

    public bool inTutorial;

    protected override void MB_Listen(bool status)
    {
        if (status)
        {
            GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Restart, GameStatus_Restart);
        }
        else
        {
            GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Restart, GameStatus_Restart);
        }
    }

    protected override void MB_Start()
    {
        gameObject.transform.DOScale(0, 1f).OnComplete(() =>
        {
            inTutorial = ((GameLevel) LevelManager.Instance.levelData).tutorial;
        });
       
    }
    

    private void GameStatus_Restart(object[] arguments)
    {
        playerSoldiers.Clear();
        redSoldiers.Clear();
        greenSoldiers.Clear();
    }
}
