using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplacementControl : MonoBehaviour
{
    [SerializeField] private float displacementAmount;
    [SerializeField] private ParticleSystem shootParticles;
    [SerializeField] MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        displacementAmount = Mathf.Lerp(displacementAmount, 0, Time.deltaTime);
        meshRenderer.material.SetFloat("_Amount", displacementAmount);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            displacementAmount += 1f;
            shootParticles.Play();
        }
    }
}
