using UnityEngine;

public interface IResult
{
    QRResult Decode(Color32[] colors, int width, int height);

}