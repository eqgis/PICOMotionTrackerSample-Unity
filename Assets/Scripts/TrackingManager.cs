/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MotionTracking;
using Unity.XR.PXR;
using TMPro;
public class TrackingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject motionTrackerPrefab;
    //[SerializeField]
    //private UIShowTrackerName uIShowTrackerName;
    private float _timeStamp;
    [HideInInspector]
    public readonly Dictionary<string, MotionTrackerSampler> _trackers = new Dictionary<string, MotionTrackerSampler>();
    private void Awake()
    {
        PXR_MotionTracking.MotionTrackerNumberOfConnections += OnFitnessBandNumberOfConnections;
    }
    void Start()
    {
        UpdateTrackerState();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnFitnessBandNumberOfConnections(int state, int value)
    {
        UpdateTrackerState();
        Debug.Log($"MotionTrackingManager.OnFitnessBandNumberOfConnections: state = {state}, value = {value}");
    }
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
                    trackerSampler.id = trackerSn;
                    trackerSampler.motionTrakcerIndex = index;
                    Debug.Log("LakerInstance::" + trackerSn.value);
                    trackerSampler.textSN.GetComponent<TMP_Text>().text = "Motion Tracker " + trackerSn.value.ToString();
                    _trackers.Add(trackerSn.value, trackerSampler);
                    //uIShowTrackerName.motionTrackerSampler.Add(trackerSampler);
                    index++;
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

                        Destroy(_trackers[item].gameObject);
                        _trackers.Remove(item);
                    }
                }
            }
        }
    }
