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
using UnityEngine.Events;
using Arcade4VR;
using UnityEngine.XR;
using Unity.XR;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using System;

public class SelectGameObjectModel : MonoBehaviour
{
    public GameObject[] model;
    private int selectModelIndex=0;
    private int preModelIndex=0;
    private Vector3 selectModelPos;
    private Quaternion selectModelRow;
    private Vector3 selectModelPosAss;
    private Quaternion selectModelRowAss;
    [SerializeField]
    private GameObject particleSystem;
    void Start()
    {
        // leftControllerXAction.action.performed += OnPerformedX;
    }


    // Update is called once per frame
    void Update()
    {

    }
    public void SelectModel(int index,Transform transform)
    {
        if (preModelIndex ==0&& index == 0)
        {
            model[preModelIndex].SetActive(false);
        }
        else
        {
            if (index == 0)
            {
                Debug.Log(preModelIndex);
                model[preModelIndex].SetActive(false);
                //startModelPostion[selectModelIndex] = transform.position;
                //startModelRotation[selectModelIndex] = transform.rotation;
                model[selectModelIndex].transform.position = transform.position;
                model[selectModelIndex].transform.rotation = transform.rotation;
                BindModelParent(transform);
            }
            else
            {
                model[preModelIndex].SetActive(false);
                selectModelIndex = index - 1;
                model[selectModelIndex].SetActive(true);
                //startModelPostion[selectModelIndex] = transform.position;
                //startModelRotation[selectModelIndex] = transform.rotation;
                model[selectModelIndex].transform.position = transform.position;
                model[selectModelIndex].transform.rotation = transform.rotation;
                BindModelParent(transform);
                preModelIndex = selectModelIndex;
            }
        }
    }
    public void SelectModelJson(int index, Vector3 motionTrackerPos, Quaternion motionTrackerRow, Vector3 jsonOffestRow)
    {
       // Debug.Log("Laker" + "::" + transform.position);
          model[preModelIndex].SetActive(false);
          selectModelIndex = index - 1;
          model[selectModelIndex].SetActive(true);
          //startModelPostion[selectModelIndex] = motionTrackerPos;
          //startModelRotation[selectModelIndex] = motionTrackerRow;
          model[selectModelIndex].transform.position = motionTrackerPos;
          model[selectModelIndex].transform.rotation = Quaternion.Euler(jsonOffestRow);
          preModelIndex = selectModelIndex;
    }
    public void BindModelParent(Transform motionTracker)
    {
        model[selectModelIndex].transform.SetParent(motionTracker, true);
        selectModelPos = model[selectModelIndex].transform.localPosition;
        selectModelRow = model[selectModelIndex].transform.localRotation;
    }
    public void BindModelParentAss(Transform motionTracker)
    {
        model[selectModelIndex].transform.SetParent(motionTracker, true);
        selectModelPosAss = model[selectModelIndex].transform.localPosition;
        selectModelRowAss = model[selectModelIndex].transform.localRotation;
    }
    public void BindModelParent(Transform motionTracker,Vector3 offfestPos, Vector3 offestRow)
    {
        model[selectModelIndex].transform.SetParent(motionTracker,true);
        Debug.Log("Lakeroffset" + offfestPos);
        model[selectModelIndex].transform.localPosition = selectModelPos;
        model[selectModelIndex].transform.localRotation = selectModelRow;
        //model[selectModelIndex].transform.localEulerAngles = offestRow;
        //model[selectModelIndex].transform.localEulerAngles = Vector3.zero;
    }
    public void BindModelAssParent(Transform motionTracker, Vector3 offfestPos, Vector3 offestRow)
    {

        model[selectModelIndex].transform.SetParent(motionTracker, true);
        Debug.Log("LakeroffsetAss" + offfestPos);
        model[selectModelIndex].transform.localPosition = selectModelPosAss;
        model[selectModelIndex].transform.localRotation = selectModelRowAss;
        //offfestPos = new Vector3(-offfestPos.x, -offfestPos.y, -offfestPos.z);
        //model[selectModelIndex].transform.localPosition = offfestPos;
        //model[selectModelIndex].transform.localEulerAngles = offestRow;
    }
    public void ResetModel(Vector3 postion, Quaternion quaternion)
    {
        //model[selectModelIndex].transform.SetParent(transform);
        model[selectModelIndex].transform.position = postion;
        model[selectModelIndex].transform.rotation = quaternion;
    }
    public void RestModelTransform()
    {
        model[selectModelIndex].transform.SetParent(transform);
        for (int i = 0; i < model.Length; i++)
        {
            model[i].transform.localPosition = Vector3.zero;
            model[i].transform.eulerAngles =Vector3.zero;
        }
        //particleSystem.GetComponent<ParticleSystem>().Stop();
        //particleSystem.SetActive(false);
    }
    public void SetModelPostion(Vector3 vector)
    {
        model[selectModelIndex].transform.localPosition += vector ;
        selectModelPos = model[selectModelIndex].transform.localPosition;


    }
    public void SetModelRotation(Vector3 vector)
    {
        model[selectModelIndex].transform.localRotation= Quaternion.Euler( model[selectModelIndex].transform.localRotation.eulerAngles+vector);
    }
    public void RemoveBinding()
    {
        model[selectModelIndex].transform.SetParent(transform);
    }
    public void Penparticle(bool isPlay)
    {
        particleSystem.SetActive(true);
        if (isPlay)
        {
            particleSystem.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            particleSystem.GetComponent<ParticleSystem>().Stop();
        }
    }
}
