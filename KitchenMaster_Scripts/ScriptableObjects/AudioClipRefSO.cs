using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefSO : ScriptableObject
{
    public AudioClip[] chop;
    public AudioClip[] deliveryFail;
    public AudioClip[] deliverySuccess;
    public AudioClip[] footStep;
    public AudioClip[] objectDrop;
    public AudioClip[] objectPickUp;
    public AudioClip frying;
    public AudioClip[] trash;
    public AudioClip[] warning;
    public AudioClip[] buttonClick;
    public AudioClip[] purchase;
}
