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
    public class UIDebug : MonoBehaviour
    {
        public Toggle toggleShowJoint;
        public Toggle toggleCameraGizmo;
        public Button btnRiseCamera;
        public Button btnFallCamera;

        private void Awake()
        {
            toggleShowJoint.onValueChanged.AddListener(OnShowJoint);
            toggleCameraGizmo.onValueChanged.AddListener(OnShowCameraGizmo);
            btnRiseCamera.onClick.AddListener(OnRiseCamera);
            btnFallCamera.onClick.AddListener(OnFallCamera);
        }

        private void Start()
        {
            toggleShowJoint.isOn = PlayerPrefManager.Instance.PlayerPrefData.showJoint;
            toggleCameraGizmo.isOn = false;
        }
        
        private void OnFallCamera()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.Log("There is no main camera!");
                return;
            }

            var offsetTransform = mainCamera.transform.parent;
            var pos = offsetTransform.localPosition;
            pos.y -= 0.01f;
            offsetTransform.localPosition = pos;
        }

        private void OnRiseCamera()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.Log("There is no main camera!");
                return;
            }
            
            var offsetTransform = mainCamera.transform.parent;
            var pos = offsetTransform.localPosition;
            pos.y += 0.01f;
            offsetTransform.localPosition = pos;
        }

        private void OnShowJoint(bool value)
        {
            PlayerPrefManager.Instance.PlayerPrefData.showJoint = value;
            BodyTrackingManager.Instance.ShowJoint(value);
        }
        
        private void OnShowCameraGizmo(bool value)
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.Log("There is no main camera!");
                return;
            }

            const string cameraGizmoName = "CameraGizmo";
            if (value)
            {
                var cameraGizmo = mainCamera.transform.Find(cameraGizmoName);
                if (cameraGizmo == null)
                {
                    cameraGizmo = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                    cameraGizmo.SetParent(mainCamera.transform);
                    cameraGizmo.localPosition = Vector3.zero;
                    cameraGizmo.localScale = Vector3.one * .1f;
                    cameraGizmo.transform.name = cameraGizmoName;
                }
                cameraGizmo.gameObject.SetActive(true);
            }
            else
            {
                var cameraGizmo = mainCamera.transform.Find(cameraGizmoName);
                if (cameraGizmo != null)
                {
                    cameraGizmo.gameObject.SetActive(false);
                }
            }
            
        }
    }
}
