using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperBehaviour : MonoBehaviour
{
    public Transform waypoint1;
    public Transform wayPoint2;
    //public LayerMask layerEnemigos;
    private Transform currentWayPoint;
    public float speed = 10;
    public float distMin = 2;
    private float minDistanceSqr;
    private bool rotate;
    public float rotationTime;
    public bool chasing;
    private bool falling;
    private GameObject player;
    private GameManager manager;
    Vector3 chomperCenter;

    private ChomperAnimation chompAnimation;
    // Start is called before the first frame update
    void Start()
    {
        chomperCenter = new Vector3(transform.position.x,transform.position.y+0.4f, transform.position.z);
        player = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        //Para que empiece dirigiendose al punto 2 por ejemplo:
        currentWayPoint = wayPoint2;


        chompAnimation = GetComponent<ChomperAnimation>();
        minDistanceSqr = distMin * distMin;
    }

    // Update is called once per frame
    void Update()
    {
        rayCastFunction();

        if (chasing)
        {
            currentWayPoint = player.transform;
            StartCoroutine(Rotate());
        }
        if (!falling)
        {
            Vector3 direction = currentWayPoint.position - transform.position;
            direction.Normalize();
            //Debug.Log(currentWayPoint.name);
            transform.position += speed * direction * Time.deltaTime;
            chompAnimation.Updatefordward(speed / 2);
            checkArrived();
        }
        else {
            Vector3 direction = Vector3.down;
            direction.Normalize();
            //Debug.Log(currentWayPoint.name);
            transform.position += speed*4 * direction * Time.deltaTime;
        }

        
    }

    void checkArrived()
    {
        
        float remDist = Vector3.SqrMagnitude(currentWayPoint.position - transform.position);

        if (remDist < minDistanceSqr)
        {
            if (currentWayPoint == waypoint1)
            {
                currentWayPoint = wayPoint2;

            }
            else
            {
                currentWayPoint = waypoint1;
            }
            StartCoroutine(Rotate());

        }
    }
    private void OnTriggerEnter(Collider other)//Si pilla al jugador respawnea, corrige su rotación y deja de perseguirlo.
    {
        if (other.tag == "Player") {
            StopCoroutine(Chase());
            
            chasing = false; 
            
            
            currentWayPoint = wayPoint2;
            StartCoroutine(Rotate());
            manager.RespawnPlayer();

        }
    }

    void rayCastFunction()
    {

        
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Vector3 down = transform.TransformDirection(Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, down, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "KillZone"/*gameObject.layer != 9*/) {
                Debug.DrawRay(transform.position, hit.transform.position, Color.blue);
                Debug.Log("DeathZone");
                falling = true;
            }
        }
        if (Physics.Raycast(chomperCenter, fwd, out hit, 10)) {
        Debug.DrawRay(transform.position, hit.transform.position, Color.red);

        if (hit.transform.tag == "Player")
        {
            currentWayPoint = player.transform;
            //checkArrived();
            StartCoroutine(Rotate());

            StartCoroutine(Chase());
        }
        }
    }
    IEnumerator Chase() {
        
        chasing = true;
        yield return new WaitForSeconds(3);
        chasing = false;
        currentWayPoint = waypoint1;
        StartCoroutine(Rotate());
        
    }

    IEnumerator Rotate()
    {
        float time = 0;
        rotate = true;
        Quaternion newRotation = Quaternion.LookRotation(currentWayPoint.position - transform.position);//Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * realAngle);
        Quaternion originalRotation = transform.rotation;
        while (time < rotationTime)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(originalRotation, newRotation, time / rotationTime);

            yield return new WaitForEndOfFrame();
        }
        rotate = false;
    }
}
