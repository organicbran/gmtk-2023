using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [HideInInspector] public Prop parentProp;

    public void DestroyProp()
    {
        parentProp.Destroyed(this);
        Destroy(gameObject);
    }
}
