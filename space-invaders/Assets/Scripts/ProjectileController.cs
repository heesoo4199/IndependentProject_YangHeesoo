using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{

    public int projDirection;

    int playerCoef;
    float timePassed;
    Collider2D projColl;
    ProjectileLauncher playerScript;

	// Use this for initialization
	void Start ()
    {
        timePassed = 0f;

        GameObject test = GameObject.FindGameObjectWithTag("Responsible");
        GameObject theShooter = test.transform.parent.gameObject;
        if (test.name.Equals("TheAlienTest"))
        {
            playerScript = theShooter.GetComponent<EnemyController>();
            projDirection = -1;
            playerCoef = 1;
        }
        else
        {
            playerScript = theShooter.GetComponent<PlayerController>();
            projDirection = 1;
            playerCoef = 2;
        }
        DestroyImmediate(test);

        projColl = gameObject.GetComponent<BoxCollider2D>();
        projColl.isTrigger = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (playerScript.isShot())
        {
            if (timePassed > 1f)
            {
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (playerScript.isShot())
        {
            if (timePassed > 2f)
            {
                playerScript.changeShot(false);
                timePassed = 0f;
            }
            transform.position += new Vector3(0, 9f * Time.deltaTime * playerCoef * projDirection, 0);
            timePassed += Time.deltaTime;
        }
        else
        {
            Destroy(transform.gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Projectile")
        {
            Destroy(coll.gameObject);
            Destroy(gameObject);
        }
    }
}
