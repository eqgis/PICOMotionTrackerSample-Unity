/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace BodyTrackingDemo
{
    public class UITab : MonoBehaviour
    {
        [SerializeField] private Button[] btnTabs;
        [SerializeField] private GameObject[] pages;
        [SerializeField] private RawImage rawImage;
        public UIDisplaySetting uIDisplaySetting;
        
        private void Awake()
        {
            for (int i = 0; i < btnTabs.Length; i++)
            {
                int idx = i;
                btnTabs[i].onClick.AddListener(()=>
                {
                    OnTab(idx);
                });
            }
            ChangePage(0);
        }
        private void Start()
        {
            uIDisplaySetting.selectCamerStandModeAction += ChangeCameraModel;
        }
        private void Update()
        {
           
        }
        private void OnTab(int idx)
        {
            ChangePage(idx);
        }
        private void ChangeCameraModel()
        {
            Debug.Log( "ChangeCameraModel::" + PlayerPrefManager.Instance.PlayerPrefData.cameraStandMode);
            switch (PlayerPrefManager.Instance.PlayerPrefData.cameraStandMode)
            {
                case 0:

                    rawImage.uvRect = PlayerPrefManager.Instance.PlayerPrefData.DanceGamePlaying ? new Rect(0, 0, 1, 1) : new Rect(0, 0, -1, 1);
                    break;
                case 1:
                    rawImage.uvRect = new Rect(0, 0, -1, 1);
                    break;
                case 2:
                    rawImage.uvRect = new Rect(0, 0, -1, 1);
                    break;
                case 3:
                    rawImage.uvRect = new Rect(0, 0, 1, 1);
                    break;
                case 4:
                    rawImage.uvRect = new Rect(0, 0, 1, 1);
                    break;
                default:
                    break;
            }
        }
        private void ChangePage(int idx)
        {
            /*if (idx == 1)
            {
                rawImage.transform.localPosition = new Vector3(229, -370, 0);
                rawImage.transform.localScale = new Vector3(0.18f, 0.5f, 0.5f);
                PlayerPrefManager.Instance.PlayerPrefData.DanceGamePlaying = true;
                PlayerPrefManager.Instance.PlayerPrefData.webBrow = true;
                switch (PlayerPrefManager.Instance.PlayerPrefData.cameraStandMode)
                {
                    case 0:

                        rawImage.uvRect = PlayerPrefManager.Instance.PlayerPrefData.DanceGamePlaying ? new Rect(0.33f, 0, 0.35f, 1) : new Rect(0.67f, 0, -0.35f, 1);
                        break;
                    case 1:
                        rawImage.uvRect = new Rect(0.67f, 0, -0.35f, 1);
                        break;
                    case 2:
                        rawImage.uvRect = new Rect(0.67f, 0, -0.35f, 1);
                        break;
                    case 3:
                        rawImage.uvRect = new Rect(0.33f, 0, 0.35f, 1);
                        break;
                    case 4:
                        rawImage.uvRect = new Rect(0.33f, 0, 0.35f, 1);
                        break;
                    default:
                        break;
                }
                pages[1].SetActive(true);
                pages[2].SetActive(false);
            }
            else
            {*/
                rawImage.transform.localPosition = new Vector3(637, -370,0);
                rawImage.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                PlayerPrefManager.Instance.PlayerPrefData.DanceGamePlaying = false;
                PlayerPrefManager.Instance.PlayerPrefData.webBrow = false;
                switch (PlayerPrefManager.Instance.PlayerPrefData.cameraStandMode)
                {
                    case 0:

                        rawImage.uvRect = PlayerPrefManager.Instance.PlayerPrefData.DanceGamePlaying ? new Rect(0, 0,1, 1) : new Rect(0, 0, -1, 1);
                        break;
                    case 1:
                        rawImage.uvRect = new Rect(0, 0, -1, 1);
                        break;
                    case 2:
                        rawImage.uvRect = new Rect(0, 0, -1, 1);
                        break;
                    case 3:
                        rawImage.uvRect = new Rect(0, 0, 1, 1);
                        break;
                    case 4:
                        rawImage.uvRect = new Rect(0, 0, 1, 1);
                        break;
                    default:
                        break;
                }
                //pages[1].SetActive(false);
                //pages[2].SetActive(true);
                pages[1].SetActive(true);
            //}
        }
    }
}