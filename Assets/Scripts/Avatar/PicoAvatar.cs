/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System;
using Pico.Avatar;
using Pico.Avatar.Sample;
using Unity.XR.PXR;
using UnityEngine;

namespace BodyTrackingDemo.Avatar
{
    [RequireComponent(typeof(ActionAvatar))]
    public class PicoAvatar : MonoBehaviour, IBodyTrackerSampler
    {
        #region Property
        
        public float EyeOffsetY { get; private set; } //= 0.1f;
        public float AvatarHeight { get; } = 170;
        public float SoleHeight { get; } = 0;
        public bool IsLoaded => _isLoaded;

        #endregion

        #region PrivateField
        private int currentNumber;
        private DateTime startTime;
        private GameObject gameCube;
        private ActionAvatar _actionAvatar;
        private BodyTrackerJoint[] _joints;
        private List<GameObject> _cubeJoints;
        private bool _isLoaded;
        private BodyTrackerResult bodyTrackerResult;
        [SerializeField]
        private Material cubeMaterial;
        [SerializeField]
        private Material lineMaterial;
        [SerializeField]
        private Material headMaterial;
        private UIRecorder uiRecorder;
        private CSVBodyTrackerResult csvBodyTrackerResult;
        private GameObject _gameObject;
        private GameObject[] headAndController;
        private List<GameObject> cubeJoint;
        private Quaternion m_JointRotation;
        private Vector3 avatarHipsPostion;
        private Transform[] SkeletonNodes = new Transform[24];
        private int[,] m_Betweens = { { 0,0},{ 0, 1 },
                                  { 0, 2 },
                                  { 1,4 },
                                  { 4,7 },
                                  { 7,10 },
                                  { 2,5 },
                                  { 5,8 },
                                  { 8,11 },
                                  { 0,3 },
                                  { 3,6 },
                                  { 6,9 },
                                  { 9,12 },
                                  { 12,15 },
                                  { 9, 13 },
                                  { 13,16 },
                                  { 16,18 },
                                  { 18,20},
                                  {20,22 },
                                  { 9,14},
                                  { 14,17},
                                  { 17,19},
                                  { 19,21},{21,23} };
        #endregion

        #region Static

        private static readonly JointType[] JointTypes = {
            JointType.Hips,
            JointType.LeftLegUpper,
            JointType.RightLegUpper,
            JointType.SpineLower,
            JointType.LeftLegLower,
            JointType.RightLegLower,
            JointType.SpineMiddle,
            JointType.LeftFootAnkle,
            JointType.RightFootAnkle,
            JointType.SpineUpper,
            JointType.LeftToe,
            JointType.RightToe,
            JointType.Neck,
            JointType.LeftShoulder,
            JointType.RightShoulder,
            JointType.Head,
            JointType.LeftArmUpper,
            JointType.RightArmUpper,
            JointType.LeftArmLower,
            JointType.RightArmLower,
            JointType.LeftHandWrist,
            JointType.RightHandWrist,
            JointType.LeftHandMiddleMetacarpal,
            JointType.RightHandMiddleMetacarpal,
            JointType.LeftEye,
            JointType.RightEye,
        };

        #endregion
 
        private void Awake()
        {
            _gameObject = gameObject;
            cubeJoint = new List<GameObject>(27);
            headAndController = new GameObject[3];
            bodyTrackerResult = new BodyTrackerResult();
            bodyTrackerResult.trackingdata = new BodyTrackerTransform[24];
            csvBodyTrackerResult = new CSVBodyTrackerResult();
            csvBodyTrackerResult.csvtrackingdata = new CSVBodyTrackerTransform[24];
            _gameObject = gameObject;
            _actionAvatar = GetComponent<ActionAvatar>();
            _actionAvatar.criticalJoints = JointTypes;
            _actionAvatar.loadedFinishCall += OnAvatarLoaded;
            _joints = new BodyTrackerJoint[(int) BodyTrackerRole.ROLE_NUM];
             startTime = DateTime.Now;
        }
        void Update()
        {
            if (!_isLoaded)
            {
                return;
            }
            

            var state = PXR_Input.GetBodyTrackingPose(PXR_System.GetPredictedDisplayTime(), ref bodyTrackerResult);
            if (state != 0)
            {
                return;
            }
            
            // var localPose = bodyTrackerResult.trackingdata[0].localpose;
            // var hipsLocalPos = new Vector3((float)localPose.PosX, (float)localPose.PosY, (float)localPose.PosZ);
            for (int i = 0; i < (int)BodyTrackerRole.ROLE_NUM; i++)
            {
                var localPose = bodyTrackerResult.trackingdata[i].localpose;

                _joints[i].TrackingData = bodyTrackerResult.trackingdata[i];
                _joints[i].RotationOffset = new Quaternion((float)localPose.RotQx, (float)localPose.RotQy, (float)localPose.RotQz, (float)localPose.RotQw);
                if (_cubeJoints != null)
                {
                    cubeJoint[i].transform.localPosition = new Vector3((float)bodyTrackerResult.trackingdata[i].localpose.PosX,
                   (float)bodyTrackerResult.trackingdata[i].localpose.PosY, (float)bodyTrackerResult.trackingdata[i].localpose.PosZ);
                    cubeJoint[i].transform.rotation = _joints[i].RotationOffset;
                }
                if (_cubeJoints != null && _cubeJoints.Count == 27)
                {
                    SkeletonNodes[i] = _cubeJoints[i].transform;
                    SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(0, SkeletonNodes[m_Betweens[i, 0]].position);
                    SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(1, SkeletonNodes[m_Betweens[i, 1]].position);
                }
                if (uiRecorder != null && uiRecorder.isStartRecorderData)
                {
                    uiRecorder.RecorderBodyTrackingData(((DateTime.Now - startTime).TotalMilliseconds / 1000).ToString("F6"), bodyTrackerResult);
                }
            }
            //if (uiRecorder != null && uiRecorder.isStartRecorderData)
            //{
            //    _joints[0].TrackingData = GetPosition(bodyTrackerResult.trackingdata[0]);
            //    StartBodyTracking(false);
            //    uiRecorder.RecorderBodyTrackingData(((DateTime.Now - startTime).TotalMilliseconds / 1000).ToString("F6"), m_BodyTrackerResult);
            //}
            //else if (uIReadData != null && uIReadData.isStartRead)
            //{
            //    uIReadData.CSVTrackerResult(currentNumber, ref csvBodyTrackerResult);
            //    BonesList[0].localPosition = GetPosition(csvBodyTrackerResult.csvtrackingdata[0]);
            //    transform.localPosition = new Vector3(-BonesList[0].localPosition.x, 0, (BonesList[0].localPosition.z));
            //    //gameCube.transform.localPosition = new Vector3(-BonesList[0].localPosition.x,1- BonesList[0].localPosition.y, -(BonesList[0].localPosition.z + 0.1f));
            //    StartBodyTracking(true);
            //    currentNumber++;
            //}
            //else
            //{
            //    BonesList[0].localPosition = GetPosition(m_BodyTrackerResult.trackingdata[0]);
            //    StartBodyTracking(false);
            //}
        }
        private void StartBodyTracking(bool isReadCSV)
        {
            for (int i = 0; i < (int)BodyTrackerRole.ROLE_NUM; i++)
            {
                var localPose = bodyTrackerResult.trackingdata[i].localpose;
                if (!isReadCSV)
                    {
                    _joints[i].TrackingData = bodyTrackerResult.trackingdata[i];
                    _joints[i].RotationOffset = new Quaternion((float)localPose.RotQx, (float)localPose.RotQy, (float)localPose.RotQz, (float)localPose.RotQw);
                    if (_cubeJoints != null)
                        {
                            cubeJoint[i].transform.localPosition = new Vector3((float)bodyTrackerResult.trackingdata[i].localpose.PosX,
                        (float)bodyTrackerResult.trackingdata[i].localpose.PosY, (float)bodyTrackerResult.trackingdata[i].localpose.PosZ);
                            cubeJoint[i].transform.rotation = _joints[i].RotationOffset;
                        }
                    }
                    else
                    {
                        m_JointRotation.x = (float)csvBodyTrackerResult.csvtrackingdata[i].localpose.RotQx;
                        m_JointRotation.y = (float)csvBodyTrackerResult.csvtrackingdata[i].localpose.RotQy;
                        m_JointRotation.z = (float)csvBodyTrackerResult.csvtrackingdata[i].localpose.RotQz;
                        m_JointRotation.w = (float)csvBodyTrackerResult.csvtrackingdata[i].localpose.RotQw;
                        if (_cubeJoints != null)
                        {
                            cubeJoint[i].transform.localPosition = new Vector3((float)csvBodyTrackerResult.csvtrackingdata[i].localpose.PosX,
                  (float)csvBodyTrackerResult.csvtrackingdata[i].localpose.PosY, (float)csvBodyTrackerResult.csvtrackingdata[i].localpose.PosZ);
                            cubeJoint[i].transform.rotation = m_JointRotation;
                        }
                    }
                    _joints[i].RotationOffset = m_JointRotation * _joints[i].StartRotation;
                    if (_cubeJoints != null && _cubeJoints.Count == 27)
                    {
                        SkeletonNodes[i] = _cubeJoints[i].transform;
                        SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(0, SkeletonNodes[m_Betweens[i, 0]].position);
                        SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(1, SkeletonNodes[m_Betweens[i, 1]].position);
                    }
                }
        }
        private void OnAvatarLoaded(ActionAvatar avatar)
        {
            // uiRecorder = GameObject.Find("UIRecorder").GetComponent<UIRecorder>();
            Debug.Log("LakerOnavatarLoaded");
            IEnumerator WaitForAFrame()
            {//Wait for joint instance.
                yield return new WaitForEndOfFrame();
                _isLoaded = true;
                var count = (int) BodyTrackerRole.ROLE_NUM;
                for (int i = 0; i < count; i++)
                {
                    var joint = _actionAvatar.Avatar.entity.transform.Find(JointTypes[i].ToString());
                    var bodyTrackerJoint = joint.GetComponent<BodyTrackerJoint>();
                    if (bodyTrackerJoint == null)
                    {
                        bodyTrackerJoint = joint.gameObject.AddComponent<BodyTrackerJoint>();
                        bodyTrackerJoint.bodyTrackerRole = (BodyTrackerRole) i;
                    }

                    bodyTrackerJoint.StartRotation = joint.rotation;
                    _joints[i] = bodyTrackerJoint;
                }

                var leftEye = _actionAvatar.Avatar.GetJointObject(JointType.LeftEye);
                if (leftEye != null && leftEye.transform.localPosition != Vector3.zero)
                {
                    EyeOffsetY = leftEye.transform.position.y - GetJoint(BodyTrackerRole.HEAD).transform.position.y;
                }
                
                var leftAnkle = GetJoint(BodyTrackerRole.LEFT_ANKLE);
                var leftCollider = leftAnkle.gameObject.AddComponent<BoxCollider>();
                
                leftCollider.size = new Vector3(0.1f, 0.24f, 0.1f);
                leftCollider.center = new Vector3(-0.055f, -0.07f, 0);
                leftCollider.isTrigger = true;
                leftAnkle.gameObject.layer = LayerMask.NameToLayer("PlayerFoot");
                var rightAnkle = GetJoint(BodyTrackerRole.RIGHT_ANKLE);
                var rightCollider = rightAnkle.gameObject.AddComponent<BoxCollider>();
                rightCollider.size = new Vector3(0.1f, 0.24f, 0.1f);
                rightCollider.center = new Vector3(0.055f, 0.07f, 0);
                rightCollider.isTrigger = true;
                rightCollider.gameObject.layer = LayerMask.NameToLayer("PlayerFoot");
                
                Debug.Log("LakerWaitForAFrame");
            }

            StartCoroutine(WaitForAFrame());
        }

        public void SetCubeJointActive(bool value)
        {
            //var skinnedMeshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            //foreach (var item in skinnedMeshRenderers)
            //{
            //    item.gameObject.SetActive(!value);
            //}
            //if (_cubeJoints == null)
            //{
            //    int count = (int) BodyTrackerRole.ROLE_NUM;
            //    _cubeJoints = new List<GameObject>(count);

            //    for (var i = 0; i < count; i++)
            //    {
            //        var cubeJoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //        cubeJoint.transform.SetParent(_joints[i].transform);
            //        cubeJoint.transform.localPosition = Vector3.zero;
            //        cubeJoint.transform.localScale = Vector3.one * .03f;
            //        cubeJoint.transform.localRotation = Quaternion.identity;
            //        _cubeJoints.Add(cubeJoint);
            //    }
            //}

            //foreach (var item in _cubeJoints)
            //{
            //    item.SetActive(value);
            //}

            Debug.Log("LakerTest");
            var skinnedMeshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            //headAndController[0] = GameObject.Find("Main Camera");
            //headAndController[1] = GameObject.Find("LeftHand Controller");
            //headAndController[2] = GameObject.Find("RightHand Controller");
            //gameCube = GameObject.Find("GameCube");
            foreach (var item in skinnedMeshRenderers)
            {
                item.gameObject.SetActive(!value);
            }
            int SkeletonNodesNumber = 0;
            if (_cubeJoints == null)
            {
                var count = (int)BodyTrackerRole.ROLE_NUM;
                _cubeJoints = new List<GameObject>(count);
                for (var i = 0; i < count; i++)
                {

                    cubeJoint.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
                    cubeJoint[SkeletonNodesNumber].transform.SetParent(gameCube.transform);
                    cubeJoint[SkeletonNodesNumber].transform.localPosition = new Vector3((float)bodyTrackerResult.trackingdata[SkeletonNodesNumber].localpose.PosX,
                    (float)bodyTrackerResult.trackingdata[SkeletonNodesNumber].localpose.PosY, (float)bodyTrackerResult.trackingdata[SkeletonNodesNumber].localpose.PosZ);
                    cubeJoint[SkeletonNodesNumber].transform.localScale = Vector3.one * .03f;
                    cubeJoint[SkeletonNodesNumber].transform.localRotation = Quaternion.identity;
                    _cubeJoints.Add(cubeJoint[SkeletonNodesNumber]);
                    cubeJoint[SkeletonNodesNumber].AddComponent<LineRenderer>();
                    cubeJoint[SkeletonNodesNumber].GetComponent<Renderer>().material = cubeMaterial;
                    SkeletonNodes[SkeletonNodesNumber] = cubeJoint[SkeletonNodesNumber].transform;
                    SkeletonNodes[SkeletonNodesNumber].GetComponent<LineRenderer>().startColor = Color.blue;
                    SkeletonNodes[SkeletonNodesNumber].GetComponent<LineRenderer>().endColor = Color.blue;
                    SkeletonNodes[SkeletonNodesNumber].GetComponent<LineRenderer>().startWidth = 0.01f;
                    SkeletonNodes[SkeletonNodesNumber].GetComponent<LineRenderer>().endWidth = 0.01f;
                    SkeletonNodes[SkeletonNodesNumber].GetComponent<LineRenderer>().material = lineMaterial;
                    SkeletonNodesNumber++;
                }
                for (int i = 0; i < 3; i++)
                {
                    cubeJoint.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
                    cubeJoint[24 + i].transform.SetParent(headAndController[i].transform);
                    cubeJoint[24 + i].transform.localPosition = Vector3.zero;
                    cubeJoint[24 + i].transform.localScale = Vector3.one * .03f;
                    cubeJoint[24 + i].transform.localRotation = Quaternion.identity;
                    _cubeJoints.Add(cubeJoint[24 + i]);
                    cubeJoint[24 + i].GetComponent<Renderer>().material = headMaterial;
                }
            }

            foreach (var item in _cubeJoints)
            {
                item.SetActive(value);
            }


        }
        public static Vector3 GetPosition(BodyTrackerTransform bodyTrackerTransform)
        {
            return new((float)bodyTrackerTransform.localpose.PosX, (float)bodyTrackerTransform.localpose.PosY, (float)bodyTrackerTransform.localpose.PosZ);
        }
        public static Vector3 GetPosition(CSVBodyTrackerTransform bodyTrackerTransform)
        {
            return new((float)bodyTrackerTransform.localpose.PosX, (float)bodyTrackerTransform.localpose.PosY, (float)bodyTrackerTransform.localpose.PosZ);
        }
        public void UpdateBonesLength(float height)
        {
            
        }

        public BodyTrackerJoint GetJoint(BodyTrackerRole bodyTrackerRole)
        {
            return !_isLoaded ? null : _joints[(int)bodyTrackerRole];
        }

        public void SetActive(bool value)
        {
            _gameObject.SetActive(value);
        }
        private void ResetTransform(bool isReadFinish)
        {
            //Debug.Log("LakerFinish");
            //if (isReadFinish)
            //{
            //    transform.localPosition = new Vector3(0, 0, 0);
            //    gameCube.transform.localPosition = new Vector3(0, 0, 0);
            //    //BonesList[0].localPosition = avatarHipsPostion;
            //    //for (int i = 0; i < BonesList.Count; i++)
            //    //{

            //    //    BonesList[i].rotation = _joints[i].StartRotation;
            //    //}
            //    currentNumber = 0;
            //}
        }
        public void Destroy()
        {
            foreach (GameObject var in cubeJoint)
            {
                Destroy(var);
            }
            Destroy(_gameObject);
            _gameObject = null;
        }

        public void FindGameObject(List<GameObject> findGameObjects)
        {
            headAndController[0] = findGameObjects[0];
            headAndController[1] = findGameObjects[1];
            headAndController[2] = findGameObjects[2];
            uiRecorder = findGameObjects[3].GetComponent<UIRecorder>();
            gameCube = findGameObjects[5];
        }
    }
}
