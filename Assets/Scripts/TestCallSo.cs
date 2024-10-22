/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Text;
using Unity.Mathematics;
using UnityEngine.XR;
using Unity.XR.PXR;
using Utility;
using BodyTrackingDemo;
using UnityEditor;
public class TestCallSo : MonoBehaviour
{
    enum AvatarDataFrom
    { 
        Swift=0,
        CVdata=1
    }
    enum Model
    { 
        Cube=0,
        avatar=1
    }
    public const string LibFileName = "libBodyTracking";
    [DllImport(LibFileName)]
    private static extern Int32 initializeBodyTrackingProcess();
    [DllImport(LibFileName)]
    private static extern Int32 shutdownBodyTrackingProcess();
    [DllImport("libBodyTracking")]
    private static extern void getLatestData(ref PicoBodyPose picoBodyPose);
    [DllImport("libBodyTracking")]
    private static extern void getLatestData(IntPtr picoBodyPose);
    [DllImport("libBodyTracking")]
    private static extern void getLatestData(double [] body_pose, double [] body_point);
    [DllImport("libBodyTracking")]
    private static extern void giveHeadPose(Vectf value);
    [DllImport("libBodyTracking")]
    private static extern UInt32 just_test();
    private double pConfidence=0;
    private IntPtr pConfidence_Ptr;
    private string pLUResImage=" ";
    private IntPtr pLUResImage_Ptr;
    private string pRUResImage=" ";
    private IntPtr pRUResImage_Ptr;
    private string pLResImage=" ";
    private IntPtr pLResImage_Ptr;
    private string pRResImage;
    private IntPtr pRResImage_Ptr;
    private Vectf pBodyPointGlobal;
    private IntPtr pBodyPointGlobal_Ptr;
    private PicoBodyPose p_output;
    private IntPtr p_unity_output;
    private Vectf headPose;
    private IntPtr headPosePtr;
    private double fBodyDofGlobal=0;
    private IntPtr fBodyDofGlobal_Ptr;
    private double[] body_pose;
    private double[] body_point;
    public List<Transform> BonesList = new List<Transform>(24);
    private Vectf [] p_output2;
    [SerializeField]
    private Material cubeMaterial;
    [SerializeField]
    private Material lineMaterial;
    [SerializeField]
    private GameObject head;
    [SerializeField]
    private Material headMaterial;
    [SerializeField]
    private Vector3 positionHead;
    private Quaternion rotationHead;
    [SerializeField]
    private GameObject gameCube;
    [SerializeField]
    private AvatarDataFrom avatarDataFrom;
    [SerializeField]
    private Model model;
    private BodyTrackerJoint[] _joints;
    private Quaternion avatarmRotation;
    private List<GameObject> cubeJoint;
    private List<GameObject> _cubeJoints;
    private Transform[] SkeletonNodes = new Transform[22];
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
                                  //{20,22 },
                                  { 9,14},
                                  { 14,17},
                                  { 17,19},
                                  { 19,21}/*,{21,23} */};
    struct Vectf
    {
        public double x, y, z, rx, ry, rz,rw;
    }
    struct HeadPose
    {
      public  double x, y, z, rw, rx, ry, rz;
    }
    struct PicoBodyPose
    {
       //public IntPtr pBodyDofGlobal;             // body pose (corresponding to *pHandDofLeft but in global)
      //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
       //public Vectf [] pBodyPointGlobal;           // body keypoints in global world frame (SLAM coordinates)
       //public IntPtr pConfidence;                // confidence score of eacy body keypoint
       public IntPtr pBody6DofGlobal;
    }
    private void Awake()
    {
        _joints = new BodyTrackerJoint[BonesList.Count];
        for (int i = 0; i < BonesList.Count; i++)
        {
            if (BonesList[i] != null)
            {
                var bodyTrackerJoint = BonesList[i].GetComponent<BodyTrackerJoint>();
                if (bodyTrackerJoint == null)
                {
                    bodyTrackerJoint = BonesList[i].gameObject.AddComponent<BodyTrackerJoint>();
                    //bodyTrackerJoint.bodyTrackerRole = (BodyTrackerRole)i;
                }

                bodyTrackerJoint.StartRotation = BonesList[i].rotation;
                _joints[i] = bodyTrackerJoint;
                Debug.Log("LakerStart" + _joints[i].StartRotation);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        headPose = new Vectf();
        //pBodyPointGlobal = new Vectf();
        p_output2 = new Vectf[22];
        p_output = new PicoBodyPose();
        cubeJoint = new List<GameObject>(25);
        p_unity_output = Marshal.AllocHGlobal(Marshal.SizeOf(p_output));
        pBodyPointGlobal_Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(pBodyPointGlobal));
       // fBodyDofGlobal_Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(fBodyDofGlobal));
        pConfidence_Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(pConfidence));
        headPosePtr = Marshal.AllocHGlobal(Marshal.SizeOf(headPose));

        pLUResImage_Ptr = Marshal.StringToHGlobalAnsi(pLUResImage);
        pRUResImage_Ptr = Marshal.StringToHGlobalAnsi(pRUResImage);
        pLResImage_Ptr = Marshal.StringToHGlobalAnsi(pLResImage);
        pRResImage_Ptr = Marshal.StringToHGlobalAnsi(pRResImage);
        //p_output.pBody6DofGlobal = fBodyDofGlobal_Ptr;
        p_output.pBody6DofGlobal = pBodyPointGlobal_Ptr;
        //p_output.pConfidence = pConfidence_Ptr;
        Marshal.StructureToPtr(p_output, p_unity_output, false);
        Marshal.StructureToPtr(pBodyPointGlobal, pBodyPointGlobal_Ptr, false);
        //PXR_Boundary.UseGlobalPose(true);
        Debug.Log("LakerStart");
#if !UNITY_EDITOR
        Debug.Log("initialize:"+ initializeBodyTrackingProcess());
        InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out positionHead);
        InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.deviceRotation, out rotationHead);
        Debug.Log("LakerHead:" + positionHead+":"+ rotationHead);
        headPose.x = positionHead.x;
        headPose.y = positionHead.y;
        headPose.z = positionHead.z;
        headPose.rw = rotationHead.w;
        headPose.rx = rotationHead.x;
        headPose.ry = rotationHead.y;
        headPose.rz = rotationHead.z;
        if (avatarDataFrom == AvatarDataFrom.CVdata)
        {
            giveHeadPose(headPose);
            GetLatesData_Unity();
            if (model == Model.Cube)
            {
                SetCubeJointActive();
            }
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_EDITOR
        if (avatarDataFrom == AvatarDataFrom.CVdata)
        {
            InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out positionHead);
            InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.deviceRotation, out rotationHead);
            Debug.Log("LakerHead:" + positionHead + ":" + rotationHead);
            headPose.x = positionHead.x;
            headPose.y = positionHead.y;
            headPose.z = positionHead.z;
            headPose.rw = rotationHead.w;
            headPose.rx = rotationHead.x;
            headPose.ry = rotationHead.y;
            headPose.rz = rotationHead.z;
            giveHeadPose(headPose);
            GetLatesData_Unity();
            if (model == Model.Cube)
            {
                UpdateCubeNodes();
            }
        }
#else
        UpdateAvatarNodesEditor();
#endif
    }

        public void GetLatesData_Unity()
    {
        //getLatestData(ref p_output);
        getLatestData(p_unity_output);
        p_output = (PicoBodyPose)Marshal.PtrToStructure(p_unity_output, typeof(PicoBodyPose));
        Debug.Log("LakerPTR");
        for (int index = 0; index<22; index++)
        {
            p_output2[index] = Marshal.PtrToStructure<Vectf>(IntPtr.Add(p_output.pBody6DofGlobal, index * Marshal.SizeOf<Vectf>()));
            Debug.Log("LakerFor" +index+","+ p_output2[index].rx + ":" + p_output2[index].ry + ":" + p_output2[index].rz + ":" + p_output2[index].rw);
        }
        if (model == Model.avatar)
        {
            UpdateAvatarNodes();
        }
    }
    private void UpdateAvatarNodesEditor()
    {

       // BonesList[0].position = new Vector3((float)p_output2[0].x, (float)p_output2[0].y, (float)p_output2[0].z);
        for (int i = 0; i <=21; i++)
        {
            //BonesList[i].rotation = new Quaternion((float)p_output.pBodyPointGlobal[i].rx, (float)p_output.pBodyPointGlobal[i].ry, (float)p_output.pBodyPointGlobal[i].rz, (float)p_output.pBodyPointGlobal[i].rw);
            avatarmRotation.x = 0;
            avatarmRotation.y = 0;
            avatarmRotation.z = 0;
            avatarmRotation.w = 1;
            BonesList[i].rotation = avatarmRotation * _joints[i].StartRotation;
            Debug.Log("LakerQuaternion" + i + "," + avatarmRotation + "," + BonesList[i].rotation);
        }
        //BonesList[0].position = new Vector3((float)body_point[0], (float)body_point[1], (float)body_point[2]);
        //Debug.Log("Laker:" + body_point[0] + "," + body_point[1] + "," + body_point[2]);
        //for (int i = 0; i <22 ; i++)
        //{
        //    BonesList[i].eulerAngles = new Vector3((float)body_pose[i*3], (float)body_pose[i*3+1], (float)body_pose[i*3+2]);
        //    Debug.Log("Laker:" + body_pose[i * 3] + "," + body_pose[i * 3 + 1] + "," + body_pose[i * 3 + 2]);
        //}
        //BonesList[0].position = new Vector3((float)p_output.pBodyPointGlobal[0].x, (float)p_output.pBodyPointGlobal[0].y, (float)p_output.pBodyPointGlobal[0].z);
        //for (int i = 0; i <= p_output2.Length; i++)
        //{
        //    //BonesList[i].rotation = new Quaternion((float)p_output.pBodyPointGlobal[i].rx, (float)p_output.pBodyPointGlobal[i].ry, (float)p_output.pBodyPointGlobal[i].rz, (float)p_output.pBodyPointGlobal[i].rw);
        //    avatarmRotation.x = (float)p_output.pBodyPointGlobal[i].rx;
        //    avatarmRotation.y = (float)p_output.pBodyPointGlobal[i].ry;
        //    avatarmRotation.z = (float)p_output.pBodyPointGlobal[i].rz;
        //    avatarmRotation.w = (float)p_output.pBodyPointGlobal[i].rw;
        //    BonesList[i].rotation = avatarmRotation;
        //}
    }
    private void UpdateAvatarNodes()
    {

        BonesList[0].localPosition = new Vector3((float)p_output2[0].x, (float)p_output2[0].y+1.2f, (float)p_output2[0].z);
        Debug.Log("Lakerposition" + p_output2[0].x + "," + p_output2[0].y + "," + p_output2[0].z);
        for (int i = 0; i <= p_output2.Length; i++)
        {
            //BonesList[i].rotation = new Quaternion((float)p_output.pBodyPointGlobal[i].rx, (float)p_output.pBodyPointGlobal[i].ry, (float)p_output.pBodyPointGlobal[i].rz, (float)p_output.pBodyPointGlobal[i].rw);
            avatarmRotation.x = (float)p_output2[i].rx;
            avatarmRotation.y = (float)p_output2[i].ry;
            avatarmRotation.z = (float)p_output2[i].rz;
            avatarmRotation.w = (float)p_output2[i].rw;
            BonesList[i].rotation = avatarmRotation* _joints[i].StartRotation;
            Debug.Log("LakerQuaternion" + i +","+ avatarmRotation+"," + BonesList[i].rotation);
        }
        //BonesList[0].position = new Vector3((float)body_point[0], (float)body_point[1], (float)body_point[2]);
        //Debug.Log("Laker:" + body_point[0] + "," + body_point[1] + "," + body_point[2]);
        //for (int i = 0; i <22 ; i++)
        //{
        //    BonesList[i].eulerAngles = new Vector3((float)body_pose[i*3], (float)body_pose[i*3+1], (float)body_pose[i*3+2]);
        //    Debug.Log("Laker:" + body_pose[i * 3] + "," + body_pose[i * 3 + 1] + "," + body_pose[i * 3 + 2]);
        //}
        //BonesList[0].position = new Vector3((float)p_output.pBodyPointGlobal[0].x, (float)p_output.pBodyPointGlobal[0].y, (float)p_output.pBodyPointGlobal[0].z);
        //for (int i = 0; i <= p_output2.Length; i++)
        //{
        //    //BonesList[i].rotation = new Quaternion((float)p_output.pBodyPointGlobal[i].rx, (float)p_output.pBodyPointGlobal[i].ry, (float)p_output.pBodyPointGlobal[i].rz, (float)p_output.pBodyPointGlobal[i].rw);
        //    avatarmRotation.x = (float)p_output.pBodyPointGlobal[i].rx;
        //    avatarmRotation.y = (float)p_output.pBodyPointGlobal[i].ry;
        //    avatarmRotation.z = (float)p_output.pBodyPointGlobal[i].rz;
        //    avatarmRotation.w = (float)p_output.pBodyPointGlobal[i].rw;
        //    BonesList[i].rotation = avatarmRotation;
        //}
    }
    public void SetCubeJointActive()
    {
        if (_cubeJoints == null)
        {
            _cubeJoints = new List<GameObject>(22);
            for (int SkeletonNodesNumber = 0; SkeletonNodesNumber <= 21; SkeletonNodesNumber++)
            {
                Debug.Log("LakerCreate::" + SkeletonNodesNumber);
                cubeJoint.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
                cubeJoint[SkeletonNodesNumber].transform.SetParent(gameCube.transform);
                cubeJoint[SkeletonNodesNumber].transform.localPosition = new Vector3((float)p_output2[SkeletonNodesNumber].x,
                (float)p_output2[SkeletonNodesNumber].y, (float)p_output2[SkeletonNodesNumber].z);
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

            }
            cubeJoint.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
            cubeJoint[22].transform.SetParent(head.transform);
            cubeJoint[22].transform.localPosition = Vector3.zero;
            cubeJoint[22].transform.localScale = Vector3.one * .03f;
            cubeJoint[22].transform.localRotation = quaternion.identity;
            _cubeJoints.Add(cubeJoint[22]);
            cubeJoint[22].GetComponent<Renderer>().material = headMaterial;
            for (int i = 0; i <= 21; i++)
            {
                SkeletonNodes[i] = _cubeJoints[i].transform;
                SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(0, SkeletonNodes[m_Betweens[i, 0]].position);
                SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(1, SkeletonNodes[m_Betweens[i, 1]].position);
            }
        }
    }
    private void UpdateCubeNodes()
    {
        for (int i = 0; i <= 21; i++)
        {
            cubeJoint[i].transform.localPosition = new Vector3((float)p_output2[i].x,(float)p_output2[i].y, (float)p_output2[i].z);
            SkeletonNodes[i] = _cubeJoints[i].transform;
            SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(0, SkeletonNodes[m_Betweens[i, 0]].position);
            SkeletonNodes[i].GetComponent<LineRenderer>().SetPosition(1, SkeletonNodes[m_Betweens[i, 1]].position);

        }
    }
    [ContextMenu("BindBonesByName")]
    public void BindBonesByName()
    {
        StringBuilder stringBuilder = new StringBuilder("BindBonesByName start:");

        var children = transform.GetComponentsInChildren<Transform>(true);

        string[] names = { "Hips", "UpLeg_L", "UpLeg_R", "Spine1", "Leg_L", "Leg_R", "Spine2", "Foot_L", "Foot_R", "Chest", "Toes_L", "Toes_R", "Neck", "Shoulder_L", "Shoulder_R", "Head", "Arm_L", "Arm_R", "ForeArm_L", "ForeArm_R", "Hand_L", "Hand_R", "MiddleFinger0_L", "MiddleFinger0_R" };
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
    private void OnDestroy()
    {
        Debug.Log("LakerDestory:" + shutdownBodyTrackingProcess());
    }
}
