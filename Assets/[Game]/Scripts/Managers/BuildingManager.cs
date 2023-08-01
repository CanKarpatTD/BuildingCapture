using System.Collections;
using System.Collections.Generic;
using Game.Actors;
using Game.GlobalVariables;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : Manager<BuildingManager>
{
    // [Header("* Building Variables *")] 
    
    [Header("* Build Variables *")]
    [Space(10)] public List<GameObject> playerBuildings = new List<GameObject>();
    [Space(10)] public List<GameObject> freeBuildings = new List<GameObject>();
    [Space(10)] public List<GameObject> greenEnemyBuildings = new List<GameObject>();
    [Space(10)] public List<GameObject> redEnemyBuildings = new List<GameObject>();

    public List<GameObject> playerBases = new List<GameObject>();
    public List<GameObject> redBases = new List<GameObject>();
    public List<GameObject> greenBases = new List<GameObject>();
    
    [Header("* Mechanic Variables *")]
    public float selectedObjectHeight;
    [Space(15)]public LayerMask dragObjectsLayer, groundsLayer;

    [Header("* Materials *")] public GameObject smoke;
    public Material player, free, green, red;

    public Material baseBlue, baseFree, baseGreen, baseRed;

    public bool destructionActive;
    public Material whiteMat;
    
    [Header("UI Factors")] 
    [Space(5)]public Sprite mainBlue;
    public Sprite mainGreen, mainRed, freeZone;
    [Space(10)] public Sprite mainLv2Blue;
    public Sprite mainLv2Green,mainLv2Red;
    [Space(5)]public Color blueC, redC, greenC;

    public int soldierCount;
    public int soldierNormal,soldierBase;
    
    protected override void MB_Start()
    {
        StartCoroutine(DestructionCheck());
    }

    protected override void MB_Update()
    {
        if (soldierNormal <= 0)
            soldierNormal = 0;

        if (soldierBase <= 0)
            soldierBase = 0;

        if (soldierCount <= 0)
            soldierCount = 0;
    }

    public void Soldiers()
    {
        soldierCount = soldierBase + soldierNormal;
    }
    
    protected override void MB_Listen(bool status)
    {
        if (status)
        {
            GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Init, GameStatus_Init);
            GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Restart, GameStatus_Restart);
            GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Play,DestructionCheckSecond);
        }
        else
        {
            GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Init, GameStatus_Init);
            GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Restart, GameStatus_Restart);
        }
    }

    private void DestructionCheckSecond(object[] arguments)
    {
        if (redBases.Count == 0 || greenBases.Count == 0)
            destructionActive = true;
        else
        {
            destructionActive = false;
        }
    }

    public void DestructionActive()
    {
        if (redBases.Count == 0 || greenBases.Count == 0) 
            destructionActive = true;
    }

    IEnumerator DestructionCheck()
    {
        yield return new WaitForSeconds(1f);
        if (redBases.Count == 0 || greenBases.Count == 0)
            destructionActive = true;
    }
    
    
    private void GameStatus_Init(object[] args)
    {
        
    }

    private void GameStatus_Restart(object[] args)
    {
        DestructionActive();
        playerBuildings.Clear();
        greenEnemyBuildings.Clear();
        redEnemyBuildings.Clear();
        freeBuildings.Clear();
        
        greenBases.Clear();
        redBases.Clear();
        playerBases.Clear();
    }
}
