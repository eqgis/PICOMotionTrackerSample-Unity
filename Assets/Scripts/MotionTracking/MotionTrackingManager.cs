/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace MotionTracking
{
    public class MotionTrackingManager : MonoBehaviour
    {
        [SerializeField] private GameObject motionTrackerPrefab;
        [SerializeField]
        private GameObject[] motionTrackerSetting;
        [SerializeField]
        private UIShowTrackerName uIShowTrackerName;
        private float _timeStamp;
        private List<string> snName;
        [HideInInspector]
        public readonly Dictionary<string, MotionTrackerSampler> _trackers = new Dictionary<string, MotionTrackerSampler>();
        
        private void Awake()
        {
            PXR_MotionTracking.MotionTrackerNumberOfConnections += OnFitnessBandNumberOfConnections;
            snName = new List<string>();
        }

        private void Start()
        {
#if !UNITY_EDITOR
            UpdateTrackerState();
#else
            UpdateTrackerStateEditor();

#endif
        }

        private void OnFitnessBandNumberOfConnections(int state, int value)
        {
#if !UNITY_EDITOR
            UpdateTrackerState();
#else
            UpdateTrackerStateEditor();

#endif
            Debug.Log($"MotionTrackingManager.OnFitnessBandNumberOfConnections: state = {state}, value = {value}");
        }
        /// <summary>
        /// motionTrackerSetting数组下标和生成的MotionTrackerSampler的motionTrakcerIndex是一一对应的。
        /// 通过motionTrakcerIndex做到两个类之间的绑定
        /// </summary>
        private void UpdateTrackerState()
        {
            int index = _trackers.Count;
            MotionTrackerConnectState motionTrackerConnectState = new MotionTrackerConnectState();
            var trackerState = PXR_MotionTracking.GetMotionTrackerConnectStateWithSN(ref motionTrackerConnectState);
            if (motionTrackerConnectState.trackersSN != null)
            {
                //Create tracker
                foreach (var trackerSn in motionTrackerConnectState.trackersSN)
                {
                    if (!string.IsNullOrEmpty(trackerSn.value) && (!_trackers.TryGetValue(trackerSn.value, out var trackerSampler) || trackerSampler == null))
                    {
                     
                        var trackerGo = Instantiate(motionTrackerPrefab);
                        trackerGo.layer = LayerMask.NameToLayer("UI");
                        trackerSampler = trackerGo.GetComponent<MotionTrackerSampler>();
                        trackerSampler.id =trackerSn;
                        trackerSampler.motionTrakcerIndex = index;
                        Debug.Log("LakerInstance::"+ trackerSn.value);
                        trackerSampler.textSN.GetComponent<TMP_Text>().text= "Motion Tracker "+trackerSn.value.ToString();
                        motionTrackerSetting[index].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler = trackerSampler;
                        motionTrackerSetting[index].GetComponent<UIMotionTrackerSetting>().trackerName.text = "Motion Tracker " + trackerSn.value;
                        snName.Add(trackerSn.value);
                        motionTrackerSetting[index].SetActive(true);
                        _trackers.Add(trackerSn.value, trackerSampler);
                        uIShowTrackerName.motionTrackerSampler.Add(trackerSampler);
                        motionTrackerSetting[index].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.ClearOptions();
                        motionTrackerSetting[index].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("None"));
                        index++;
                        switch (index)
                        {
                            case 1:
                                break;
                            case 2:
                                motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[1]));
                                motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[0]));
                                motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted1 = motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                                motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted1 = motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                                break;
                            case 3:
                                motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[2]));
                                motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted2 = motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                                motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[2]));
                                motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted2 = motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                                motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[0]));
                                motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted1 = motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                                motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[1]));
                                motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted2 = motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                                break;
                        }
                    }
                }
                //Remove disconnected tracker
                if (_trackers.Count > motionTrackerConnectState.trackerSum)
                {
                    List<string> disconnects = new List<string>();
                    int discooectsIndex = 0;
                    int motionTrackerIndex = 0;
                    foreach (var iTracker in _trackers)
                    {
                        bool isContain = false;
                        foreach (var trackerSn in motionTrackerConnectState.trackersSN)
                        {
                            if (trackerSn.value == iTracker.Key)
                            {
                                isContain = true;
                                break;
                            }
                            discooectsIndex++;
                        }
                        discooectsIndex = 0;
                        if (!isContain)
                        {
                            disconnects.Add(iTracker.Key);
                        }
                    }

                    foreach (var item in disconnects)
                    {
                        
                        motionTrackerIndex = _trackers[item].motionTrakcerIndex;
                        motionTrackerSetting[motionTrackerIndex].SetActive(false);
                        motionTrackerSetting[motionTrackerIndex].GetComponent<UIMotionTrackerSetting>().selectGameObjectModel.RemoveBinding();
                        Debug.Log("LakerName" + motionTrackerSetting[motionTrackerIndex].name);
                        foreach (TMP_Dropdown.OptionData option in motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options)
                        {
                            if (option.text == "Motion Tracker "+ item)
                            {
                                motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Remove(option);
                                motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.RefreshShownValue();
                                break;
                            }
                        }
                        foreach (TMP_Dropdown.OptionData option in motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options)
                        {
                            if (option.text == "Motion Tracker " + item)
                            {
                                motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Remove(option);
                                motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.RefreshShownValue();
                                break;
                            }
                        }
                        foreach (TMP_Dropdown.OptionData option in motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options)
                        {
                            if (option.text == "Motion Tracker " + item)
                            {
                                motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Remove(option);
                                motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.RefreshShownValue();
                                break;
                            }
                        }
                        
                        Destroy(_trackers[item].gameObject);
                        _trackers.Remove(item);
                    }
                    }
            }

            Debug.Log($"MotionTrackingManager.UpdateTrackerState: connectedNum = {motionTrackerConnectState.trackerSum}, trackingState = {trackerState}");
        }
        private void UpdateTrackerStateEditor()
        {
            int i =_trackers.Count;
            Debug.Log(_trackers.Count);
                //Create tracker
                for (int index = 0; index <= 1; index++)
                {
                var trackerGo = Instantiate(motionTrackerPrefab);
                trackerGo.layer = LayerMask.NameToLayer("UI");
                var trackerSampler = trackerGo.GetComponent<MotionTrackerSampler>();
                trackerSampler.id.value = index.ToString();
                trackerSampler.motionTrakcerIndex = index;
                trackerSampler.textSN.GetComponent<TMP_Text>().text = "Motion Tracker "+ index.ToString();
                motionTrackerSetting[index].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler = trackerSampler;
                motionTrackerSetting[index].GetComponent<UIMotionTrackerSetting>().trackerName.text = "Motion Tracker " + index;
                snName.Add(index.ToString());
                motionTrackerSetting[index].SetActive(true);
                _trackers.Add(index.ToString(), trackerSampler);
                uIShowTrackerName.motionTrackerSampler.Add(trackerSampler);
                motionTrackerSetting[index].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.ClearOptions();
                motionTrackerSetting[index].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("None"));
                i++;
                switch (i)
                {
                    case 1:
                        break;
                    case 2:
                        motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[1]));
                        motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[0]));
                        motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted1 = motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                        motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted1 = motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                        break;
                    case 3:
                        motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[2]));
                        motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted2 = motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;

                        motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[0]));
                        motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted1 = motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                        motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[2]));
                        motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted2 = motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                        motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[0]));
                        motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted1 = motionTrackerSetting[0].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                        motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().dropdownModelAssisted.options.Add(new TMPro.TMP_Dropdown.OptionData("Motion Tracker " + snName[1]));
                        motionTrackerSetting[2].GetComponent<UIMotionTrackerSetting>().motionTrackerSamplerAssisted2 = motionTrackerSetting[1].GetComponent<UIMotionTrackerSetting>().motionTrackerSampler;
                        break;
                }
            }
        }
    }
}