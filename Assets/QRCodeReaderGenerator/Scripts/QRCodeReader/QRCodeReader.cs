using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class QRCodeReader : IReader {

    private DeviceCamera.ReadStatus status;
    public DeviceCamera.ReadStatus Status {
        get { return status; }
        private set {
            status = value;
            if (StatusChanged != null)
            {
                StatusChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public IResult Result { get; private set; }
    public IDeviceCam Camera { get; private set; }
    public DeviceCameraOptions deviceCamOptions { get; private set; }

    public event EventHandler StatusChanged;
    public event EventHandler OnReady;

    // Store information about last image / results (use the update loop to access camera and callback)
    private Color32[] pixels = null;
    private Action<string, string> Callback;
    private QRResult qrResult;

    private bool parserPixelAvailable = false;
    private float mainThreadLastDecode = 0;
    private int webcamFrameDelayed = 0;
    private int webcamLastChecksum = -1;


    public QRCodeReader() : this(null, null, null) { }
    public QRCodeReader(DeviceCameraOptions settings) : this (settings, null, null) { }
    public QRCodeReader(IResult parser, IDeviceCam webcam) : this(null, parser, webcam) { }

    public QRCodeReader(DeviceCameraOptions settings, IResult result, IDeviceCam webcam) {
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
            //throw new Exception("You need to have permission to use Webcam!");
        }

        status = DeviceCamera.ReadStatus.İnitialize;

        // Default Properties
        deviceCamOptions = (deviceCamOptions == null) ? new DeviceCameraOptions(512,512, FilterMode.Trilinear) : deviceCamOptions;
        Result = (result == null) ? new ZXingParser(deviceCamOptions) : result;
        Camera = (webcam == null) ? new DeviceCamera(deviceCamOptions) : webcam;
    }

    public void Destroy()
    {
        // clean events
        OnReady = null;
        StatusChanged = null;

        // Stop it
        Stop(true);

        // clean returns
        Callback = null;
        Result = null;
        pixels = null;
        parserPixelAvailable = false;

        // clean camera
        Camera.Destroy();
        Camera = null;
        qrResult = null;
    }

    public void Scan(Action<string, string> callback)
    {
        if (Callback != null)
        {
            Debug.LogWarning("Already scan");
            return;
        }

        Callback = callback;

        Debug.Log("Started scanning QR code");
        Status = DeviceCamera.ReadStatus.Running;


#if !UNITY_WEBGL
        if (deviceCamOptions.ScannerBackgroundThread)
        {
            CodeScannerThread = new Thread(ThreadDecodeQR);
            CodeScannerThread.Start();
        }
#endif
    }

    private void Stop(bool forced) {
        if (!forced && Callback == null)
        {
            Debug.Log("No scan running");
            return;
        }

        Debug.Log("Stop scanning");
#if !UNITY_WEBGL
        if (CodeScannerThread != null)
        {
            CodeScannerThread.Abort();
        }
#endif

        Callback = null;
        Status = DeviceCamera.ReadStatus.Paused;

    }

    public void Stop()
    {
        Stop(false);
    }

    #region Background Thread

#if !UNITY_WEBGL
    private Thread CodeScannerThread;

    /// <summary>
    /// Process Image Decoding in a Background Thread
    /// Background Thread : OFF
    /// </summary>
    public void ThreadDecodeQR()
    {
        Debug.Log("Scan QR Code thread..");

        while (qrResult == null)
        {
            // Wait
            if (Status != DeviceCamera.ReadStatus.Running || !parserPixelAvailable || Camera.Width == 0)
            {
                Thread.Sleep(Mathf.FloorToInt(deviceCamOptions.ScannerDecodeInterval * 1000));
                continue;
            }

            // Process
            Debug.Log(this + " SimpleScanner -> Scan ... " + Camera.Width + " / " + Camera.Height);
            try
            {
                qrResult = Result.Decode(pixels, Camera.Width, Camera.Height);
                parserPixelAvailable = false;

                // Sleep a little bit and set the signal to get the next frame
                Thread.Sleep(Mathf.FloorToInt(deviceCamOptions.ScannerDecodeInterval * 1000));
            }
            catch (Exception e)
            {
                Debug.Log(e.GetType() + " " + e.Message);
            }
        }
    }
#endif

    #endregion
    
	// Update is called once per frame
	public void Update () {

        //Wait for camera to get ready
        if (!Camera.IsReady())
        {
            Debug.Log("Camera is not ready yet.");
            if (status != DeviceCamera.ReadStatus.İnitialize)
            {
                status = DeviceCamera.ReadStatus.İnitialize;
            }

            return;
        }

        if (Status == DeviceCamera.ReadStatus.İnitialize)
        {
            if (WebcamInitialized())
            {
                Debug.Log("Camera is READY!");
                Status = DeviceCamera.ReadStatus.Paused;

                if (OnReady != null)
                {
                    OnReady.Invoke(this, EventArgs.Empty);
                }
            }
        }

        if (Status == DeviceCamera.ReadStatus.Running)
        {
            if (qrResult != null)
            {
                Debug.Log(qrResult);
                Callback(qrResult.Type, qrResult.Value);

                //Empty
                qrResult = null;
                parserPixelAvailable = false;
                return;
            }

            pixels = Camera.GetPixels(pixels);
            parserPixelAvailable = true;

            // If background thread OFF, do the decode main thread with 500ms of pause for UI
            if (!deviceCamOptions.ScannerBackgroundThread && mainThreadLastDecode < Time.realtimeSinceStartup - deviceCamOptions.ScannerDecodeInterval)
            {
                DecodeQR();
                mainThreadLastDecode = Time.realtimeSinceStartup;
            }
        }
	}

    public void DecodeQR()
    {
        //wait
        if (Status != DeviceCamera.ReadStatus.Running || !parserPixelAvailable || Camera.Width == 0)
        {
            return;
        }

        Debug.Log("Scan..." + Camera.Width + " / " + Camera.Height);

        try
        {
            qrResult = Result.Decode(pixels, Camera.Width, Camera.Height);
            parserPixelAvailable = false;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }

    private bool WebcamInitialized()
    {
        if (webcamLastChecksum != Camera.GetChecksum())
        {
            webcamLastChecksum = Camera.GetChecksum();
            webcamFrameDelayed = 0;
            return false;
        }

        //Increment delayFrame ScannerDelayFrameMin = 3;

        if (webcamFrameDelayed < 3)
        {
            webcamFrameDelayed++;
            return false;
        }

        Camera.SetSize();
        webcamFrameDelayed = 0;
        return true;
    }
}
