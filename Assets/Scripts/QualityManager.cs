/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

namespace BodyTrackingDemo
{
    public class QualityManager : MonoBehaviour
    {
        [SerializeField] private UniversalRenderPipelineAsset urpAsset;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)] 
        public static void InitializeBeforeSplashScreen()
        {
            XRSettings.eyeTextureResolutionScale = 2f;
            Debug.Log($"QualityManager.InitializeBeforeSplashScreen: eyeTextureResolutionScale = {XRSettings.eyeTextureResolutionScale}, DeviceName = {XRSettings.loadedDeviceName}");
        }
        
        private void Awake()
        {
            float resolutionScale = 1;
            var curDeviceType = PXR_Input.GetControllerDeviceType();
            switch (curDeviceType)
            {
                case PXR_Input.ControllerDevice.Neo3:
                    resolutionScale = 1.21f;
                    break;
                case PXR_Input.ControllerDevice.PICO_4:
                    resolutionScale = 1.436f;
                    break;
                case PXR_Input.ControllerDevice.NewController:
                    resolutionScale = 1.44f;
                    break;
            }

            resolutionScale *= .9f;
            
            if (urpAsset != null)
            {
                urpAsset.renderScale = resolutionScale;    
            }
            XRSettings.eyeTextureResolutionScale = resolutionScale;
            
            Debug.Log($"QualityManager.Awake: eyeTextureResolutionScale = {resolutionScale}");
        }
    }
}