using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectso;
    public event EventHandler OnPlayerGrabbedObject;

    public override void Interact(Player player)
    {
        Transform kitchenTransform = Instantiate(kitchenObjectso.prefab);
        kitchenTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);

    }

}
