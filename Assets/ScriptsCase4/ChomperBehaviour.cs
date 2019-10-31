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

    public Transform eyes;
    Vector3 chomperCenter;

    private ChomperAnimation chompAnimation;
    // Start is called before the first frame update
    void Start()
    {
        //chomperCenter = new Vector3(transform.position.x,transform.position.y+0.4f, transform.position.z);
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
        if (!falling)
        {
            if (chasing)
            {
            //Vector3 posPersonaje = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            currentWayPoint = player.transform/*.position = /*posPersonaje*/;
            StartCoroutine(Rotate());
            }
           
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
            else if(currentWayPoint == wayPoint2)
            {
                currentWayPoint = waypoint1;
            }
            StartCoroutine(Rotate());

        }
    }
    private IEnumerator OnTriggerEnter(Collider other)//Si pilla al jugador respawnea, corrige su rotación y deja de perseguirlo.
    {
        if (other.tag == "Player") {
            
            chasing = false;
            chompAnimation.Attack();
            
            
            yield return new WaitForSeconds(1.1f);
            manager.RespawnPlayer();
            //gameObject.GetComponent<Collider>().enabled = true;
            currentWayPoint = waypoint1;
            StartCoroutine(Rotate());
            
            
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
                //Debug.Log("DeathZone");
                falling = true;
            }
        }
        Debug.DrawRay(eyes.position, fwd, Color.red);
        if (Physics.Raycast(eyes.position, fwd, out hit, 10)) {
        

            if (hit.transform.tag == "Player")
            {
                currentWayPoint = player.transform;
                //checkArrived();
                StartCoroutine(Rotate());
                chasing = true;
            }
            else if(chasing) {
                chasing = false;
                currentWayPoint = waypoint1;
                StartCoroutine(Rotate());
            }
        }
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
