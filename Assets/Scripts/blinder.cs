using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blinder : MonoBehaviour {

    public GameObject target;
    public Vector3 offset;
    private float angle = 0;

    public bool rotate;

    private SpriteRenderer ren;

	// Use this for initialization
	void Start () {
        ren = GetComponent<SpriteRenderer>();
	}
	
    public void hide()
    {
        ren.enabled = false;
    }

    public void show()
    {
        ren.enabled = true;
    }

	// Update is called once per frame
	void Update () {

        transform.position = target.transform.position + offset;

        if (rotate)
        {

            Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3 lookAt = mouseScreenPosition;

            float AngleRad = Mathf.Atan2(lookAt.y - this.transform.position.y, this.transform.position.x - lookAt.x);

            float AngleDeg = (180 / Mathf.PI) * AngleRad;

            transform.RotateAround(target.transform.position, new Vector3(0, 0, 1), angle - AngleDeg);
            angle = AngleDeg;
        }
    }
}
