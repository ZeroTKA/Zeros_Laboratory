using System.Collections.Generic;
using UnityEngine;

public class TripodAttack : MonoBehaviour
{
    private List<GameObject> enemyList = new();
    [SerializeField] float fireRate = .3f;
    private float lastTimeFired = 0;
    private float currentTime = 0;
    private GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyList.Add(other.gameObject);
        }
    }
    private void Update()
    {
        if (enemyList.Count > 0)
        {
            Shoot();
        }
    }
    private void Shoot()
    {
        currentTime = Time.time;
        if (currentTime - lastTimeFired >= fireRate)
        {
            target = enemyList[0];
            Rotate(target);
            PoolManager.Instance.PutBack(target);
            enemyList.RemoveAt(0);
            lastTimeFired = currentTime;
        }
    }
    void Rotate(GameObject target)
    {
        Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.LookAt(targetPos);
    }
}
