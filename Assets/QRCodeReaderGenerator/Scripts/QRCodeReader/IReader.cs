using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReader {

    event EventHandler StatusChanged;
    event EventHandler OnReady;

    DeviceCamera.ReadStatus Status { get; }

    IResult Result { get; }
    IDeviceCam Camera { get; }
    DeviceCameraOptions deviceCamOptions { get; }

    void Scan(Action<string, string> Callback);
    void Stop();
    void Update();
    void Destroy();
}
