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
using UnityEngine.XR;
using Unity.XR;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using System;

public class LineRenderDrawing : MonoBehaviour
{
    private LineRenderer line;
    private float lineWidth = 0.005f;
    private bool isDrawing;
    private int vertexCount = 0;
    [SerializeField]
    private InputActionReference leftControllerXAction;
    [SerializeField]
    private InputActionReference leftControllerYAction;
    private List<GameObject> lineList;
    [SerializeField]
    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        lineList = new List<GameObject>();
        leftControllerXAction.action.started += OnXAction;
        leftControllerYAction.action.started += OnYAction;
        //CreateDrawing();
    }
    private void OnDestroy()
    {
        leftControllerXAction.action.started -= OnXAction;
        leftControllerYAction.action.started -= OnYAction;
    }
    private void OnYAction(InputAction.CallbackContext obj)
    {
        Debug.Log("LakerY");
        DestroyDrawing();
    }

    private void OnXAction(InputAction.CallbackContext obj)
    {
        Debug.Log("LakerX");
        if (!isDrawing)
        {
            CreateDrawing();
        }
        else
        {
            isDrawing = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawing)
        {

            AddDrawingPoint(line.transform.position);
        }
    }
    private void CreateDrawing()
    {
        Debug.Log("LakerDrawing");
        GameObject obj = new GameObject();
        obj.AddComponent<LineRenderer>();
        //obj.transform.SetParent(transform);
        line = obj.GetComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = material;
        line.material.color = Color.black;
        line.transform.SetParent(transform);
        line.transform.localEulerAngles = new Vector3(0, 90, 0);
        line.transform.localPosition = new Vector3(0, 0, -0.18f);
        vertexCount = 0;
        isDrawing = true;
        lineList.Add(obj);
    }
    private void AddDrawingPoint(Vector3 position)
    {
        line.positionCount = vertexCount + 1;
        line.SetPosition(vertexCount, position);
        vertexCount++;
    }
    private void DestroyDrawing()
    {
        isDrawing = false;
        foreach (var line in lineList)
        {
            Destroy(line);
        }
    }
}
