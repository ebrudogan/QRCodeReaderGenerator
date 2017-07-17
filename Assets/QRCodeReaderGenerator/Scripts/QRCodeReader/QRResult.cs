using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRResult {

    public string Type { get; private set; }
    public string Value { get; private set; }

    public QRResult(string type, string value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return string.Format("[Result {0}:{1}]", Type, Value);
    }
}
