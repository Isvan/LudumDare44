using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{

    public float attackSpeed;
    public float attackSpeedMod = 1.0f;
    public int attackDamage;
    public PolygonCollider2D col;
    public int cleaveNumber;

    public int weaponId;


    public Vector2 attackOffSet;
    private float timeSinceLastAttack = 0;
    // Use this for initialization
    void Start()
    {
        col = GetComponent<PolygonCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {

        if (timeSinceLastAttack <= attackSpeed * attackSpeedMod)
        {
            timeSinceLastAttack += Time.deltaTime * 10;
        }
    }



    public bool Attack(Vector2 position, float angle)
    {
        bool attacked = false;

        if (timeSinceLastAttack > attackSpeed * attackSpeedMod)
        {
            attacked = true;
            timeSinceLastAttack = 0;
            //Move to same place
            transform.position = position;



            if (angle >= -45 && angle < 45)
            {

                //  Debug.Log("Right");

                transform.position = new Vector3(transform.position.x + attackOffSet.x, transform.position.y, transform.position.z);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (angle >= 45 && angle < 135)
            {
                //Debug.Log("Up");

                transform.position = new Vector3(transform.position.x, transform.position.y + attackOffSet.y, transform.position.z);
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }


            if (angle >= 135 || angle < -135)
            {
                ///  Debug.Log("Left");

                transform.position = new Vector3(transform.position.x - attackOffSet.x, transform.position.y, transform.position.z);
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }


            if (angle >= -135 && angle < -45)
            {
                //    Debug.Log("Down");

                transform.position = new Vector3(transform.position.x, transform.position.y - attackOffSet.y, transform.position.z);
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }



            Collider2D[] results = new Collider2D[cleaveNumber];

            col.OverlapCollider(new ContactFilter2D(), results);


            int hit = 0;
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] != null && !results[i].gameObject.tag.Equals("Player"))
                {
                    hit++;
                    if (results[i].gameObject.tag.Equals("Baddie"))
                    {
                        //Object.Destroy(results[i].gameObject);
                        Baddie b = results[i].GetComponent<Baddie>();
                        b.takeDamg(attackDamage);
                    }
                }
            }

        }

        return attacked;
    }
}
