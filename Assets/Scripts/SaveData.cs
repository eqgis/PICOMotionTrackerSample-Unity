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
using System.IO;
using System;
using UnityEngine.XR;
using System.Text;
public class SaveData : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 rightController;
    private Vector3 head;
    private Vector3 leftController;
    private Quaternion headRotation;
    private Quaternion leftControllerRotation;
    private Quaternion rightControllerRotation;
    StreamWriter sw;
    FileInfo fileInfo;
    FileInfo fileInfoLeft;
    FileInfo fileInfoRight;
    DateTime startTime;
#if UNITY_EDITOR_WIN
    private string path = "C:/text/";///����ģ��洢����
#else
    private string path = "/sdcard/Download/";
#endif
    StringBuilder saveInfo;
    int i = 0;
    bool iStartrecord=false;
    string time ;
    void Start()
    {
        startTime = DateTime.Now;
        saveInfo = new StringBuilder();
        fileInfo = new FileInfo(path + "swiftbodypose_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv");
        //Debug.Log("Laker"+DateTime.Now.ToString("yyyyMMddHHmm"));
    }

    // Update is called once per frame
    void Update()
    {
       
#if UNITY_EDITOR///����ģ��洢����
        if (iStartrecord)
        {
            
            saveInfo.Append("Head:" + ((DateTime.Now - startTime).TotalMilliseconds / 1000).ToString("F6")+"," +i.ToString()+""+"\n");
            sw.WriteAsync(saveInfo.ToString());
            saveInfo.Clear();
            i++;
        }
#endif
    }
    public void WriteData(DateTime time,List<Transform> BonesList, bool iStartrecord)
    {
        if (iStartrecord == false)
        {
            sw = fileInfo.CreateText();
            iStartrecord = true;
        }
        else 
        {
            return;
        }
    }
    public void StopWrite()
    {
        if (iStartrecord == true)
        {
            sw.Flush();
            sw.Close();
            iStartrecord = false;
        }
        else 
        {
            return;
        }
    }
    public void TestAPI()
    {
        Debug.Log("LakerSuccess");
    }
}
