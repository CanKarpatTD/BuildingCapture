using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.GlobalVariables;
using Game.Helpers;
using TriflesGames.Helpers;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Actors
{
    public class CameraFollowActor : Actor<LevelManager>
    {
        private Vector3 touchStart;
        public Camera MainCamera;
        [FormerlySerializedAs("asd")] public GameObject cameraHolder;
        public Rigidbody cameraObjRb;
        public float groundZ = 0; //Z position of ground floor where to Pan from
        public float zoomOutMin = 10;
        public float zoomOutMax = 100;
        private bool holding;

    
        [Space(20)]public float clampValue;
        public float zClamp,zClampPlus;

        public Slider slider;
        public EventTrigger sliderEvent;
        public Button handle;
        
        private bool disable;

        public bool inTutorial;

        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                slider.onValueChanged.AddListener(CameraZoom);
                GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Start,StartRepeat);
                GameManager.Instance.Subscribe(ManagerEvents.BtnClick_Continue,FinishLevel);
                BuildingManager.Instance.Subscribe(CustomManagerEvents.MovingObject,Active);
                BuildingManager.Instance.Subscribe(CustomManagerEvents.ResetMergeBools,Deactive);
                
                SoldierManager.Instance.Subscribe(CustomManagerEvents.MakeCameraMove,CameraZoomTutorial);
            }
            else
            {
                slider.onValueChanged.RemoveListener(CameraZoom);
                GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Start ,StartRepeat);
                GameManager.Instance.Unsubscribe(ManagerEvents.BtnClick_Continue,FinishLevel);
                BuildingManager.Instance.Unsubscribe(CustomManagerEvents.MovingObject,Active);
                BuildingManager.Instance.Unsubscribe(CustomManagerEvents.ResetMergeBools,Deactive);
                
                SoldierManager.Instance.Unsubscribe(CustomManagerEvents.MakeCameraMove,CameraZoomTutorial);
            }
        }

        private void CameraZoomTutorial(object[] arguments)
        {
            var obj = slider.gameObject.transform.GetChild(0);

            if (SoldierManager.Instance.inTutorial)
            {
                //47
                obj.gameObject.SetActive(true);
                obj.DOLocalMoveX(47, 2f).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    obj.DOLocalMoveX(-47f, 2f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        obj.gameObject.SetActive(false);
                    });
                });
            }
        }

        private void Deactive(object[] arguments)
        {
            disable = false;
        }

        private void Active(object[] arguments)
        {
            disable = true;
        }


        public float testFloat;
        protected override void MB_Start()
        {
            inTutorial = ((GameLevel) LevelManager.Instance.levelData).tutorial;
            
            // cameraHolder.transform.position = ((GameLevel) LevelManager.Instance.levelData).cameraStartPosition;
            // MainCamera.fieldOfView = 100;
            // MainCamera.DOFieldOfView(60, 1f);
        }

        public void StopZoom()
        {
            disable = false;
        }
        private void CameraZoom(float arg0)
        {
            disable = true;
            MainCamera.fieldOfView = slider.value;
        }
        
        private void StartRepeat(object[] arguments)
        {
            if (MainCamera == null)
            {
                MainCamera = Camera.main;
            }

            //cameraHolder.transform.position = ((GameLevel) LevelManager.Instance.levelData).cameraStartPosition;
            MainCamera.fieldOfView = 100;
            //MainCamera.DOFieldOfView(60, 1f);
        }

        private void FinishLevel(object[] arguments)
        {
            StartCoroutine(RestartCamera());
        }

        IEnumerator RestartCamera()
        {
            yield return new WaitForSeconds(.01f);
            // var startPosition = ((GameLevel) LevelManager.Instance.levelData).cameraStartPosition;
            //cameraHolder.transform.DOMove(startPosition, .5f);
            MainCamera.fieldOfView = 100;
        }
        
        protected override void MB_Update()
        {
            if (!disable)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    cameraHolder.transform.position =
                        new Vector3(Mathf.Clamp(cameraObjRb.transform.position.x, -clampValue, clampValue),
                            cameraObjRb.transform.position.y,
                            Mathf.Clamp(cameraObjRb.transform.position.z, -zClamp, zClampPlus));

                    if (Input.GetMouseButtonDown(0))
                    {
                        touchStart = GetWorldPosition(groundZ);

                        cameraHolder.transform.position =
                            new Vector3(Mathf.Clamp(cameraObjRb.transform.position.x, -clampValue, clampValue),
                                cameraObjRb.transform.position.y,
                                Mathf.Clamp(cameraObjRb.transform.position.z, -zClamp, zClampPlus) * 100 *
                                Time.deltaTime);
                        holding = true;
                    }

                    if (Input.touchCount == 2)
                    {
                        Touch touchZero = Input.GetTouch(0);
                        Touch touchOne = Input.GetTouch(1);

                        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                        float difference = currentMagnitude - prevMagnitude;
                        holding = false;
                        zoom(difference * 0.1f); //slow down the zoom on touch devices
                    }
                    else if (Input.GetMouseButton(0) && holding)
                    {
                        Vector3 direction = touchStart - GetWorldPosition(groundZ);
                        cameraHolder.transform.position += direction;

                        cameraHolder.transform.position =
                            new Vector3(Mathf.Clamp(cameraObjRb.transform.position.x, -clampValue, clampValue),
                                cameraObjRb.transform.position.y,
                                Mathf.Clamp(cameraObjRb.transform.position.z, -zClamp, zClampPlus));

                    }
                }
            }
        }
        
        void zoom(float increment){
            // MainCamera.fieldOfView = Mathf.Clamp(MainCamera.fieldOfView - increment, zoomOutMin, zoomOutMax);
        }
        private Vector3 GetWorldPosition(float z){
            Ray mousePos = MainCamera.ScreenPointToRay(Input.mousePosition);
            Plane ground = new Plane(Vector3.up, new Vector3(0,z,0));
            float distance;
            ground.Raycast(mousePos, out distance);
            return mousePos.GetPoint(distance);
        }
    }
}