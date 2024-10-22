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
using UnityEngine.Events;
namespace BodyTrackingDemo
{
    public class UIDisplaySetting : MonoBehaviour
    {
        public Button btnAlignGround;
        public TMP_Dropdown dropdownSteppingEffect;
        public TMP_Dropdown dropdownMirror;
        public TMP_Dropdown dropdownInterationRay;
        public TMP_Dropdown dropdownEnvironment;
        public Player player;
        public UnityAction selectCamerStandModeAction;
        private void Awake()
        {
            btnAlignGround.onClick.AddListener(OnAlignGround);
            dropdownSteppingEffect.onValueChanged.AddListener(OnSteppingEffectChanged);
            dropdownMirror.onValueChanged.AddListener(OnMirrorChanged);
            dropdownInterationRay.onValueChanged.AddListener(OnInterationRayChanged);
            dropdownEnvironment.onValueChanged.AddListener(OnEnvironmentChanged);
            
            dropdownEnvironment.ClearOptions();
            dropdownEnvironment.AddOptions(EnvironmentManager.Instance.GetSceneDisplayNames());
        }

        private void Start()
        {
            dropdownSteppingEffect.value = 0;
            dropdownMirror.value = PlayerPrefManager.Instance.PlayerPrefData.cameraStandMode;
            dropdownInterationRay.value = PlayerPrefManager.Instance.PlayerPrefData.interactionRayMode;
            dropdownEnvironment.value = 1;
            PlayerPrefManager.Instance.PlayerPrefData.steppingEffect = 0;
        }

        private void OnSteppingEffectChanged(int value)
        {
            PlayerPrefManager.Instance.PlayerPrefData.steppingEffect = value;
            Debug.Log($"UIDisplaySetting.OnSteppingEffectChanged: value = {value}");
        }
        
        private void OnMirrorChanged(int value)
        {
            PlayerPrefManager.Instance.PlayerPrefData.cameraStandMode = value;
            CameraManager.Instance.SetCameraStandType((CameraStandMode)value);
            if (selectCamerStandModeAction != null)
            {
                selectCamerStandModeAction.Invoke();
            }
            Debug.Log($"UIDisplaySetting.OnMirrorChanged: value = {value}");
        }
        
        private void OnInterationRayChanged(int value)
        {
            PlayerPrefManager.Instance.PlayerPrefData.interactionRayMode = value;
            player.UpdateInteractionRay(value);
            Debug.Log($"UIDisplaySetting.OnInterationRayChanged: value = {value}");
        }
        
        private void OnEnvironmentChanged(int value)
        {
            PlayerPrefManager.Instance.PlayerPrefData.environmentScene = value;
            StartCoroutine(EnvironmentManager.Instance.ChangeEnvironmentAsync(value));
        }

        private void OnAlignGround()
        {
            BodyTrackingManager.Instance.AlignGround();
        }
    }
}