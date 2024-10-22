/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using UnityEngine;

namespace BodyTrackingDemo
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource backgroundMusic;
        [SerializeField] private AudioSource vfxTemplate;
        [SerializeField] private AudioBank vfxBank;

        private SimpleObjectPool<AudioSource> _vfxPool;
        
        public AudioSource BackgroundMusic => backgroundMusic;

        public static AudioManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            _vfxPool = new SimpleObjectPool<AudioSource>(vfxTemplate);
        }

        private void Start()
        {
            //if (PlayerPrefManager.Instance.PlayerPrefData.backgroundMusic)
            //{
            //    PlayMusic();
            //}
        }

        public void PlayEffect(AudioSource audioSource, Vector3 targetPos)
        {
            var sfx = Instantiate(audioSource, targetPos, Quaternion.identity);
            sfx.gameObject.SetActive(true);
            sfx.Play();
        }
        
        public void PlayEffect(AudioEffectID effectID)
        {
            foreach (var audioData in vfxBank.audioData)
            {
                if (audioData.id == effectID)
                {
                    var audioSource = _vfxPool.GetObject();
                    audioSource.clip = audioData.audioClip;
                    audioSource.volume = audioData.volume;
                    StartCoroutine(WaitForRecycle(audioSource));
                    break;
                }
            }
        }

        private IEnumerator WaitForRecycle(AudioSource audioSource)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
            _vfxPool.ReturnObject(audioSource);
        }

        public void PlayMusic()
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
        
        public void PauseMusic()
        {
            backgroundMusic.Pause();
        }
    }

    [Serializable]
    public class AudioBank
    {
        public AudioData[] audioData;
    }
    
    [Serializable]
    public class AudioData
    {
        public AudioEffectID id;
        public AudioClip audioClip;
        public float volume;
    }
    
    public enum AudioEffectID : int
    {
        FootStepHeel,
        FootStepToe,
    }
}