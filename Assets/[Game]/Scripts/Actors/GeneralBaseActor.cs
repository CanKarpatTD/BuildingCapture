using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.GlobalVariables;
using Game.Helpers;
using TMPro;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Actors
{
    public class GeneralBaseActor : Actor<BuildingManager>
    {
        public enum BuildType
        {
            None, Player, EnemyGreen, EnemyRed
        }
        public BuildType buildType;
        [HideInInspector]public int colorParse;// 0=Player / 1=Red / 2=Green
        
        public enum Level
        {
            Level1,Level2,Level3
        }
        [Space(5)]public Level level;

        [HideInInspector]public GameObject level1, level2, level3;

        [Space(5)]public GameObject baseGround;

        [Header("Level Factors")]
        [HideInInspector][Space(5)]public BaseLevelOneActor BaseLevelOneActor;
        [HideInInspector][Space(5)]public BaseLevelTwoActor BaseLevelTwoActor;
        [HideInInspector][Space(5)]public BaseLevelThreeActor BaseLevelThreeActor;
        
        [HideInInspector][Header("* Mesh Objects *")] public List<MeshRenderer> buildUvMesh;
        [HideInInspector]public List<MeshRenderer> insideMesh;
        
        [Header("Soldier Factors")]
        [HideInInspector]public int activeSoldier;
        [HideInInspector]public int soldierCapacity;
        [HideInInspector]public float spawnTime;
        [HideInInspector]public List<Transform> spawnPoses = new List<Transform>();

        [HideInInspector][Space(10)] public int buildHealth;
        [HideInInspector]public int level1Hp,level2Hp;


        [Header("UI Factors")] public Transform canvas; 
        public GameObject fourCanvas;
        public GameObject eightCanvas;
        public TextMeshProUGUI soldierText,soldierLv2Text;
        [Space(20)]public List<Image> colorsMain = new List<Image>();

        public float spawnAiTime;
        // [HideInInspector]public List<Image> colorsBtn = new List<Image>();
        // [HideInInspector]public List<GameObject> btnLv1 = new List<GameObject>();
        // [HideInInspector]public List<GameObject> btnLv2 = new List<GameObject>();

        private float enemySpawnTimer;
        
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Start,SetUI);
                Manager.Subscribe(CustomManagerEvents.SpawnSoldier,SpawnSoldier);
                SoldierManager.Instance.Subscribe(CustomManagerEvents.GiveDamage, TakeDamage);
            }
            else
            {
                GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Start,SetUI);
                Manager.Unsubscribe(CustomManagerEvents.SpawnSoldier,SpawnSoldier);
                SoldierManager.Instance.Unsubscribe(CustomManagerEvents.GiveDamage, TakeDamage);
            }
        }

        private void SetUI(object[] arguments)
        {
            canvas.DOLookAt(Camera.main.transform.position, 0.1f);
            var sit = ((GameLevel) LevelManager.Instance.levelData).tutorial;

            
            if (sit)
            {
                spawnAiTime = 10;
                enemySpawnTimer = .7f;
            }
            else
            {
                spawnAiTime = 4.5f;
                enemySpawnTimer = .7f;
            }
        }

        protected override void MB_Update()
        {
            canvas.DOLookAt(Camera.main.transform.position, 0.1f);
            
            
            if (GameManager.Instance.IsGameStarted)
            {
                if (activeSoldier != soldierCapacity)
                {
                    spawnTime -= 1 * Time.deltaTime;
                    if (spawnTime <= 0)
                    {
                        if (buildType == BuildType.Player)
                            spawnTime = 1.3f;
                        else
                        {
                            spawnTime = 0.5f;
                        }
                        activeSoldier++;
                        Manager.soldierBase++;
                        Manager.Soldiers();
                        if (level == Level.Level1)
                            CheckSoldierUI(true, false, true);
                        if (level == Level.Level2)
                            CheckSoldierUI(false, true, true);
                    }
                }

                if (activeSoldier >= soldierCapacity)
                    activeSoldier = soldierCapacity;

                if (activeSoldier <= 0)
                    activeSoldier = 0;

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
                                SpawnSoldierSpawn(1, true,SoldierManager.Instance.captureSoldier);
                            if (level == Level.Level2)
                                SpawnSoldierSpawn(2, true,SoldierManager.Instance.captureSoldier);
                        }

                        if (randStyle == 1) //Attack
                        {
                            if (level == Level.Level1)
                                SpawnSoldierSpawn(1, false,SoldierManager.Instance.lvOneSoldier);
                            if (level == Level.Level2)
                                SpawnSoldierSpawn(2, false,SoldierManager.Instance.lvTwoSoldier);
                        }
                    }

                    if (buildType == BuildType.EnemyRed)
                    {
                        var randStyle = Random.Range(0, 2);

                        if (randStyle == 0) //Capture
                        {
                            if (level == Level.Level1)
                                SpawnSoldierSpawn(1, true,SoldierManager.Instance.captureSoldier);
                            if (level == Level.Level2)
                                SpawnSoldierSpawn(2, true,SoldierManager.Instance.captureSoldier);
                        }

                        if (randStyle == 1) //Attack
                        {
                            if (level == Level.Level1)
                                SpawnSoldierSpawn(1, false,SoldierManager.Instance.lvOneSoldier);
                            if (level == Level.Level2)
                                SpawnSoldierSpawn(2, false,SoldierManager.Instance.lvTwoSoldier);
                        }
                    }
                }
            }

            FindNearestTower();
        }

        public Transform fireTarget;
        
        private void FindNearestTower()
        {
            // float minimumDistance = Mathf.Infinity;
            //
            // fireTarget = null;
            //
            // foreach(var enemy in targetList)
            // {
            //     float distance = (transform.position - enemy.transform.position).sqrMagnitude;
            //     
            //     if (distance < minimumDistance)
            //     {
            //         minimumDistance = distance;
            //
            //         
            //     }
            // }
        }
        protected override void MB_Start()
        {
            CheckTowerSituation();
            enemySpawnTimer = 4.5f;
        }

        public void CheckTowerSituation()
        {
            if (buildType == BuildType.Player)
            {
                    if (!Manager.playerBuildings.Contains(gameObject))
                        Manager.playerBuildings.Add(gameObject);

                    if (!Manager.playerBases.Contains(gameObject))
                        Manager.playerBases.Add(gameObject);
                

                ChangeAllMat(Manager.player,0);

                gameObject.layer = 6;
                colorParse = 0;
                Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
            }
            
            if (buildType == BuildType.EnemyGreen)
            {
                    if (!Manager.greenEnemyBuildings.Contains(gameObject))
                        Manager.greenEnemyBuildings.Add(gameObject);

                    if (!Manager.greenBases.Contains(gameObject))
                        Manager.greenBases.Add(gameObject);
                

                ChangeAllMat(Manager.green,2);
                
                gameObject.layer = 8;
                colorParse = 2;
                Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
            }
            
            if (buildType == BuildType.EnemyRed)
            {
                    if (!Manager.redEnemyBuildings.Contains(gameObject))
                        Manager.redEnemyBuildings.Add(gameObject);

                    if (!Manager.redBases.Contains(gameObject))
                        Manager.redBases.Add(gameObject);
                

                ChangeAllMat(Manager.red,1);
                
                gameObject.layer = 8;
                colorParse = 1;
                Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
            }

            
            if (level == Level.Level1)
            {
                buildHealth = level1Hp;
                soldierCapacity = 4;
                
                soldierText.text = activeSoldier + "/" + soldierCapacity;
                soldierLv2Text.text = activeSoldier + "/" + soldierCapacity;
                fourCanvas.SetActive(true);
                eightCanvas.SetActive(false);
            }

            if (level == Level.Level2)
            {
                buildHealth = level2Hp;
                soldierCapacity = 8;
                
                soldierText.text = activeSoldier + "/" + soldierCapacity;
                soldierLv2Text.text = activeSoldier + "/" + soldierCapacity;
                fourCanvas.SetActive(false);
                eightCanvas.SetActive(true);
            }
        }
        
        private void ChangeAllMat(Material mat, int parse)// 0=Player / 1=red / 2=green / 3=red
        {
            foreach (var mesh in buildUvMesh)
            {
                mesh.material = mat;
            }

            foreach (var mesh in insideMesh)
            {
                var mats = mesh.materials;
                mats[0] = mat;
                mesh.materials = mats;
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

            if (parse == 3)
            {
                foreach (var mainC in colorsMain)
                {
                    mainC.sprite = Manager.freeZone;
                }

                // foreach (var btn in colorsBtn)
                // {
                //     btn.gameObject.SetActive(false);
                // }
            }
        }

        private void TakeDamage(object[] args)
        {
            //type = 0 baseBuilding 1 normalBuilding
            
            // other.gameObject , _damage , 0 , soldierColorParse
            
            var obj = (GameObject) args[0];

            var value = (int) args[1];
            
            var color = (int) args[3];

            
            
            if (obj.name == gameObject.name && color != colorParse)// Layer değiştirmeyi unutma ve aynı şeyden çıkanları çarptırma
            {
                buildHealth -= value;
                
                // gameObject.transform.GetChild(0).transform.DOPunchScale(new Vector3(.1f, .1f, .1f), .2f).SetEase(Ease.OutQuad).OnComplete(
                //     () => { gameObject.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);});


                foreach (var mesh in buildUvMesh)
                {
                    if (mesh.gameObject.activeSelf)
                    {
                        var mat = mesh.material;
                        mesh.material = Manager.whiteMat;
                        if (buildHealth > 0)
                            Manager.gameObject.transform.DOScale(0, .2f).OnComplete(() =>
                            {
                                // mesh.material = mat;
                                if (colorParse == 0)
                                {
                                    mesh.material = Manager.player;
                                }

                                if (colorParse == 1)
                                {
                                    mesh.material = Manager.red;
                                }

                                if (colorParse == 2)
                                {
                                    mesh.material = Manager.green;
                                }
                            });
                    }
                }

                if (Manager.destructionActive)
                {
                    if (level == Level.Level1)
                        CheckTowerHealth(true,false);
                    
                    if (level == Level.Level2)
                        CheckTowerHealth(false,true);
                }

                if (buildHealth <= 0)
                {
                    if (colorParse == 0)
                    {
                        LevelManager.Instance.levelActor.FinishLevel(false, 0.5f);
                    }
                    
                    if (color == 0)
                    {
                        buildType = BuildType.Player;
                        ChangeAllMat(Manager.player,0);
                        
                        if(Manager.greenEnemyBuildings.Contains(gameObject))
                            Manager.greenEnemyBuildings.Remove(gameObject);
                        
                        if(Manager.redEnemyBuildings.Contains(gameObject))
                            Manager.redEnemyBuildings.Remove(gameObject);
                        
                        if (Manager.redBases.Contains(gameObject))
                            Manager.redBases.Remove(gameObject);
                        if (Manager.greenBases.Contains(gameObject))
                            Manager.greenBases.Remove(gameObject);
                        
                        Manager.playerBases.Add(gameObject);
                        Manager.playerBuildings.Add(gameObject);
                        colorParse = 0;
                        gameObject.layer = 6;
                        Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
                    }

                    if (color == 1)
                    {
                        buildType = BuildType.EnemyRed;
                        ChangeAllMat(Manager.red,1);
                        
                        if(Manager.greenEnemyBuildings.Contains(gameObject))
                            Manager.greenEnemyBuildings.Remove(gameObject);
                        
                        if(Manager.playerBuildings.Contains(gameObject))
                            Manager.playerBuildings.Remove(gameObject);
                        
                        if (Manager.greenBases.Contains(gameObject))
                            Manager.greenBases.Remove(gameObject);
                        if (Manager.playerBases.Contains(gameObject))
                            Manager.playerBases.Remove(gameObject);
                        
                        Manager.redBases.Add(gameObject);
                        Manager.redEnemyBuildings.Add(gameObject);
                        colorParse = 1;
                        gameObject.layer = 8;
                        Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
                    }

                    if (color == 2)
                    {
                        buildType = BuildType.EnemyGreen;
                        ChangeAllMat(Manager.green,2);
                        
                        if(Manager.playerBuildings.Contains(gameObject))
                            Manager.playerBuildings.Remove(gameObject);
                        
                        if(Manager.redEnemyBuildings.Contains(gameObject))
                            Manager.redEnemyBuildings.Remove(gameObject);

                        if (Manager.redBases.Contains(gameObject))
                            Manager.redBases.Remove(gameObject);
                        if (Manager.playerBases.Contains(gameObject))
                            Manager.playerBases.Remove(gameObject);
                        
                        Manager.greenBases.Add(gameObject);
                        Manager.greenEnemyBuildings.Add(gameObject);
                        colorParse = 2;
                        gameObject.layer = 8;
                        Push(CustomManagerEvents.ChangeGroundColor,colorParse,baseGround.gameObject);
                    }

                    CheckTowerSituation();
                    Manager.DestructionActive();
                }
            }
        }
        
        #region SpawnSoldier
        private void SpawnSoldier(object[] args)
        {
                var value = (bool) args[0];
                if (colorParse == 0)
                {
                    gameObject.transform.DOPunchScale(new Vector3(0.1f, -0.1f, 0.1f), 0.15f);
                    if (activeSoldier > 0)
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

                        activeSoldier--;
                        Manager.soldierBase--;
                        Manager.Soldiers();
                        if (level == Level.Level1)
                            CheckSoldierUI(true, false, false);
                        if (level == Level.Level2)
                            CheckSoldierUI(false, true, false);
                    }
                }
                else
                {
                    // if (activeSoldier > 0)
                    // {
                    //     var rand = Random.Range(0, 2);
                    //
                    //     if (rand == 0)
                    //     {
                    //         if (colorParse == 0)
                    //         {
                    //             SpawnSoldierSpawn(1,value);
                    //         }
                    //
                    //         if (colorParse == 1)
                    //         {
                    //             SpawnSoldierSpawn(1,value);
                    //         }
                    //
                    //         if (colorParse == 2)
                    //         {
                    //             SpawnSoldierSpawn(1,value);
                    //         }
                    //
                    //         activeSoldier--;
                    //         if (level == Level.Level1)
                    //             CheckSoldierUI(true, false, false);
                    //         if (level == Level.Level2)
                    //             CheckSoldierUI(false, true, false);
                    //     }
                    // }
                }
            
        }
        
        private void SpawnSoldierSpawn(int lv,bool action,GameObject sldr)
        {
                if (lv == 1)
                {
                    var soldier = Instantiate(sldr, spawnPoses[Random.Range(0, spawnPoses.Count)].position,
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
                        Instantiate(SoldierManager.Instance.playerSoldierSpawn, soldier.transform.position,
                            Quaternion.identity);
                        SoldierManager.Instance.playerSoldiers.Add(soldier);
                        soldier.GetComponent<SoldierActor>().ChangeMat(SoldierManager.Instance.blueSoldier);
                        soldier.GetComponent<SoldierActor>().typeParse = 0;
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

                if (lv == 2)
                {
                    var soldier = Instantiate(sldr, spawnPoses[Random.Range(0, spawnPoses.Count)].position,
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
                        Instantiate(SoldierManager.Instance.playerSoldierSpawn, soldier.transform.position,
                            Quaternion.identity);
                        SoldierManager.Instance.playerSoldiers.Add(soldier);
                        soldier.GetComponent<SoldierActor>().ChangeMat(SoldierManager.Instance.blueSoldier);
                        soldier.GetComponent<SoldierActor>().typeParse = 0;
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
        #endregion
        
        
        
        private void CheckTowerHealth(bool levelOne,bool levelTwo)
        {
            if (levelOne)
            {
                if (buildHealth is >= 375 and <= 425) 
                {
                    BaseLevelOneActor.case1.SetActive(false);

                    BaseLevelOneActor.case2.SetActive(true);
                    Instantiate(Manager.smoke, gameObject.transform.position, Quaternion.identity);

                    BaseLevelOneActor.case3.SetActive(false);
                    BaseLevelOneActor.case4.SetActive(false);
                    BaseLevelOneActor.case5.SetActive(false);
                }

                if (buildHealth is >= 275 and <= 325) 
                {
                    BaseLevelOneActor.case1.SetActive(false);
                    BaseLevelOneActor.case2.SetActive(false);

                    BaseLevelOneActor.case3.SetActive(true);
                    Instantiate(Manager.smoke, gameObject.transform.position, Quaternion.identity);

                    BaseLevelOneActor.case4.SetActive(false);
                    BaseLevelOneActor.case5.SetActive(false);
                }

                if (buildHealth is >= 175 and <= 225) 
                {
                    BaseLevelOneActor.case1.SetActive(false);
                    BaseLevelOneActor.case2.SetActive(false);
                    BaseLevelOneActor.case3.SetActive(false);

                    BaseLevelOneActor.case4.SetActive(true);
                    Instantiate(Manager.smoke, gameObject.transform.position, Quaternion.identity);

                    BaseLevelOneActor.case5.SetActive(false);
                }

                if (buildHealth is >= 75 and <= 125) 
                {
                    BaseLevelOneActor.case1.SetActive(false);
                    BaseLevelOneActor.case2.SetActive(false);
                    BaseLevelOneActor.case3.SetActive(false);
                    BaseLevelOneActor.case4.SetActive(false);

                    BaseLevelOneActor.case5.SetActive(true);
                    Instantiate(Manager.smoke, gameObject.transform.position, Quaternion.identity);
                }
            }

            if (levelTwo)
            {
                if (buildHealth == 800)
                {
                    BaseLevelOneActor.case1.SetActive(false);

                    BaseLevelOneActor.case2.SetActive(true);
                    Instantiate(Manager.smoke, gameObject.transform.position, Quaternion.identity);

                    BaseLevelOneActor.case3.SetActive(false);
                    BaseLevelOneActor.case4.SetActive(false);
                    BaseLevelOneActor.case5.SetActive(false);
                }

                if (buildHealth == 600)
                {
                    BaseLevelOneActor.case1.SetActive(false);
                    BaseLevelOneActor.case2.SetActive(false);

                    BaseLevelOneActor.case3.SetActive(true);
                    Instantiate(Manager.smoke, gameObject.transform.position, Quaternion.identity);

                    BaseLevelOneActor.case4.SetActive(false);
                    BaseLevelOneActor.case5.SetActive(false);
                }

                if (buildHealth == 400)
                {
                    BaseLevelOneActor.case1.SetActive(false);
                    BaseLevelOneActor.case2.SetActive(false);
                    BaseLevelOneActor.case3.SetActive(false);

                    BaseLevelOneActor.case4.SetActive(true);
                    Instantiate(Manager.smoke, gameObject.transform.position, Quaternion.identity);

                    BaseLevelOneActor.case5.SetActive(false);
                }

                if (buildHealth == 200)
                {
                    BaseLevelOneActor.case1.SetActive(false);
                    BaseLevelOneActor.case2.SetActive(false);
                    BaseLevelOneActor.case3.SetActive(false);
                    BaseLevelOneActor.case4.SetActive(false);

                    BaseLevelOneActor.case5.SetActive(true);
                    Instantiate(Manager.smoke, gameObject.transform.position, Quaternion.identity);
                }
            }

            
            if (buildHealth <= 0)
            {
                if (colorParse != 0)
                {
                    LevelManager.Instance.levelActor.FinishLevel(true, 0.5f);
                }
                else
                {
                    LevelManager.Instance.levelActor.FinishLevel(false, 0.5f);
                }
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
                    // btnLv1[activeSoldier - 1].SetActive(check);
                    // btnLv1[activeSoldier - 1].transform.localScale = new Vector3(0, 0, 0);
                    // btnLv1[activeSoldier - 1].transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
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