using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{

    public float attackSpeed;
    public float attackSpeedMod = 1.0f;
    public float attackDamage;
    public float attackNumber;

    private float timeSinceLastAttack = 0;

    public float drawAmount = 0;
    public float drawMax = 2;

    public int peirceMod = 0;

    public GameObject projecile;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (timeSinceLastAttack <= attackSpeed * attackSpeedMod)
        {
            timeSinceLastAttack += Time.deltaTime * 1;
        }

    }

    public void DrawBow()
    {
        if (drawAmount < drawMax)
        {
            drawAmount += Time.deltaTime * attackSpeedMod;
        }
    }
    public void Shoot(Vector2 startPos, float angle)
    {

        if (timeSinceLastAttack > attackSpeed * attackSpeedMod)
        {

            GameObject pObj = Instantiate(projecile, startPos, Quaternion.Euler(0, 0, angle));
            Projectile p = pObj.GetComponent<Projectile>();
            p.baddieProjectile = false;

            if (drawAmount < 0.2)
            {
                p.peirce = 0 * 1 + peirceMod;
                p.speed = 0.2f;
                p.damage = 3 + 1 * peirceMod;
            }
            else if (drawAmount < 0.5)
            {
                p.peirce = 1 + 1 * peirceMod;
                p.speed = 0.4f;
                p.damage = 5 + 1 * peirceMod;
            }
            else if (drawAmount < 0.8)
            {
                p.peirce = 2 + 1 * peirceMod;

                p.speed = 0.6f;
                p.damage = 8 + 1 * peirceMod;
            }
            else
            {
                p.peirce = 3 + 1 * peirceMod;
                p.speed = 1f;
                p.damage = 10 + 1 * peirceMod;
            }




            timeSinceLastAttack = 0;
            drawAmount = 0;
        }
    }

}
