using DG.Tweening;
using Game.Actors;
using Game.GlobalVariables;
using Game.Managers;
using TMPro;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace Game.Actors
{
    public class BuildingControllerActor : Actor<BuildingManager>
    {
        public GameObject _selectedObject;
        private bool _holding;
        private float _defaultHeight;
        private Vector3 _firstPos;
        
        private Vector3 _pos1;
        
        private TapInputControl _gameInputControls;
        private DragDropControl _dragDropControl;
        private bool _wrongMerge;

        public TextMeshProUGUI soldierCountText;

        public Button captureButton, attackButton;
        public GameObject attackArrow1,arw2,arw3, captureArrow1,arrw2,arrw3;
        public float pngCounter;
        public bool atck, cptr;

        protected override void MB_Awake()
        {
            // _gameInputControls = new TapInputControl();
            _dragDropControl = new DragDropControl();
        }

        protected override void MB_OnEnable()
        {
            // _gameInputControls.MainMap.Tap.started += Select;
            // _gameInputControls.MainMap.Tap.canceled += Deselect;
            // _gameInputControls.MainMap.Tap.Enable();

            _dragDropControl.MainMap.DragDrop.performed += DragDropHold;
            _dragDropControl.MainMap.DragDrop.canceled += HoldRelease;
            _dragDropControl.MainMap.DragDrop.Enable();
        }

        protected override void MB_OnDisable()
        {
            // _gameInputControls.MainMap.Tap.started -= Select;
            // _gameInputControls.MainMap.Tap.canceled -= Deselect;
            // _gameInputControls.MainMap.Tap.Disable();

            _dragDropControl.MainMap.DragDrop.performed -= DragDropHold;
            _dragDropControl.MainMap.DragDrop.canceled -= HoldRelease;
            _dragDropControl.MainMap.DragDrop.Disable();
        }
        
        private void Select(InputAction.CallbackContext obj)
        {
            // RaycastHit hit;
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //
            // if (Physics.Raycast(ray, out hit, Mathf.Infinity, Manager.dragObjectsLayer))
            // {
            //     _selectedObject = hit.transform.gameObject;
            // }
        }
        
        private void Deselect(InputAction.CallbackContext obj)
        {
            // if (_selectedObject == null) return;
            //
            // //Instantiate soldier.
            // if (!_holding)
            // {
            //     Push(CustomManagerEvents.SpawnSoldier, _selectedObject);
            //     _selectedObject.transform.DOPunchScale(new Vector3(0.1f, -0.1f, 0.1f), 0.15f);
            //     _selectedObject = null;
            // }
        }

        
        private void DragDropHold(InputAction.CallbackContext obj)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Manager.dragObjectsLayer))
            {
                _selectedObject = hit.transform.gameObject;
                
                _firstPos = _selectedObject.transform.position;

                _selectedObject.GetComponent<BuildMergeControl>().moving = true;
                _selectedObject.GetComponent<BuildMergeControl>().canMerge = true;
                _selectedObject.GetComponent<BuildMergeControl>().merger = true;

                if (_selectedObject.CompareTag("BaseBuilding"))
                    Push(CustomManagerEvents.MovingObject, _selectedObject.gameObject,0);
                
                if(_selectedObject != null)
                    if (_selectedObject.CompareTag("NormalBuilding"))
                        Push(CustomManagerEvents.MovingObject, _selectedObject.gameObject, 1);
                
                _selectedObject.transform.localScale = new Vector3(1.2f, 1.7f, 1.2f);
                    
                _defaultHeight = _selectedObject.transform.position.y;
                
                _holding = true;
            }
        }
        
        private void HoldRelease(InputAction.CallbackContext obj)
        {
            if(!_holding) return;
            if (_selectedObject == null) return;
            
            _selectedObject.transform.localScale = new Vector3(1f, 1.5f, 1f);
            
            Vector3 dropPosition = _selectedObject.transform.position;
            
            dropPosition.y = _defaultHeight;
            
            _selectedObject.transform.position = dropPosition;

            _selectedObject.GetComponent<BuildMergeControl>().moving = false;
            
            _holding = false;
            
            if (_selectedObject.CompareTag("BaseBuilding"))
            {
                print("Base");
                Push(CustomManagerEvents.Merging, _selectedObject.gameObject, 0);
            }

            if(_selectedObject != null)
                if (_selectedObject.CompareTag("NormalBuilding"))
                {
                    print("Normal");
                    Push(CustomManagerEvents.Merging, _selectedObject.gameObject, 1);
                }
        }
        
        protected override void MB_Update()
        {
            if (Manager.soldierCount <= 0)
                Manager.soldierCount = 0;
            
            soldierCountText.text = Manager.soldierCount.ToString();

            if (atck)
            {
                pngCounter += 3 * Time.deltaTime;
                if (pngCounter >= 0f)
                {
                    attackArrow1.SetActive(true);
                }

                if (pngCounter >= 0.3f)
                {
                    arw2.SetActive(true);
                }

                if (pngCounter >= 0.6f)
                {
                    arw3.SetActive(true);
                }
                
                if (pngCounter >= 0.9f)
                {
                    atck = false;
                    attackArrow1.SetActive(false);
                    arw2.SetActive(false);
                    arw3.SetActive(false);
                    pngCounter = 0;
                }
            }

            if (cptr)
            {
                pngCounter += 3 * Time.deltaTime;
                if (pngCounter >= 0f)
                {
                    captureArrow1.SetActive(true);
                }

                if (pngCounter >= 0.3f)
                {
                    arrw2.SetActive(true);
                }

                if (pngCounter >= 0.6f)
                {
                    arrw3.SetActive(true);
                }
                
                if (pngCounter >= 0.9f)
                {
                    cptr = false;
                    captureArrow1.SetActive(false);
                    arrw2.SetActive(false);
                    arrw3.SetActive(false);
                    pngCounter = 0;
                }
            }
            
            if (_holding)
            {
                RaycastHit hit;
        
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
                if (Physics.Raycast(ray, out hit, Mathf.Infinity,Manager.groundsLayer))
                {
                    _pos1 = hit.point;
        
                    _pos1.y = Manager.selectedObjectHeight;

                    // if (_selectedObject != null)
                    //     _selectedObject.transform.position = _pos1;

                    if (_selectedObject != null)
                        _selectedObject.transform.position = Vector3.Lerp(_selectedObject.transform.position, _pos1, 0.2f);
                }
            }
        }
        
        protected override void MB_Listen(bool status)
        {
            if (status)
            {
                Manager.Subscribe(CustomManagerEvents.WrongMerge,WrongMerge);
                captureButton.onClick.AddListener(CaptureButton);
                attackButton.onClick.AddListener(AttackButton);
            }
            else
            {
                Manager.Unsubscribe(CustomManagerEvents.WrongMerge,WrongMerge);
                captureButton.onClick.RemoveListener(CaptureButton);
                attackButton.onClick.RemoveListener(AttackButton);
            }
        }

        private void AttackButton()
        {
            atck = true;
            attackButton.transform.DOPunchScale(new Vector3(0.1f, -0.1f, 0.1f), 0.15f).OnComplete(() =>
            {
                attackButton.transform.localScale = new Vector3(1, 1, 1);
            });
            
            Push(CustomManagerEvents.SpawnSoldier, false);
            VibrationManager.Instance.TriggerSoftImpact();
        }

        private void CaptureButton()
        {
            cptr = true;
            captureButton.transform.DOPunchScale(new Vector3(0.1f, -0.1f, 0.1f), 0.15f).OnComplete(() =>
            {
                captureButton.transform.localScale = new Vector3(1, 1, 1);
            });
            
            Push(CustomManagerEvents.SpawnSoldier, true);
            VibrationManager.Instance.TriggerSoftImpact();
        }
        
        
        private void WrongMerge(object[] args)
        {
            if (_selectedObject != null)
            {
                var obj1 = (bool) args[0];
                var obj2 = (bool) args[1];
                
                if (!obj1 && obj2)
                {
                    _selectedObject.transform.position = _firstPos;
                    _selectedObject = null;
                }
                Push(CustomManagerEvents.ResetMergeBools, false);
                VibrationManager.Instance.TriggerFailure();
            }
        }
    }
}