using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassicModeManager : MonoBehaviour {

	public float speed;
	public float acceleration;

	public GameObject[] tetriminos = new GameObject[7];
	public Transform[,] grid = new Transform[10, 20];
    public Queue<int> queue = new Queue<int>();
    public GameObject hold;
    public bool held; // true if hold was pressed already while current piece is live. Gets reset when next piece is spawned.

    public bool isPaused;
    public int score;
    public Text scoreText;

	// Use this for initialization
	void Start () {
        // Generate first 3 numbers in tetrimino queue
        for (int i = 0; i < 3; i++) {
            queue.Enqueue((int)Random.Range(0f, 7f));
        }
        GenerateTetrimino();
	}
	
	// Update is called once per frame
	void Update () {
		speed = (speed + acceleration * Time.deltaTime);
    }

    // Instantiates and prepares requested tetrimino.
	public void GenerateTetrimino() {
        if (!isPaused) {
            // Tetrimino Queue stuff
            int chooser = queue.Dequeue();
            queue.Enqueue((int)Random.Range(0f, 7f));
            UpdateQueue();
			// Prep tetrimino
			GameObject tetrimino = (GameObject) Instantiate (tetriminos [chooser]);
			TetriminoManager manager = tetrimino.GetComponent<TetriminoManager> ();
			manager.velocity = speed;
            CreateCopy(tetrimino);
			// Prep input
			GameObject inputManager = GameObject.FindGameObjectWithTag ("InputManager");
			inputManager.GetComponent<InputManager>().GetNewActiveTetrimino();
            held = false;
		}
		scoreText.text = "Score: " + score;
	}

    void CreateCopy(GameObject g) {
        // Prep copy
        GameObject copy = (GameObject)Instantiate(g);
        copy.tag = "TetriminoCopy";
        Destroy(copy.GetComponent<Rigidbody2D>());
        Destroy(copy.GetComponent<TetriminoManager>());
        for (int i = 0; i < 4; i++)
        {
            GameObject child = copy.transform.GetChild(i).gameObject;
            Destroy(child.GetComponent<BoxCollider2D>());
            child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f);
        }
        TetriminoManager manager = g.GetComponent<TetriminoManager>();
        manager.copy = copy;
        manager.MoveCopy();
    }

	public void ClearLines() {
		int count = 0;
		for (int i = 0; i < 20; i++) {
			List<Transform> line = GetLine (i);
			if (line.Count == 10) {
				Clear (line, i);
				i--;
				count++;
			}
		}
        score += (int) Mathf.Pow(2, count);
        UpdateGrid();
	}

	// 0 is bottom, 19 is top.
	void Clear(List<Transform> line, int row) {
		// Remove squares in line
		foreach (Transform square in line) {
			Destroy (square.gameObject);
		}
		// Shift all lines above down
		for (int i = row + 1; i < 20; i++) {
			List<Transform> l = GetLine (i);
			foreach (Transform s in l) {
				s.position = new Vector3 (s.position.x, s.position.y - 1f);
			}
		}
	}

	//
	List<Transform> GetLine(int row) {
		Transform child = transform.GetChild (row);
		RaycastHit2D[] hit = Physics2D.RaycastAll (child.position, Vector2.right, Mathf.Infinity);
		List<Transform> ret = new List<Transform>();
		foreach (RaycastHit2D h in hit) {
			ret.Add (h.collider.transform);
		}
		ret.RemoveAt (0);
		ret.RemoveAt (ret.Count - 1);
		return ret;
	}

    void UpdateGrid() {
        Transform[,] g = new Transform[10, 20];
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("TetriminoInactive")) {
            int x = Mathf.RoundToInt(t.transform.position.x);
            int y = Mathf.RoundToInt(t.transform.position.y);
            if (y >= 20) {
                Stop();
                break;
            }
            g[x, y] = t.transform;
        }
        grid = g;
    }

    void UpdateQueue() {
        GameObject q = GameObject.FindWithTag("Queue");
        int[] arr = queue.ToArray();
        for (int i = 0; i < 3; i++) {
            Destroy(q.transform.GetChild(i).gameObject);

            GameObject temp = Instantiate(tetriminos[arr[i]]);
            temp.tag = "Untagged";
            temp.transform.parent = q.transform;
            temp.transform.localPosition = new Vector3(0, -5 * i);
            Destroy(temp.GetComponent<Rigidbody2D>());
            Destroy(temp.GetComponent<TetriminoManager>());
            foreach (Transform t in temp.transform) {
                Destroy(t.GetComponent<BoxCollider2D>());
            }
        }
    }

    // Hold pos = (-10, 15), default pos = (5, 21.5)
    public void Hold() {
        if (!held) {
            GameObject active = GameObject.FindGameObjectWithTag("TetriminoActive");
            if (hold == null) {
                
                active.tag = "Hold";
                active.transform.position = new Vector3(-10, 15);
                active.GetComponent<TetriminoManager>().velocity = 0;
                Destroy(active.GetComponent<TetriminoManager>().copy);

                hold = active;
                GenerateTetrimino();
            }
            else {
                
                active.tag = "Hold";
                active.transform.position = new Vector3(-10, 15);
                active.GetComponent<TetriminoManager>().velocity = 0;
                Destroy(active.GetComponent<TetriminoManager>().copy);

                hold.tag = "TetriminoActive";
                hold.transform.position = new Vector3(5, 21.5f);
                hold.GetComponent<TetriminoManager>().velocity = speed;
                CreateCopy(hold);

                hold = active;
                GameObject inputManager = GameObject.FindGameObjectWithTag("InputManager");
                inputManager.GetComponent<InputManager>().GetNewActiveTetrimino();
            }
        }
        held = true;
    }

	public void Stop() {
		isPaused = true;
	}

}
