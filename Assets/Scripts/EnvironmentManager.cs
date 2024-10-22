/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BodyTrackingDemo
{
    // [CreateAssetMenu(fileName = "EnvironmentManager", menuName = "MotionTracking/EnvironmentManager", order = 2)]
    public class EnvironmentManager : MonoBehaviour
    {
        private static EnvironmentManager _instance;

        public static EnvironmentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var prefab = Resources.Load<EnvironmentManager>("EnvironmentManager");
                    _instance = Instantiate(prefab);
                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }
        
        [Serializable]
        public class EnvironmentItem
        {
            public string assetName;
            public string displayName;
            public Material skybox;
        }
        
        [SerializeField] private EnvironmentItem[] environmentItems;

        private int _curEnvironmentId = -1;

        public void StartLoadEnvironment(int id)
        {
            StartCoroutine(LoadEnvironmentAsync(id));
        }
        
        public IEnumerator LoadEnvironmentAsync(int id)
        {
            _curEnvironmentId = id;
            
            yield return SceneManager.LoadSceneAsync(environmentItems[id].assetName, LoadSceneMode.Additive);

            ChangeSkybox(id);
        }

        public IEnumerator ChangeEnvironmentAsync(int id)
        {
            if (_curEnvironmentId == id)
            {
                yield break;
            }
            
            if (_curEnvironmentId != -1)
            {
                yield return SceneManager.UnloadSceneAsync(environmentItems[_curEnvironmentId].assetName);
            }

            yield return LoadEnvironmentAsync(id);
        }

        public void ChangeSkybox(int id)
        {
            RenderSettings.skybox = environmentItems[id].skybox;
            DynamicGI.UpdateEnvironment();
        }
        
        public List<string> GetSceneDisplayNames()
        {
            List<string> displayNames = new List<string>(environmentItems.Length);
            foreach (var item in environmentItems)
            {
                displayNames.Add(item.displayName);
            }
            
            return displayNames;
        }
    }
}