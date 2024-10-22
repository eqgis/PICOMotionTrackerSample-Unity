/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using TMPro;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.UI;

namespace BodyTrackingDemo
{
    public class UIBodyTrackerSetting : MonoBehaviour
    {
        public Button btnCalibration;
        public TMP_Dropdown dropdownMode;
        public Slider sliderSensitivity;
        public TextMeshProUGUI textSensitivityValue;
        public Slider sliderHeight;
        public TextMeshProUGUI textHeightValue;
        public TMP_Dropdown dropdownAvatar;
        
        
        private void OnEnable()
        {
            Events.UserHeightChanged += OnHeightChanged;
        }

        private void OnDisable()
        {
            Events.UserHeightChanged -= OnHeightChanged;
        }

        private void Start()
        {
            dropdownMode.value = PlayerPrefManager.Instance.PlayerPrefData.bodyTrackMode;
            sliderSensitivity.value = PlayerPrefManager.Instance.PlayerPrefData.steppingSensitivity;
            sliderHeight.value = PlayerPrefManager.Instance.PlayerPrefData.height;

            var avatarNames = (List<string>) AvatarManager.Instance.GetAvatarNames();
            dropdownAvatar.ClearOptions();
            dropdownAvatar.AddOptions(avatarNames);
            for (var i = 0; i < avatarNames.Count; i++)
            {
                var avatarName = avatarNames[i];
                if (avatarName == PlayerPrefManager.Instance.PlayerPrefData.avatarName)
                {
                    dropdownAvatar.value = i;
                    break;
                }
            }
            textSensitivityValue.text = sliderSensitivity.value.ToString("f2");
            textHeightValue.text = sliderHeight.value.ToString("f0");
            
            btnCalibration.onClick.AddListener(OnCalibration);
            dropdownMode.onValueChanged.AddListener(OnModeChanged);
            sliderSensitivity.onValueChanged.AddListener(OnSensitivityChanged);
            sliderHeight.onValueChanged.AddListener(OnHeightChanged);
            dropdownAvatar.onValueChanged.AddListener(OnAvatarChanged);
        }
        
        private void OnHeightChanged(float value)
        {
            PlayerPrefManager.Instance.PlayerPrefData.height = value;
            textHeightValue.text = value.ToString("f0");
            Debug.Log($"UIBodyTrackerSetting.OnHeightChanged: value = {value}");
        }
        
        private void OnSensitivityChanged(float value)
        {
            textSensitivityValue.text = value.ToString("f2");
            PlayerPrefManager.Instance.PlayerPrefData.steppingSensitivity = value;
            Debug.Log($"UIBodyTrackerSetting.OnSensitivityChanged: value = {value}");
        }

        public void OnModeChanged(int modeIdx)
        {
            PlayerPrefManager.Instance.PlayerPrefData.bodyTrackMode = modeIdx;
            //PXR_Input.SetSwiftMode(modeIdx);
            switch (modeIdx)
            {
                case 0:
                    PXR_Input.SetBodyTrackingMode(BodyTrackingMode.BTM_FULL_BODY_LOW);
                    break;
                case 1:
                    PXR_Input.SetBodyTrackingMode(BodyTrackingMode.BTM_FULL_BODY_HIGH);
                    break;

            }
            
            Debug.Log($"UIBodyTrackerSetting.OnModeChanged: modeIdx = {modeIdx}");
        }

        private void OnCalibration()
        {
            BodyTrackingManager.Instance.StartCalibrate();
            Debug.Log($"UIBodyTrackerSetting.OnCalibration");
        }
        
        private void OnAvatarChanged(int value)
        {
            var avatarName = AvatarManager.Instance.GetAvatarNames()[value];
            PlayerPrefManager.Instance.PlayerPrefData.avatarName = avatarName;
            BodyTrackingManager.Instance.ReloadAvatar(avatarName);
        }
    }
}