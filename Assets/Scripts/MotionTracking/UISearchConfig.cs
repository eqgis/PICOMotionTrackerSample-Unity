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
public class UISearchConfig : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    // Start is called before the first frame update
    [HideInInspector]
    public string selectJson;
    [HideInInspector]
    public int selectIndex;
    private TMP_Dropdown selectDropDwon;
#if UNITY_EDITOR
    private string directoryPath = "C:/MotionTrackerDemo/";
#else
   private string directoryPath = "/sdcard/MotionTrackerDemo";
#endif
    private string[] filesJson;
    private string fileName;
    private bool isJsonDelete;
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
        filesJson = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

        if (filesJson.Length > 0)
        {
            foreach (string file in filesJson)
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
        selectJson = selectDropDwon.transform.GetChild(0).GetComponent<TMP_Text>().text;

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        selectDropDwon.RefreshShownValue();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isJsonDelete = false;
        filesJson = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        if (filesJson.Length > 0)
        {
            selectDropDwon.ClearOptions();
            foreach (string file in filesJson)
            {
                fileName = Path.GetFileName(file);
                selectDropDwon.options.Add(new TMP_Dropdown.OptionData(fileName));
                if (selectJson.Equals(fileName))
                {
                    isJsonDelete = true;
                    selectDropDwon.transform.GetChild(0).GetComponent<TMP_Text>().text = selectJson;
                    selectDropDwon.value = selectIndex;
                }
            }
            if (!isJsonDelete)
            {
                Debug.Log("LakerSAVE" + selectJson);
                selectDropDwon.transform.GetChild(0).GetComponent<TMP_Text>().text = selectDropDwon.options[0].text;
                selectJson = selectDropDwon.options[0].text;
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
