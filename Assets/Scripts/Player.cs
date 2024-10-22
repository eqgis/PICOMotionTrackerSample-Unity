/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace BodyTrackingDemo
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private GameObject leftHandRay;
        [SerializeField] private GameObject rightHandRay;
        [SerializeField] private InputActionReference leftGrip;
        [SerializeField] private InputActionReference rightGrip;

        private XRInteractorLineVisual _leftLineRenderer;
        private XRInteractorLineVisual _rightLineRenderer;
        private int _curRayMode;
        
        private void Awake()
        {
            _leftLineRenderer = leftHandRay.GetComponent<XRInteractorLineVisual>();
            _rightLineRenderer = rightHandRay.GetComponent<XRInteractorLineVisual>();
        }

        private void Start()
        {
            UpdateInteractionRay(PlayerPrefManager.Instance.PlayerPrefData.interactionRayMode);
        }

        private void OnRightGripStarted(InputAction.CallbackContext obj)
        {
            _rightLineRenderer.enabled = true;
        }
        
        private void OnRightGripCanceled(InputAction.CallbackContext obj)
        {
            _rightLineRenderer.enabled = false;
        }

        private void OnLeftGripStarted(InputAction.CallbackContext obj)
        {
            _leftLineRenderer.enabled = true;
        }

        private void OnLeftGripCanceled(InputAction.CallbackContext obj)
        {
            _leftLineRenderer.enabled = false;
        }

        public void UpdateInteractionRay(int rayMode)
        {
            switch (rayMode)
            {
                case 0:
                    leftGrip.action.started += OnLeftGripStarted;
                    leftGrip.action.canceled += OnLeftGripCanceled;
                    rightGrip.action.started += OnRightGripStarted;
                    rightGrip.action.canceled += OnRightGripCanceled;
                    _leftLineRenderer.enabled = false;
                    _rightLineRenderer.enabled = false;    
                    break;
                case 1:
                    _leftLineRenderer.enabled = true;
                    _rightLineRenderer.enabled = true;
                    if (_curRayMode == 0)
                    {
                        leftGrip.action.started -= OnLeftGripStarted;
                        leftGrip.action.canceled -= OnLeftGripCanceled;
                        rightGrip.action.started -= OnRightGripStarted;
                        rightGrip.action.canceled -= OnRightGripCanceled;
                    }
                    break;
            }

            _curRayMode = rayMode;
        }
    }
}