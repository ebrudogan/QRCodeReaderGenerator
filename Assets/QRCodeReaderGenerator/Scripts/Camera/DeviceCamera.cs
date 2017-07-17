using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceCamera : IDeviceCam {

    public Texture Texture
    {
        get
        {
            return WebCam;
        }
    }

    public WebCamTexture WebCam { get; private set; }

    public int Width
    {
        get;
        private set;
    }
    
    public int Height
    {
        get;
        private set;
    }

    public enum ReadStatus {
        İnitialize,
        Running,
        Paused
    }

    public void Destroy()
    {
        if (WebCam.isPlaying)
        {
            WebCam.Stop();
        }
    }

    public DeviceCamera() : this (new DeviceCameraOptions(512,512, FilterMode.Trilinear)) {

    }

    public DeviceCamera(DeviceCameraOptions cameraOptions) {

        //Initialize WebCamTexture and its size
        WebCam = new WebCamTexture();
        WebCam.requestedWidth = cameraOptions.WebcamTextureRequestedWidth;
        WebCam.requestedHeight = cameraOptions.WebcamTextureRequestedHeight;

        Width = 0;
        Height = 0;
    }

    public int GetChecksum()
    {
        return (WebCam.width + WebCam.height + WebCam.deviceName + WebCam.videoRotationAngle).GetHashCode();
    }

    public Vector3 GetEulerAngles()
    {
        return new Vector3(0,0, GetRotation());
    }

    public Color32[] GetPixels(Color32[] frameData)
    {
        if (frameData == null || frameData.Length != WebCam.height * WebCam.width)
        {
            return WebCam.GetPixels32();
        }

        return WebCam.GetPixels32(frameData);
    }

    public float GetRotation()
    {
        return -WebCam.videoRotationAngle;
    }

    public Vector3 GetScale()
    {
        return new Vector3(1, IsVerticallyMirrored() ? -1f : 1f, 1);
    }

    public bool IsPlaying()
    {
        return WebCam.isPlaying;
    }

    public bool IsReady()
    {
        if (WebCam != null && WebCam.width >= 100 && WebCam.videoRotationAngle%90 == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsVerticallyMirrored()
    {
        return WebCam.videoVerticallyMirrored;
    }

    public void Play()
    {
        WebCam.Play();
    }

    public void SetSize()
    {
        Width = Mathf.RoundToInt(WebCam.width);
        Height = Mathf.RoundToInt(WebCam.height);
    }

    public void Stop()
    {
        WebCam.Stop();
    }

}
