using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovementScript : MonoBehaviour
{
    public GameObject turret;
    private NavMeshAgent navMesh;
    private Vector3 mousePos;
    void Start()
    {
        //Agafem el component NavMeshAgent del objecte d'aquest scrpit
        navMesh = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        Movement();
        Aiming();
    }

    void Movement()
    {
        //Quan cliques el mouse dret, fer que l'agent es mogui cap a on has clicat
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(camRay, out hit, 100))
            {
                navMesh.destination = hit.point;
            }
        }
    }

    void Aiming()
    {
        mousePos = Input.mousePosition;
        var turretGlobalPos = Camera.main.WorldToScreenPoint(turret.transform.position);
        var angle = Mathf.Atan2(mousePos.y - turretGlobalPos.y, mousePos.x - turretGlobalPos.x) * Mathf.Rad2Deg;
        turret.transform.rotation = Quaternion.Euler(new Vector3(0, -angle+180, 0));
    }
}
