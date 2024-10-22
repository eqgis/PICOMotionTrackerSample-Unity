/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MotionTracking;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text;
using TMPro;
using LitJson;
using UnityEngine.Events;
public class UIWriteConfig : MonoBehaviour
{
    // Start is called before the first frame update

    [HideInInspector]
    public string selectModelName;
    [HideInInspector]
    public string selectmotionTrackerAssisted;
    [SerializeField]
    private Button btnRecording;
    //[SerializeField]
    //private TMP_Text textStatus;
    private StreamWriter sw;
    private FileInfo fileInfo;
    private JsonData jsonData;
#if UNITY_EDITOR_WIN
    private string path = "C:/MotionTrackerDemo/";///����ģ��洢����
#else
    private string path = "/sdcard/MotionTrackerDemo/";
#endif
    string jsonPath;
    private void Awake()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        jsonData = new JsonData();
    }
    void Start()
    {
        btnRecording.onClick.AddListener(OnRecording);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnRecording()
    {
        if (jsonData == null)
        {
            jsonData = new JsonData();
        }
        string jsonContent = jsonData.ToJson(); // ��ȡ������JSON�ַ���
        File.WriteAllText(jsonPath, jsonContent); // ��JSON�ַ���д���ļ�
    }
    public void SaveMotionTrackerTransform(Vector3 offsetPos,Vector3 offsetRow , string motionTrackerSN,  int selectModelIndex)
    {
        jsonPath = path + motionTrackerSN + ".json";
        if (jsonData == null)
        {
            jsonData = new JsonData();
        }
        jsonData["motionTrackerSN"] = motionTrackerSN;
        jsonData["Model"] = selectModelIndex;
        jsonData["motionTrackerPoX"] = offsetPos.x.ToString();
        jsonData["motionTrackerPoY"] = offsetPos.y.ToString();
        jsonData["motionTrackerPoZ"] = offsetPos.z.ToString();       
        jsonData["motionTrackerRowX"] = offsetRow.x.ToString();
        jsonData["motionTrackerRowY"] = offsetRow.y.ToString();
        jsonData["motionTrackerRowZ"] = offsetRow.z.ToString(); 
        //Debug.Log("SaveMotion::" + offsetPos + ":" + offsetRow);
    }
    public void SaveMotionTrackerTransformAssisted(MotionTrackerSampler motionTrackerSampler,MotionTrackerSampler AssetMotionTracker, string motionTrackerSN)
    {
        if (jsonData == null)
        {
            jsonData = new JsonData();
        }
        jsonData["motionTrackerAssistedSN"] = motionTrackerSN;
        jsonData["motionTrackerAssistedPoX"] = (motionTrackerSampler.transform.position.x- AssetMotionTracker.transform.position.x).ToString();
        jsonData["motionTrackerAssistedPoY"] = (motionTrackerSampler.transform.position.y - AssetMotionTracker.transform.position.y).ToString();
        jsonData["motionTrackerAssistedPoZ"] = (motionTrackerSampler.transform.position.z - AssetMotionTracker.transform.position.z).ToString();
        jsonData["motionTrackerAssistedRowX"] =(motionTrackerSampler.transform.rotation.eulerAngles.x - AssetMotionTracker.transform.rotation.eulerAngles.x).ToString();
        jsonData["motionTrackerAssistedRowY"] =(motionTrackerSampler.transform.rotation.eulerAngles.y - AssetMotionTracker.transform.rotation.eulerAngles.y).ToString();
        jsonData["motionTrackerAssistedRowZ"] = (motionTrackerSampler.transform.rotation.eulerAngles.z - AssetMotionTracker.transform.rotation.eulerAngles.z).ToString();
    }
}
