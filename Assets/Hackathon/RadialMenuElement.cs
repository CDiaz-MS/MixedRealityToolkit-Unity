using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuElement : MonoBehaviour
{
    public Transform menuElementOrigin;
    public Vector3 targetLocation;

    [SerializeField]
    private TransitionState state;
    //float threshold = 0.005f;

    public LineRenderer connector;


    private void Start()
    {
        targetLocation = this.transform.position;
        connector = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(menuElementOrigin.position, targetLocation);

        switch (state)
        {
            case TransitionState.emitting:
                transform.position = Vector3.MoveTowards(transform.position, targetLocation, distance * Time.deltaTime);
                break;
            case TransitionState.stationary:
                break;
            case TransitionState.retracting:
                transform.position = Vector3.MoveTowards(transform.position, menuElementOrigin.position, 1.0f * Time.deltaTime);
                break;
        }

        connector.SetPositions(new Vector3[] { transform.position, menuElementOrigin.position});
    }

    public void Emit()
    {
        state = TransitionState.emitting;
    }

    public void Retract()
    {
        state = TransitionState.retracting;
    }
}

public enum TransitionState
{
    emitting,
    stationary,
    retracting
}
