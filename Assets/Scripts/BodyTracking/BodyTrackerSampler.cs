/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Unity.Mathematics;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;
using Utility;
enum BodyTrackingDataFrom
{ 
    SDK=0,
    Config=1
}
namespace BodyTrackingDemo
{
    public class BodyTrackerSampler : MonoBehaviour, IBodyTrackerSampler
    {

        #region SerializeField

        public List<Transform> BonesList = new List<Transform>(new Transform[(int) BodyTrackerRole.ROLE_NUM]);
        public Transform leftEye;
        
        [SerializeField] private float avatarHeight = 170;
        
        [SerializeField] private float soleHeight = 0.022f;
        public float[] SkeletonLens = new float[11];
        private Vector3 avatarHipsPostion;
        public SkinnedMeshRenderer[] otherMeshRenderers;
        //[SerializeField]
        #endregion

        #region Property

        public bool IsLoaded { get; private set; }
        public Transform HeadBone => BonesList[15];
        public float EyeOffsetY { get; private set; } = 0.07f;
        public float AvatarHeight => avatarHeight;
        public float SoleHeight => soleHeight;
        #endregion

        #region privateField

        //public Text InfoText;
        private BodyTrackerResult m_BodyTrackerResult;
        private double mDisplayTime;
        private UIRecorder uiRecorder;
        private UIReadData uIReadData;
        [SerializeField]
        private Material cubeMaterial;
        [SerializeField]
        private Material lineMaterial;
        [SerializeField]
        private Material headMaterial;
        private Vector3 m_hipJointPosition;
        private Quaternion m_JointRotation;
        private BodyTrackerJoint[] _joints;
        private CSVBodyTrackerResult csvBodyTrackerResult;
        private List<GameObject> _cubeJoints;
        private GameObject _gameObject;
        private DateTime startTime;
        private int currentNumber;
        private GameObject gameCube;
        private GameObject[] headAndController;
        private List <GameObject> cubeJoint;
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

        private void Awake()
        {
            _gameObject = gameObject;
            m_BodyTrackerResult = new BodyTrackerResult();
            m_BodyTrackerResult.trackingdata = new BodyTrackerTransform[24];
            cubeJoint = new List<GameObject>(27);
            csvBodyTrackerResult = new CSVBodyTrackerResult();
            csvBodyTrackerResult.csvtrackingdata = new CSVBodyTrackerTransform[24];
            headAndController = new GameObject[3];
            _joints = new BodyTrackerJoint[BonesList.Count];
            for (int i = 0; i < BonesList.Count; i++)
            {
                if (BonesList[i] != null)
                {
                   var bodyTrackerJoint = BonesList[i].GetComponent<BodyTrackerJoint>();
                    if (bodyTrackerJoint == null)
                    {
                        bodyTrackerJoint = BonesList[i].gameObject.AddComponent<BodyTrackerJoint>();
                        bodyTrackerJoint.bodyTrackerRole = (BodyTrackerRole) i;
                    }

                    bodyTrackerJoint.StartRotation = BonesList[i].rotation;
                    _joints[i] = bodyTrackerJoint;
                }
            }
            avatarHipsPostion = BonesList[0].position;
            if (leftEye != null)
            {
                EyeOffsetY = leftEye.transform.position.y - HeadBone.transform.position.y;
            }
            startTime = DateTime.Now;
            currentNumber = 0;
            Update();
            IsLoaded = true;
        }
        // Update is called once per frame
        void Update()
        {
            mDisplayTime = PXR_System.GetPredictedDisplayTime();
            var state = PXR_Input.GetBodyTrackingPose(mDisplayTime, ref m_BodyTrackerResult);//获取了全身的数据
            if (state!= 0)
            {
#if UNITY_EDITOR
                if (uIReadData != null && uIReadData.isStartRead)
                {
                    uIReadData.CSVTrackerResult(currentNumber, ref csvBodyTrackerResult);
                    BonesList[0].localPosition = GetPosition(csvBodyTrackerResult.csvtrackingdata[0]);
                    transform.localPosition = new Vector3(-BonesList[0].localPosition.x, 1 - BonesList[0].localPosition.y, -(BonesList[0].localPosition.z + 0.1f));
                    gameCube.transform.localPosition = new Vector3(-BonesList[0].localPosition.x, 0, -(BonesList[0].localPosition.z + 0.1f));
                    //transform.localPosition = new Vector3(BonesList[0].localPosition.x,  BonesList[0].localPosition.y, (BonesList[0].localPosition.z));
                    //gameCube.transform.localPosition = new Vector3(BonesList[0].localPosition.x, 0, (BonesList[0].localPosition.z ));
                    StartBodyTracking(true);
                    currentNumber++;
                }

                if (uiRecorder != null && uiRecorder.isStartRecorderData)
                {
                    uiRecorder.RecorderBodyTrackingData(((DateTime.Now - startTime).TotalMilliseconds / 1000).ToString("F6"), csvBodyTrackerResult);
                }

#endif
                return;
            }
            if (uiRecorder != null && uiRecorder.isStartRecorderData)
            {
                BonesList[0].localPosition = GetPosition(m_BodyTrackerResult.trackingdata[0]);
                StartBodyTracking(false);
                uiRecorder.RecorderBodyTrackingData(((DateTime.Now - startTime).TotalMilliseconds / 1000).ToString("F6"), m_BodyTrackerResult);
            }
            else if (uIReadData != null && uIReadData.isStartRead)
            {
                uIReadData.CSVTrackerResult(currentNumber, ref csvBodyTrackerResult);
                BonesList[0].localPosition = GetPosition(csvBodyTrackerResult.csvtrackingdata[0]);
                transform.localPosition = new Vector3(-BonesList[0].localPosition.x, 0, (BonesList[0].localPosition.z));
                //gameCube.transform.localPosition = new Vector3(-BonesList[0].localPosition.x,1- BonesList[0].localPosition.y, -(BonesList[0].localPosition.z + 0.1f));
                StartBodyTracking(true);
                currentNumber++;
            }
            else
            {
                BonesList[0].localPosition = GetPosition(m_BodyTrackerResult.trackingdata[0]);
                Debug.Log("UpdateLocalPostion::" + BonesList[0].localPosition);
                StartBodyTracking(false);
            }
        }
        private void StartBodyTracking(bool isReadCSV)
        {
                for (int i = 0; i < BonesList.Count; i++)
                {
                    if (BonesList[i] != null)
                    {
                    if (!isReadCSV)
                    {
                        m_JointRotation.x = (float)m_BodyTrackerResult.trackingdata[i].localpose.RotQx;
                        m_JointRotation.y = (float)m_BodyTrackerResult.trackingdata[i].localpose.RotQy;
                        m_JointRotation.z = (float)m_BodyTrackerResult.trackingdata[i].localpose.RotQz;
                        m_JointRotation.w = (float)m_BodyTrackerResult.trackingdata[i].localpose.RotQw;
                        if (_cubeJoints != null)
                        {
                            cubeJoint[i].transform.localPosition = new Vector3((float)m_BodyTrackerResult.trackingdata[i].localpose.PosX,
                        (float)m_BodyTrackerResult.trackingdata[i].localpose.PosY, (float)m_BodyTrackerResult.trackingdata[i].localpose.PosZ);
                            cubeJoint[i].transform.rotation = m_JointRotation;
                        }
                        _joints[i].TrackingData = m_BodyTrackerResult.trackingdata[i];
                        _joints[i].RotationOffset = m_JointRotation;
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
                    BonesList[i].rotation = m_JointRotation * _joints[i].StartRotation;
                    if (_cubeJoints != null && _cubeJoints.Count == 27)
                        {
                        SkeletonNodes[i] = _cubeJoints[i].transform;
                        SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(0, SkeletonNodes[m_Betweens[i, 0]].position);
                        SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(1, SkeletonNodes[m_Betweens[i, 1]].position);
                        }
                    }
                }
        }
        public BodyTrackerJoint GetJoint(BodyTrackerRole bodyTrackerRole)
        {
            return _joints[(int) bodyTrackerRole];
        }
        
        public void UpdateBonesLength(float height)
        {
            var scale = height / avatarHeight;
            BonesList[0].localScale = Vector3.one * scale;

            BodyTrackingBoneLength boneLength = new BodyTrackingBoneLength();
            boneLength.headLen = 100 * SkeletonLens[0] * scale;
            boneLength.neckLen = 100 * SkeletonLens[1] * scale; //6.1f;
            boneLength.torsoLen = 100 * SkeletonLens[2] * scale; //37.1f;
            boneLength.hipLen = 100 * SkeletonLens[3] * scale; //9.1f;
            boneLength.upperLegLen = 100 * SkeletonLens[4] * scale; //34.1f;
            boneLength.lowerLegLen = 100 * SkeletonLens[5] * scale; //40.1f;
            boneLength.footLen = 100 * SkeletonLens[6] * scale; //14.1f;
            boneLength.shoulderLen = 100 * SkeletonLens[7] * scale; //27.1f;
            boneLength.upperArmLen = 100 * SkeletonLens[8] * scale; //20.1f;
            boneLength.lowerArmLen = 100 * SkeletonLens[9] * scale; //22.1f;
            boneLength.handLen = 100 * SkeletonLens[10] * scale;

            int result = PXR_Input.SetBodyTrackingBoneLength(boneLength);

            Update();
            
            Debug.Log($"BodyTrackerSampler.UpdateBonesLength: boneLength = {boneLength}, result = {result}");
        }
        /// <summary>
        /// 创建显示身体关节点
        /// </summary>
        /// <param name="value"></param>
        public void SetCubeJointActive(bool value)
        {
            var skinnedMeshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var item in skinnedMeshRenderers)
            {
                item.gameObject.SetActive(!value);
            }
            int SkeletonNodesNumber=0;
            if (_cubeJoints == null)
            {
                var count = BonesList.Count;
                _cubeJoints = new List<GameObject>(count);
                foreach (var item in BonesList)
                {
              
                    cubeJoint.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
                    cubeJoint[SkeletonNodesNumber].transform.SetParent(gameCube.transform);
                    cubeJoint[SkeletonNodesNumber].transform.localPosition = new Vector3((float)m_BodyTrackerResult.trackingdata[SkeletonNodesNumber].localpose.PosX,
                    (float)m_BodyTrackerResult.trackingdata[SkeletonNodesNumber].localpose.PosY, (float)m_BodyTrackerResult.trackingdata[SkeletonNodesNumber].localpose.PosZ);
                    cubeJoint[SkeletonNodesNumber].transform.localScale = Vector3.one * .03f;
                    cubeJoint[SkeletonNodesNumber].transform.localRotation = quaternion.identity;
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
                    cubeJoint.Add (GameObject.CreatePrimitive(PrimitiveType.Cube));
                    cubeJoint[24 + i].transform.SetParent(headAndController[i].transform);
                    cubeJoint[24 + i].transform.localPosition = Vector3.zero;
                    cubeJoint[24 + i].transform.localScale = Vector3.one * .03f;
                    cubeJoint[24 + i].transform.localRotation = quaternion.identity;
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
        public void SetActive(bool value)
        {
            _gameObject.SetActive(value);
        }

        public void Destroy()
        {
            foreach (GameObject var in cubeJoint)
            {
                Destroy(var);
            }
            Destroy(_gameObject);
        }
       
        #region ContentMenu

        [ContextMenu("BindBonesByName")]
        public void BindBonesByName()
        {
            StringBuilder stringBuilder = new StringBuilder("BindBonesByName start:");
            
            var children = transform.GetComponentsInChildren<Transform>(true);
            
            string[] names = {"Hips", "UpLeg_L", "UpLeg_R", "Spine1", "Leg_L", "Leg_R", "Spine2", "Foot_L", "Foot_R", "Chest", "Toes_L", "Toes_R", "Neck", "Shoulder_L", "Shoulder_R", "Head", "Arm_L", "Arm_R", "ForeArm_L", "ForeArm_R", "Hand_L", "Hand_R", "MiddleFinger0_L", "MiddleFinger0_R"};
            for (int i = 0; i < names.Length; i++)
            {
                for (var j = 1; j < children.Length; j++)
                {
                    var child = children[j];
                    Debug.Log(child.name);
                    if (string.Equals(child.name, names[i], StringComparison.CurrentCultureIgnoreCase))
                    {
                        BonesList[i] = child;
                        stringBuilder.Append($"{names[i]}, ");
                    }
                }
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
            Debug.Log(stringBuilder.ToString());
        }
        private void ResetTransform(bool isReadFinish)
        {
            if (isReadFinish)
            {
                transform.localPosition = new Vector3(0, 0, 0);
                gameCube.transform.localPosition = new Vector3(0, 0, 0);
                BonesList[0].localPosition = avatarHipsPostion;
                for (int i=0; i < BonesList.Count; i++)
                {
                   
                    BonesList[i].rotation = _joints[i].StartRotation;
                }
                currentNumber = 0;
            }
        }
        [ContextMenu("AutoFindAvatarBonesLenth")]
        public void FindBonesLength()
        {
            SkeletonLens[0] = 0.2f; //HeadLen
            SkeletonLens[1] = (BonesList[12].position - BonesList[15].position).magnitude; //NeckLen
            SkeletonLens[2] = (BonesList[12].position - (BonesList[0].position + BonesList[3].position) * 0.5f).magnitude; //TorsoLen
            SkeletonLens[3] = ((BonesList[0].position + BonesList[3].position) * 0.5f - (BonesList[1].position + BonesList[2].position) * 0.5f).magnitude; //HipLen
            SkeletonLens[4] = (BonesList[1].position - BonesList[4].position).magnitude; //UpperLegLen
            SkeletonLens[5] = (BonesList[4].position - BonesList[7].position).magnitude; //LowerLegLen
            SkeletonLens[6] = (BonesList[7].position - BonesList[10].position).magnitude; //FootLen
            SkeletonLens[7] = (BonesList[16].position - BonesList[17].position).magnitude; //ShoulderLen
            SkeletonLens[8] = (BonesList[16].position - BonesList[18].position).magnitude; //UpperArmLen
            SkeletonLens[9] = (BonesList[18].position - BonesList[20].position).magnitude; //LowerArmLen
            SkeletonLens[10] = 0.169f; //HandLen
#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
            Debug.Log($"BodyTrackerSampler.FindBonesLength: NeckLen = {SkeletonLens[1]}, TorsoLen = {SkeletonLens[2]}, HipLen = {SkeletonLens[3]}, UpperLegLen = {SkeletonLens[4]}, LowerLegLen = {SkeletonLens[5]}, FootLen = {SkeletonLens[6]}, ShoulderLen = {SkeletonLens[7]}, UpperArmLen = {SkeletonLens[8]}, LowerArmLen = {SkeletonLens[9]}");
        }

        [ContextMenu("CombineMesh")]
        public void CombineMesh()
        {
            List<Transform> listBone = new List<Transform>();

            List<AvatarUtilities.SkinnedMeshData> meshes = new List<AvatarUtilities.SkinnedMeshData>();
            foreach(SkinnedMeshRenderer smr in otherMeshRenderers)
            {
                meshes.Add(new AvatarUtilities.SkinnedMeshData(smr.sharedMesh,smr.materials));
                listBone.AddRange(smr.bones);
       
            }
            AvatarUtilities.Combine(BonesList[0], listBone.ToArray(), meshes.ToArray());
        }

        public void FindGameObject(List<GameObject> findGameObjects)
        {
            if (findGameObjects.Count > 0)
            {
                headAndController[0] = findGameObjects[0];
                headAndController[1] = findGameObjects[1];
                headAndController[2] = findGameObjects[2];
                uiRecorder = findGameObjects[3].GetComponent<UIRecorder>();
                uIReadData = findGameObjects[4].GetComponent<UIReadData>();
                gameCube = findGameObjects[5];
                uIReadData.finishReadAction = ResetTransform;
            }
        }
        #endregion
    }
}