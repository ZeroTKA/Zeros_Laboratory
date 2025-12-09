using UnityEngine;

public class EnemyTemp : MonoBehaviour
{
    Transform movePoint; // delete this. It's only for testing.
    [SerializeField] float speed;


    private void Start()
    {
        movePoint = GameObject.Find("Move Point").transform;
        if (movePoint == null )
        {
            Debug.LogError("Move Point doesn't exist for some reason. What.");
        }
    }

    private void Update()
    {
        Move();
    }

    // -- Testing Purposes. Delete! -- //
    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
    }


}
