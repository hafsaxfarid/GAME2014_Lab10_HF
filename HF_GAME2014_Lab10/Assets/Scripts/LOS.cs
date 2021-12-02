using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[System.Serializable]
public class LOS : MonoBehaviour
{
    [Header("Detection Properties")]
    public Collider2D collidesWith;
    public ContactFilter2D contactFilter;
    public List<Collider2D> colliderList;

    private PolygonCollider2D LOSCollider;

    void Start()
    {
        LOSCollider = GetComponent<PolygonCollider2D>();
    }

    void FixedUpdate()
    {
        Physics2D.GetContacts(LOSCollider, contactFilter, colliderList);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       collidesWith = other;
    }
}