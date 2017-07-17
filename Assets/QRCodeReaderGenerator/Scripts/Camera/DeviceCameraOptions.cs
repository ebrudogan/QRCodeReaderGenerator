using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceCameraOptions
{
    //Device camera settings for webcamtexture
    public int WebcamTextureRequestedWidth { get; set; }
    public int WebcamTextureRequestedHeight { get; set; }
    public FilterMode WebcamTextureFilterMode { get; set; }

    // Parser Options
    public bool ParserAutoRotate { get; set; }
    public bool ParserTryInverted { get; set; }
    public bool ParserTryHarder { get; set; }

    // Scanner Options
    public bool ScannerBackgroundThread { get; set; }
    public int ScannerDelayFrameMin { get; set; }
    public float ScannerDecodeInterval { get; set; }

    public DeviceCameraOptions(int width, int height, FilterMode filterMode)
    {
        WebcamTextureRequestedWidth = width;
        WebcamTextureRequestedHeight = height;
        WebcamTextureFilterMode = filterMode;

        ScannerBackgroundThread = true;
        ScannerDelayFrameMin = 3;
        ScannerDecodeInterval = 0.1f;
    }

}
