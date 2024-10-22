/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SimpleObjectPool<T> where T : Component
{
    [SerializeField] private T prefab;

    [SerializeField] private Transform parent;

    private Stack<T> _instanceStack = new();
    private HashSet<T> _markedReturnSet = new();

    public SimpleObjectPool(T thePrefab)
    {
        prefab = thePrefab;
        parent = thePrefab.transform.parent;
    }

    public SimpleObjectPool(T prefab, Transform parent)
    {
        this.parent = parent;
        this.prefab = prefab;
    }

    public T GetObject()
    {
        T instance;
        if (_instanceStack.Count > 0)
            instance = _instanceStack.Pop();
        else
            instance = Object.Instantiate(prefab, parent);

        instance.transform.SetParent(parent);
        instance.gameObject.SetActive(true);

        return instance;
    }

    public void ReturnObject(T toReturn)
    {
        toReturn.gameObject.SetActive(false);
        _instanceStack.Push(toReturn);
    }

    public void MarkReturnObject(T toReturn)
    {
        _markedReturnSet.Add(toReturn);
    }

    public void ReturnMarkedObjects()
    {
        foreach (var item in _markedReturnSet) ReturnObject(item);
        _markedReturnSet.Clear();
    }
}