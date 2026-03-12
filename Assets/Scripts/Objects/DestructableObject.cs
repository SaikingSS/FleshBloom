using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    public float durability;
    public bool flamable;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject destoryedObject, buringParticles;
    [SerializeField] private Collider objectCollider;

    private float burnTimer;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.LogError(rb.velocity.magnitude);

        //Damage of its collicion
        if (rb.linearVelocity.magnitude > 9f)
        {
            durability -= 25;
        }
        else if (rb.linearVelocity.magnitude > 5f)
        {
            durability -= 7;
        }
        else if (rb.linearVelocity.magnitude > 2.5f)
        {
            durability -= 4;
        }


        //Damage of something else colliding in it

        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            Rigidbody collisonRb = collision.gameObject.GetComponent<Rigidbody>();

            if (collisonRb.linearVelocity.magnitude > 10f)
            {
                durability -= 35;
            }
            else if (collisonRb.linearVelocity.magnitude > 5f)
            {
                durability -= 10;
            }
            else if (collisonRb.linearVelocity.magnitude > 3f)
            {
                durability -= 7;
            }
        }

       

        if (durability <= 0)
        {
            DestoryObject();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (flamable)
        {
            if (other.CompareTag("fire") && !buringParticles.activeInHierarchy)
            {
                StartCoroutine(BuringCo());
            }
            else if (other.CompareTag("fire"))
            {
                burnTimer += 10;
            }
        }
    }

    public void BurnStart()
    {
        StartCoroutine(BuringCo());
    }

    public IEnumerator BuringCo()
    {
        burnTimer += 10;
        buringParticles.SetActive(true);
        yield return new WaitForSeconds(1f);
        buringParticles.GetComponent<Collider>().enabled = true;

        while (durability > 0 || burnTimer < 0)
        {
            durability -= 3.5f;
            burnTimer--;
            if(durability <= 0)
            {
                DestoryObject();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void DestoryObject()
    {
        objectCollider.enabled = false;
        if (destoryedObject)
        {
            GameObject remains = Instantiate(destoryedObject, transform.position, transform.rotation);
            remains.transform.SetParent(gameObject.transform);
            remains.transform.localPosition = Vector3.zero;
            remains.transform.localEulerAngles = Vector3.zero;
            remains.transform.localScale = Vector3.one;
            remains.transform.SetParent(null);
            /*if(burnTimer > 0)
            {
                if (destoryedObject.GetComponent<DestructableObject>())
                {
                    destoryedObject.GetComponent<DestructableObject>().BurnStart();
                }
                for (int i = 0; i < destoryedObject.transform.childCount; i++)
                {
                    if (destoryedObject.transform.GetChild(i).GetComponent<DestructableObject>())
                    {
                        destoryedObject.transform.GetChild(i).GetComponent<DestructableObject>().BurnStart();
                    }
                }
            }*/
        }    
        Destroy(gameObject);
    }
}
