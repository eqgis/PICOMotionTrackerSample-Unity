/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.IO;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using TMPro;
using BodyTrackingDemo;
using UnityEngine.Events;
using LitJson;
public class UIReadJson : MonoBehaviour
{
    [HideInInspector]
    public bool isStartRead;
    public Button btnReadJsonData;
    public UnityAction<Vector3,Vector3, int, string> readJsonTrackerAction;
    public UnityAction<Vector3, Vector3, string> readJsonTrackerAssistAction;
    [SerializeField]
    private TMP_Dropdown selectDropDwon;
#if UNITY_EDITOR
    private string jsonFilePath = "C:/text/";
#else
    private string jsonFilePath;
#endif
    private string selectJsonName;
    private Vector3 jsonPostion;
    private Vector3 jsonRotation;
    private Vector3 jsonAssistPostion;
    private Vector3 jsonAssistRotation;
    private int jsonModelIndex;
    private string jsonTrackerSN;
    private string jsonTrackerAssistSN;
    JsonData jsonData;
    // Start is called before the first frame update
    void Start()
    {
        selectDropDwon.onValueChanged.AddListener(ReadJsonFile);
        btnReadJsonData.onClick.AddListener(ReadJsonFile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ReadJsonFile(int value)
    {

        selectDropDwon.GetComponent<UISearchConfig>().selectJson = selectDropDwon.options[value].text;
        selectDropDwon.GetComponent<UISearchConfig>().selectIndex = value;
#if UNITY_EDITOR
        jsonFilePath = "C:/MotionTrackerDemo/" + selectDropDwon.options[value].text;
#else
        jsonFilePath = "/sdcard/MotionTrackerDemo/" + selectDropDwon.options[value].text;
#endif
        selectJsonName = selectDropDwon.options[value].text;
    }
    private void ReadJsonFile()
    {
        if (!isStartRead)
        {
            selectJsonName = selectDropDwon.GetComponent<UISearchConfig>().selectJson;
#if UNITY_EDITOR
            jsonFilePath = "C:/MotionTrackerDemo/" + selectJsonName;
#else
                    jsonFilePath = "/sdcard/MotionTrackerDemo/"+ selectJsonName;
#endif
            if (!selectJsonName.Equals("Null"))
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                jsonData = JsonMapper.ToObject(jsonString);
                jsonTrackerSN = jsonData["motionTrackerSN"].ToString();
                jsonModelIndex = (int)jsonData["Model"];
                jsonPostion.x =float.Parse(jsonData["motionTrackerPoX"].ToString());
                jsonPostion.y = float.Parse(jsonData["motionTrackerPoY"].ToString());
                jsonPostion.z = float.Parse(jsonData["motionTrackerPoZ"].ToString());
                jsonRotation.x = float.Parse(jsonData["motionTrackerRowX"].ToString());
                jsonRotation.y = float.Parse(jsonData["motionTrackerRowY"].ToString());
                jsonRotation.z = float.Parse(jsonData["motionTrackerRowZ"].ToString());
                readJsonTrackerAction.Invoke(jsonPostion, jsonRotation, jsonModelIndex, jsonTrackerSN);
                if (jsonData.ContainsKey("motionTrackerAssistedSN"))
                {
                    jsonTrackerAssistSN = jsonData["motionTrackerAssistedSN"].ToString();
                    Debug.Log("LakerSNAD"+jsonTrackerAssistSN);
                    jsonAssistPostion.x = float.Parse(jsonData["motionTrackerAssistedPoX"].ToString());
                    jsonAssistPostion.y = float.Parse(jsonData["motionTrackerAssistedPoY"].ToString());
                    jsonAssistPostion.z = float.Parse(jsonData["motionTrackerAssistedPoZ"].ToString());
                    jsonAssistRotation.x = float.Parse(jsonData["motionTrackerAssistedRowX"].ToString());
                    jsonAssistRotation.y = float.Parse(jsonData["motionTrackerAssistedRowY"].ToString());
                    jsonAssistRotation.z = float.Parse(jsonData["motionTrackerAssistedRowZ"].ToString());
                    readJsonTrackerAssistAction.Invoke(jsonAssistPostion, jsonAssistRotation, jsonTrackerAssistSN);
                }
            }
        }
    }
}
