using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.GlobalVariables;
using TriflesGames.ManagerFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Actors
{
    public class BuildMergeControl : Actor<BuildingManager>
    {
        [HideInInspector] public bool canMerge;
        [HideInInspector] public bool moving;
        [HideInInspector] public bool willMerge;
        [HideInInspector] public bool merger;
        [HideInInspector] public bool mergable;

        [HideInInspector]public GeneralBaseActor gba;
        [HideInInspector]public GeneralNormalActor gna;
        
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                Manager.Subscribe(CustomManagerEvents.MovingObject, CheckMergeSituation);
                Manager.Subscribe(CustomManagerEvents.Merging, Merging);
                Manager.Subscribe(CustomManagerEvents.ResetMergeBools, ResetBools);
            }
            else
            {
                Manager.Unsubscribe(CustomManagerEvents.MovingObject, CheckMergeSituation);
                Manager.Unsubscribe(CustomManagerEvents.Merging, Merging);
                Manager.Unsubscribe(CustomManagerEvents.ResetMergeBools, ResetBools);
            }
        }

        private void CheckMergeSituation(object[] args)
        {
            var obj = (GameObject) args[0];
            var value = (int)args[1];
            

            if (value == 0)
            {
                var script = obj.GetComponent<GeneralBaseActor>();
                if (gba != null)
                {
                    if (script.level == gba.level && script.buildType == gba.buildType)
                    {
                        if (!moving)
                        {
                            canMerge = true;
                            willMerge = true;
                        }
                    }
                }
            }

            if (value == 1)
            {
                var script = obj.GetComponent<GeneralNormalActor>();
                if (gna != null)
                {
                    if (script.level == gna.level && script.buildType == gna.buildType)
                    {
                        if (!moving)
                        {
                            canMerge = true;
                            willMerge = true;
                        }
                    }
                }
            }
        }
        
        private void Merging(object[] args)
        {
            if (mergable)
            {
                var value = (int)args[1];

                var obj = (GameObject) args[0];
                
                if (value == 0)
                {
                    if (gba.level == GeneralBaseActor.Level.Level2)
                    {
                        gba.level3.SetActive(true);
                
                        gba.level2.SetActive(false);
                        gba.level1.SetActive(false);
                
                        gba.level = GeneralBaseActor.Level.Level3;
                        mergable = false;
                        gba.CheckTowerSituation();
                    }
                    
                    if (gba.level == GeneralBaseActor.Level.Level1)
                    {
                        gba.level2.SetActive(true);

                        gba.level3.SetActive(false);
                        gba.level1.SetActive(false);
                    
                        gba.level = GeneralBaseActor.Level.Level2;
                        mergable = false;
                        gba.CheckTowerSituation();
                    }
                }

                if (value == 1)
                {
                    if (gna.level == GeneralNormalActor.Level.Level1)
                    {
                        gna.level2.SetActive(true);

                        gna.level1.SetActive(false);
                    
                        gna.level = GeneralNormalActor.Level.Level2;
                        mergable = false;
                        gna.CheckTowerSituation();
                    }
                }

                if(gna != null)
                    gna.TowerUpWindowEffect(false,true);
                
                gameObject.transform.DOPunchScale(new Vector3(0.5f, -0.5f, 0.5f), 0.3f);

                if (Manager.playerBuildings.Contains(obj))
                    Manager.playerBuildings.Remove(obj);
                
                Destroy(obj);

                print("MergeSuccess, Build LevelUp");
            }
            else
            {
                if (merger)
                    Push(CustomManagerEvents.WrongMerge, mergable, merger);
            }
        }
        
        private void ResetBools(object[] args)
        {
            bool control = (bool) args[0];

            canMerge = control;
            merger = control;
            willMerge = control;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (willMerge && canMerge)
            {
                if (other.gameObject.CompareTag("NormalBuilding"))
                {
                    if (canMerge)
                        mergable = true;
                }

                if (other.gameObject.CompareTag("BaseBuilding"))
                {
                    if (canMerge)
                        mergable = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (willMerge && canMerge)
            {
                if (other.gameObject.CompareTag("NormalBuilding"))
                {
                    if (canMerge)
                        mergable = false;
                }

                if (other.gameObject.CompareTag("BaseBuilding"))
                {
                    if (canMerge)
                        mergable = false;
                }
            }
        }
    }
}