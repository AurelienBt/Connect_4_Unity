using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouvement : MonoBehaviour
{
    [SerializeField] // permet d'acc�der aux variables dans l'Inspector de Unity
    float speed = 10f;
    public Vector3 targetPostion;

    // Fonction appel�e � chaque frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPostion, step);
    }
}
