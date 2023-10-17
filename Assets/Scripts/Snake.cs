using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Vector2Int direction = Vector2Int.right;
    public Transform segmentPrefab;
    public Transform tail;
    public int initialSize = 4;
    public int score = 0;

    private List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;

    private void Start()
    {
        ResetState();
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log(score);
        if(direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                direction = Vector2Int.up;
                transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                direction = Vector2Int.down;
                transform.localEulerAngles = new Vector3(0, 0, -90);
            }
        }
        if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                direction = Vector2Int.right;
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                direction = Vector2Int.left;
                transform.localEulerAngles = new Vector3(0, 0, 180);
            }
        }
    }

    private void FixedUpdate()
    {
        if (Time.time < nextUpdate)
        {
            return;
        }

        if (input != Vector2Int.zero)
        {
            direction = input;
        }

        float cur_HEAD_position = transform.position.x;
        Debug.Log("Pozycja_X_Glowy: " + cur_HEAD_position);
        //int changedir = 0;

        for (int i = segments.Count - 1; i > 0; i--) 
        {
            float cur_SEGMENT_X_position = segments[i - 1].position.x ; //to jest [i-2]
            float cur_SEGMENT_Y_position = segments[i - 1].position.y; //to jest [i-2]
            


            if (segments[i].position.x == cur_SEGMENT_X_position - 1.0f)
            {
                //Debug.Log("sneak porusza sie w prawo");
                segments[i].transform.localEulerAngles = new Vector3(0, 0, 0);
                

            }
            else if (segments[i].position.x == cur_SEGMENT_X_position + 1.0f)
            {
                //Debug.Log("sneak porusza sie w lewo");
                segments[i].transform.localEulerAngles = new Vector3(0, 0, 180);
            }

            if (segments[i].position.y == cur_SEGMENT_Y_position - 1.0f)
            {
                if(segments[i].localEulerAngles == Vector3.zero)
                {
                    //Debug.Log("zmiana, segment:" + i );
                }

                //Debug.Log("sneak porusza sie w gore");
                segments[i].transform.localEulerAngles = new Vector3(0, 0, 90);  
            }
            else if (segments[i].position.y == cur_SEGMENT_Y_position + 1.0f)
            {
                //Debug.Log("sneak porusza sie w dol");
                segments[i].transform.localEulerAngles = new Vector3(0, 0, -90);
            }

            segments[i].position = segments[i-1].position;

        }

        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

    }


    private void InitialGrow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    private void Grow()
    {
        Destroy(segments[segments.Count - 1].gameObject);
        segments.RemoveAt(segments.Count - 1);

        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position; 
        segments.Add(segment);

        addTail();
    }

    private void addTail()
    {
        Transform segment = Instantiate(tail);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    private void ResetState()
    {
        score = 0;
        direction = Vector2Int.right;
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.position = Vector3.zero;

        for (int i = 1; i < segments.Count; i++) 
        {
            Destroy(segments[i].gameObject);
        }

        segments.Clear();
        segments.Add(this.transform);

        for (int j = 0; j < initialSize - 1; j++)
        {
            InitialGrow();
        }
        addTail();
    }
    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }

        return false;
    }
    private void AddScore(int points)
    {
	    score += points;
	    Debug.Log("Dodano punkt, Aktualny wynik:" + score);	
    }
private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
            AddScore(1);
        } 
        else if (other.tag =="Bounce")
        {
            ResetState();
        }
    }
}
