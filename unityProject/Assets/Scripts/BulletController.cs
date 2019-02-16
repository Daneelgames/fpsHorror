using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 5;
    public GameObject explosion;
    public Rigidbody rb;
    Vector3 previousPosition;

    private void Start()
    {
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        previousPosition = transform.position;

        rb.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        //RaycastHit[] hits = Physics.RaycastAll(new Ray(previousPosition, (transform.position - previousPosition).normalized), (transform.position - previousPosition).magnitude);
        Ray newRay = new Ray(previousPosition, (transform.position - previousPosition).normalized);
        RaycastHit hit = Physics.SphereCast(newRay, 0.25f);
        

        for(int i = hits.Length - 1; i >= 0; i --)
        {
            if (hits[i].collider.gameObject.layer == 11 || hits[i].collider.gameObject.layer == 12)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            if (hits[i].collider.gameObject.layer == 9 || hits[i].collider.gameObject.layer == 10)
            {
                hits[i].collider.gameObject.GetComponent<HealthController>().Damage(hits[i].point);
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}