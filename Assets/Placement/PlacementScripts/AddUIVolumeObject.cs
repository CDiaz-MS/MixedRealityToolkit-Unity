using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddUIVolumeObject : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    public GameObject Prefab
    {
        get => prefab;
        set => prefab = value;
    }

    [SerializeField]
    private BaseInteractiveElement interactiveElement;

    public BaseInteractiveElement InteractiveElement
    {
        get => interactiveElement;
        set => interactiveElement = value;
    }

    private void Start()
    {
        ClickedEvents clickedEvents = InteractiveElement.GetStateEvents<ClickedEvents>("Clicked");

        clickedEvents.OnClicked.AddListener(() =>
        {
            Vector3 position = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y - 0.2f, transform.parent.localPosition.z);

            GameObject cube = Instantiate(prefab, transform.parent);

            cube.transform.localPosition = position;
        });
    }
}
