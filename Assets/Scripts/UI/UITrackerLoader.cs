/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using PicoPorting.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using TMPro;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BodyTrackingDemo
{
    public class UITrackerLoader : MonoBehaviour
    {
        [SerializeField] private Button btnBodyTracking;
        [SerializeField] private Button btnIndependentTracking;
        [SerializeField] private Button btnMotionTrackerApp;
        [SerializeField] private TextMeshProUGUI textContent;
        private bool preBodyTracking;
        private bool preMotionTracking;
        private static readonly string[] Contents =
        {
            "The current tracker is in Body Tracking mode. If you want to experience Independent Tracking mode, please jump to the Settings page of the Motion tracker APP to set it", 
            "The current tracker is in Independent Tracking mode. If you want to experience Body Tracking mode, please jump to the Settings page of the Motion tracker APP to set it",
        };
        
        private void Awake()
        {
            btnBodyTracking.onClick.AddListener(OnBtnBodyTracking);
            btnIndependentTracking.onClick.AddListener(OnBtnIndependentTracking);
            btnMotionTrackerApp.onClick.AddListener(OnBtnMotionTrackerApp);
        }
        
        public void Start()
        {
            UpdateTrackerMode();
            PXR_Plugin.System.FocusStateAcquired += UpdateTrackerMode;
        }
        
        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
        //        Debug.Log("LakerTest");
               UpdateTrackerMode();
            }
        }

        private void OnBtnMotionTrackerApp()
        {
           PXR_MotionTracking.StartMotionTrackerCalibApp();
        }

        private void OnBtnIndependentTracking()
        {
            if (PXR_MotionTracking.GetMotionTrackerMode() != MotionTrackerMode.MotionTracking)
            {
                preMotionTracking = true;
                Debug.Log("LakerStartSwift");
                PXR_MotionTracking.CheckMotionTrackerModeAndNumber(MotionTrackerMode.MotionTracking,0);

            }
            else
            {
                SceneManager.LoadScene("ObjectTracking");
            }
        }

        private void OnBtnBodyTracking()
        {
            if (PXR_MotionTracking.GetMotionTrackerMode() != MotionTrackerMode.BodyTracking)
            {
                preBodyTracking = true;
                PXR_MotionTracking.CheckMotionTrackerModeAndNumber(MotionTrackerMode.BodyTracking,0);
            }
            else
            {
                SceneManager.LoadSceneAsync("BodyTracking");
                var environmentIdx = PlayerPrefManager.Instance.PlayerPrefData.environmentScene;
                // PXR_MotionTracking.CheckMotionTrackerModeAndNumber(MotionTrackerMode.BodyTracking, MotionTrackerNum.NONE);
                EnvironmentManager.Instance.StartLoadEnvironment(environmentIdx);
            }
        }
        IEnumerator PreLoadBodyTrackingScene()
        {
            yield return new WaitForSeconds(1f);
            preBodyTracking = false;
            SceneManager.LoadSceneAsync("BodyTracking");
            var environmentIdx = PlayerPrefManager.Instance.PlayerPrefData.environmentScene;
            // PXR_MotionTracking.CheckMotionTrackerModeAndNumber(MotionTrackerMode.BodyTracking, MotionTrackerNum.NONE);
            EnvironmentManager.Instance.StartLoadEnvironment(environmentIdx);

        }
        IEnumerator PreLoadMotionTrackingScene()
        {
            yield return new WaitForSeconds(1f);
            preMotionTracking = false;
            SceneManager.LoadScene("ObjectTracking");

        }
        private void UpdateTrackerMode()
        {
            var trackerMode = PXR_MotionTracking.GetMotionTrackerMode();
            if (preBodyTracking&& trackerMode == MotionTrackerMode.BodyTracking)
            {
                StartCoroutine(PreLoadBodyTrackingScene());
            }
            if (preMotionTracking&& trackerMode == MotionTrackerMode.MotionTracking)
            {
                StartCoroutine(PreLoadMotionTrackingScene());
            }
            //switch (trackerMode)
            //{
            //    case MotionTrackerMode.BodyTracking:
            //        btnBodyTracking.interactable = true;
            //        btnIndependentTracking.interactable = false;
            //        break;
            //    case MotionTrackerMode.MotionTracking:
            //        btnBodyTracking.interactable = false;
            //        btnIndependentTracking.interactable = true;
            //        break;
            //}

            textContent.text = Loc.Translate(Contents[(int) trackerMode]);

            //Debug.Log($"UITrackerLoader.UpdateTrackerMode: trackerMode = {trackerMode}");
        }
    }
}