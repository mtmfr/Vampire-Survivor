using UnityEngine;

public class MinValue : PropertyAttribute
{
    
    MinValue(byte value, byte min)
    {
        if (value < min)
            value = min;
    }
    MinValue(int value, int min)
    {
        if (value < min)
            value = min;
    }

    MinValue(float value, float min)
    {
        if (value < min)
            value = min;
    }
}
