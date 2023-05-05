using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Niantic.ARDK.Extensions.Gameboard;
using UnityEngine;
using ZXing.Common;
using Path = System.IO.Path;

public class InteractableComponent : MonoBehaviour
{
    public float InteractionRange = 6.0f;
    public bool PhotoDiscoverable = true;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _onTapClip;

    public void SaveTexture()
    {
        Camera cam = Camera.main;

        RenderTexture rt = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);
        RenderTexture oldrt = cam.targetTexture;
        cam.targetTexture = rt;
        cam.Render();
        cam.targetTexture = oldrt;

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width / 8, rt.height / 8, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, "JournalPhotos");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllBytes(Path.Combine(path, name + ".png"), bytes);
        Destroy(tex);
    }

    public virtual void OnObjectTapped()
    {
        if (PlayerManager.Instance.IsAccusing)
        {
            return;
        }
        Debug.Log(name + " tapped.");
        if (_audioSource && _onTapClip)
        {
            _audioSource.PlayOneShot(_onTapClip);
        }

        if (PhotoDiscoverable)
        {
            SaveTexture();
        }
    }
}
