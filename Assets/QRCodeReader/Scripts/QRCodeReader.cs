using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing.QrCode;
using ZXing;

public class QRCodeReader : MonoBehaviour {

    private WebCamTexture _webcamTexture;
    private Rect screenRect;

	// Use this for initialization
	void Start () {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        _webcamTexture = new WebCamTexture();
        _webcamTexture.requestedWidth = Screen.width;
        _webcamTexture.requestedHeight = Screen.height;

        if (_webcamTexture != null)
        {
            _webcamTexture.Play();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI() {
        GUI.DrawTexture(screenRect, _webcamTexture, ScaleMode.ScaleToFit);

        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();

            var result = barcodeReader.Decode(_webcamTexture.GetPixels32(), _webcamTexture.width, _webcamTexture.height);

            if (result != null)
            {
                Debug.Log("DECODED TEXT FROM QR:" + result.Text);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Exception: " + e.Message);
            throw;
        }
    }

}
