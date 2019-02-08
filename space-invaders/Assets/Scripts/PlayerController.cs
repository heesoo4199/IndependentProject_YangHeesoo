using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour, ProjectileLauncher
{

    public float speed = 3f;
    Rigidbody2D playerBody;
    bool ableToShoot = true;

    Transform projectile;
    public Transform whatToCopy;
    public bool Shot;

    // Use this for initialization
    void Start()
    {
        Shot = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-1 * Time.deltaTime * speed, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(1 * Time.deltaTime * speed, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ableToShoot)
            {
                Shot = true;
                CreateNewProjectile();
            }
        }
    }

    public void CreateNewProjectile()
    {
        projectile = Instantiate(whatToCopy, transform.position, transform.rotation) as Transform;
        projectile.position = transform.position;
        GameObject test = new GameObject("TheResponsibleTest");
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
        return "Player";
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Projectile")
        {
            if (!coll.gameObject.name.Equals("PlayerProjectile(Clone)"))
            {
                gameObject.GetComponent<AudioSource>().Play();
                Destroy(coll.gameObject);
                Score theScore = GameObject.Find("ScoreManager").GetComponent<Score>();
                theScore.lives--;
                if (theScore.lives > 0)
                {
                    Vector3 pos = new Vector3(-0.15f, -3.87f, 0);
                    Instantiate(transform.gameObject, pos, transform.rotation);
                }
                Kill();
                Destroy(gameObject, 0.3f);
            }
        }
        
    }

    void Kill()
    {
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        Destroy(gameObject.GetComponent<SpriteRenderer>());
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        ableToShoot = false;
    }

}