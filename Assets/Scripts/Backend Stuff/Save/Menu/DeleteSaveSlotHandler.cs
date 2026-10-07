using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSaveSlotHandler : MonoBehaviour
{
    [HideInInspector] public List<GameObject> DeleteSlotObjs = new List<GameObject>();
    private DeleteSaveSlot[] _deleteSlots;

    private void Awake()
    {
        _deleteSlots = GetComponentsInChildren<DeleteSaveSlot>();

        for (int i = 0; i < _deleteSlots.Length; i++)
        {
            DeleteSlotObjs.Add(_deleteSlots[i].gameObject);
        }
    }
}
