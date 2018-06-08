using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetriminoManager : MonoBehaviour {

	public float velocity;
	public GameObject copy;

    private ClassicModeManager manager;
	private float velocity_original;

	private void Awake() {
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

    /// <summary>
    /// Rotates this tetrimino clockwise 90 degrees.
    /// </summary>
	public void Rotate() {
		transform.Rotate(new Vector3 (0, 0, -90));
        if (Intersects())
            transform.Rotate(new Vector3(0, 0, 90));
		MoveCopy();
	}

    /// <summary>
    /// Moves this tetrimino to the right 1 unit.
    /// </summary>
	public void Left() {
        transform.position = new Vector3(transform.position.x - 1f, transform.position.y);
        if (Intersects())
            transform.position = new Vector3(transform.position.x + 1f, transform.position.y);
        MoveCopy();
	}

    /// <summary>
    /// Moves this tetrimino to the left 1 unit.
    /// </summary>
	public void Right() {
        transform.position = new Vector3(transform.position.x + 1f, transform.position.y);
        if (Intersects())
            transform.position = new Vector3(transform.position.x - 1f, transform.position.y);
        MoveCopy();
	}

    /// <summary>
    /// Triples the rate at which this tetrimino moves downward.
    /// </summary>
	public void Accelerate() {
		velocity = velocity_original * 3f;
	}

    /// <summary>
    /// Reverts the velocity of the tetrimino to its starting velocity.
    /// </summary>
	public void UnAccelerate() {
		velocity = velocity_original;
	}

    /// <summary>
    /// Drops this tetrimino to the lowest position possible in the given orientation and horizontal position.
    /// </summary>
	public void Drop() {
		velocity = 0f;
        float dist = FloorMeasure();
        transform.position = new Vector3(transform.position.x, RoundHalf(transform.position.y - dist + 0.5f));
        Next();
	}

	/// <summary>
    /// Rounds this tetrimino's horizontal position to the nearest integer.
    /// </summary>
	void RoundX() {
        transform.position = new Vector3(Mathf.Round (transform.position.x), transform.position.y);
    }

	/// <summary>
    /// Returns true if current piece is intersecting the wall or another piece.
    /// </summary>
    bool Intersects() {
        foreach (Transform child in transform) {
            if (child.position.x < -0.5 || child.position.x > 9.5)
                return true;
            if (child.position.y < 19) {
                if (manager.grid[RoundWhole(child.position.x), RoundWhole(child.position.y)] != null)
                    return true;
                if (manager.grid[RoundWhole(child.position.x), RoundWhole(child.position.y + 0.5f)] != null)
                    return true;
            }
        }
        return false;
	}

    /// <summary>
    /// Calculate the lowest Y coordinate that this piece can achieve in the current orientation and x position.
    /// </summary>
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

    /// <summary>
    /// Rounds a float to the nearest integer. Returns an int.
    /// </summary>
    int RoundWhole (float a) {
        return Mathf.RoundToInt(a);

    }

    /// <summary>
    /// Rounds a float to the nearest 0.5f. Returns a float.
    /// </summary>
	float RoundHalf(float a) {
        return Mathf.Round(a * 2) / 2f;
	}

    // If collision is detected and is with an inactive piece or the floor, call Next() to clear lines and resume gameplay.
	void OnCollisionEnter2D(Collision2D coll) {
		if (gameObject.tag == "TetriminoActive" && (coll.gameObject.tag == "TetriminoInactive" || coll.gameObject.tag == "Wall")) {
            Next();
		}
        // If this piece is inactive and collides with an active piece, then make sure the piece doesnt get pushed in any way.
        else if (gameObject.tag == "TetriminoInactive") {
            foreach (Transform child in transform) {
                child.position = new Vector3(RoundWhole(child.position.x), RoundWhole(child.position.y));
            }
        }
	}

    // If piece contacts ceiling, game over.
	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Wall") {
			Die ();
		}
	}

    /// <summary>
    /// Returns smallest distance from the current piece to the floor.
    /// </summary>
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

    /// <summary>
    /// Clears the line, makes this tetrimino inactive, and generate the next tetrimino.
    /// </summary>
    void Next() {
        velocity = 0f;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        foreach (Transform child in transform)
        {
            int x = RoundWhole(child.position.x);
            int y = RoundWhole(child.position.y);
            if (y >= 20) {
                Die();
                break;
            }
            manager.grid[x, y] = child;
            child.transform.position = new Vector3(x, y);
            child.tag = "TetriminoInactive";
        }
        Destroy(copy);
        gameObject.tag = "TetriminoInactive";
        manager.ClearLines();
        manager.GenerateTetrimino();
    }

    /// <summary>
    /// Ends the current game.
    /// </summary>
	void Die() {
		manager.Stop();
	}

    /// <summary>
    /// Update the location of the preview copy.
    /// </summary>
	public void MoveCopy() {
		copy.transform.rotation = transform.rotation;
        float dist = FloorMeasure();
        copy.transform.position = new Vector3(transform.position.x, RoundHalf(transform.position.y - dist + 0.5f));
	}

}
