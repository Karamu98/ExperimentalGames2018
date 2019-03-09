using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[ExecuteInEditMode]
public class CS_Spawnpoints : MonoBehaviour
{
    [SerializeField] List<GameObject> points;

    private int iPreviousAmount;

    // Update is called once per frame
    void Update()
    {
        if (iPreviousAmount < transform.childCount)
        {
            if (transform.childCount == 0)
            {
                points.Clear();
            }
            else
            {
                points.Clear();
                for (int i = 0; i < transform.childCount; i++)
                {
                    points.Add(transform.GetChild(i).gameObject);
                }
            }
        }
        else if (iPreviousAmount > transform.childCount)
        {
            // Find the one deleted, remove from list
            for (int i = 0; i < transform.childCount; i++)
            {
                if (points[i] == null)
                {
                    points.RemoveAt(i);
                }
                points.TrimExcess();
            }
        }

        iPreviousAmount = transform.childCount;
    }

    private void Awake()
    {

    }


    public Vector3 GetValidSpawnPoint()
    {
        Vector3 Position = new Vector3(0, 0, 0);

        for (int i = 0; i < transform.childCount; ++i)
        {
            if (Position.magnitude < points[i].transform.position.magnitude)
            {
                Position = points[i].transform.position;
            }
        }

        return Position;
    }
}
