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
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.IO;
public class UISearchCSV : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler
{
    // Start is called before the first frame update
    [HideInInspector]
    public string selectCSV;
    [HideInInspector]
    public int selectIndex;
    private TMP_Dropdown selectDropDwon;
#if UNITY_EDITOR
    private string directoryPath = "C:/text/";
#else
   private string directoryPath = "/sdcard/SwiftDemo";
#endif
    private string[] filesCSV;
    private string fileName;
    private bool isCSVDelete;
    private void Awake()
    {
        selectDropDwon = transform.GetComponent<TMP_Dropdown>();
    }
    void Start()
    {
        ReadFileCSV();

    }

    // Update is called once per frame
    void Update()
    {
       // selectDropDwon.RefreshShownValue();
    }
    private void ReadFileCSV()
    {
        selectDropDwon.ClearOptions();
        filesCSV = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
       
        if (filesCSV.Length > 0)
        {
            foreach (string file in filesCSV)
            {
                fileName = Path.GetFileName(file);
                selectDropDwon.options.Add(new TMP_Dropdown.OptionData(fileName));
            }
        }
        else
        {
            selectDropDwon.options.Add(new TMP_Dropdown.OptionData("Null"));
        }
        selectDropDwon.RefreshShownValue();
        selectCSV = selectDropDwon.transform.GetChild(0).GetComponent<TMP_Text>().text;

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        selectDropDwon.RefreshShownValue();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isCSVDelete = false;
        filesCSV = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        if (filesCSV.Length > 0)
        {
            selectDropDwon.ClearOptions();
            foreach (string file in filesCSV)
            {
                fileName = Path.GetFileName(file);
                selectDropDwon.options.Add(new TMP_Dropdown.OptionData(fileName));
                if (selectCSV.Equals(fileName))
                {
                    isCSVDelete = true;
                    selectDropDwon.transform.GetChild(0).GetComponent<TMP_Text>().text = selectCSV;
                    selectDropDwon.value = selectIndex;
                }
            }
            if (!isCSVDelete)
            {
                Debug.Log("LakerSAVE" + selectCSV);
                selectDropDwon.transform.GetChild(0).GetComponent<TMP_Text>().text = selectDropDwon.options[0].text;
                selectCSV = selectDropDwon.options[0].text;
                selectDropDwon.value = 0;
            }
        }
        else
        {
            if (selectDropDwon.options.Count == 0)
            {
                selectDropDwon.options.Add(new TMP_Dropdown.OptionData("Null"));
            }
        }
    }
}
