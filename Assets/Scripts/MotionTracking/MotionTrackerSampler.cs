/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

namespace MotionTracking
{
    public class MotionTrackerSampler : MonoBehaviour
    {
        public TrackerSN id;
        [HideInInspector]
        public int motionTrakcerIndex;
        [HideInInspector]
        public bool isReadJson;
        [HideInInspector]
        public bool isBindAss;
        [HideInInspector]
        public UnityAction<bool> trackerConfidenceAction;
        [HideInInspector]
        public UnityAction<bool> trackerConfidenceActionPen;
        public GameObject textSN;
        [HideInInspector]
        public Transform _transform;
        [HideInInspector]
        public Vector3 jsonOffestTrackerPostion;
        [HideInInspector]
        public Vector3 jsonOffestTrackerRotation;
        [HideInInspector]
        public Vector3 startPostion;
        [HideInInspector]
        public Quaternion startRotation;
        [SerializeField]
        private Vector3 testRow;
        [SerializeField]
        private Vector3 testPOS;
        private MotionTrackerLocations locations;
        private MotionTrackerConfidence confidence;
        [SerializeField]
        private GameObject particle;
        [SerializeField]
        private Material material;
        [SerializeField]
        private Material materialAlph;
        private string objSN;
        [SerializeField]
        private GameObject motionTracker;
        private bool isTrackerAction;
        private void Awake()
        {
            _transform = transform;
            PXR_MotionTracking.MotionTrackerKeyAction += MotionTrackerKeyAction;
            
        }
        private void OnDestroy()
        {
            PXR_MotionTracking.MotionTrackerKeyAction -= MotionTrackerKeyAction;
        }
        private void MotionTrackerKeyAction(MotionTrackerEventData obj)
        {
            objSN = obj.trackerSN.value;
            objSN = objSN.Substring(0,objSN.Length - 7);
            if (objSN.Equals(id.value))
            {
                particle.SetActive(true);
                particle.GetComponentInChildren<ParticleSystem>().Play();
            }
            if (isTrackerAction)
            {
                isTrackerAction = false;
            }
            else
            {
                isTrackerAction = true;
            }
            trackerConfidenceActionPen.Invoke(isTrackerAction);
        }

        private void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;
        }

        private void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }
        private void Start()
        {
            motionTracker.GetComponent<MeshRenderer>().materials[0] = material;
#if !UNITY_EDITOR
            PXR_MotionTracking.GetMotionTrackerLocations(id, ref locations, ref confidence);
            startPostion = locations.localLocation.pose.Position.ToVector3();
            startRotation =locations.localLocation.pose.Orientation.ToQuat();
            Debug.Log("LakerStart::"+ id);
#endif
        }
        private void Update()
        { 
            if (string.IsNullOrEmpty(id.value))
            {
                return;
            }
            PXR_MotionTracking.GetMotionTrackerLocations(id, ref locations, ref confidence);
            var predictRotation = locations.localLocation.pose.Orientation.ToQuat();
            var predictPosition = locations.localLocation.pose.Position.ToVector3();
#if !UNITY_EDITOR
            if (!isBindAss)
            {
                if (confidence == MotionTrackerConfidence.PXR_3DOF_NOT_ACCURATE || confidence == MotionTrackerConfidence.PXR_6DOF_NOT_ACCURATE)
                {
                    motionTracker.GetComponent<Renderer>().material = materialAlph;
                    trackerConfidenceAction.Invoke(false);
                }
                else if (confidence == MotionTrackerConfidence.PXR_6DOF_ACCURATE || confidence == MotionTrackerConfidence.PXR_STATIC_ACCURATE)
                {
                    motionTracker.GetComponent<Renderer>().material = material;
                    trackerConfidenceAction.Invoke(true);
                }
            }
            else
            {
                if (confidence == MotionTrackerConfidence.PXR_3DOF_NOT_ACCURATE || confidence == MotionTrackerConfidence.PXR_6DOF_NOT_ACCURATE)
                {
                    motionTracker.GetComponent<Renderer>().material = materialAlph;
                }
                else if (confidence == MotionTrackerConfidence.PXR_6DOF_ACCURATE || confidence == MotionTrackerConfidence.PXR_STATIC_ACCURATE)
                {
                    motionTracker.GetComponent<Renderer>().material = material;
                }
            }
#endif
#if !UNITY_EDITOR
            startPostion = locations.localLocation.pose.Position.ToVector3();
            startRotation = locations.localLocation.pose.Orientation.ToQuat();
#endif
            if (!isReadJson)
                {
#if !UNITY_EDITOR

                    _transform.position = predictPosition;
                    _transform.rotation = Quaternion.Euler(predictRotation.eulerAngles);
                    //offestTrackerRotation = (Quaternion.Inverse(predictRotation) * startRotation).eulerAngles;
                    //offestTrackerPostion= predictPosition- startPostion;

#endif
            }
            else
                {

#if !UNITY_EDITOR
                    //_transform.rotation = Quaternion.Euler(predictRotation.eulerAngles)* Quaternion.Euler(jsonOffestTrackerRotation);
                    _transform.rotation = Quaternion.Euler(predictRotation.eulerAngles+jsonOffestTrackerRotation);
                    _transform.position = predictPosition + jsonOffestTrackerPostion;
#else
                    _transform.rotation = Quaternion.Euler(testRow + jsonOffestTrackerRotation);

                    _transform.position = testPOS+jsonOffestTrackerPostion;
#endif
                }

                //Debug.Log($"MotionTrackerSampler.Update: id = {id}, rotation = {predictRotation}, position = {predictPosition}");

            }
        private void OnBeforeRender()
        {
#if !UNITY_EDITOR
            Update();
#endif
        }
    }
}