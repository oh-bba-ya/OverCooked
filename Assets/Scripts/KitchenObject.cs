using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectso;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectso;
    }
}
