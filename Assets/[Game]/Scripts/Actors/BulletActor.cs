using System;
using DG.Tweening;
using Game.GlobalVariables;
using TriflesGames.ManagerFramework;
using UnityEngine;

namespace Game.Actors
{
    public class BulletActor : Actor<SoldierManager>
    {
        public int _damage;
        
        public enum SoldierColor
        {
            PlayerBlue,
            EnemyRed,
            EnemyGreen
        }

        [HideInInspector]public SoldierColor soldierColor;

        [HideInInspector]public int soldierColorParse;
        [HideInInspector]public Transform target;

        protected override void MB_Update()
        {
            transform.Translate(Vector3.forward * 1f);
        }

        protected override void MB_Start()
        {
            // transform.DOMove(target.position, 0.1f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Soldier"))
            {
                if (target != null)
                {
                    if (other.gameObject.name == target.gameObject.name)
                    {
                        other.gameObject.GetComponent<SoldierActor>().HealthSituation(_damage);
                    }
                }

                Destroy(gameObject);
                
            }
            
            if (other.gameObject.CompareTag("BaseBuilding"))
            {
                if (soldierColor == SoldierColor.PlayerBlue)
                {
                    soldierColorParse = 0;
                }
                if (soldierColor == SoldierColor.EnemyRed)
                {
                    soldierColorParse = 1;
                }
                if (soldierColor == SoldierColor.EnemyGreen)
                {
                    soldierColorParse = 2;
                }

                if(target!=null)
                    if (other.gameObject.name == target.gameObject.name)
                        Push(CustomManagerEvents.GiveDamage, other.gameObject, _damage, 0, soldierColorParse);
                
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("NormalBuilding"))
            {
                if (soldierColor == SoldierColor.PlayerBlue)
                {
                    soldierColorParse = 0;
                }
                if (soldierColor == SoldierColor.EnemyRed)
                {
                    soldierColorParse = 1;
                }
                if (soldierColor == SoldierColor.EnemyGreen)
                {
                    soldierColorParse = 2;
                }

                if(target!= null)
                    if (other.gameObject.name == target.gameObject.name)
                        Push(CustomManagerEvents.GiveDamage, other.gameObject, _damage, 1, soldierColorParse);
                Destroy(gameObject);
            }
        }
    }
}