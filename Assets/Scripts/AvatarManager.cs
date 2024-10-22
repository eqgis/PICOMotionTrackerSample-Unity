/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using BodyTrackingDemo.Avatar;
using UnityEngine;

namespace BodyTrackingDemo
{
    public class AvatarManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] avatarPrefabs;
        [SerializeField] private PicoAvatarLoader picoAvatarLoader;
        
        public const float DefaultAvatarHeight = 170;

        public static AvatarManager Instance { get; private set; }

        private List<string> _avatarNames;
        private PicoAvatarLoader _picoAvatarLoader;

        private void Awake()
        {
            Instance = this;
        }

        public GameObject LoadAvatar(string avatarName, Transform parent)
        {
            Debug.Log(avatarName + ":" + avatarPrefabs[0]);
            var avatarPrefab = avatarPrefabs[0];
            foreach (var avatar in avatarPrefabs)
            {
                if (avatar.name == avatarName)
                {
                    avatarPrefab = avatar;
                    break;
                }
            }
            
            var avatarObj = Instantiate(avatarPrefab, parent);
            avatarObj.transform.localScale = Vector3.one;
            avatarObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (string.Equals(avatarName, "Pico Avatar", StringComparison.OrdinalIgnoreCase))
            {
                if (_picoAvatarLoader == null)
                {
                    _picoAvatarLoader = Instantiate(picoAvatarLoader);
                }
                _picoAvatarLoader.LoadAvatar(avatarObj);
            }
            
            Debug.Log($"AvatarManager.LoadAvatar: Avatar = {avatarObj.name}");
            return avatarObj;
        }

        public IReadOnlyList<string> GetAvatarNames()
        {
            if (_avatarNames == null)
            {
                _avatarNames = new List<string>(avatarPrefabs.Length);
            }
            _avatarNames.Clear();

            foreach (var avatarPrefab in avatarPrefabs)
            {
                _avatarNames.Add(avatarPrefab.name);
            }

            return _avatarNames;
        }
    }
}