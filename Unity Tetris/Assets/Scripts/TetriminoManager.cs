using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetriminoManager : MonoBehaviour {

	public float velocity;
	public GameObject copy;

    private ClassicModeManager manager;
	private float velocity_original;

	private void Awake()
	{
        manager = GameObject.FindGameObjectWithTag("ModeManager").GetComponent<ClassicModeManager>();
	}

	// Use this for initialization
	void Start() {
        velocity_original = velocity;
	}
	
	// Update is called once per frame
	void Update() {
		transform.position = new Vector3(transform.position.x, transform.position.y - (velocity * Time.deltaTime));
        RoundX();
	}

	public void Rotate() {
		transform.Rotate(new Vector3 (0, 0, -90));
        if (Intersects())
            transform.Rotate(new Vector3(0, 0, 90));
		MoveCopy();
	}

	public void Left() {
        transform.position = new Vector3(transform.position.x - 1f, transform.position.y);
        if (Intersects())
            Right();
        MoveCopy();
	}

	public void Right() {
        transform.position = new Vector3(transform.position.x + 1f, transform.position.y);
        if (Intersects())
            Left();
        MoveCopy();
	}

	public void Accelerate() {
		velocity = velocity_original * 2f;
	}

	public void UnAccelerate() {
		velocity = velocity_original / 2f;
	}

	public void Drop() {
		velocity = 0f;
        float dist = FloorMeasure();
        transform.position = new Vector3(transform.position.x, transform.position.y - dist + 0.5f);
        //transform.position = new Vector3(transform.position.x, LowestY());
        //Next();
	}

	// Somehow there are errors in the position even though I am only moving +- 1, so I round it to the nearest 1.
	void RoundX() {
        transform.position = new Vector3(Mathf.Round (transform.position.x), transform.position.y);
    }

	// returns true if current piece is intersecting the wall or another piece.
    bool Intersects() {
        foreach (Transform child in transform) {
            if (child.position.x < 0 || child.position.x > 10)
                return true;
            if (manager.grid[RoundWhole(child.position.x), RoundWhole(child.position.y)] != null)
                return true;
            if (manager.grid[RoundWhole(child.position.x), RoundWhole(child.position.y + 0.5f)] != null)
                return true;
        }
        return false;
	}

    // returns the lowest Y coordinate that this piece can achieve in the current orientation and x position.
    int LowestY() {
        List<int> x = new List<int>();
        int minY = 0;
        foreach (Transform child in transform) {
            x.Add(RoundWhole(child.position.x));
        }
        foreach (int coord in x) {
            for (int i = 19; i >= 0; i--) {
                if (manager.grid[coord, i] != null) {
                    i++;
                    if (i > minY)
                        minY = i;
                    break;
                }
            }
        }
        return minY;
    }

    int RoundWhole (float a) {
        return Mathf.RoundToInt(a);
    }

	float RoundHalf(float a) {
        return Mathf.Round(a * 2) / 2f;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (gameObject.tag == "TetriminoActive" && (coll.gameObject.tag == "TetriminoInactive" || coll.gameObject.tag == "Wall")) {
            Next();
		}
	}

    // ceiling contact estop
	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Wall") {
			Die ();
		}
	}

    // returns smallest distance from the current piece to the floor.
    float FloorMeasure()
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < 4; i++)
        {
            list.Add(transform.GetChild(i));
        }
        float minDist = float.MaxValue;
        foreach (Transform child in list)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(child.position, Vector2.down, Mathf.Infinity);
            if (hit[1].collider.transform.parent.gameObject.tag == "TetriminoActive")
            {
                continue;
            }
            if (hit[1])
            {
                if (hit[1].distance < minDist)
                {
                    minDist = hit[1].distance;
                }
            }
            else
            {
                Debug.Log("Did not Hit");
            }
        }
        return minDist;
    }

    // left = -1, right = 1;
    float SideMeasure(int dir)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < 4; i++)
        {
            list.Add(transform.GetChild(i));
        }
        float minDist = float.MaxValue;
        foreach (Transform child in list)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(child.position, dir * Vector2.right, Mathf.Infinity);
            if (hit[1].collider.transform.parent.gameObject.tag == "TetriminoActive")
            {
                continue;
            }
            if (hit[1])
            {
                if (hit[1].distance < minDist)
                {
                    minDist = hit[1].distance;
                }
            }
            else
            {
                Debug.Log("Did not Hit");
            }
        }
        return minDist;
    }

    void Next() {
        velocity = 0f;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        foreach (Transform child in transform)
        {
            int x = RoundWhole(child.position.x);
            int y = RoundWhole(child.position.y);
            print(x + ", " + y);
            manager.grid[x, y] = child;
            child.transform.position = new Vector3(x, y);
            child.tag = "TetriminoInactive";
        }
        Destroy(copy);
        gameObject.tag = "TetriminoInactive";
        if (transform.position.y >= 20)
        {
            Die();
        }
        manager.ClearLines();
        manager.GenerateTetrimino();
    }

	void Die() {
		//manager.Stop();
	}

	public void MoveCopy() {
		copy.transform.rotation = transform.rotation;
        float dist = FloorMeasure();
        copy.transform.position = new Vector3(transform.position.x, transform.position.y - dist + 0.5f);
        //copy.transform.position = new Vector3 (transform.position.x, LowestY());
	}

}
