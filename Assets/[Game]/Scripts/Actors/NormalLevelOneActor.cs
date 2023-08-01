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
    public class NormalLevelOneActor : Actor<BuildingManager>
    {
        public List<GameObject> build1Windows = new List<GameObject>();
        public List<GameObject> build2Windows = new List<GameObject>();
        public List<GameObject> build3Windows = new List<GameObject>();

        public MeshRenderer build1Mesh,build2Mesh,build3Mesh;

        public GameObject fadeMesh;

        
        

        public void SoldierIn(int count)
        {
            if (count <= 3)
            {
                build1Windows[count].SetActive(true);
                if (count != 3)
                    build2Windows[count].SetActive(true);
                if (count != 3)
                    build3Windows[count].SetActive(true);
            }
        }

        public void SoldierOut(int count)
        {
            if (count is >= 0 and <= 3)
            {
                build1Windows[count].SetActive(false);
                if (count != 3)
                    build2Windows[count].SetActive(false);
                if (count != 3)
                    build3Windows[count].SetActive(false);
            }
        }

        public void Fade()
        {
            fadeMesh.transform.DOPunchScale(new Vector3(.1f, .1f, .1f), 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                fadeMesh.transform.localScale = new Vector3(1, 1, 1);
            });
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