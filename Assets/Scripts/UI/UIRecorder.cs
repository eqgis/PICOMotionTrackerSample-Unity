/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.XR.PXR;
using System.Text;
using System.IO;
using System;
using System.Collections;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace BodyTrackingDemo
{
    public class UIRecorder : MonoBehaviour
    {
        public Toggle toggleDataRecording;
        public Button btnRecording;
        public TextMeshProUGUI textStatus;
        public CameraRecorder cameraRecorder;
        private BodyTrackerResult recorderBodyTrackerResult;
        private CSVBodyTrackerResult csvRecorderBodyTrackerResult;
        private string recordTime;
        [HideInInspector]
        public bool isStartRecorderData;
        private bool isToggleDataRecorder;
        private StreamWriter sw;
        private FileInfo fileInfo;
        private int saveLine = 0;
#if UNITY_EDITOR_WIN
        private string path = "C:/SwiftDemo/";///����ģ��洢����
#else
    private string path = "/sdcard/SwiftDemo/";
#endif
        private StringBuilder saveInfo;
        private int cout=0;
        private void Start()
        {
            csvRecorderBodyTrackerResult.csvtrackingdata = new CSVBodyTrackerTransform[24];
            toggleDataRecording.onValueChanged.AddListener(OnDataRecordingChanged);
            btnRecording.onClick.AddListener(OnRecording);
            cameraRecorder.gameObject.SetActive(false);
            saveInfo = new StringBuilder();
            UpdateStatusText();
        }
        private void OnDataRecordingChanged(bool value)
        {
            isToggleDataRecorder = value;
        }
        public void RecorderBodyTrackingData(string time, BodyTrackerResult bodyTrackerResult)
        {
            recordTime = time;
            recorderBodyTrackerResult = bodyTrackerResult;
        }
        public void RecorderBodyTrackingData(string time, CSVBodyTrackerResult bodyTrackerResult)
        {
            recordTime = time;
            csvRecorderBodyTrackerResult = bodyTrackerResult;
        }
        private void OnRecording()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            fileInfo = new FileInfo(path + "swiftbodypose_sdk_" + DateTime.Now.ToString("MMddHHmmss") + ".csv");
            if (isToggleDataRecorder && !isStartRecorderData)
            {
                sw = fileInfo.CreateText();
                Debug.Log("LakerCreat");
                isStartRecorderData = true;
            }
            else if (isStartRecorderData)
            {
                Debug.Log("Lakerclose");
                isStartRecorderData = false;
                sw.Flush();
                sw.Close();

            }

#if !UNITY_EDITOR
            if (cameraRecorder.IsRecording)
            {
                cameraRecorder.gameObject.SetActive(false);
                cameraRecorder.StopRecording();
            }
            else
            {
                cameraRecorder.gameObject.SetActive(true);
                cameraRecorder.StartRecording();
            }
#endif
            UpdateStatusText();
        }
        private void Update()
        {
            if (isStartRecorderData)
            {
                saveInfo.Append(recordTime);
                for (; cout <= 23; cout++)
                {
#if UNITY_EDITOR
                    saveInfo.Append("," + csvRecorderBodyTrackerResult.csvtrackingdata[cout].localpose.PosX.ToString("F6") + "," + csvRecorderBodyTrackerResult.csvtrackingdata[cout].localpose.PosY.ToString("F6") + "," + csvRecorderBodyTrackerResult.csvtrackingdata[cout].localpose.PosZ.ToString("F6") + "," + csvRecorderBodyTrackerResult.csvtrackingdata[cout].localpose.RotQw.ToString("F6") + ","
    + csvRecorderBodyTrackerResult.csvtrackingdata[cout].localpose.RotQx.ToString("F6") + "," + csvRecorderBodyTrackerResult.csvtrackingdata[cout].localpose.RotQy.ToString("F6") + "," + csvRecorderBodyTrackerResult.csvtrackingdata[cout].localpose.RotQz.ToString("F6"));
#else
                saveInfo.Append( "," + recorderBodyTrackerResult.trackingdata[cout].localpose.PosX.ToString("F6") + "," + recorderBodyTrackerResult.trackingdata[cout].localpose.PosY.ToString("F6") + "," 
                 + recorderBodyTrackerResult.trackingdata[cout].localpose.PosZ.ToString("F6") + "," + recorderBodyTrackerResult.trackingdata[cout].localpose.RotQw.ToString("F6") + ","
                 + recorderBodyTrackerResult.trackingdata[cout].localpose.RotQx.ToString("F6") + "," + recorderBodyTrackerResult.trackingdata[cout].localpose.RotQy.ToString("F6") + "," +
                 recorderBodyTrackerResult.trackingdata[cout].localpose.RotQz.ToString("F6"));
#endif
                }
                cout = 0;
                saveInfo.AppendLine();
                sw.WriteAsync(saveInfo.ToString());
                saveInfo.Clear();
                saveLine++;
            }
        }
        private void UpdateStatusText()
        {
            textStatus.text = LocalizationSettings.Instance.GetStringDatabase().GetTable("StringTable").GetEntry(cameraRecorder.IsRecording ? "Stop" : "Start").GetLocalizedString();
            
        }
        private void OnDestroy()
        {
            if (isStartRecorderData)
            {
                isStartRecorderData = false;
                sw.Flush();
                sw.Close();

            }
        }
    }
}