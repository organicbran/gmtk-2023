using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : MonoBehaviour
{
    [SerializeField] private GameObject model;

    [HideInInspector] public bool activated;

    private GameManager manager;

    private void Start()
    {
        Activate(false);
    }

    public void Activate(bool activate)
    {
        if (activate && !activated)
        {
            activated = true;
            model.SetActive(true);
        }
        else
        {
            activated = false;
            model.SetActive(false);
        }
    }
}
