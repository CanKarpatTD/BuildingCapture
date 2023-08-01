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
    public class NormalLevelTwoActor : Actor<BuildingManager>
    {
        public List<GameObject> buildWindows = new List<GameObject>();

        public MeshRenderer build1Mesh;
        public GeneralNormalActor gna;

        public GameObject fadeMesh;
        
        public void SoldierIn(int count)
        {
            if (gna.activeSoldier == 1)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(false);
                buildWindows[2].SetActive(false);
                buildWindows[3].SetActive(false);
                buildWindows[4].SetActive(false);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 2)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(false);
                buildWindows[3].SetActive(false);
                buildWindows[4].SetActive(false);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 3)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(false);
                buildWindows[4].SetActive(false);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 4)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(false);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 5)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(true);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 6)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(true);
                buildWindows[5].SetActive(true);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 7)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(true);
                buildWindows[5].SetActive(true);
                buildWindows[6].SetActive(true);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 8)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(true);
                buildWindows[5].SetActive(true);
                buildWindows[6].SetActive(true);
                buildWindows[7].SetActive(true);
            }
        }

        public void SoldierOut(int count)
        {
            if (gna.activeSoldier == 0)
            {
                buildWindows[0].SetActive(false);
                buildWindows[1].SetActive(false);
                buildWindows[2].SetActive(false);
                buildWindows[3].SetActive(false);
                buildWindows[4].SetActive(false);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 1)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(false);
                buildWindows[2].SetActive(false);
                buildWindows[3].SetActive(false);
                buildWindows[4].SetActive(false);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 2)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(false);
                buildWindows[3].SetActive(false);
                buildWindows[4].SetActive(false);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 3)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(false);
                buildWindows[4].SetActive(false);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 4)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(false);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 5)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(true);
                buildWindows[5].SetActive(false);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 6)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(true);
                buildWindows[5].SetActive(true);
                buildWindows[6].SetActive(false);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 7)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(true);
                buildWindows[5].SetActive(true);
                buildWindows[6].SetActive(true);
                buildWindows[7].SetActive(false);
            }
            if (gna.activeSoldier == 8)
            {
                buildWindows[0].SetActive(true);
                buildWindows[1].SetActive(true);
                buildWindows[2].SetActive(true);
                buildWindows[3].SetActive(true);
                buildWindows[4].SetActive(true);
                buildWindows[5].SetActive(true);
                buildWindows[6].SetActive(true);
                buildWindows[7].SetActive(true);
            }
        }

        public void Fade()
        {
            gameObject.transform.DOPunchScale(new Vector3(.1f, .1f, .1f), 0.2f).SetEase(Ease.OutQuad);
            fadeMesh.SetActive(true);
            StartCoroutine(FadeBack());
        }

        private IEnumerator FadeBack()
        {
            yield return new WaitForSeconds(.2f);
            fadeMesh.SetActive(false);
        }
    }
}