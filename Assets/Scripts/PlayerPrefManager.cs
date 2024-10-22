/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

namespace BodyTrackingDemo
{
    public class PlayerPrefManager : MonoBehaviour
    {
        private static PlayerPrefManager _instance;
        public static PlayerPrefManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("PlayerPrefData");
                    _instance = go.AddComponent<PlayerPrefManager>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        private const string FileName = "PlayerPref";
        
        public PlayerPrefData PlayerPrefData
        {
            get;
            private set;
        }

        #region Field

        private string _userID = "default";

        #endregion

        #region UnityMessage

        private void Awake()
        {
            _instance = this;
            Load("default");
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        #endregion

        #region PublicMethod

        public void Load(string userID)
        {
            _userID = userID;
            var content = PlayerPrefs.GetString($"{FileName}_{userID}");
            if (string.IsNullOrEmpty(content))
            {
                PlayerPrefData = new PlayerPrefData();
            }
            else
            {
                PlayerPrefData = JsonUtility.FromJson<PlayerPrefData>(content);    
            }
            Debug.Log($"PlayerPrefManager.Save: content = {PlayerPrefData}");
        }
        
        public void Save()
        {
            string content = JsonUtility.ToJson(PlayerPrefData);
            PlayerPrefs.SetString($"{FileName}_{_userID}", content);
            PlayerPrefs.Save();
            Debug.Log($"PlayerPrefManager.Save: userID = {_userID}, content = {content}");
        }

        #endregion
    }

    [Serializable]
    public class PlayerPrefData
    {
        public int bodyTrackMode;
        public float steppingSensitivity = .8f;
        public float height = 170;
        public string avatarName = "";
        public bool showJoint;

        public int steppingEffect = 5;
        public int cameraStandMode = 0;
        public int interactionRayMode = 1;
        public bool backgroundMusic;
        public bool DanceGamePlaying = false;
        public bool autoRecording;
        public bool webBrow;
        public int environmentScene=1;

        public override string ToString()
        {
            return $"PlayerPrefData: bodyTrackNode = {bodyTrackMode}, steppingSensitivity = {steppingSensitivity}, height = {height}, steppingEffect = {steppingEffect}, cameraStandMode = {cameraStandMode}, autoRecording = {autoRecording}, avatarName = {avatarName}";
        }
    }
}