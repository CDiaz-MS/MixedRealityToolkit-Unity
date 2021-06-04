using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuElement : MonoBehaviour
{
    public Transform menuElementOrigin;
    public Vector3 targetLocation;

    public TransitionState state;
    float threshold = 0.005f;

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(menuElementOrigin.position, targetLocation);

        switch (state)
        {
            case TransitionState.emitting:
                transform.position = Vector3.MoveTowards(transform.position, targetLocation, distance * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetLocation) < threshold)
                    state = TransitionState.stationary;
                break;
            case TransitionState.stationary:
                break;
            case TransitionState.retracting:
                transform.position = Vector3.MoveTowards(transform.position, menuElementOrigin.position, 1.0f * Time.deltaTime);
                if (Vector3.Distance(transform.position, menuElementOrigin.position) < threshold)
                    state = TransitionState.stationary;
                break;
        }
    }
}

public enum TransitionState
{
    emitting,
    stationary,
    retracting
}
