/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using BodyTrackingDemo;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.UI;
public class DancePadHole : MonoBehaviour
{
    [Flags]
    public enum DirectionType
    {
        Left = 1 << 0,
        Up = 1 << 2,
        Right = 1 << 3,
        Down = 1 << 4,
    }
    
    [SerializeField] private DirectionType direction;
    [SerializeField] private GameObject stepOnEffect;
    [SerializeField] private AudioSource scoreSFX;

    public Action<DancePadHole> onTrigger;
    
    public bool IsActive => _isActive;
    public LittleMoleController LittleMole => m_LittleMole;
    public DirectionType Direction => direction;
    public DancePadsManager DancePadsManager { get; set; }

    // Start is called before the first frame update    
    private bool _isActive;
    private Material m_Material;
    private LittleMoleController m_LittleMole;
    private int _lastScore;
    private int _triggerState;
    private bool _stoppingEffectsState;
    void Start()
    {
        _isActive = false;
        if (m_LittleMole == null)
        {
            m_LittleMole = GetComponentInChildren<LittleMoleController>();
        }
        m_LittleMole.OnStateIdle = SetHoleInactive;
        m_Material = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_triggerState > 0)
        {
            return;
        }

        var curBodyTrackerJoints = other.GetComponent<BodyTrackerJoint>();
        if (curBodyTrackerJoints == null)
        {
            return;
        }
        
        
        //if (curBodyTrackerJoints.TrackingData.Action * 0.001f >= PlayerPrefManager.Instance.PlayerPrefData.steppingSensitivity)
        {
            var actionValue = curBodyTrackerJoints.TrackingData.Action;
            // switch (curBodyTrackerJoints.bodyTrackerRole)
            // {
            //     case BodyTrackerRole.LEFT_FOOT:
            //         actionValue = DancePadsManager.LeftLegAction;
            //         break;
            //     case BodyTrackerRole.RIGHT_FOOT:
            //         actionValue = DancePadsManager.RightLegAction;
            //         break;
            // }

            if (actionValue >= (int)BodyActionList.PxrTouchGround)
            {
                _triggerState = 1;
                _lastScore = 0;

                SetHoleColor(actionValue);
                Debug.Log("LakerEffect::" + LittleMole.Kickable);
                if (LittleMole.Kickable)
                {
                   
                    PlayStepOnEffect();
                    _lastScore = LittleMole.OnLittleMoleKicked();
                }

                onTrigger?.Invoke(this);
                    
                Debug.Log($"DancePadHole.OnTriggerStay: other = {other.name}, LeftLegAction = {DancePadsManager.LeftLegAction}, RightLegAction = {DancePadsManager.RightLegAction}, FootAction = {curBodyTrackerJoints.TrackingData.Action}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _triggerState = 0;
        SetHoleColor(0);
        
        Debug.Log($"DancePadHole.OnTriggerExit: other = {other.name}");
    }

    public void SetHoleActive()
    {
        _isActive = true;
        //Sliding window open, TBD
        m_LittleMole.Show();

    }

    public void SetHoleInactive()
    {
        _isActive = false;
    }


    public void SetHoleColor(uint action)
    {
        Color color = new Color(0.14f,0.84f, 0.413f);
         
        if ((action & (int) BodyActionList.PxrTouchGroundToe) != 0 && (action & (int) BodyActionList.PxrTouchGround) != 0)
        {
            color = (Color.red + Color.blue) * .5f;
        }
        else if ((action & (int) BodyActionList.PxrTouchGroundToe) != 0)
        {
            color = Color.blue;
        }
        else if ((action & (int) BodyActionList.PxrTouchGround) != 0)
        {
            color = Color.red;
        }
        
        m_Material.SetColor("_BaseColor", color);
    }

    private void PlayStepOnEffect()
    {
        var targetPos = transform.position + new Vector3(0, 0.1f, 0);
        
        GameObject obj = Instantiate(stepOnEffect, targetPos, stepOnEffect.transform.rotation);
        obj.SetActive(true);
        obj.GetComponent<ParticleSystem>().Play();
        //Debug.Log("LakerEffect::" + DancePadsManager.stepEffectState);
        //if (DancePadsManager.stepEffectState!=0)
        //{
        //    obj.GetComponent<ParticleSystem>().Play();
        //}
        
        var sfx = Instantiate(scoreSFX, targetPos, Quaternion.identity);
        sfx.gameObject.SetActive(true);
        sfx.Play();
    }
}
