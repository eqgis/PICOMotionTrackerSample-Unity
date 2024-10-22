using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class TestUpdate : MonoBehaviour
{
    private float leftBattery;
    private float rightBattery;
    private bool X;
    // Start is called before the first frame update
    void Start()
    {
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.batteryLevel, out leftBattery);
        Debug.Log("Left Controller Start Power:" + leftBattery);
    }

    // Update is called once per frame
    void Update()
    {
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.batteryLevel, out leftBattery);
        Debug.Log("Left Controller Current Power:" + leftBattery);
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out X);
        if(X)
        {
            Application.Quit();
        }
    }
    private void LateUpdate()
    {
        Debug.Log("LakerLateUpdate");
    }
}
