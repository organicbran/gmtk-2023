using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePrefab : MonoBehaviour
{
    [SerializeField] private AudioSource soundEffect;
    [SerializeField] private ParticleSystem particles;

    private void Start()
    {
        soundEffect.Play();
    }

    private void Update()
    {
        if (!particles.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
