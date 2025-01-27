using UnityEngine;

public class MinValue : PropertyAttribute
{
    MinValue(int value)
    {
        if (value < 0)
            value = 0;
    }

    MinValue(float value)
    {
        if (value < 0)
            value = 0;
    }
}
