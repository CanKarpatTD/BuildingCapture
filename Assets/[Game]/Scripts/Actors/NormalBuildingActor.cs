using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TriflesGames.ManagerFramework;
using UnityEngine;

namespace Game.Actors
{
    public class NormalBuildingActor : Actor<BuildingManager>
    {
        public List<GameObject> build1Windows = new List<GameObject>();
        public List<GameObject> build2Windows = new List<GameObject>();
        public List<GameObject> build3Windows = new List<GameObject>();

        public MeshRenderer build1Mesh,build2Mesh,build3Mesh;


        public List<GameObject> level2Windows = new List<GameObject>();

        public GameObject level1, level2;

        public MeshRenderer level2Mesh;

        public GeneralNormalActor generalBuildingActor;

        public GameObject fadeMesh;
        
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                
            }
            else
            {
            
            }
        }


        public void HealthUpFirst()
        {
            build1Windows[0].SetActive(true);
            build2Windows[0].SetActive(true);
            build3Windows[0].SetActive(true);
        }
        public void HealthUpSecond()
        {
            build1Windows[1].SetActive(true);
            build2Windows[1].SetActive(true);
            build3Windows[1].SetActive(true);
        }
        public void HealthUpThird()
        {
            build1Windows[2].SetActive(true);
            build2Windows[2].SetActive(true);
            build3Windows[2].SetActive(true);
        }
        public void HealthUpFourth()
        {
            build1Windows[3].SetActive(true);
        }
        
        
        public void HealthDownFirst()
        {
            build1Windows[3].SetActive(false);
        }
        public void HealthDownSecond()
        {
            build1Windows[2].SetActive(false);
            build2Windows[2].SetActive(false);
            build3Windows[2].SetActive(false);
        }
        public void HealthDownThird()
        {
            build1Windows[1].SetActive(false);
            build2Windows[1].SetActive(false);
            build3Windows[1].SetActive(false);
        }
        public void HealthDownFourth()
        {
            build1Windows[0].SetActive(false);
            build2Windows[0].SetActive(false);
            build3Windows[0].SetActive(false);
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