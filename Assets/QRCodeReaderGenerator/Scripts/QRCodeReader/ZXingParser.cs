using System;
using UnityEngine;
using ZXing;

internal class ZXingParser : IResult
{
    public BarcodeReader Scanner { get; private set; }

    public ZXingParser() : this(new DeviceCameraOptions(512,512, FilterMode.Trilinear))
    {
    }

    public ZXingParser(DeviceCameraOptions settings)
    {
        Scanner = new BarcodeReader();
        Scanner.AutoRotate = settings.ParserAutoRotate;
        Scanner.TryInverted = settings.ParserTryInverted;
        Scanner.Options.TryHarder = settings.ParserTryHarder;
    }

    public QRResult Decode(Color32[] colors, int width, int height)
    {
        if (colors == null || colors.Length == 0 || width == 0 || height == 0)
        {
            return null;
        }

        QRResult value = null;

        try
        {
            var result = Scanner.Decode(colors, width, height);
            if (result != null)
            {
                value = new QRResult(result.BarcodeFormat.ToString(), result.Text);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }

        return value;
    }
}