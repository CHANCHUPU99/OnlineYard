using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{

    [SerializeField]string _name;
    [TextArea(5, 6)] public string dialog;
}
