using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectso;
    [SerializeField] private Transform counterTopPoint;

    public void Interact()
    {
        Transform kitchenTransform = Instantiate(kitchenObjectso.prefab, counterTopPoint);
        kitchenTransform.localPosition = Vector3.zero;
    }
}
