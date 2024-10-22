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

namespace BodyTrackingDemo
{
    public class UIStartup : MonoBehaviour
    {
        public GameObject startMenu;
        public Button btnContinue;
        public TMP_Dropdown dropdownMode;
        public Slider sliderHeight;
        public TextMeshProUGUI textHeightValue;
        public TextMeshProUGUI textRayHint;
        
        private void Awake()
        {
            btnContinue.onClick.AddListener(OnContinue);
            dropdownMode.onValueChanged.AddListener(OnModeChanged);
            sliderHeight.onValueChanged.AddListener(OnHeightChanged);

        }

        private void OnEnable()
        {
            dropdownMode.value = PlayerPrefManager.Instance.PlayerPrefData.bodyTrackMode;
            sliderHeight.value = PlayerPrefManager.Instance.PlayerPrefData.height;
            textRayHint.gameObject.SetActive(PlayerPrefManager.Instance.PlayerPrefData.interactionRayMode == 0);
            textHeightValue.text = sliderHeight.value.ToString("f0");
        }

        private void OnHeightChanged(float value)
        {
            PlayerPrefManager.Instance.PlayerPrefData.height = value;
            textHeightValue.text = value.ToString("f0");
        }
        
        private void OnModeChanged(int modeIdx)
        {
            PlayerPrefManager.Instance.PlayerPrefData.bodyTrackMode = modeIdx;
            switch (modeIdx)
            {
                case 0:
                    PXR_Input.SetBodyTrackingMode(BodyTrackingMode.BTM_FULL_BODY_LOW);
                    break;
                case 1:
                    PXR_Input.SetBodyTrackingMode(BodyTrackingMode.BTM_FULL_BODY_HIGH);
                    break;

            }
            //PXR_Input.SetBodyTrackingMode(BodyTrackingMode.BTM_FULL_BODY);
            Debug.Log($"UIStartup.OnModeChanged: modeIdx = {modeIdx}");
        }

        private void OnContinue()
        {
            startMenu.SetActive(false);
            BodyTrackingManager.Instance.StartGame();
        }

        public void OnDemoStart()
        {
            startMenu.SetActive(false);
            BodyTrackingManager.Instance.StartCalibrate();
        }
    }
}