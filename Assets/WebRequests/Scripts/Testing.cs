using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Testing : MonoBehaviour {

    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start() {
        /*
        string url = "http://google.com/";
        WebRequests.Get(url, (string error) => {
            // Error
            Debug.Log("Error: " + error);
            textMesh.SetText("Error: " + error);
        }, (string text) => { 
            // Successfully contacted URL
            Debug.Log("Received: " + text);
            textMesh.SetText(text);
        });
        */

        string url = "https://i.ytimg.com/vi/CSeUMTaNFYk/hqdefault.jpg";
        WebRequests.GetTexture(url, (string error) => { 
            // Error
            Debug.Log("Error: " + error);
            textMesh.SetText("Error: " + error);
        }, (Texture2D texture2D) => {
            // Successfully contacted URL
            textMesh.SetText("Success!");
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f), 10f);
            spriteRenderer.sprite = sprite;
        });
    }

}
