using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "KitchenObjectList Data", menuName = "Scriptable Object/KitchenObjectList Data", order = int.MaxValue)]
public class KitchenObjectListSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;


}
