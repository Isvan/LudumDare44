using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public Vector2 direction;
    public float speed = 1;

    public int peirce;
    private int hits = 0;

    public int damage = 3;
    public bool baddieProjectile;
    private BoxCollider2D col;
    private SpriteRenderer render;
    // Use this for initialization
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        Collider2D[] firsHit = new Collider2D[1];

        col.OverlapCollider(new ContactFilter2D(), firsHit);

        if (firsHit[0] != null)
        {

            if (baddieProjectile)
            {
                if (firsHit[0].gameObject.tag.Equals("Player"))
                {

                    // Object.Destroy(firsHit[0].gameObject);
                    mainChar b = firsHit[0].GetComponent<mainChar>();
                    
                    b.takeDamage(damage);
                    Object.Destroy(gameObject);
                }


                if (!firsHit[0].gameObject.tag.Equals("Baddie"))
                {
                    Object.Destroy(gameObject);

                }
            }
            else
            {
                if (firsHit[0].gameObject.tag.Equals("Baddie"))
                {

                    // Object.Destroy(firsHit[0].gameObject);
                    Baddie b = firsHit[0].GetComponent<Baddie>();


                    if (b.hp - damage < 0)
                    {


                        int newdamage = damage - b.hp;

                        b.takeDamg(damage);
                        damage = newdamage;

                    }
                    else
                    {
                        b.takeDamg(damage);
                        hits = peirce;
                    }
                    hits++;
                }


                if (!firsHit[0].gameObject.tag.Equals("Player") && hits > peirce || firsHit[0].gameObject.tag.Equals("Wall"))
                {
                    Object.Destroy(gameObject);

                }
            }

           

        }

        transform.Translate(new Vector3(Time.deltaTime * speed * 50, 0, 0));

        if (!render.isVisible)
        {
            Object.Destroy(gameObject);
        }

    }



}
