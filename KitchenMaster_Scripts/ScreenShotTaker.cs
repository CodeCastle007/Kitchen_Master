using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotTaker : MonoBehaviour
{
    int i = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Screen Shot Taken");
            ScreenCapture.CaptureScreenshot("ScreenShot_" + i + ".png", 1);

            i++;
        }
    }
}
