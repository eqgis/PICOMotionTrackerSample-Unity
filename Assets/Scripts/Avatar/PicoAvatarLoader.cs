/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using Pico.Avatar;
using Pico.Avatar.Sample;
using Pico.Platform;
using UnityEngine;

namespace BodyTrackingDemo.Avatar
{
    public class PicoAvatarLoader : MonoBehaviour
    {
        [SerializeField] private GameObject picoAvatarApp;
        
        /// <summary>
        /// get platform userID by platformSDK UserService
        /// </summary>
        private string _userServiceUserID = "";
        /// <summary>
        /// get platform accessToken by platformSDK UserService
        /// </summary>
        private string _userServiceAccessToken = "";
        /// <summary>
        /// get platform nativeCode by platformSDK UserService
        /// </summary>
        private string _userServiceStoreRegion = "";

        private bool _hasAvatarPermission;
        private bool m_loginPlatformSDK = false;
        private PicoAvatarApp _picoAvatarApp;
        
        public void LoadAvatar(GameObject avatar)
        {
            CoreService.Initialize();
            ReqPermissions();
            ReqUserInfo();
            ReqAccessToken();
            StartCoroutine(WaitForLoadPicoAvatar(avatar));
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Instantiate(picoAvatarApp);
            }
        }
        private void ReqPermissions()
        {
            if (_hasAvatarPermission)
            {
                return;
            }
            
            Debug.Log($"Platform RequestUserPermissions Start");
            //, "avatar" 
            UserService.RequestUserPermissions("user_info", "avatar").OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Debug.LogError($"Platform RequestUserPermissions failed:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Debug.Log($"Platform Request permission successfully:{(msg.Data)}");
            });
        }
        
        private void ReqUserInfo()
        {
            if (!string.IsNullOrEmpty(_userServiceUserID))
            {
                return;
            }
            Debug.Log($"Platform GetLoggedInUser Start");
            UserService.GetLoggedInUser().OnComplete((user) =>
            {
                if (user == null || user.IsError)
                {
                    Debug.LogError($"Got user isError");
                    return;
                }
               
                Debug.Log($"Got userID {user.Data.ID},name = {user.Data.DisplayName}");
                _userServiceUserID = user.Data.ID;
                _userServiceStoreRegion = user.Data.StoreRegion;
                if (string.IsNullOrEmpty(_userServiceStoreRegion))
                    _userServiceStoreRegion = "";
                //platform_SvrAccessToken();
            });
        }
        
        private void ReqAccessToken()
        {
            if (!string.IsNullOrEmpty(_userServiceAccessToken))
            {
                return;
            }
            
            Debug.Log($"Platform GetAccessToken");
            UserService.GetAccessToken().OnComplete(delegate (Message<string> message)
            {
                if (message.IsError)
                {
                    var err = message.Error;
                    Debug.LogError($"Got access token error {err.Message} code={err.Code}");
                    return;
                }

                string accessToken = message.Data;
                Debug.Log($"Got accessToken {accessToken}");    

                this._userServiceAccessToken = (accessToken);
            });
        }
        private void InitBodyTrackingHead(ActionAvatar obj)
        {
            Debug.Log("SetAvatarHeadLayer");
            _picoAvatarApp.extraSettings.avatarSceneLayer = 15;
        }

        IEnumerator WaitForLoadPicoAvatar(GameObject avatar)
        {
            while (string.IsNullOrEmpty(_userServiceUserID))
                yield return null;
            
            while (string.IsNullOrEmpty(_userServiceAccessToken))
                yield return null;

            if (_picoAvatarApp == null)
            {
                Debug.Log($"InstancePICOAvatarApp");
                var picoAvatarAppGo = Instantiate(picoAvatarApp);
                picoAvatarAppGo.SetActive(true);
                _picoAvatarApp = picoAvatarAppGo.GetComponent<PicoAvatarApp>();
            }

            //waiting PicoAvatarApp finished
            while (!PicoAvatarApp.isWorking)
                yield return null;
            PicoAvatarAppStart();

            //waiting Manager finished
           Debug.Log($"wait Loading Avatar: userID = {_userServiceUserID}");
           while (!PicoAvatarManager.canLoadAvatar)
                yield return null;
            Debug.Log($"wait Loading Avatar: userID = {_userServiceUserID}");
            //start AvatarLoading
            var actAvatar = avatar.GetComponent<ActionAvatar>();//TODO: Write an own one.
            Debug.Log($"Start Avatar: userID = {_userServiceUserID}");
            actAvatar.StartAvatar(_userServiceUserID);
            actAvatar.loadedFinishCall += InitBodyTrackingHead; 
        }
        
        private void PicoAvatarAppStart()
        {
            _picoAvatarApp.loginSettings.accessToken = _userServiceAccessToken;
            Debug.Log("AvatarAPPStart");
            _picoAvatarApp.accessType = AccessType.ThirdApp;
            _picoAvatarApp.StartAvatarManager();
        }
    }
}