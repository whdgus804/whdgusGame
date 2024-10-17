using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWholeBody : MonoBehaviour
{
    PlayerInput input;
    SpriteRenderer spriteRenderer;
    public GameObject bodyPrefab;
    private void Awake()
    {
        input= GetComponent<PlayerInput>();
        spriteRenderer=bodyPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    public void DisableBody()
    {
        if (spriteRenderer.enabled)
        {
            spriteRenderer.enabled = false;
        }
    }
    public void EnableBody()
    {
        if(!spriteRenderer.enabled)
        {
            spriteRenderer.enabled = true;

        }
    }

}
