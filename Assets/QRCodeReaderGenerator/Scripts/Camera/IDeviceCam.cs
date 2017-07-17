using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeviceCam {

    //Data members
    Texture Texture { get; }
    int Width { get; }
    int Height { get; }

    //Functions
    void SetSize();
    void Play();
    void Stop();
    void Destroy();
    bool IsPlaying();
    bool IsReady();
    int GetChecksum();
    Color32[] GetPixels(Color32[] data);
    float GetRotation();
    bool IsVerticallyMirrored();
    Vector3 GetEulerAngles();
    Vector3 GetScale();
}
