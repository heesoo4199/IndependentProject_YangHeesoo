using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class EnemyController : MonoBehaviour, ProjectileLauncher
{

    Transform Alien;
    float speed = 20f;
    float direction;
    float timePassed;
    float shootTimePassed;
    bool ableToShoot = true;


    public bool randomShoot = false;
    public int whatNum;

    Transform projectile;
    public bool Shot;
    public Transform whatToCopy;

    GridController control;
    
    // Use this for initialization
    void Start ()
    {
        GameObject theGrid = GameObject.Find("Grid");
        control = theGrid.GetComponent<GridController>();
        if (gameObject.name.Length == 9)
        {
            whatNum = int.Parse(gameObject.name.Substring(7, 1));
        }
        else if (gameObject.name.Length == 10)
        {
            whatNum = int.Parse(gameObject.name.Substring(7, 2));
        }
        else
        {
            whatNum = 0;
        }
        //Heesoo is a God  
        Shot = false;
        direction = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimePassed = control.timePassed;
        if (control.intList[control.whatToShoot] == whatNum)
        {
            randomShoot = true;
        }
        direction = control.direction;
        control.timePassed = timePassed;

        if (shootTimePassed > 0.9f)
        {
            if (ableToShoot && randomShoot)
            {
                Shot = true;
                CreateNewProjectile();
                randomShoot = false;
            }
        }

        if (timePassed > 1)
        {
            transform.position += new Vector3(direction * Time.deltaTime * speed, 0, 0);
            /*if (ableToShoot && randomShoot)
            {
                Shot = true;
                CreateNewProjectile();
                randomShoot = false;
            }*/
            timePassed = 0f;
        }
        timePassed += Time.deltaTime;
    }

    //void OnCollisionEnter2D(Collision2D coll)
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Walls")
        {
            if (coll.gameObject.name.Equals("Wall_Right"))
            {
                control.direction = -1f;
            }
            else
            {
                control.direction = 1f;
            }
        }
        else if (coll.gameObject.tag == "Projectile")
        {
            if (!coll.gameObject.name.Equals("AlienShoot(Clone)"))
            {
                control.intList[whatNum] = 420;
                gameObject.GetComponent<AudioSource>().Play();
                Destroy(coll.gameObject);

                Score theScore = GameObject.Find("ScoreManager").GetComponent<Score>();
                theScore.scoreCount += 5;


                Kill();
                Destroy(gameObject, 0.3f);
            }
        }
    }

    void Kill()
    {
        Destroy(gameObject.GetComponent<CircleCollider2D>());
        Destroy(gameObject.GetComponent<SpriteRenderer>());
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        ableToShoot = false;
    }

    public void CreateNewProjectile()
    {
        projectile = Instantiate(whatToCopy, transform.position, transform.rotation) as Transform;
        projectile.position = transform.position;
        GameObject test = new GameObject("TheAlienTest");
        test.transform.parent = transform;
        test.tag = "Responsible";
    }

    public bool isShot()
    {
        return Shot;
    }

    public void changeShot(bool a)
    {
        Shot = a;
    }

    public string toString()
    {
        return "Alien";
    }

}

