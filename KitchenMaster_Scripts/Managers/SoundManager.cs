using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYERPREFS_SOUND_VOLUME = "SoundVolume";
    public event Action<float> OnSoundVolumeChanged;

    [SerializeField] private AudioClipRefSO audioClipRefSO;
    private float volume = .5f;
    public static SoundManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        volume = PlayerPrefs.GetFloat(PLAYERPREFS_SOUND_VOLUME, .5f);
    }

    private void OptionsUI_OnSoundValueChanged(float obj)
    {
        SetVolume(obj);
    }

    private void Start()
    {
        DeliveryTable.OnAnySuccessfulDelivery += DeliveryTable_OnAnySuccesfulDelivery;
        DeliveryTable.OnAnyFailedDelivery += DeliveryTable_OnAnyFailedDelivery;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;

        Player.Instance.OnPickSomething += Player_OnPickSomething;

        OptionsUI.Instance.OnSoundValueChanged += OptionsUI_OnSoundValueChanged;
    }

    private void TrashCounter_OnAnyObjectTrashed(Transform obj)
    {
        PlaySound(audioClipRefSO.trash, obj.position);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(Transform obj)
    {
        PlaySound(audioClipRefSO.objectDrop, obj.position);
    }

    private void Player_OnPickSomething(Transform obj)
    {
        PlaySound(audioClipRefSO.objectPickUp, obj.position);
    }

    private void CuttingCounter_OnAnyCut(Transform obj)
    {
        PlaySound(audioClipRefSO.chop, obj.position);
    }

    private void DeliveryTable_OnAnyFailedDelivery(Transform obj)
    {
        PlaySound(audioClipRefSO.deliveryFail, obj.position);
    }

    private void DeliveryTable_OnAnySuccesfulDelivery(Transform obj)
    {
        PlaySound(audioClipRefSO.deliverySuccess, obj.position);
    }

    public void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0,audioClipArray.Length)], position, volume);
    }

    public void PlaySound(AudioClip audioClip,Vector3 position,float volumeMultiplyer = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplyer * volume);
    }

    public void PlayFootStepSound(Vector3 position,float volume)
    {
        PlaySound(audioClipRefSO.footStep, position, volume);
    }

    public void PlayButtonClickSound()
    {
        PlaySound(audioClipRefSO.buttonClick, Camera.main.transform.position, .5f);
    }
    public void PlayPurchaseSound()
    {
        PlaySound(audioClipRefSO.purchase, Camera.main.transform.position, .5f);
    }

    public void SetVolume(float volume)
    {
        this.volume = volume;
        OnSoundVolumeChanged?.Invoke(this.volume);
    }

    //public float GetVolume()
    //{
    //    return volume;
    //}
}
