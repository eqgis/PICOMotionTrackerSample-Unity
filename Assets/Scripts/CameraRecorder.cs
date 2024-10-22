/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

#if RECORDER
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;
#endif

namespace BodyTrackingDemo
{
    public class CameraRecorder : MonoBehaviour
    {
        [Header(@"Recording")]
        public int videoWidth = 1280;
        public int videoHeight = 720;
        public bool recordMicrophone;
        private Camera targetCamera;
        public Camera backCamera;
        public Camera forntCamera;
        public AudioListener autioListener;
        // private AudioSource _microphoneSource;
#if RECORDER
        private AudioInput _audioInputListener;
        private CameraInput _cameraInput;
        private MP4Recorder _recorder;
        private RealtimeClock _clock;
#endif

        public bool IsRecording { get; private set; }

        private void Start()
        {
            if (CameraManager.Instance._curCameraStandMode == CameraStandMode.FixedBack || CameraManager.Instance._curCameraStandMode == CameraStandMode.FollowingBack)
            {
                targetCamera = backCamera;
            }
            else
            {
                targetCamera = forntCamera;
            }
        }
        // private IEnumerator Start()
        // {
        //     // Start microphone
        //     _microphoneSource = gameObject.AddComponent<AudioSource>();
        //     _microphoneSource.mute =
        //         _microphoneSource.loop = true;
        //     _microphoneSource.bypassEffects =
        //         _microphoneSource.bypassListenerEffects = false;
        //     _microphoneSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
        //     yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
        //     _microphoneSource.Play();
        // }

        private void OnDestroy()
        {
            // Stop microphone
            // _microphoneSource.Stop();
            // Microphone.End(null);
            StopRecording();
        }
        [ContextMenu("StartRecording")]
        public void StartRecording()
        {
            // Start recording
            if (CameraManager.Instance._curCameraStandMode == CameraStandMode.FixedBack || CameraManager.Instance._curCameraStandMode == CameraStandMode.FollowingBack)
            {
                targetCamera = backCamera;
            }
            else
            {
                targetCamera = forntCamera;
            }
            var frameRate = 30;
            var sampleRate = recordMicrophone ? AudioSettings.outputSampleRate : 0;
            var channelCount = recordMicrophone ? (int) AudioSettings.speakerMode : 0;
#if RECORDER
            _clock = new RealtimeClock();
            _recorder = new MP4Recorder(videoWidth, videoHeight, frameRate, sampleRate, channelCount, audioBitRate: 96_000);
            _cameraInput = new CameraInput(_recorder, _clock, targetCamera);

            var audioListener = FindObjectOfType<AudioListener>();
            _audioInputListener = new AudioInput(_recorder, _clock, audioListener);
#endif

            IsRecording = true;
            
            Debug.Log($"CameraRecorder.StartRecording: resolution = {videoWidth}x{videoHeight}, camera = {targetCamera.name}");
        }

        [ContextMenu("StopRecording")]
        public async void StopRecording()
        {
            if (IsRecording)
            {
                IsRecording = false;
                // Mute microphone
                // _microphoneSource.mute = true;
                // Stop recording
#if RECORDER
                _audioInputListener?.Dispose();
                _cameraInput.Dispose();
                var path = await _recorder.FinishWriting();
                // Playback recording
                Debug.Log($"Saved recording to: {path}");
#endif
                // #if !UNITY_STANDALONE
                //                 Handheld.PlayFullScreenMovie($"file://{path}");
                // #endif
                Debug.Log($"CameraRecorder.StopRecording: resolution = {videoWidth}x{videoHeight}, camera = {targetCamera.name}");
            }
        }
    }
}