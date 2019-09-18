using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baddie : MonoBehaviour
{


    public float speed = 1;

    public float minDistance = 0.3f;
    public float attackSpeed;
    public int damage;
    public bool rangedBaddie;
    public float range;
    public GameObject targetObj;
    public int hp = 10;
    public Vector2 target;
    public Animator anim;
    public CircleCollider2D col;
    public GameObject projectile;
    public bool inrange = false;
    private float currentAttack = 0;

    public GameObject hpPotRef;

    public float staggerTime;
    private float staggered;

    // Use this for initialization
    void Start()
    {
        target = transform.position;
        anim = GetComponent<Animator>();
         col = GetComponent<CircleCollider2D>();
    }

    public void takeDamg(int damage)
    {
        staggered = 0;
        hp -= damage;
        if (hp < 0)
        {
            if (Random.Range(0.0f, 1.0f) > 0.9f)
            {
                Instantiate(hpPotRef, transform.position, new Quaternion());
            }

            Object.Destroy(gameObject);
            
            

        }
    }

    // Update is called once per frame
    void Update()
    {

        if(currentAttack < attackSpeed && staggered > staggerTime)
        {
            currentAttack += Time.deltaTime * 5;
        }

        if(staggered < staggerTime)
        {
            staggered += Time.deltaTime;
            anim.SetTrigger("stagger");
        }

        target = targetObj.transform.position;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        if (!rangedBaddie)
        {
            if (Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) > minDistance && !inrange && staggered > staggerTime)
            {

                float AngleRad = Mathf.Atan2(target.y - this.transform.position.y, target.x - this.transform.position.x);

                float AngleDeg = (180 / Mathf.PI) * AngleRad;

                if (AngleDeg >= -45 && AngleDeg < 45)
                {
                    //  Debug.Log("Right");
                    //       transform.rotation = Quaternion.Euler(0, 0, 0);

                    transform.localScale = new Vector3(-1 * transform.localScale.y, transform.localScale.y, transform.localScale.z);
                }

                // if (AngleDeg >= 45 && AngleDeg < 135)
                // {
                //     //Debug.Log("Up");

                //     transform.rotation = Quaternion.Euler(0, 0, 90);
                // }


                if (AngleDeg >= 135 || AngleDeg < -135)
                {
                    ///  Debug.Log("Left");
                    transform.localScale = new Vector3(transform.localScale.y, transform.localScale.y, transform.localScale.z);
                    // transform.rotation = Quaternion.Euler(0, 0, 180);
                }

                float yMove = 0;
                float xMove = 0;

                if (target.x > transform.position.x)
                {
                    xMove = 1;
                }
                else
                {
                    xMove = -1;
                }

                if (target.y > transform.position.y)
                {
                    yMove = 1;
                }
                else
                {
                    yMove = -1;
                }


                yMove *= Time.deltaTime * speed * 50;
                xMove *= Time.deltaTime * speed * 50;

                GetComponent<Rigidbody2D>().velocity = new Vector2(xMove, yMove);

            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            }
        }
        else
        {
            //Do ranged attack if in range
            if (Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) < range &&  currentAttack > attackSpeed && staggered > staggerTime)
            {
                currentAttack = 0;

                float AngleRad = Mathf.Atan2(target.y - this.transform.position.y, target.x - this.transform.position.x);

                float AngleDeg = (180 / Mathf.PI) * AngleRad;

                Vector3 spawnLoc = transform.position;

                spawnLoc.x += -1;

                GameObject pObj = Instantiate(projectile, spawnLoc, Quaternion.Euler(0, 0, AngleDeg));
                Projectile p = pObj.GetComponent<Projectile>();
                p.baddieProjectile = true;
                p.damage = damage;
                anim.SetTrigger("attacking");

            }
            else if (Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) > range && staggered > staggerTime)
            {
                float AngleRad = Mathf.Atan2(target.y - this.transform.position.y, target.x - this.transform.position.x);

                float AngleDeg = (180 / Mathf.PI) * AngleRad;

                if (AngleDeg >= -45 && AngleDeg < 45)
                {
                    //  Debug.Log("Right");
                    //       transform.rotation = Quaternion.Euler(0, 0, 0);

                    transform.localScale = new Vector3(-1 * transform.localScale.y, transform.localScale.y, transform.localScale.z);
                }

                // if (AngleDeg >= 45 && AngleDeg < 135)
                // {
                //     //Debug.Log("Up");

                //     transform.rotation = Quaternion.Euler(0, 0, 90);
                // }


                if (AngleDeg >= 135 || AngleDeg < -135)
                {
                    ///  Debug.Log("Left");
                    transform.localScale = new Vector3(transform.localScale.y, transform.localScale.y, transform.localScale.z);
                    // transform.rotation = Quaternion.Euler(0, 0, 180);
                }

                float yMove = 0;
                float xMove = 0;

                if (target.x > transform.position.x)
                {
                    xMove = 1;
                }
                else
                {
                    xMove = -1;
                }

                if (target.y > transform.position.y)
                {
                    yMove = 1;
                }
                else
                {
                    yMove = -1;
                }


                yMove *= Time.deltaTime * speed * 50;
                xMove *= Time.deltaTime * speed * 50;

                GetComponent<Rigidbody2D>().velocity = new Vector2(xMove, yMove);
            }else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }


    }

    void OnCollisionStay2D(Collision2D collision)
    {
       

            if (collision.gameObject.tag == "Player")
            {
            inrange = true;
            if (currentAttack > attackSpeed && staggered > staggerTime)
            {
                collision.gameObject.GetComponent<mainChar>().takeDamage(damage);
                anim.SetTrigger("attacking");
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inrange = false;
        }
        }



}
