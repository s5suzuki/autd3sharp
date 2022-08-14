﻿/*
 * File: SimpleAUTDController.cs
 * Project: Example
 * Created Date: 08/03/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 14/08/2022
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Shun Suzuki. All rights reserved.
 * 
 */

using AUTD3Sharp;
using UnityEngine;

public class SimpleAUTDController : MonoBehaviour
{
    Controller _autd = new Controller();
    Link? _link = null;
    public GameObject? Target = null;

    void Awake()
    {
        _autd.AddDevice(gameObject.transform.position, gameObject.transform.rotation);

        _link = new SOEM(_autd.NumDevices).Build();
        _autd.Open(_link);

        _autd.CheckTrials = 50;

        _autd.Clear();

        _autd.Synchronize();

        _autd.Send(new Sine(150)); // 150 Hz
    }

    void Update()
    {
        if (Target != null)
            _autd.Send(new Focus(Target.transform.position, 1.0));
    }

    private void OnApplicationQuit()
    {
        _autd.Dispose();
    }
}
