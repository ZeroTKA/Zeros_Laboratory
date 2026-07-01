using UnityEngine;

public class Enemy : MonoBehaviour
{
    string name;
    int i = 0;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3(0, 0, 4) * Time.deltaTime;
    }
    private void OnEnable()
    {
        name = gameObject.name;
        switch(name)
        {
            case "":
                break;
        }
    }
    private void OnDisable()
    {
        SpawnManager.Instance.UnregisterSpawn();
    }
}
