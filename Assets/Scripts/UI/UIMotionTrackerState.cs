/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using PicoPorting.Localization;
using TMPro;
using Unity.XR.PXR;
using UnityEngine;

namespace BodyTrackingDemo
{
    public class UIMotionTrackerState : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textType;
        [SerializeField] private TextMeshProUGUI textCount;
        [SerializeField] private TextMeshProUGUI textCalibrateState;
        private string deviceType;
        private static readonly string[] CalibrationStateNames = {"Uncalibrated", "Calibrated", "Calibration Expired"};
        
        private void OnEnable()
        {
            PXR_MotionTracking.MotionTrackerNumberOfConnections += OnFitnessBandNumberOfConnections;
            PXR_MotionTracking.BodyTrackingAbnormalCalibrationData += OnFitnessBandAbnormalCalibrationData;
            PXR_MotionTracking.MotionTrackerBatteryLevel += OnFitnessBandElectricQuantity;

        }

        private void OnDisable()
        {
            PXR_MotionTracking.MotionTrackerNumberOfConnections -= OnFitnessBandNumberOfConnections;
            PXR_MotionTracking.BodyTrackingAbnormalCalibrationData -= OnFitnessBandAbnormalCalibrationData;
            PXR_MotionTracking.MotionTrackerBatteryLevel -= OnFitnessBandElectricQuantity;
        }

        private void OnFitnessBandNumberOfConnections(int state, int value)
        {
            textCount.text = value.ToString();
            Debug.Log($"UIMotionTrackerState.OnFitnessBandNumberOfConnections: state = {state}, value = {value}");
        }
        
        private void OnFitnessBandElectricQuantity(int trackerID, int battery)
        {
            Debug.Log($"UIMotionTrackerState.OnFitnessBandElectricQuantity: trackerID = {trackerID}, battery = {battery}");
        }

        private void OnFitnessBandAbnormalCalibrationData(int state, int value)
        {
            textCalibrateState.text = Loc.Translate(CalibrationStateNames[2]);
            Debug.Log($"UIMotionTrackerState.OnFitnessBandAbnormalCalibrationData: state = {state}, value = {value}");
        }

        public void UpdateTrackerState(MotionTrackerType type, int count, int calibrationState)
        {
            switch (type)
            {
                case MotionTrackerType.MT_1:
                    deviceType = "PICO Motion Tracker DK";
                    break;
                case MotionTrackerType.MT_2:
                    deviceType = "PICO Motion Tracker";
                    break;
            }

            textType.text = deviceType;
            textCount.text = count.ToString();
            textCalibrateState.text = Loc.Translate(CalibrationStateNames[calibrationState]);
            
            // Debug.Log($"UIMotionTrackerState.UpdateTrackerState: state = {state}, type = {type}");
        }
    }
}