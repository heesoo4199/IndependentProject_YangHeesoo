using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour
{

    float speed = 20f;

    public Transform Alien;
    public float direction;
    public float timePassed;
    public int whatToShoot;

    public int[] intList;
    int count = 35;

    // Use this for initialization
    void Start()
    {
        int offsetx = 2;
        int offsety = 1;
        int counter = 0;

        Vector3 origin = new Vector3(-6, 0, 0);

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Vector3 position = new Vector3(i * offsetx, j * offsety, 0) + origin;
                GameObject theAlien;
                theAlien = Instantiate(Alien.gameObject, position, transform.rotation) as GameObject;
                if (counter == 0)
                {
                    theAlien.name = "Alien";
                }
                else
                {
                    theAlien.name = "Alien (" + counter + ")";
                }

                theAlien.transform.parent = transform;

                counter++;
            }
        }

        intList = new int[count];

        direction = 1f;
        for (int i = 0; i < count; i++)
        {
            intList[i] = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timePassed > 0.9f)
        {
            whatToShoot = Random.Range(0, count);
            while (intList[whatToShoot] == 420)
            {
                whatToShoot++;
                if (whatToShoot > count)
                {
                    break;
                }
                //whatToShoot = Random.Range(0, count);
            }
            timePassed = 0;
            if (!(whatToShoot > count))
            {
                AudioSource toPlay = GameObject.Find("invaderkilled").GetComponent<AudioSource>();
                toPlay.PlayDelayed(0.05f);
            }
        }
        timePassed += Time.deltaTime;
    }

}
