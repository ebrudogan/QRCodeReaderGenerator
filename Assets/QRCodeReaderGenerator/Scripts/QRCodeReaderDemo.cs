using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QRCodeReaderDemo : MonoBehaviour {

    private IReader QRReader;
    public Text resultText;
    public RawImage image;


	void Awake () {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
	}

    // Use this for initialization
    private void Start()
    {
        QRReader = new QRCodeReader();
        QRReader.Camera.Play();

        QRReader.OnReady += StartReadingQR;

        QRReader.StatusChanged += QRReader_StatusChanged;
    }

    private void QRReader_StatusChanged(object sender, System.EventArgs e)
    {
        resultText.text = "Status: " + QRReader.Status;
    }

    private void StartReadingQR(object sender, System.EventArgs e)
    {
        image.transform.localEulerAngles = QRReader.Camera.GetEulerAngles();
        image.transform.localScale = QRReader.Camera.GetScale();
        image.texture = QRReader.Camera.Texture;

        RectTransform rectTransform = image.GetComponent<RectTransform>();
        float height = rectTransform.sizeDelta.x * (QRReader.Camera.Height / QRReader.Camera.Width);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    // Update is called once per frame
    void Update () {

        if (QRReader == null)
        {
            return;
        }

        QRReader.Update();
	}

    public void StartScanning()
    {
        if (QRReader == null)
        {
            Debug.LogWarning("No valid camera - Click Start");
            return;
        }

        // Start Scanning
        QRReader.Scan((barCodeType, barCodeValue) => {
            QRReader.Stop();
            resultText.text = "Found: [" + barCodeType + "] " + "<b>" + barCodeValue +"</b>";

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        });
    }
}
