/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using Pico.Platform;
using Unity.XR.CoreUtils;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR;

namespace BodyTrackingDemo
{
    public class BodyTrackingManager : MonoBehaviour
    {
        public enum LegTrackingDemoState
        {
            CALIBRATING,
            CALIBRATED,
            PLAYING
        }

        public enum FootSide
        {
            Left,
            Right
        }
        
        [Serializable]
        public class FootStepData
        {
            public FootSide footSide;
            public GameObject stepOnToeEffect;
            public GameObject stepOnHeelEffect;
            public GameObject stepOnEffect;
            public GameObject stepOnActionEffect;
            public float effectInterval = 1 / 8f;

            public int FootStepOnAction { get; set; }
            public BodyActionList FootStepOnActionCallBack { get; set; }
            public int FootStepGroundLastAction { get; set; }
            public int FootStepToeOnLastAction { get; set; }
            
            public GameObject CurHeelVFX { get; set; }
            public GameObject CurToeVFX { get; set; }
            public Transform FootBone { get; set; }
            public Transform FootToeBone { get; set; }

            public float CurEffectTime { get; set; }
            public Quaternion HeelRotationOffset { get; set; }
            public Quaternion ToeRotationOffset { get; set; }
        }

        public static BodyTrackingManager Instance;

        private static List<XRInputSubsystem> s_InputSubsystems = new();

        public UIStartup startCanvas;
        public UIMotionTrackerState UIMotionTrackerState;
        public DancePadsManager DancePadManager;
        public GameObject RecorderUI;
        public GameObject gameCanvas;
        public GameObject XROrigin;
        public List<GameObject> FindGameObjects;
        [SerializeField] private AudioSource stepOnToeSFX;
        [SerializeField] private AudioSource stepOnHeelSFX;
        [SerializeField] private FootStepData[] stepDataItems;


        [HideInInspector] public LegTrackingDemoState m_CurrentLegTrackingDemoState;
        private IBodyTrackerSampler _bodyTrackerSampler;
        private float _startFootHeight;
        private float _startXROriginY;
        private float _initXROriginY;
        private Transform _avatarLeftFoot;
        
        private Transform _avatarRightFoot;
        private float _avatarScale;
        private GameObject _curLeftHeelVFX;
        private GameObject _curLeftToeVFX;
        private GameObject _curRightHeelVFX;
        private GameObject _curRightToeVFX;
        private BodyActionList bodylist;
        private bool isFootDownAction;
        private float _timeStamp;
        private int effectType;
        private int bodyIndex;
        private void Awake()
        {
            Instance = this;

            _initXROriginY = _startXROriginY = XROrigin.transform.localPosition.y;
            
            RecorderUI.SetActive(false);
            DancePadManager.gameObject.SetActive(false);
            gameCanvas.SetActive(false);
            UIMotionTrackerState.gameObject.SetActive(false);
            PXR_MotionTracking.BodyTrackingAction += BTAction;
        }

        // Start is called before the first frame update
        private void Start()
        {
            SubsystemManager.GetInstances(s_InputSubsystems);
            foreach (var t in s_InputSubsystems)
            {
                t.TryRecenter();
            }
            UpdateFitnessBandState();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            SetAvatarActive(false);
            if (!pauseStatus)
            {
                UpdateFitnessBandState();
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
               PlayStepOnEffect(BodyActionList.PxrFootDownAction, stepDataItems[1].stepOnActionEffect.transform);
            }
        }
        // Update is called once per frame
        private void LateUpdate()
        {

            if (m_CurrentLegTrackingDemoState == LegTrackingDemoState.PLAYING)
            {
                if (_bodyTrackerSampler == null || !_bodyTrackerSampler.IsLoaded)
                {
                    return;
                }
                stepDataItems[0].FootStepOnAction = (int)_bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_ANKLE).TrackingData.Action;
                stepDataItems[1].FootStepOnAction = (int)_bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_ANKLE).TrackingData.Action;
               // Debug.Log("LakerUpdate0:left:" + _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_ANKLE).TrackingData.Action +":"+ (int)_bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_ANKLE).TrackingData.Action+"right:"+ _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_ANKLE).TrackingData.Action+":"+ (int)_bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_ANKLE).TrackingData.Action);
                stepDataItems[0].FootBone = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_ANKLE).transform;
                stepDataItems[1].FootBone = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_ANKLE).transform;
                stepDataItems[0].FootToeBone = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_FOOT).transform;
                stepDataItems[1].FootToeBone = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_FOOT).transform;
                stepDataItems[0].HeelRotationOffset = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_ANKLE).RotationOffset;
                stepDataItems[1].HeelRotationOffset = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_ANKLE).RotationOffset;
                stepDataItems[0].ToeRotationOffset = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_FOOT).RotationOffset;
                stepDataItems[1].ToeRotationOffset = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_FOOT).RotationOffset;
                //Debug.Log("LakerUpdate:" + stepDataItems[0].FootStepOnAction+":" + stepDataItems[1].FootStepOnAction+":" + stepDataItems[0].FootStepGroundLastAction+":"+stepDataItems[1].FootStepGroundLastAction +":"+ stepDataItems[0].FootStepToeOnLastAction+":"+ stepDataItems[1].FootStepToeOnLastAction);
                //var leftFootPos = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_FOOT).transform.position;
                //var rightFootPos = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_FOOT).transform.position;
                //DancePadManager.DancePadHoleStepOnDetection(leftFootPos, rightFootPos, stepDataItems[0].FootStepOnAction, stepDataItems[1].FootStepOnAction, stepDataItems[0].FootStepOnLastAction, stepDataItems[1].FootStepOnLastAction);
                //Debug.Log("LakerAction::" + stepDataItems[0].FootStepOnAction + "::" + stepDataItems[1].FootStepOnAction);
                if (DancePadManager.IsDancePadGamePlaying)
                {
                    return;
                }
                if (effectType == 2)
                {
                    foreach (var item in stepDataItems)
                    {
                        if ((item.FootStepOnAction & (int)BodyActionList.PxrTouchGround) != 0 && (item.FootStepGroundLastAction & (int)BodyActionList.PxrTouchGround) == 0
                        || (item.FootStepOnAction & (int)BodyActionList.PxrTouchGroundToe) != 0 && (item.FootStepToeOnLastAction & (int)BodyActionList.PxrTouchGroundToe) == 0)
                        {
                            if (Time.time - item.CurEffectTime >= item.effectInterval)
                            {
                                if (item.CurToeVFX != null)
                                {
                                    var particle = item.CurToeVFX.GetComponentInChildren<ParticleSystem>();
                                    particle.Stop(true);
                                }
                                Debug.Log("LakerUpdate2:"+ item.FootStepOnAction+":"+ item.FootStepGroundLastAction+":"+ item.FootStepToeOnLastAction);
                                item.CurToeVFX = PlayStepOnEffect((BodyActionList)((int)BodyActionList.PxrTouchGroundToe | (int)BodyActionList.PxrTouchGround), item);
                                item.CurEffectTime = Time.time;
                            }
                        }
                    }
                }
                else
                {
                    if (effectType == 3)
                    {
                        foreach (var item in stepDataItems)
                        {
                            Debug.Log("LakerUpdate3Action:"+ item.FootStepOnAction+":"+item.FootStepToeOnLastAction + ":Ground:"+(item.FootStepOnAction & (int)BodyActionList.PxrTouchGround) + ":" + (item.FootStepGroundLastAction & (int)BodyActionList.PxrTouchGround)+":Toe:"+ (item.FootStepOnAction & (int)BodyActionList.PxrTouchGroundToe) + ":" + (item.FootStepToeOnLastAction & (int)BodyActionList.PxrTouchGroundToe));
                            if ((item.FootStepOnAction & (int)BodyActionList.PxrTouchGround) != 0 && (item.FootStepGroundLastAction & (int)BodyActionList.PxrTouchGround) == 0)
                            {
                                if (Time.time - item.CurEffectTime >= item.effectInterval)
                                {
                                    if (item.CurHeelVFX != null)
                                    {
                                        var particle = item.CurHeelVFX.GetComponentInChildren<ParticleSystem>();
                                        particle.Stop(true);
                                    }
                                    Debug.Log("LakerUpdate3Ground:"+ item.FootStepOnAction+":"+ item.FootStepGroundLastAction);
                                    item.CurHeelVFX = PlayStepOnEffect(BodyActionList.PxrTouchGround, item);
                                    item.CurEffectTime = Time.time;
                                }
                            }
                            if ((item.FootStepOnAction & (int)BodyActionList.PxrTouchGroundToe) != 0 && (item.FootStepToeOnLastAction & (int)BodyActionList.PxrTouchGroundToe) == 0)
                            {
                                if (Time.time - item.CurEffectTime >= item.effectInterval)
                                {
                                    if (item.CurToeVFX!= null)
                                    {
                                        var particle = item.CurToeVFX.GetComponentInChildren<ParticleSystem>();
                                        particle.Stop(true);
                                    }
                                    Debug.Log("LakerUpdate3Toe:"+ item.FootStepOnAction + ":" + item.FootStepToeOnLastAction);
                                    item.CurToeVFX = PlayStepOnEffect(BodyActionList.PxrTouchGroundToe, item);
                                    item.CurEffectTime = Time.time;
                                }
                            }
                        }
                    }
                    else if (effectType == 4)
                    {
                        foreach (var item in stepDataItems)
                        {
                            if ((item.FootStepOnAction & (int)BodyActionList.PxrTouchGround) != 0 && (item.FootStepGroundLastAction & (int)BodyActionList.PxrTouchGround) == 0)
                            {
                                if (Time.time - item.CurEffectTime >= item.effectInterval)
                                {
                                    if (item.CurHeelVFX != null)
                                    {
                                        var particle = item.CurHeelVFX.GetComponentInChildren<ParticleSystem>();
                                        particle.Stop(true);
                                    }
                                    Debug.Log("LakerUpdate4Ground:"+ item.FootStepOnAction+":"+ item.FootStepGroundLastAction);
                                    item.CurHeelVFX = PlayStepOnEffect(BodyActionList.PxrTouchGround, item);
                                    item.CurEffectTime = Time.time;
                                }
                            }
                        }
                    }
                    else if (effectType == 5)
                    {
                        foreach (var item in stepDataItems)
                        {
                            if ((item.FootStepOnAction & (int)BodyActionList.PxrTouchGroundToe) != 0 && (item.FootStepToeOnLastAction & (int)BodyActionList.PxrTouchGroundToe) == 0)
                            {
                                if (Time.time - item.CurEffectTime >= item.effectInterval)
                                {
                                    if (item.CurToeVFX != null)
                                    {
                                        var particle = item.CurToeVFX.GetComponentInChildren<ParticleSystem>();
                                        particle.Stop(true);
                                    }
                                    Debug.Log("LakerUpdate5Toe:"+ item.FootStepOnAction+":"+ item.FootStepToeOnLastAction);
                                    item.CurToeVFX = PlayStepOnEffect(BodyActionList.PxrTouchGroundToe, item);
                                    item.CurEffectTime = Time.time;
                                }
                            }
                        }
                    }
                }
                foreach (var item in stepDataItems)
                {
                    item.FootStepGroundLastAction = item.FootStepOnAction;
                    item.FootStepToeOnLastAction = item.FootStepOnAction;
                }
            }
        }

        private void OnApplicationFocus(bool focus)
        {
#if !UNITY_EDITOR
            SetAvatarActive(false);
#endif
            if (focus)
            {
                UpdateFitnessBandState();
            }
        }

        private void StartGame(float height)
        {
            if (m_CurrentLegTrackingDemoState == LegTrackingDemoState.CALIBRATED)
            {
                return;
            }
            
            m_CurrentLegTrackingDemoState = LegTrackingDemoState.CALIBRATED;
            
            var xrOriginPos = XROrigin.transform.localPosition;
            xrOriginPos.y = _startXROriginY = _initXROriginY;
            XROrigin.transform.localPosition = xrOriginPos;

            //load avatar
            StartCoroutine(LoadAvatar(PlayerPrefManager.Instance.PlayerPrefData.avatarName, height));
        }
        
        private IEnumerator LoadAvatar(string avatarName, float height)
        {
            if (height <= 50)
            {
                height = AvatarManager.DefaultAvatarHeight;
                Debug.LogWarning($"LoadAvatar: Height = {height} is too small, it be set to {AvatarManager.DefaultAvatarHeight}, please check!");
            }

            XROrigin.transform.rotation = Quaternion.identity;

            if (_bodyTrackerSampler == null)
            {
                var avatarObj = AvatarManager.Instance.LoadAvatar(avatarName, XROrigin.transform);
                avatarObj.transform.localScale = Vector3.one;
                avatarObj.transform.localPosition = Vector3.zero;
                avatarObj.name = avatarName;
                avatarObj.SetActive(true);
                _bodyTrackerSampler = avatarObj.GetComponent<IBodyTrackerSampler>();    
            }

            if (_bodyTrackerSampler != null)
            {
             
                yield return new WaitUntil(() => _bodyTrackerSampler.IsLoaded);
#if UNITY_EDITOR
                height = _bodyTrackerSampler.AvatarHeight;
#endif
                _avatarScale = height / _bodyTrackerSampler.AvatarHeight;
                _bodyTrackerSampler.FindGameObject(FindGameObjects);
                //_bodyTrackerSampler.UpdateBonesLength(height);//????????????????
                _bodyTrackerSampler.SetCubeJointActive(PlayerPrefManager.Instance.PlayerPrefData.showJoint);

                yield return new WaitForEndOfFrame();
            
                AlignGround();
                AlignEyes();

                _bodyTrackerSampler.SetActive(true);
            }
            
            gameCanvas.SetActive(true);
            UIMotionTrackerState.gameObject.SetActive(true);
#if RECORDER
            RecorderUI.SetActive(true);
#endif

            m_CurrentLegTrackingDemoState = LegTrackingDemoState.PLAYING;

            Debug.Log($"BodyTrackingManager.LoadAvatar: Avatar = {avatarName}, height = {height}");
        }

        private void UpdateFitnessBandState()
        {
            //PXR_Input.SetSwiftMode(PlayerPrefManager.Instance.PlayerPrefData.bodyTrackMode);

            switch (PlayerPrefManager.Instance.PlayerPrefData.bodyTrackMode)
            {
                case 0:
                    PXR_Input.SetBodyTrackingMode(BodyTrackingMode.BTM_FULL_BODY_LOW);
                    break;
                case 1:
                    PXR_Input.SetBodyTrackingMode(BodyTrackingMode.BTM_FULL_BODY_HIGH);
                    break;

            }
  
            //Update Swift calibration state after resuming
            var calibrated = 0;
            PXR_Input.GetMotionTrackerCalibState(ref calibrated);

            var connectState = new PxrMotionTracker1ConnectState();
            PXR_Input.GetMotionTrackerConnectStateWithID(ref connectState);

            if (calibrated == 1)
            {
                startCanvas.startMenu.SetActive(false);
                StartGame();
                Debug.Log($"BodyTrackingManager.UpdateFitnessBandState: calibrated = {calibrated}");
            }
            else
            {
                bool isTracking = false;
                BodyTrackingState trackingState = default(BodyTrackingState);
                PXR_MotionTracking.GetBodyTrackingState(ref isTracking, ref trackingState);

                startCanvas.startMenu.SetActive(true);
#if !UNITY_EDITOR && !DEBUG
                startCanvas.btnContinue.gameObject.SetActive(trackingState.connectedBandCount >= 2 && trackingState.code == TrackingStateCode.PXR_MT_SUCCESS && trackingState.stateCode != BodyTrackingStatusCode.BT_INVALID);
#endif
                if (trackingState.stateCode == BodyTrackingStatusCode.BT_LIMITED)
                {
                    calibrated = 2;
                }
                
                Debug.Log($"BodyTrackingManager.UpdateFitnessBandState: connectedNum = {connectState.num}, trackingState = {trackingState}");
            }
            
            var type = PXR_MotionTracking.GetMotionTrackerDeviceType();
            UIMotionTrackerState.UpdateTrackerState(type , connectState.num, calibrated);
        }
        private void PlayStepOnEffect(BodyActionList action, Transform footStepData)
        {
            var targetPos = footStepData.position;
            var vfx = Instantiate(footStepData, targetPos, transform.rotation);
            vfx.transform.localScale = Vector3.one * _avatarScale;
            vfx.gameObject.SetActive(true);
            AudioManager.Instance.PlayEffect(stepOnToeSFX, targetPos);
            Debug.Log($"BodyTrackingManager.PlayStepOnEffectPxrTouchGroundToe, targetPos = {targetPos}");

        }
        private GameObject PlayStepOnEffect(BodyActionList action, FootStepData footStepData)
        {
    
            if (action == 0) return null;

            if (action == BodyActionList.PxrTouchGroundToe)
            {
                var targetPos = footStepData.FootToeBone.position;
                var vfx = Instantiate(footStepData.stepOnToeEffect, targetPos, footStepData.ToeRotationOffset * footStepData.stepOnToeEffect.transform.rotation);
                vfx.transform.localScale = Vector3.one * _avatarScale;
                vfx.SetActive(true);
                AudioManager.Instance.PlayEffect(stepOnToeSFX, targetPos);
                Debug.Log($"BodyTrackingManager.PlayStepOnEffectPxrTouchGroundToe, targetPos = {targetPos}");
                return vfx;
            }
            else if (action == BodyActionList.PxrTouchGround)
            {
                var targetPos = footStepData.FootToeBone.position;
                var vfx = Instantiate(footStepData.stepOnHeelEffect, targetPos, footStepData.HeelRotationOffset * footStepData.stepOnHeelEffect.transform.rotation);
                vfx.transform.localScale = Vector3.one * _avatarScale;
                vfx.SetActive(true);
                AudioManager.Instance.PlayEffect(stepOnHeelSFX, targetPos);
                Debug.Log($"BodyTrackingManager.PlayStepOnEffectPxrTouchGroundToe, targetPos = {targetPos}");
                return vfx;
            }
            else if (action == (BodyActionList)((int)BodyActionList.PxrTouchGround | (int)BodyActionList.PxrTouchGroundToe))
            {
                var targetPos = footStepData.FootToeBone.position;
                var vfx = Instantiate(footStepData.stepOnEffect, targetPos, footStepData.ToeRotationOffset * footStepData.stepOnEffect.transform.rotation);
                vfx.transform.localScale = Vector3.one * _avatarScale;
                vfx.SetActive(true);
                AudioManager.Instance.PlayEffect(stepOnToeSFX, targetPos);
                Debug.Log($"BodyTrackingManager.PlayStepOnEffectPxrTouchGroundToe&&PxrTouchGround, targetPos = {targetPos}");
                return vfx;
            }
            else if (action==BodyActionList.PxrFootDownAction)
            {
                var targetPos = footStepData.FootToeBone.position;
                var vfx = Instantiate(footStepData.stepOnActionEffect, targetPos, footStepData.ToeRotationOffset * footStepData.stepOnActionEffect.transform.rotation);
                vfx.transform.localScale = Vector3.one * _avatarScale;
                vfx.SetActive(true);
                AudioManager.Instance.PlayEffect(stepOnToeSFX, targetPos);
                //bodylist=BodyActionList.PxrNoneAction;
                Debug.Log($"BodyTrackingManager.PlayStepOnEffectPxrFootDownAction, targetPos = {targetPos}");
                return vfx;
            }

            return null;
        }
        private void BTAction(int joint, BodyActionList action)
        {
            bodyIndex = joint - 7;
            if (m_CurrentLegTrackingDemoState == LegTrackingDemoState.PLAYING)
            {
                if (_bodyTrackerSampler == null || !_bodyTrackerSampler.IsLoaded)
                {
                    return;
                }
                effectType = PlayerPrefManager.Instance.PlayerPrefData.steppingEffect;
                if (effectType == 1)
                {
                    if (action == BodyActionList.PxrFootDownAction)
                    {
                        stepDataItems[0].FootStepOnActionCallBack = action;
                        stepDataItems[1].FootStepOnActionCallBack = action;
                        stepDataItems[0].FootBone = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_ANKLE).transform;
                        stepDataItems[1].FootBone = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_ANKLE).transform;
                        stepDataItems[0].FootToeBone = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_FOOT).transform;
                        stepDataItems[1].FootToeBone = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_FOOT).transform;
                        stepDataItems[0].HeelRotationOffset = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_ANKLE).RotationOffset;
                        stepDataItems[1].HeelRotationOffset = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_ANKLE).RotationOffset;
                        stepDataItems[0].ToeRotationOffset = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_FOOT).RotationOffset;
                        stepDataItems[1].ToeRotationOffset = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_FOOT).RotationOffset;
                        if (Time.time - stepDataItems[bodyIndex].CurEffectTime >= stepDataItems[bodyIndex].effectInterval)
                        {
                            if (stepDataItems[bodyIndex].CurToeVFX != null)
                            {
                                var particle = stepDataItems[bodyIndex].CurToeVFX.GetComponentInChildren<ParticleSystem>();
                                particle.Stop(true);
                            }
                            Debug.Log("LakerCallBackPxrFootDownAction");
                            stepDataItems[bodyIndex].CurToeVFX = PlayStepOnEffect(BodyActionList.PxrFootDownAction, stepDataItems[bodyIndex]);
                            stepDataItems[bodyIndex].CurEffectTime = Time.time;
                        }
                    }
                }
            }
        }
        private void AlignEyes()
        {
            if (_bodyTrackerSampler == null)
            {
                Debug.LogError("There is no loaded avatar!");
                return;
            }
            
            var xrOrigin = XROrigin.GetComponent<XROrigin>();
            
            var offsetObjectPos = xrOrigin.CameraFloorOffsetObject.transform.localPosition;

#if UNITY_EDITOR
            offsetObjectPos.y = _bodyTrackerSampler.AvatarHeight * 0.01f - _bodyTrackerSampler.EyeOffsetY;
#else
            if (_bodyTrackerSampler.EyeOffsetY > 0)
                offsetObjectPos.y = _bodyTrackerSampler.EyeOffsetY * _avatarScale;
#endif
            xrOrigin.CameraFloorOffsetObject.transform.localPosition = offsetObjectPos;
            
            Debug.Log($"BodyTrackingManager.AlignEyes end: offsetObjectPos = {offsetObjectPos}, avatarScale = {_avatarScale}");
        }
        
        #region Public Method

        public void AlignGround()
        {
            if (_bodyTrackerSampler == null || !_bodyTrackerSampler.IsLoaded)
            {
                Debug.LogError("There is no loaded avatar!");
                return;
            }

            var leftFootPos = _bodyTrackerSampler.GetJoint(BodyTrackerRole.LEFT_FOOT).transform.position;
            var rightFootPos = _bodyTrackerSampler.GetJoint(BodyTrackerRole.RIGHT_FOOT).transform.position;
            
            _startFootHeight = Mathf.Min(leftFootPos.y, rightFootPos.y);

            var xrOriginPos = XROrigin.transform.localPosition;

            xrOriginPos.y = _startXROriginY + -(_startFootHeight - _bodyTrackerSampler.SoleHeight);

            XROrigin.transform.localPosition = xrOriginPos;
            _startXROriginY = xrOriginPos.y;

            Debug.Log($"BodyTrackingManager.AlignGround: StartFootHeight = {_startFootHeight}, xrOriginPos = {xrOriginPos}");
        }

        public void StartGame()
        {
            try
            {
                var task = SportService.GetUserInfo();
                if (task != null)
                    task.OnComplete(rsp =>
                    {
                        if (!rsp.IsError)
                        {
                            if (rsp.Data.Stature > 50)
                            {
                                if (Math.Abs(PlayerPrefManager.Instance.PlayerPrefData.height - rsp.Data.Stature) > Mathf.Epsilon)
                                {
                                    PlayerPrefManager.Instance.PlayerPrefData.height = rsp.Data.Stature;
                                    Events.OnUserHeightChanged(rsp.Data.Stature);
                                }
                            }
                            Debug.Log($"SportService.Ge tUserInfo: Success, Height = {rsp.Data.Stature}");
                        }
                        else
                        {
                            Debug.LogWarning($"SportService.GetUserInfo: Failed, msg = {rsp.Error}");
                        }

                        StartGame(PlayerPrefManager.Instance.PlayerPrefData.height);
                    });
                else
                    StartGame(PlayerPrefManager.Instance.PlayerPrefData.height);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                StartGame(PlayerPrefManager.Instance.PlayerPrefData.height);
            }
        }
        
        public void SetAvatarActive(bool active)
        {
            _bodyTrackerSampler?.SetActive(active);
        }
        
        public void ReloadAvatar(string avatarName)
        {
            if (_bodyTrackerSampler != null)
            {
                _bodyTrackerSampler.Destroy();
                _bodyTrackerSampler = null;
            }

            StartCoroutine(LoadAvatar(avatarName, PlayerPrefManager.Instance.PlayerPrefData.height));
        }

        public void ShowJoint(bool value)
        {
            _bodyTrackerSampler?.SetCubeJointActive(value);
        }

        public void StartCalibrate()
        {
            PXR_MotionTracking.StartMotionTrackerCalibApp();
            m_CurrentLegTrackingDemoState = LegTrackingDemoState.CALIBRATING;
            SetAvatarActive(false);
        }
        public void StartOpenSwiftAPP()
        {
            PXR_MotionTracking.StartMotionTrackerCalibApp();
        }
        #endregion
    }
}