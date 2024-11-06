using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ForecastPos : MonoBehaviour
{
    public float timeCheck = 5f;
    public float force = 10f;
    public Vector3 direction = new Vector3(1, 0, 0);

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        direction = direction.normalized;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Get current velocity and angularVelocity
            Vector3 defaultVelocity = rb.velocity;
            Vector3 defaultAngularVelocity = rb.angularVelocity;

            //Starts the simulation
            Physics.simulationMode = SimulationMode.Script;
            rb.AddForce(direction * force, ForceMode.Impulse);
            Vector3 futurePosition = CheckPosition(rb, defaultVelocity, defaultAngularVelocity);
            print("Future position at " + futurePosition);

            //and move the GameObject for real
            rb.AddForce(direction * force, ForceMode.Impulse);

        }

    }

    Vector3 CheckPosition(Rigidbody defaultRb, Vector3 defaultVelocity, Vector3 defaultAngularVelocity)
    {

        Vector3 defaultPos = defaultRb.position;
        Quaternion defaultRot = defaultRb.rotation;

        float timeInSec = timeCheck;

        while (timeInSec >= Time.fixedDeltaTime)
        {
            timeInSec -= Time.fixedDeltaTime;
            Physics.Simulate(Time.fixedDeltaTime);

        }

        Vector3 futurePos = defaultRb.position;


        Physics.simulationMode = SimulationMode.FixedUpdate;

        //Stop object force
        defaultRb.velocity = Vector3.zero;
        defaultRb.angularVelocity = Vector3.zero;

        //Reset Position
        defaultRb.transform.position = defaultPos;
        defaultRb.transform.rotation = defaultRot;

        //Apply old velocity and angularVelocity
        defaultRb.velocity = defaultVelocity;
        defaultRb.angularVelocity = defaultAngularVelocity;

        return futurePos;
    }
}

