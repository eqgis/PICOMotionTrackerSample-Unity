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
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }
        
        [SerializeField]
        private GameObject frontCamera;
        
        [SerializeField]
        private GameObject backCamera;
        
        [SerializeField]
        private RawImage screen;

        private bool _isDanceGamePlaying;
        public CameraStandMode _curCameraStandMode;

            
        private void Awake()
        {
            Instance = this;
            Events.DanceGameStart += OnDanceGameStart;
            Events.DanceGameStop += OnDanceGameStop;
        }

        private void Start()
        {
            SetCameraStandType((CameraStandMode) PlayerPrefManager.Instance.PlayerPrefData.cameraStandMode);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnDanceGameStart()
        {
            _isDanceGamePlaying = true;
            PlayerPrefManager.Instance.PlayerPrefData.DanceGamePlaying = true;
            if (_curCameraStandMode == CameraStandMode.Auto)
            {
                SetCameraStandType(CameraStandMode.Auto);
            }
        }
        
        private void OnDanceGameStop()
        {
            _isDanceGamePlaying = false;
            PlayerPrefManager.Instance.PlayerPrefData.DanceGamePlaying = false;
            if (_curCameraStandMode == CameraStandMode.Auto)
            {
                SetCameraStandType(CameraStandMode.Auto);
            }
        }
        
        public void SetCameraStandType(CameraStandMode standMode)
        {
            _curCameraStandMode = standMode;
            switch (standMode)
            {
                case CameraStandMode.Auto:
                    backCamera.SetActive(_isDanceGamePlaying);
                    backCamera.GetComponent<SnapFollow>().enabled = false;
                    frontCamera.GetComponent<SnapFollow>().enabled = false;
                    screen.uvRect = _isDanceGamePlaying ? new Rect(0, 0, 1, 1) : new Rect(0, 0, -1, 1); 
                    break;
                case CameraStandMode.FixedFront:
                    backCamera.SetActive(false);
                    frontCamera.GetComponent<SnapFollow>().enabled = false;
                    screen.uvRect = new Rect(0, 0, -1, 1); 
                    break;
                case CameraStandMode.FollowingFront:
                    backCamera.SetActive(false);
                    frontCamera.GetComponent<SnapFollow>().enabled = true;
                    screen.uvRect = new Rect(0, 0, -1, 1); 
                    break;
                case CameraStandMode.FixedBack:
                    backCamera.SetActive(true);
                    backCamera.GetComponent<SnapFollow>().enabled = false;
                    screen.uvRect = new Rect(0, 0, 1, 1); 
                    break;
                case CameraStandMode.FollowingBack:
                    backCamera.SetActive(true);
                    backCamera.GetComponent<SnapFollow>().enabled = true;
                    screen.uvRect = new Rect(0, 0, 1, 1); 
                    break;
                default:
                    backCamera.SetActive(false);
                    break;
            }
            frontCamera.SetActive(!backCamera.activeSelf);
        }
    }

    public enum CameraStandMode
    {
        Auto,
        FixedFront,
        FixedBack,
        FollowingFront,
        FollowingBack,
    }
}