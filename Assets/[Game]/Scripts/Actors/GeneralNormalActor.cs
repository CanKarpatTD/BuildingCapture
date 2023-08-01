using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.GlobalVariables;
using Game.Helpers;
using TMPro;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Actors
{
    public class GeneralNormalActor : Actor<BuildingManager>
    {
        public enum BuildType
        {
            None, Player, EnemyGreen, EnemyRed,FreeZone
        }
        public BuildType buildType;
        public int colorParse;
        
        public enum Level
        {
            Level1,Level2
        }
        public Level level;
        
        public GameObject level1, level2;

        [HideInInspector]public NormalLevelOneActor normalLevelOneActor;
        [HideInInspector]public NormalLevelTwoActor normalLevelTwoActor;
        
        [HideInInspector][Header("* Mesh Objects *")] public List<MeshRenderer> buildUvMesh;
        public GameObject baseGround;

        public int buildHealth;
        
        [Space(10)][Header("Soldier Factors")]
        public int activeSoldier;
        [HideInInspector]public int soldierCapacity;
        [HideInInspector]public List<Transform> spawnPosesPlayer = new List<Transform>();
        [HideInInspector]public List<Transform> spawnPosesEnemy = new List<Transform>();
        [HideInInspector]public int hitCount;
        private float spawnTime;
        
        [HideInInspector][Header("UI Factors")] public Transform canvas; 
        public GameObject fourCanvas;
        public GameObject eightCanvas;
        public TextMeshProUGUI soldierText,soldierLv2Text;
        [HideInInspector][Space(20)]public List<Image> colorsMain = new List<Image>();
        // [HideInInspector]public List<Image> colorsBtn = new List<Image>();
        // [HideInInspector]public List<GameObject> btnLv1 = new List<GameObject>();
        // [HideInInspector]public List<GameObject> btnLv2 = new List<GameObject>();

        [HideInInspector]public bool canCapture;
        private float enemySpawnTimer;
        public float spawnAiTime;
        
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Start,SetUI);
                Manager.Subscribe(CustomManagerEvents.SpawnSoldier,SpawnSoldier);
                SoldierManager.Instance.Subscribe(CustomManagerEvents.GiveDamage, TakeDamage);
                SoldierManager.Instance.Subscribe(CustomManagerEvents.SoldierAdd,AddSoldier);
            }
            else
            {
                GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Start,SetUI);
                Manager.Unsubscribe(CustomManagerEvents.SpawnSoldier,SpawnSoldier);
                SoldierManager.Instance.Unsubscribe(CustomManagerEvents.GiveDamage, TakeDamage);
                SoldierManager.Instance.Unsubscribe(CustomManagerEvents.SoldierAdd,AddSoldier);
            }
        }

        private void SetUI(object[] arguments)
        {
            canvas.DOLookAt(Camera.main.transform.position, 0.1f);
            
            var sit = ((GameLevel) LevelManager.Instance.levelData).tutorial;

            
            if (sit)
            {
                // spawnAiTime = 10;
                enemySpawnTimer = 10;
            }
            else
            {
                // spawnAiTime = 4.5f;
                enemySpawnTimer = .7f;
            }
        }

        protected override void MB_Start()
        {
            CheckTowerSituation();
            CheckHealth();
            spawnTime = 3;
            // enemySpawnTimer = 4.5f;
        }

        protected override void MB_Update()
        {
            canvas.DOLookAt(Camera.main.transform.position, 0.1f);
            
            if (level == Level.Level1)
                if (activeSoldier >= 4)
                {
                    activeSoldier = 4;
                    CheckSoldierUI(true,true,true);
                }

            
            if (level == Level.Level2)
                if (activeSoldier >= 8)
                {
                    activeSoldier = 8;
                    CheckSoldierUI(true,true,true);
                }


            if (activeSoldier <= 0)
            {
                activeSoldier = 0;
                CheckSoldierUI(true,true,true);
            }
                
            

            if (buildHealth <= 0)
            {
                canCapture = true;
                buildHealth = 0;
            }



            if (buildType != BuildType.FreeZone)
            {
                var sit = ((GameLevel) LevelManager.Instance.levelData).tutorial;

                if (!sit)
                {
                    if (GameManager.Instance.IsGameStarted)
                    {
                        
                            spawnTime -= 1 * Time.deltaTime;

                            if (spawnTime <= 0)
                            {
                                if (buildType == BuildType.Player)
                                    spawnTime = 2.3f;
                                else
                                {
                                    spawnTime = .7f;
                                }

                                activeSoldier++;
                                if (level == Level.Level1)
                                {
                                    TowerUpWindowEffect(true, false);
                                }

                                if (level == Level.Level2)
                                {
                                    TowerUpWindowEffect(false, true);
                                }
                            }
                        
                    }
                }
                else
                {
                    if (colorParse == 0)
                    {
                            spawnTime -= 1 * Time.deltaTime;

                            if (spawnTime <= 0)
                            {
                                if (buildType == BuildType.Player)
                                    spawnTime = 2.3f;
                                else
                                {
                                    spawnTime = .7f;
                                }

                                activeSoldier++;
                                Manager.soldierNormal++;
                                Manager.Soldiers();
                                Manager.Soldiers();
                                if (level == Level.Level1)
                                {
                                    TowerUpWindowEffect(true, false);
                                }

                                if (level == Level.Level2)
                                {
                                    TowerUpWindowEffect(false, true);
                                }
                            }
                        
                    }
                }
            }
            
               
                    enemySpawnTimer -= 1 * Time.deltaTime;

                    if (enemySpawnTimer <= 0)
                    {
                        enemySpawnTimer = spawnAiTime;

                        if (buildType == BuildType.EnemyGreen)
                        {
                            var randStyle = Random.Range(0, 2);

                            if (randStyle == 0) //Capture
                            {
                                if (level == Level.Level1)
                                    SpawnSoldierSpawn(1, true, SoldierManager.Instance.captureSoldier);
                                if (level == Level.Level2)
                                    SpawnSoldierSpawn(2, true, SoldierManager.Instance.captureSoldier);
                            }

                            if (randStyle == 1) //Attack
                            {
                                if (level == Level.Level1)
                                    SpawnSoldierSpawn(1, false, SoldierManager.Instance.lvOneSoldier);
                                if (level == Level.Level2)
                                    SpawnSoldierSpawn(2, false, SoldierManager.Instance.lvTwoSoldier);
                            }
                        }

                        if (buildType == BuildType.EnemyRed)
                        {
                            var randStyle = Random.Range(0, 2);

                            if (randStyle == 0) //Capture
                            {
                                if (level == Level.Level1)
                                    SpawnSoldierSpawn(1, true, SoldierManager.Instance.captureSoldier);
                                if (level == Level.Level2)
                                    SpawnSoldierSpawn(2, true, SoldierManager.Instance.captureSoldier);
                            }

                            if (randStyle == 1) //Attack
                            {
                                if (level == Level.Level1)
                                    SpawnSoldierSpawn(1, false, SoldierManager.Instance.lvOneSoldier);
                                if (level == Level.Level2)
                                    SpawnSoldierSpawn(2, false, SoldierManager.Instance.lvTwoSoldier);
                            }
                        }
                    }
                
            
        }

        public void CheckTowerSituation()
        {
            if (buildType == BuildType.Player)
            {
                    if (!Manager.playerBuildings.Contains(gameObject))
                        Manager.playerBuildings.Add(gameObject);
                    if (Manager.redEnemyBuildings.Contains(gameObject))
                        Manager.redEnemyBuildings.Remove(gameObject);
                    if (Manager.greenEnemyBuildings.Contains(gameObject))
                        Manager.greenEnemyBuildings.Remove(gameObject);
                    if (Manager.freeBuildings.Contains(gameObject))
                        Manager.freeBuildings.Remove(gameObject);
                

                ChangeAllMat(Manager.player,0);
                
                gameObject.layer = 6;
                colorParse = 0;
                canvas.gameObject.SetActive(true);
                Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
            }
            
            if (buildType == BuildType.EnemyGreen)
            {
                    if (!Manager.greenEnemyBuildings.Contains(gameObject))
                        Manager.greenEnemyBuildings.Add(gameObject);

                    if (Manager.playerBuildings.Contains(gameObject))
                        Manager.playerBuildings.Remove(gameObject);

                    if (Manager.redEnemyBuildings.Contains(gameObject))
                        Manager.redEnemyBuildings.Remove(gameObject);

                    if (Manager.freeBuildings.Contains(gameObject))
                        Manager.freeBuildings.Remove(gameObject);
                

                ChangeAllMat(Manager.green,2);
                
                gameObject.layer = 8;
                colorParse = 2;
                canvas.gameObject.SetActive(true);
                Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
            }
            
            if (buildType == BuildType.EnemyRed)
            {
                    if (!Manager.redEnemyBuildings.Contains(gameObject))
                        Manager.redEnemyBuildings.Add(gameObject);

                    if (Manager.playerBuildings.Contains(gameObject))
                        Manager.playerBuildings.Remove(gameObject);

                    if (Manager.greenEnemyBuildings.Contains(gameObject))
                        Manager.greenEnemyBuildings.Remove(gameObject);

                    if (Manager.freeBuildings.Contains(gameObject))
                        Manager.freeBuildings.Remove(gameObject);
                

                ChangeAllMat(Manager.red,1);
                
                gameObject.layer = 8;
                colorParse = 1;
                canvas.gameObject.SetActive(true);
                Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
            }

            if (buildType == BuildType.FreeZone)
            {
                    if (!Manager.freeBuildings.Contains(gameObject))
                        Manager.freeBuildings.Add(gameObject);
                

                ChangeAllMat(Manager.free,3);
                
                canCapture = true;
                
                gameObject.layer = 8;
                colorParse = 3;
                canvas.gameObject.SetActive(false);
                Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
            }
            
            if (level == Level.Level1)
            {
                soldierCapacity = 4;

                soldierText.text = activeSoldier + "/" + soldierCapacity;
                soldierLv2Text.text = activeSoldier + "/" + soldierCapacity;
                fourCanvas.SetActive(true);
                eightCanvas.SetActive(false);
            }

            if (level == Level.Level2)
            {
                soldierCapacity = 8;
                
                level1.SetActive(false);
                level2.SetActive(true);
                
                soldierText.text = activeSoldier + "/" + soldierCapacity;
                soldierLv2Text.text = activeSoldier + "/" + soldierCapacity;
                fourCanvas.SetActive(false);
                eightCanvas.SetActive(true);
            }
        }

        public void CheckHealth()
        {
            buildHealth = activeSoldier * 100;

            if (buildHealth <= 0)
            {
                canCapture = true;
                buildHealth = 0;
            }

            if (buildHealth >= activeSoldier * 100)
            {
                buildHealth = activeSoldier * 100;
            }
        }
        
        private void ChangeAllMat(Material mat,int parse) // 0=Player / 1=red / 2=green / 3=red
        {
            foreach (var mesh in buildUvMesh)
            {
                mesh.material = mat;
            }

            if (parse == 0)
            {
                if (level == Level.Level1)
                {
                    foreach (var mainC in colorsMain)
                    {
                        mainC.sprite = Manager.mainBlue;
                    } 
                }
                
                if (level == Level.Level2)
                {
                    foreach (var mainC in colorsMain)
                    {
                        mainC.sprite = Manager.mainLv2Blue;
                    } 
                }
                
                soldierText.color = Manager.blueC;
                soldierLv2Text.color = Manager.blueC;
            }

            if (parse == 1)
            {
                if (level == Level.Level1)
                {
                    foreach (var mainC in colorsMain)
                    {
                        mainC.sprite = Manager.mainRed;
                    } 
                }
                if (level == Level.Level2)
                {
                    foreach (var mainC in colorsMain)
                    {
                        mainC.sprite = Manager.mainLv2Red;
                    } 
                }

                soldierText.color = Manager.redC;
                soldierLv2Text.color = Manager.redC;
            }
            if (parse == 2)
            {
                if (level == Level.Level1)
                {
                    foreach (var mainC in colorsMain)
                    {
                        mainC.sprite = Manager.mainGreen;
                    } 
                }
                if (level == Level.Level2)
                {
                    foreach (var mainC in colorsMain)
                    {
                        mainC.sprite = Manager.mainLv2Green;
                    } 
                }
                
                soldierText.color = Manager.greenC;
                soldierLv2Text.color = Manager.greenC;
            }
        }

        
        
        private void TakeDamage(object[] args)
        {
            var obj = (GameObject) args[0];

            var value = (int) args[1];
            
            var color = (int) args[3];

            if (obj.name == gameObject.name && color != colorParse) // Layer değiştirmeyi unutma ve aynı şeyden çıkanları çarptırma
            {
                buildHealth -= value;
                hitCount++;
                
                print(gameObject.transform.GetChild(0));
                
                // gameObject.transform.GetChild(0).transform.DOPunchScale(new Vector3(.1f, .1f, .1f), .2f).SetEase(Ease.OutQuad).OnComplete(
                //     () => { gameObject.transform.GetChild(0).localScale = new Vector3(1, 1, 1);});
                
                if (level == Level.Level1)
                {
                    normalLevelOneActor.Fade();
                }

                if (level == Level.Level2)
                {
                    normalLevelTwoActor.Fade();
                }

                if (hitCount >= 2)
                {
                    activeSoldier--;
                    Manager.soldierNormal--;
                    Manager.Soldiers();
                    if (level == Level.Level1)
                    {
                        TowerDownWindowEffect(true, false);
                    }

                    if (level == Level.Level2)
                    {
                        TowerDownWindowEffect(false, true);
                    }
                    hitCount = 0;
                }
                
                if (buildHealth <= 0)//TODO: 0 ise sabit kalacak ve (bool)capture aktif olacak. aktif olduğu zaman düşman veya bizim oyuncumuz girip rengini değiştirecek.
                {
                    canCapture = true;
                    
                    print("Can Capture");
                    
                    // CheckTowerSituation();
                }
            }
        }

        public void TowerDownWindowEffect(bool lv1, bool lv2)
        {
            if (lv1)
            {
                normalLevelOneActor.SoldierOut(activeSoldier);
                CheckSoldierUI(false,false,false);
            }

            if (lv2)
            {
                normalLevelTwoActor.SoldierOut(activeSoldier);
                CheckSoldierUI(false,true,false);
            }
        }

        private void SpawnSoldier(object[] args)
        {
                var value = (bool) args[0];
                if (colorParse == 0)
                {
                    gameObject.transform.DOPunchScale(new Vector3(0.1f, -0.1f, 0.1f), 0.15f);

                    if (activeSoldier != 0)
                    {
                        activeSoldier--;
                        Manager.soldierNormal--;
                        Manager.Soldiers();
                        if (level == Level.Level1)
                        {
                            if (colorParse == 0)
                            {
                                if (value)
                                {
                                    SpawnSoldierSpawn(1, value, SoldierManager.Instance.captureSoldier);
                                }
                                else
                                {
                                    SpawnSoldierSpawn(1, value, SoldierManager.Instance.lvOneSoldier);
                                }
                            }

                            TowerDownWindowEffect(true, false);
                        }

                        if (level == Level.Level2)
                        {
                            if (colorParse == 0)
                            {
                                if (value)
                                {
                                    SpawnSoldierSpawn(2, value, SoldierManager.Instance.captureSoldier);
                                }
                                else
                                {
                                    SpawnSoldierSpawn(2, value, SoldierManager.Instance.lvTwoSoldier);
                                }
                            }

                            TowerDownWindowEffect(false, true);
                        }
                    }
                }

                CheckHealth();
            
        }
        
        private void SpawnSoldierSpawn(int lv,bool action,GameObject sldr)
        {
                if (lv == 1)
                {
                    if (buildType == BuildType.Player)
                    {
                        var soldier = Instantiate(sldr,
                            spawnPosesPlayer[Random.Range(0, spawnPosesPlayer.Count)].position, Quaternion.identity);

                        Instantiate(SoldierManager.Instance.playerSoldierSpawn, soldier.transform.position,
                            Quaternion.identity);

                        if (action)
                        {
                            soldier.GetComponent<SoldierActor>().capture = true;
                            soldier.GetComponent<SoldierActor>().attack = false;
                        }
                        else
                        {
                            soldier.GetComponent<SoldierActor>().capture = false;
                            soldier.GetComponent<SoldierActor>().attack = true;
                        }

                        soldier.GetComponent<SoldierActor>().myTower = gameObject;

                        if (colorParse == 0)
                        {
                            SoldierManager.Instance.playerSoldiers.Add(soldier);
                            soldier.GetComponent<SoldierActor>().ChangeMat(SoldierManager.Instance.blueSoldier);
                            soldier.GetComponent<SoldierActor>().typeParse = 0;
                        }
                    }
                    else
                    {
                        var soldier = Instantiate(sldr,
                            spawnPosesEnemy[Random.Range(0, spawnPosesEnemy.Count)].position, Quaternion.identity);

                        soldier.GetComponent<SoldierActor>().myTower = gameObject;

                        if (action)
                        {
                            soldier.GetComponent<SoldierActor>().capture = true;
                            soldier.GetComponent<SoldierActor>().attack = false;
                        }
                        else
                        {
                            soldier.GetComponent<SoldierActor>().capture = false;
                            soldier.GetComponent<SoldierActor>().attack = true;
                        }

                        if (colorParse == 1)
                        {
                            Instantiate(SoldierManager.Instance.redSoldierSpawn, soldier.transform.position,
                                Quaternion.identity);
                            SoldierManager.Instance.redSoldiers.Add(soldier);
                            soldier.GetComponent<SoldierActor>().ChangeMat(SoldierManager.Instance.redSoldier);
                            soldier.GetComponent<SoldierActor>().typeParse = 1;
                        }

                        if (colorParse == 2)
                        {
                            Instantiate(SoldierManager.Instance.greenSoldierSpawn, soldier.transform.position,
                                Quaternion.identity);
                            SoldierManager.Instance.greenSoldiers.Add(soldier);
                            soldier.GetComponent<SoldierActor>().ChangeMat(SoldierManager.Instance.greenSoldier);
                            soldier.GetComponent<SoldierActor>().typeParse = 2;
                        }
                    }
                }

                if (lv == 2)
                {
                    if (buildType == BuildType.Player)
                    {
                        var soldier = Instantiate(sldr,
                            spawnPosesPlayer[Random.Range(0, spawnPosesPlayer.Count)].position, Quaternion.identity);

                        Instantiate(SoldierManager.Instance.playerSoldierSpawn, soldier.transform.position,
                            Quaternion.identity);
                        soldier.GetComponent<SoldierActor>().myTower = gameObject;

                        if (action)
                        {
                            soldier.GetComponent<SoldierActor>().capture = true;
                            soldier.GetComponent<SoldierActor>().attack = false;
                        }
                        else
                        {
                            soldier.GetComponent<SoldierActor>().capture = false;
                            soldier.GetComponent<SoldierActor>().attack = true;
                        }

                        if (colorParse == 0)
                        {
                            SoldierManager.Instance.playerSoldiers.Add(soldier);
                            soldier.GetComponent<SoldierActor>().ChangeMat(SoldierManager.Instance.blueSoldier);
                            soldier.GetComponent<SoldierActor>().typeParse = 0;
                        }
                    }
                    else
                    {
                        var soldier = Instantiate(sldr,
                            spawnPosesEnemy[Random.Range(0, spawnPosesEnemy.Count)].position, Quaternion.identity);
                        soldier.GetComponent<SoldierActor>().myTower = gameObject;

                        if (action)
                        {
                            soldier.GetComponent<SoldierActor>().capture = true;
                            soldier.GetComponent<SoldierActor>().attack = false;
                        }
                        else
                        {
                            soldier.GetComponent<SoldierActor>().capture = false;
                            soldier.GetComponent<SoldierActor>().attack = true;
                        }

                        if (colorParse == 1)
                        {
                            Instantiate(SoldierManager.Instance.redSoldierSpawn, soldier.transform.position,
                                Quaternion.identity);
                            SoldierManager.Instance.redSoldiers.Add(soldier);
                            soldier.GetComponent<SoldierActor>().ChangeMat(SoldierManager.Instance.redSoldier);
                            soldier.GetComponent<SoldierActor>().typeParse = 1;
                        }

                        if (colorParse == 2)
                        {
                            Instantiate(SoldierManager.Instance.greenSoldierSpawn, soldier.transform.position,
                                Quaternion.identity);
                            SoldierManager.Instance.greenSoldiers.Add(soldier);
                            soldier.GetComponent<SoldierActor>().ChangeMat(SoldierManager.Instance.greenSoldier);
                            soldier.GetComponent<SoldierActor>().typeParse = 2;
                        }
                    }
                }
            
        }
        
        private void AddSoldier(object[] args)
        {
            canCapture = false;
            var obj = (GameObject) args[0];
            var typeParse = (int) args[2];
            
            if (obj.name == gameObject.name)
            {
                if (typeParse == colorParse)
                {
                    Destroy((GameObject) args[1]);
                    activeSoldier++;
                    Manager.soldierNormal++;
                    Manager.Soldiers();
                    CheckHealth();
                    if (level == Level.Level1)
                        TowerUpWindowEffect(true, false);
                    if (level == Level.Level2)
                        TowerUpWindowEffect(false, true);
                }
                else
                {
                    if (typeParse == 0)
                    {
                        buildType = BuildType.Player;
                        ChangeAllMat(Manager.player,0);
                        
                        if(Manager.greenEnemyBuildings.Contains(gameObject))
                            Manager.greenEnemyBuildings.Remove(gameObject);
                        
                        if(Manager.redEnemyBuildings.Contains(gameObject))
                            Manager.redEnemyBuildings.Remove(gameObject);
                        
                        Manager.playerBuildings.Add(gameObject);
                        colorParse = 0;
                        gameObject.layer = 6;
                        Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
                    }
                    
                    if (typeParse == 1)
                    {
                        buildType = BuildType.EnemyRed;
                        ChangeAllMat(Manager.red,1);
                        
                        if(Manager.greenEnemyBuildings.Contains(gameObject))
                            Manager.greenEnemyBuildings.Remove(gameObject);
                        
                        if(Manager.playerBuildings.Contains(gameObject))
                            Manager.playerBuildings.Remove(gameObject);
                        
                        Manager.redEnemyBuildings.Add(gameObject);
                        colorParse = 1;
                        gameObject.layer = 8;
                        Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
                    }
                    
                    if (typeParse == 2)
                    {
                        buildType = BuildType.EnemyGreen;
                        ChangeAllMat(Manager.green,2);
                        
                        if(Manager.playerBuildings.Contains(gameObject))
                            Manager.playerBuildings.Remove(gameObject);
                        
                        if(Manager.redEnemyBuildings.Contains(gameObject))
                            Manager.redEnemyBuildings.Remove(gameObject);
                        
                        Manager.greenEnemyBuildings.Add(gameObject);
                        colorParse = 2;
                        gameObject.layer = 8;
                        Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
                    }
                    
                    CheckTowerSituation();
                    
                    activeSoldier++;
                    Manager.soldierNormal++;
                    Manager.Soldiers();
                    CheckHealth();
                    if (level == Level.Level1)
                        TowerUpWindowEffect(true, false);
                    if (level == Level.Level2)
                        TowerUpWindowEffect(false, true);
                }
            }
        }
        
        public void TowerUpWindowEffect(bool lv1, bool lv2)
        {
            if (lv1)
            {
                normalLevelOneActor.SoldierIn(activeSoldier -1);
                CheckSoldierUI(false,false,true);
            }

            if (lv2)
            {
                normalLevelTwoActor.SoldierIn(activeSoldier -1);
                CheckSoldierUI(false,true,true);
            }
        }
        
        private void CheckSoldierUI(bool levelOne,bool levelTwo,bool check)
        {
            
            soldierText.text = activeSoldier + "/" + soldierCapacity;
            soldierLv2Text.text = activeSoldier + "/" + soldierCapacity;
            
            if (check)
            {
                if (!levelTwo)
                {
                    if (activeSoldier < 4)
                    {
                        // btnLv1[activeSoldier - 1].SetActive(check);
                        // btnLv1[activeSoldier - 1].transform.localScale = new Vector3(0, 0, 0);
                        // btnLv1[activeSoldier - 1].transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
                    }
                }

                // btnLv2[activeSoldier - 1].SetActive(check);
                // btnLv2[activeSoldier - 1].transform.localScale = new Vector3(0, 0, 0);
                // btnLv2[activeSoldier - 1].transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                // if (!levelTwo)
                //     btnLv1[activeSoldier].SetActive(check);
                //
                // btnLv2[activeSoldier].SetActive(check);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("ColorfulGround"))
            {
                baseGround = other.gameObject;
            }
        }
    }
}