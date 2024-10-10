using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AdobeUtility : MonoBehaviour
{
    static public string ShowBitArray(int original)
    {
        StringBuilder answer = new StringBuilder();
        answer.Append(original < 0 ? '1' : '0');
        for (int index = 30; index > 0; --index)
        {
            if ((original & (1 << index)) > 0) answer.Append('1');
            else answer.Append('0');
        }
        return answer.ToString();
    }
}
