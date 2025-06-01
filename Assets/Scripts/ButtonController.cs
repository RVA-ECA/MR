using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static OVRHaptics;

[System.Serializable]
public class ButtonConfig
{
    //public GameObject Alvo;                // Refer�ncia ao GameObject alvo (opcional)
    //public bool ComecarAtivado;
    public Button buttonUI;                         // Refer�ncia ao componente Button
    public InputActionProperty acaoDoInput;         // Tecla ou bot�o do controle
}

public class ButtonController : MonoBehaviour
{
    public ButtonConfig[] buttons;

    void OnEnable()
    {
        foreach (var config in buttons)
            config.acaoDoInput.action.Enable();
    }

    void OnDisable()
    {
        foreach (var config in buttons)
            config.acaoDoInput.action.Disable();
    }
    void Update()
    {

        if (InputUtils.EstaEmCampoDeTexto())
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Atalho R executado");
        }

        foreach (var config in buttons)
        {
            var action = config.acaoDoInput.action;
            if (action.IsPressed())
            {
                // Simula hover pressionado visualmente (opcional)
                var colors = config.buttonUI.colors;
                config.buttonUI.targetGraphic.color = colors.pressedColor;
            }

            if (action.WasReleasedThisFrame())
            {
                // Restaura cor normal
                var colors = config.buttonUI.colors;
                config.buttonUI.targetGraphic.color = colors.normalColor;

                // Executa a��o do bot�o
                config.buttonUI.onClick.Invoke();
            }
        }
    }
}
