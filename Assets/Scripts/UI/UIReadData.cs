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
public struct CSVBodyTrackerResult
{
    /// <summary>
    /// A fixed-length array, each position transmits the data of one body joint.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
    public CSVBodyTrackerTransform[] csvtrackingdata;
}
public struct CSVBodyTrackerTransform
{
    public CSVBodyTrackerTransPose localpose;
}
public struct CSVBodyTrackerTransPose
{
    public double PosX;
    /// <summary>
    /// The joint's position on the Y axis.
    /// </summary>
    public double PosY;
    /// <summary>
    /// The joint's position on the Z axis.
    /// </summary>
    public double PosZ;
    /// <summary>
    /// The joint's rotation on the X component of the Quaternion.
    /// </summary>
    public double RotQx;
    /// <summary>
    /// The joint's rotation on the Y component of the Quaternion.
    /// </summary>
    public double RotQy;
    /// <summary>
    /// The joint's rotation on the Z component of the Quaternion.
    /// </summary>
    public double RotQz;
    /// <summary>
    /// The joint's rotation on the W component of the Quaternion.
    /// </summary>
    public double RotQw;
}
public class UIReadData : MonoBehaviour
{
    public Button btnReadData;
    public Toggle toggleAutoRecording;
    public TextMeshProUGUI textStatus;
    public CameraRecorder cameraRecorder;
    [SerializeField]
    private TMP_Dropdown selectDropDwon;
    [HideInInspector]
    public bool isStartRead;
    private CSVBodyTrackerResult m_BodyTrackerResult;
#if UNITY_EDITOR
    private string csvFilePath = "C:/text/";
#else
    private string csvFilePath;
#endif
    private string[] firstRowData;
    private string[]matchingRowsData;
    private string[] csvLine;
    private int currentNumber;
    private int currentSDKCSVNumber;
    private int bodyindex;
    private DateTime startTime;
    private double matchTime;
    private string[] currentRowData;
    private bool isFinish = true;
    private double csvTime;
    private double csvStartTime;
    public UnityAction<bool> finishReadAction;
    private string selectCSVName;
    private bool isSelectSDKCSV;
    private void Awake()
    {
        currentSDKCSVNumber = 0;
        currentNumber =1;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_BodyTrackerResult.csvtrackingdata = new CSVBodyTrackerTransform[24];
        btnReadData.onClick.AddListener(ReadCSVFile);
        toggleAutoRecording.onValueChanged.AddListener(OnAutoRecordingChanged);
        selectDropDwon.onValueChanged.AddListener(ReadCSVFile);
        cameraRecorder.gameObject.SetActive(false);
        toggleAutoRecording.isOn = PlayerPrefManager.Instance.PlayerPrefData.autoRecording;
        matchingRowsData = new string[168];
        UpdateStatusText();
    }
    private void ReadCSVFile(int value)
    {
      
        selectDropDwon.GetComponent<UISearchCSV>().selectCSV = selectDropDwon.options[value].text;
        selectDropDwon.GetComponent<UISearchCSV>().selectIndex = value;
#if UNITY_EDITOR
        csvFilePath = "C:/text/" + selectDropDwon.options[value].text;
#else
        csvFilePath = "/sdcard/SwiftDemo/" + selectDropDwon.options[value].text;
#endif
        selectCSVName = selectDropDwon.options[value].text;

    }
    private void ReadCSVFile()
    {
        if (!isStartRead)
        {
            if (isFinish)
            {
                selectCSVName = selectDropDwon.GetComponent<UISearchCSV>().selectCSV;

#if UNITY_EDITOR
                csvFilePath = "C:/text/" + selectCSVName;
#else
        csvFilePath = "/sdcard/SwiftDemo/"+selectCSVName;
#endif
                if (!Directory.Exists(csvFilePath))
                {
                    Directory.CreateDirectory(csvFilePath);
                    Debug.Log("Folder created at: " + csvFilePath);
                }
                Debug.Log("LakerFile::" + csvFilePath);
                if (!File.Exists(csvFilePath))
                {
                    return;
                }

                if (selectCSVName.Contains("_sdk"))
                {
                    isSelectSDKCSV = true;
                }
                else
                {
                    isSelectSDKCSV = false;
                }
                // ��ȡCSV�ļ�������������
                csvLine = File.ReadAllLines(csvFilePath);
                // �����һ������
                firstRowData = csvLine[0].Split(',');
                csvStartTime = double.Parse(firstRowData[0]);
                isFinish = false;
            }
            isStartRead = true;
        }
        else 
        {
            isStartRead = false;
            if (isFinish)
            {
                finishReadAction.Invoke(isFinish);
            }
        }
#if !UNITY_EDITOR
        if (toggleAutoRecording.isOn)
        {
            if (cameraRecorder.IsRecording)
            {
                cameraRecorder.gameObject.SetActive(false);
                if (isFinish)
                {
                    cameraRecorder.StopRecording();
                    Debug.Log("LakerStopRecording");
                }
            }
            else
            {
                Debug.Log("LakerRecording");
                cameraRecorder.gameObject.SetActive(true);
                cameraRecorder.StartRecording();
                Debug.Log("LakerStartRecording");
                
            }
        }
        else
        {
            if (cameraRecorder.IsRecording)
            {
                cameraRecorder.gameObject.SetActive(false);
                cameraRecorder.StopRecording();
                if (isFinish)
                {
                    cameraRecorder.StopRecording();
                }
            }
        }
#endif
        UpdateStatusText();
    }
    public void CSVTrackerResult(int frame,ref CSVBodyTrackerResult csvBodyTrackerResult)
    {
        bodyindex = 0;

        if (!isFinish)
        {
            if (isSelectSDKCSV)
            {
               
                MatchTimeData(frame);
                while (bodyindex < 24)
                {

                    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.PosX = double.Parse(matchingRowsData[1 + bodyindex * 7]);
                    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.PosY = double.Parse(matchingRowsData[2 + bodyindex * 7]);
                    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.PosZ = double.Parse(matchingRowsData[3 + bodyindex * 7]);
                    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQw = double.Parse(matchingRowsData[4 + bodyindex * 7]);
                    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQx = double.Parse(matchingRowsData[5 + bodyindex * 7]);
                    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQy = double.Parse(matchingRowsData[6 + bodyindex * 7]);
                    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQz = double.Parse(matchingRowsData[7 + bodyindex * 7]);
                    //    if (bodyindex >= 13 && bodyindex != 15)
                    //    {

                    //        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQx = double.Parse(matchingRowsData[5 + bodyindex * 7]);
                    //        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQy = double.Parse(matchingRowsData[6 + bodyindex * 7]);
                    //        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQz = double.Parse(matchingRowsData[7 + bodyindex * 7]);
                    //    }
                    //    else
                    //    {
                    //        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQx = double.Parse(matchingRowsData[5 + bodyindex * 7]);
                    //        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQy = double.Parse(matchingRowsData[6 + bodyindex * 7]);
                    //        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQz = double.Parse(matchingRowsData[7 + bodyindex * 7]);
                    //    }
                    bodyindex++;
                }
            }
            else
            {
                MatchTimeData(frame);
                while (bodyindex < 24)
                {
                //if (bodyindex == 0)
                //{
                //    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.PosX = double.Parse(matchingRowsData[1]);
                //    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.PosY = double.Parse(matchingRowsData[2]);
                //    csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.PosZ = double.Parse(matchingRowsData[3]);
                //}
                        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.PosX = double.Parse(matchingRowsData[1 + bodyindex * 7]);
                        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.PosY = double.Parse(matchingRowsData[2 + bodyindex * 7]);
                        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.PosZ = -double.Parse(matchingRowsData[3 + bodyindex * 7]);
                        csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQw = double.Parse(matchingRowsData[4 + bodyindex * 7]);
                        if (bodyindex >= 13 && bodyindex != 15)
                        {

                            csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQx = double.Parse(matchingRowsData[5 + bodyindex * 7]);
                            csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQy = -double.Parse(matchingRowsData[6 + bodyindex * 7]);
                            csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQz = -double.Parse(matchingRowsData[7 + bodyindex * 7]);
                        }
                        else
                        {
                            csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQx = -double.Parse(matchingRowsData[5 + bodyindex * 7]);
                            csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQy = double.Parse(matchingRowsData[6 + bodyindex * 7]);
                            csvBodyTrackerResult.csvtrackingdata[bodyindex].localpose.RotQz = double.Parse(matchingRowsData[7 + bodyindex * 7]);
                        }
                    bodyindex++;
                }
            }

        }
    }
    private void MatchTimeData(int frame)
    {
        if (isSelectSDKCSV)
        {
            if (currentSDKCSVNumber < csvLine.Length)
            {
                currentRowData = csvLine[currentSDKCSVNumber].Split(',');
                matchingRowsData = currentRowData;
                currentSDKCSVNumber++;
            }
            else
            {
                isFinish = true;
                UpdateStatusText();
                currentSDKCSVNumber = 0;
            }
        }
        else
        {
            if (frame == 0)
            {
                startTime = DateTime.Now;
                matchingRowsData = firstRowData;
            }
            else
            {
                matchTime = (((DateTime.Now - startTime).TotalMilliseconds) / 10000);
                while (currentNumber < csvLine.Length)
                {
                    currentRowData = csvLine[currentNumber].Split(',');
                    csvTime = (double.Parse(currentRowData[0]) - csvStartTime);
                    if (currentRowData.Length > 0 && csvTime > matchTime)
                    {
                        matchingRowsData = currentRowData;
                        currentNumber++;
                        return;
                    }
                    else
                    {
                        currentNumber++;
                    }
                }
                isFinish = true;
                UpdateStatusText();
                currentNumber = 1;
            }
        }
    }
    private void OnAutoRecordingChanged(bool value)
    {
        PlayerPrefManager.Instance.PlayerPrefData.autoRecording = value;
    }
    private void UpdateStatusText()
    {
        if (isFinish)
        {
            textStatus.text = isStartRead ? "Finish" : "Start";
        }
        else
        {
            textStatus.text = isStartRead ? "Pause" : "Start";
        }
    }
}