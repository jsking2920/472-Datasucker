using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableComponent : MonoBehaviour
{
    public float InteractionRange = 6.0f;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _onTapClip;

    public virtual void OnObjectTapped()
    {
        Debug.Log(name + " tapped.");
        if (_audioSource && _onTapClip)
        {
            _audioSource.PlayOneShot(_onTapClip);
        }
    }
}
