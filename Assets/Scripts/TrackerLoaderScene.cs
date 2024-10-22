/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

namespace BodyTrackingDemo
{
    public class TrackerLoaderScene : MonoBehaviour
    {
        private void Awake()
        {
            var environmentIdx = PlayerPrefManager.Instance.PlayerPrefData.environmentScene;
            EnvironmentManager.Instance.ChangeSkybox(environmentIdx);
        }

        public void Start()
        {
            // var trackerMode = PXR_MotionTracking.GetMotionTrackerMode();
            // switch (trackerMode)
            // {
            //     case MotionTrackerMode.BodyTracking:
            //         SceneManager.LoadScene("BodyTracking");
            //         break;
            //     case MotionTrackerMode.MotionTracking:
            //         var trackerType = PXR_MotionTracking.GetMotionTrackerType();
            //         SceneManager.LoadScene(trackerType == MotionTrackerType.Swift_1 ? "BodyTracking" : "MotionTracking");
            //         break;
            // }
            //
            // Debug.Log($"TrackerLoaderScene.Start: trackerMode = {trackerMode}");
            
            // EnvironmentManager.Instance.ChangeEnvironment()
        }
    }
}