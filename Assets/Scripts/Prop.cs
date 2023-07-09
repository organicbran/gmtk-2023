using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private Destroyable[] subprops;
    private int subCount;
    private GameManager manager;

    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<GameManager>();
        subCount = subprops.Length;
        foreach (Destroyable sub in subprops)
        {
            sub.parentProp = this;
        }
    }

    private void Update()
    {
        if (subCount <= 0)
        {
            manager.PropDestroyed();
            Destroy(gameObject);
        }
    }

    public void Destroyed(Destroyable destroyed)
    {
        subCount--;
    }
}
