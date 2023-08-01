using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.GlobalVariables;
using Game.Helpers;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Game.Actors
{
    public class SoldierActor : Actor<SoldierManager>
    {
        public SkinnedMeshRenderer bodyMesh;
        public SkinnedMeshRenderer rifleMesh;

        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        public float fireRange;

        public Transform fireTarget;

        [Header("*-Variables-*")] public int health;

        [HideInInspector]public List<GameObject> targetList = new List<GameObject>();
        private bool _dead;

        [HideInInspector]public int typeParse;// 0=Player / 1=red / 2=green / 3=free
        public Transform _firePosition;
        public GameObject fireParticle;
        public int damage;
        [HideInInspector]public GameObject bulletPrefab;
        
        public GameObject flag;
        
        private int _bulletCount;
        
        [HideInInspector]public GameObject myTower;
        public bool capture, attack;
        
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                LevelManager.Instance.Subscribe(ManagerEvents.FinishLevel,LevelWin);
                GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Restart, OnGameRestart);
            }
            else
            {
                LevelManager.Instance.Unsubscribe(ManagerEvents.FinishLevel,LevelWin);
                GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Restart, OnGameRestart);
            }
        }

        private void OnGameRestart(object[] arguments)
        {
            if (Manager.playerSoldiers.Contains(gameObject))
                Manager.playerSoldiers.Remove(gameObject);
            
            if (Manager.redSoldiers.Contains(gameObject))
                Manager.redSoldiers.Remove(gameObject);
            
            if (Manager.greenSoldiers.Contains(gameObject))
                Manager.greenSoldiers.Remove(gameObject);
            
            Destroy(gameObject);
        }

        private void LevelWin(object[] arguments)
        {
            _navMeshAgent.speed = 0;
            _animator.SetTrigger("Win");
        }

        protected override void MB_Start()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();

            _navMeshAgent.stoppingDistance = fireRange;
            
            if (_firePosition != null)
                _firePosition = gameObject.transform.GetChild(3);
        }

        protected override void MB_Update()
        {
            if (!_dead)
            {
                if (fireTarget != null)
                {
                    if (fireTarget.GetComponent<GeneralBaseActor>() != null)
                    {
                        if (fireTarget.GetComponent<GeneralBaseActor>().colorParse == typeParse)
                        {
                            if (targetList.Contains(fireTarget.gameObject))
                                targetList.Remove(fireTarget.gameObject);
                        }
                    }

                    if (fireTarget.GetComponent<GeneralNormalActor>() != null)
                    {
                        if (fireTarget.GetComponent<GeneralNormalActor>().colorParse == typeParse)
                        {
                            if (targetList.Contains(fireTarget.gameObject))
                                targetList.Remove(fireTarget.gameObject);
                        }
                    }
                }
                
                if (typeParse == 0)
                {
                    if (attack)
                    {
                        FindNearestSoldier(Manager.greenSoldiers, Manager.redSoldiers);
                        FindNearestTower(null, null, BuildingManager.Instance.greenEnemyBuildings, BuildingManager.Instance.redEnemyBuildings);
                    }
                    if (capture)
                        FindNearestTower(BuildingManager.Instance.redEnemyBuildings,  BuildingManager.Instance.greenEnemyBuildings, BuildingManager.Instance.freeBuildings, BuildingManager.Instance.playerBuildings);
                }

                if (typeParse == 1)
                {
                    if (attack)
                    {
                        FindNearestSoldier(Manager.greenSoldiers, Manager.playerSoldiers);
                        FindNearestTower(null, null, BuildingManager.Instance.playerBuildings, BuildingManager.Instance.greenEnemyBuildings);
                    }
                    if (capture)
                        FindNearestTower(BuildingManager.Instance.greenEnemyBuildings,  BuildingManager.Instance.playerBuildings, BuildingManager.Instance.freeBuildings, BuildingManager.Instance.redEnemyBuildings);
                }

                if (typeParse == 2)
                {
                    if (attack)
                    {
                        FindNearestSoldier(Manager.redSoldiers, Manager.playerSoldiers);
                        FindNearestTower(null, null, BuildingManager.Instance.playerBuildings, BuildingManager.Instance.redEnemyBuildings);
                    }

                    if (capture)
                        FindNearestTower(BuildingManager.Instance.redEnemyBuildings,  BuildingManager.Instance.playerBuildings, BuildingManager.Instance.freeBuildings, BuildingManager.Instance.greenEnemyBuildings);
                }


                foreach (var obj in targetList.ToList())
                {
                    if (obj == null)
                        targetList.Remove(obj);
                }
                
                FindFireTarget();

                if (fireTarget != null)
                {
                    if (fireTarget.GetComponent<GeneralBaseActor>() != null)
                    {
                        if (fireTarget.GetComponent<GeneralBaseActor>().colorParse == typeParse)
                        {
                            if (targetList.Contains(fireTarget.gameObject))
                            {
                                targetList.Remove(fireTarget.gameObject);
                            }
                        }
                    }
                    else if (fireTarget.GetComponent<GeneralNormalActor>() != null)
                    {
                        if (fireTarget.GetComponent<GeneralNormalActor>().colorParse == typeParse)
                        {
                            if (targetList.Contains(fireTarget.gameObject))
                            {
                                targetList.Remove(fireTarget.gameObject);
                            }
                        }
                    }
                }
                
                if (fireTarget != null)
                {
                    // if (fireTarget.GetComponent<GeneralNormalActor>() != null)
                    // {
                    //     if (fireTarget.GetComponent<GeneralNormalActor>().colorParse == typeParse || fireTarget.GetComponent<GeneralNormalActor>().colorParse == 3 || fireTarget.GetComponent<GeneralNormalActor>().canCapture)
                    //     {
                    //         if (fireTarget.GetComponent<GeneralNormalActor>().activeSoldier != fireTarget.GetComponent<GeneralNormalActor>().soldierCapacity)
                    //         {
                    //             // fireTarget.GetComponent<NavMeshObstacle>().enabled = false;
                    //             if (capture)
                    //                 _navMeshAgent.stoppingDistance = 0;
                    //             if (attack)
                    //                 _navMeshAgent.stoppingDistance = 7;
                    //         }
                    //         else
                    //         {
                    //             // fireTarget.GetComponent<NavMeshObstacle>().enabled = true;
                    //             _navMeshAgent.stoppingDistance = 7;
                    //         }
                    //     }
                    // }
                    // else if (fireTarget.GetComponent<GeneralNormalActor>() == null || fireTarget.GetComponent<GeneralNormalActor>() != null && fireTarget.GetComponent<GeneralNormalActor>().colorParse != typeParse && fireTarget.GetComponent<GeneralNormalActor>().colorParse != 3 || !fireTarget.GetComponent<GeneralNormalActor>().canCapture)
                    // {
                    //     _navMeshAgent.stoppingDistance = 7;
                    // }

                    if (fireTarget != null)
                        _navMeshAgent.destination = fireTarget.position;

                    if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
                    {
                        _animator.SetTrigger("Run");
                    }

                    if (_navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance)
                    {
                        Attacking();
                    }
                }
                
                
            }
        }

        private void Attacking()
        {
            _animator.SetTrigger("Fire");
            if (fireTarget != null)
                gameObject.transform.DOLookAt(fireTarget.position, 0.01f).SetEase(Ease.Linear);

            if (Manager.inTutorial)
            {
                Push(CustomManagerEvents.MakeCameraMove);
                Manager.inTutorial = false;
            }
        }

        public void ShootsFire()
        {
            _bulletCount = 4;
            if (_bulletCount == 4)
            {
                _bulletCount = 0;
                
                Instantiate(fireParticle, _firePosition.position, Quaternion.Euler(_firePosition.transform.eulerAngles));
                var bullet = Instantiate(bulletPrefab, _firePosition.position, Quaternion.Euler(_firePosition.transform.eulerAngles));
                if (typeParse == 0)
                {
                    bullet.GetComponent<BulletActor>().soldierColor = BulletActor.SoldierColor.PlayerBlue;
                }

                if (typeParse == 1)
                {
                    bullet.GetComponent<BulletActor>().soldierColor = BulletActor.SoldierColor.EnemyRed;
                }

                if (typeParse == 2)
                {
                    bullet.GetComponent<BulletActor>().soldierColor = BulletActor.SoldierColor.EnemyGreen;
                }

                if (fireTarget != null)
                {
                    bullet.GetComponent<BulletActor>().target = fireTarget.transform;
                    bullet.GetComponent<BulletActor>()._damage = damage;
                }

                print("Fired" + " / " + gameObject.name);
            }
            else
            {
                Instantiate(fireParticle, _firePosition.position, Quaternion.Euler(_firePosition.transform.eulerAngles));
            }
        }

        public void HealthSituation(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                _dead = true;

                if (attack)
                    _animator.SetTrigger("Dead");
                
                if (capture)
                {
                    if (Manager.playerSoldiers.Contains(gameObject))
                        Manager.playerSoldiers.Remove(gameObject);
            
                    if (Manager.greenSoldiers.Contains(gameObject))
                        Manager.greenSoldiers.Remove(gameObject);
            
                    if (Manager.redSoldiers.Contains(gameObject))
                        Manager.redSoldiers.Remove(gameObject);
                    
                    Destroy(gameObject);
                }
            }
        }

        public void DeadEffect()
        {
            Instantiate(Manager.deadParticle, gameObject.transform.position, Quaternion.identity);

            if (Manager.playerSoldiers.Contains(gameObject))
                Manager.playerSoldiers.Remove(gameObject);
            
            if (Manager.greenSoldiers.Contains(gameObject))
                Manager.greenSoldiers.Remove(gameObject);
            
            if (Manager.redSoldiers.Contains(gameObject))
                Manager.redSoldiers.Remove(gameObject);
            Destroy(gameObject);
        }
        
        private void FindNearestSoldier(List<GameObject> target1,List<GameObject> target2)
        {
            foreach (var test in target1)
            {
                if (!targetList.Contains(test))
                    targetList.Add(test);
            }
            foreach (var test in target2)
            {
                if (!targetList.Contains(test))
                    targetList.Add(test);
            }
        }

        private void FindNearestTower(List<GameObject> target1, List<GameObject> target2,List<GameObject> target3, List<GameObject> myTowers)
        {
            if (target1 != null)
            {
                foreach (var test in target1)
                {
                    if (!targetList.Contains(test))
                        targetList.Add(test);
                }
            }

            if (target2 != null)
            {
                foreach (var test in target2)
                {
                    if (!targetList.Contains(test))
                        targetList.Add(test);
                }
            }

            foreach (var test in target3)
            {
                if (!targetList.Contains(test))
                    targetList.Add(test);
            }
            
            foreach (var test in myTowers)
            {
                if (!targetList.Contains(test))
                    targetList.Add(test);
                if (targetList.Contains(myTower))
                    targetList.Remove(myTower);
            }
        }

        private void FindFireTarget()
        {
            float minimumDistance = Mathf.Infinity;

            fireTarget = null;
            
            foreach(var enemy in targetList)
            {
                //float distance = Vector3.Distance(transform.position, enemy.transform.position);
                
                float distance = (transform.position - enemy.transform.position).sqrMagnitude;
                
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;

                    if (enemy.GetComponent<GeneralNormalActor>() != null)
                    {
                        if (attack)
                        {
                            if (enemy.GetComponent<GeneralNormalActor>().activeSoldier > 0)
                            {
                                fireTarget = enemy.transform;
                            }
                        }

                        if (capture)
                        {
                            // if (enemy.GetComponent<GeneralNormalActor>().activeSoldier == 0)
                            // {
                                fireTarget = enemy.transform;
                            // }
                        }
                    }

                    if (enemy.GetComponent<GeneralBaseActor>() != null)
                    {
                        if (attack)
                        {
                            if (enemy.GetComponent<GeneralBaseActor>().buildHealth != 0)
                            {
                                fireTarget = enemy.transform;
                            }
                        }
                    }
                    
                    // if (enemy.gameObject.GetComponent<GeneralNormalActor>() != null)
                    // {
                    //     if (enemy.GetComponent<GeneralNormalActor>().activeSoldier != enemy.GetComponent<GeneralNormalActor>().soldierCapacity)
                    //         fireTarget = enemy.transform;
                    // }
                    // else
                    // {
                    //     if (enemy.gameObject.GetComponent<GeneralBaseActor>() != null)
                    //     {
                    //         if (enemy.gameObject.GetComponent<GeneralBaseActor>().colorParse != typeParse)
                    //         {
                    //             if(attack)
                    //                 if (enemy.gameObject.GetComponent<GeneralBaseActor>().buildHealth != 0)
                    //                     fireTarget = enemy.transform;
                    //         }
                    //     }
                    //
                    if (enemy.gameObject.GetComponent<SoldierActor>() != null)
                    {
                        if (enemy.gameObject.GetComponent<SoldierActor>().typeParse != typeParse)
                            fireTarget = enemy.transform;
                    }
                    // }
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("NormalBuilding"))
            {
                if(other.gameObject != myTower)
                {
                    if (capture)
                    {
                        var script = other.gameObject.GetComponent<GeneralNormalActor>();
                        if (script.activeSoldier == script.soldierCapacity)
                            DeadEffect();
                        if (script.colorParse == typeParse || script.colorParse == 3 || script.activeSoldier == 0)
                        {
                            if (typeParse == 0)
                            {
                                Manager.playerSoldiers.Remove(gameObject);
                            }

                            if (typeParse == 1)
                            {
                                Manager.redSoldiers.Remove(gameObject);
                            }

                            if (typeParse == 2)
                            {
                                Manager.greenSoldiers.Remove(gameObject);
                            }

                            Push(CustomManagerEvents.SoldierAdd, other.gameObject, gameObject, typeParse);
                            Push(CustomManagerEvents.TutorialEffect);
                            Destroy(gameObject);
                        }
                        else
                        {
                            DeadEffect();
                        }
                    }
                }
            }
        }

        public void ChangeMat(Material mat)
        {
            bodyMesh.material = mat;

            if (rifleMesh != null)
                rifleMesh.material = mat;

            if (flag != null)
                flag.GetComponent<MeshRenderer>().material = mat;
        }
    }
}