using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainChar : MonoBehaviour
{

    public float moveSpeed = 1;
    private Rigidbody2D rb;
    private PolygonCollider2D boxCol;
    public GameObject attack;
    public GameObject swapAttack;
    public GameObject swapAttack2;
    public GameObject firePeice;

    public float damageInvulTime;
    public float timeSinceLastHit = 0;


    public GameObject hpBar;
    private float hpBarFullSize;

    public int hp;
    public int currentHp;

    private ParticleSystem partySys;
    public int weapon = 0;
    public Animator anim;
    public float slowDown = 1;
    public float speedMod = 1;

    public float flamethrowerTotalCoolDown;
    public float flamethrowerPartCoolDown;

    public float flamethrowerCurrentTotalCoolDown;
    public float flamethrowerCurrentPartCoolDown;


    public float hpRegenTime;
    private float currentHpRegen = 0;

    public float teleportCoolDown;
    private float currentTeleportCoolDown = 0;

    private int spellSelected = 0;

    public bool canEat = true;

    public int[] activeSpells;
    public bool dead = false;
    public bool hpRegen = false;
    // Use this for initialization
    void Start()
    {

        activeSpells = new int[2];
        activeSpells[0] = 0;
        activeSpells[1] = 0;


        hpBarFullSize = hpBar.transform.localScale.x;
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<PolygonCollider2D>();
        //attack = GetComponentInChildren<PolygonCollider2D>();

       // attack = Instantiate(attack, transform.position, transform.rotation);
       // swapAttack = Instantiate(swapAttack, transform.position, transform.rotation);
        anim = GetComponent<Animator>();
     //   partySys = GetComponent<ParticleSystem>();


    }

    public void updateWeapon(int slot,GameObject weapon)
    {
        if(slot == 0)
        {
            attack = weapon;
        }else if(slot == 1) {
            swapAttack = weapon;
        }else if(slot == 2)
        {
            swapAttack2 = weapon;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHpRegen < hpRegenTime)
        {
            currentHpRegen += Time.deltaTime;
        }else if (hpRegen && currentHp < hp)
        {
            currentHpRegen = 0;
            currentHp++;
            updateHP();
        }

        if(currentTeleportCoolDown < teleportCoolDown)
        {
            currentTeleportCoolDown += Time.deltaTime;
        }

        rb.velocity = new Vector2(0, 0);

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject temp = attack;
            GameObject temp2 = swapAttack2;

            attack = swapAttack;
            swapAttack2 = temp;
            swapAttack = temp2;


            if (attack.tag.Equals("Melee"))
            {
                MeleeAttack mA = attack.GetComponent<MeleeAttack>();
                anim.SetInteger("weapon", mA.weaponId);
            }
            else
            {
                anim.SetInteger("weapon", 2);
            }

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(spellSelected == 0)
            {
                spellSelected = 1;
            }else
            {
                spellSelected = 0;
            }
        }

            Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 lookAt = mouseScreenPosition;

        float AngleRad = Mathf.Atan2(lookAt.y - this.transform.position.y, lookAt.x - this.transform.position.x);

        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        transform.rotation = Quaternion.Euler(0, 0, 0);


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


        // if (AngleDeg >= -135 && AngleDeg < -45)
        // {
        //     //    Debug.Log("Down");

        //     transform.rotation = Quaternion.Euler(0, 0, 270);
        // }

        float yMove = Input.GetAxis("Vertical");
        float xMove = Input.GetAxis("Horizontal");

        yMove = Time.deltaTime * moveSpeed * 50.0f * slowDown * yMove * speedMod;
        xMove = Time.deltaTime * moveSpeed * 50.0f * slowDown * xMove * speedMod;
        rb.velocity = new Vector2(xMove, yMove);


        if (Input.GetButton("Fire1") && attack != null)
        {


            if (attack.tag.Equals("Melee"))
            {

                MeleeAttack mASpec = attack.GetComponent<MeleeAttack>();
                if (mASpec.Attack(transform.position, AngleDeg))
                {
                    anim.SetTrigger("attacking");
                }


            }
            else if (attack.tag.Equals("Ranged"))
            {

                RangedAttack rASpec = attack.GetComponent<RangedAttack>();

                //rASpec.Shoot(transform.position, AngleDeg);
                rASpec.DrawBow();

                //    Debug.Log("Drawing Arrow " + rASpec.drawAmount);
                slowDown = 0.2f;

                anim.SetBool("bowDrawn", true);
            }

        }

        if (Input.GetButtonDown("Fire2") && spellSelected == 0 && currentTeleportCoolDown > teleportCoolDown && activeSpells[0] == 1)
        {

            mouseScreenPosition.z = 0;
            transform.position = mouseScreenPosition ;
            currentTeleportCoolDown = 0;

        }

        if(flamethrowerCurrentTotalCoolDown < flamethrowerTotalCoolDown)
        {
            flamethrowerCurrentTotalCoolDown += Time.deltaTime;
        }

        if (flamethrowerCurrentPartCoolDown < flamethrowerPartCoolDown)
        {
            flamethrowerCurrentPartCoolDown += Time.deltaTime;
        }



        if (Input.GetButton("Fire2") && spellSelected == 1 && flamethrowerCurrentTotalCoolDown > flamethrowerTotalCoolDown && activeSpells[1] == 1)
        {
            flamethrowerCurrentTotalCoolDown -= Time.deltaTime * 50;
            
                GameObject pObj = Instantiate(firePeice, transform.position, Quaternion.Euler(0, 0, AngleDeg + Random.Range(-20.0f, 20.0f)));
                Projectile p = pObj.GetComponent<Projectile>();
                p.baddieProjectile = false;

             pObj = Instantiate(firePeice, transform.position, Quaternion.Euler(0, 0, AngleDeg + Random.Range(-20.0f, 20.0f)));
             p = pObj.GetComponent<Projectile>();
            p.baddieProjectile = false;

             pObj = Instantiate(firePeice, transform.position, Quaternion.Euler(0, 0, AngleDeg + Random.Range(-20.0f, 20.0f)));
             p = pObj.GetComponent<Projectile>();
            p.baddieProjectile = false;


            pObj = Instantiate(firePeice, transform.position, Quaternion.Euler(0, 0, AngleDeg + Random.Range(-20.0f, 20.0f)));
            p = pObj.GetComponent<Projectile>();
            p.baddieProjectile = false;

            pObj = Instantiate(firePeice, transform.position, Quaternion.Euler(0, 0, AngleDeg + Random.Range(-20.0f, 20.0f)));
            p = pObj.GetComponent<Projectile>();
            p.baddieProjectile = false;

            pObj = Instantiate(firePeice, transform.position, Quaternion.Euler(0, 0, AngleDeg + Random.Range(-20.0f, 20.0f)));
            p = pObj.GetComponent<Projectile>();
            p.baddieProjectile = false;

            pObj = Instantiate(firePeice, transform.position, Quaternion.Euler(0, 0, AngleDeg + Random.Range(-20.0f, 20.0f)));
            p = pObj.GetComponent<Projectile>();
            p.baddieProjectile = false;

        }

        if (Input.GetButtonUp("Fire1") && attack != null)
        {
            //anim.SetInteger("attacking", 0);
            if (attack.tag.Equals("Ranged"))
            {
                anim.SetBool("bowDrawn", false);
                //    Debug.Log("Firing Arrow");
                RangedAttack rASpec = attack.GetComponent<RangedAttack>();
                rASpec.Shoot(transform.position, AngleDeg);
                slowDown = 1f;
            }

        }

        if(timeSinceLastHit < damageInvulTime)
        {
       //     if (!partySys.isPlaying)
         //   {
         //       partySys.Play();
         //   }
            timeSinceLastHit += Time.deltaTime;
        }else
        {
    //        if (partySys.isPlaying)
     //       {
     //           partySys.Stop();
     //       }
        }



    }

    public void updateHP()
    {
        hpBar.transform.localScale = new Vector3((float)currentHp / (float)hp * hpBarFullSize, hpBar.transform.localScale.y, hpBar.transform.localScale.z);

    }

    public void takeDamage(int damg)
    {



        if(timeSinceLastHit > damageInvulTime)
        {
            timeSinceLastHit = 0;
            currentHp -= damg;
            updateHP();
            if(currentHp < 0)
            {
                dead = true;
            }

        }



    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject.tag == "hpPot" && canEat)
        {
            
            if(currentHp < hp)
            {
                Destroy(collision.gameObject);
                currentHp += 10;
                if(currentHp > hp)
                {
                    currentHp = hp;
                }
                updateHP();
            }
        }
       
    }
}
