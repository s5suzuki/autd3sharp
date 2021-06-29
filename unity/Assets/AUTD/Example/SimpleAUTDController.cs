/*
 * File: SimpleAUTDController.cs
 * Project: Example
 * Created Date: 08/03/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 29/06/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using AUTD3Sharp;
using UnityEngine;

public class SimpleAUTDController : MonoBehaviour
{
    AUTD _autd = new AUTD();
    Link? _link = null;
    public GameObject? Target = null;

    void Awake()
    {
        _autd = new AUTD();

        _autd.AddDevice(gameObject.transform.position, gameObject.transform.rotation);

        string ifname = @"write your interface name here";
        _link = Link.SOEM(ifname, _autd.NumDevices);
        _autd.Open(_link);

        _autd.Clear();

        _autd.Send(Modulation.Sine(150)); // 150 Hz
    }

    void Update()
    {
        if (Target != null)
            _autd.Send(Gain.FocalPoint(Target.transform.position));
    }

    private void OnApplicationQuit()
    {
        _autd.Dispose();
    }
}
