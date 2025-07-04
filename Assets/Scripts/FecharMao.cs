using System.Collections.Generic;
using UnityEngine;

public class FecharMao : MonoBehaviour
{
    public GameObject MenuIncial;
    private bool MenuAtivado = true;
    private bool menuTravado = false;

    private HashSet<string> dedosEsperados = new HashSet<string> { "Indicador", "Medio", "Anelar", "Mindinho" };
    private HashSet<string> dedosDetectados = new HashSet<string>();

    private void OnTriggerEnter(Collider other)
    {
        if (dedosEsperados.Contains(other.tag))
        {
            dedosDetectados.Add(other.tag);

            if (dedosDetectados.Count == dedosEsperados.Count && !menuTravado)
            {
                MenuAtivado = !MenuAtivado;
                MenuIncial.SetActive(MenuAtivado);
                menuTravado = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (dedosEsperados.Contains(other.tag))
        {
            dedosDetectados.Remove(other.tag);
            menuTravado = false; // libera para nova ativação
        }
    }
}
