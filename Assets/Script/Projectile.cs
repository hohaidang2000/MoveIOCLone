using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    private Transform spawnTransform;
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward *Time.deltaTime * speed);
    }
}
