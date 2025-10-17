using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define un contrato para cualquier clase que sea capaz de leer y procesar la información
/// de diálogo de un NPC.
/// </summary>
public interface INPCReader
{
    void ReadNPCInfo(NPCDialog npc);
}
