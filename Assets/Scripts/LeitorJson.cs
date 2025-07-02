using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Text;

public class LeitorJsonAvancado : MonoBehaviour
{
    [Header("JSON Input")]
    [TextArea(10, 40)]
    public string jsonInput;

    [Header("Text UI Element")]
    public TextMeshProUGUI textElement;

    void Awake()
    {
        if (textElement == null)
            textElement = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        if (string.IsNullOrWhiteSpace(jsonInput))
        {
            Debug.LogWarning("JSON vazio. Insira um JSON válido.");
            return;
        }

        AtualizarJson(jsonInput);
    }

    public void AtualizarJson(string novoJson)
    {
        try
        {
            JObject json = JObject.Parse(novoJson);
            string formatado = FormatJsonRecursivo(json, 0);

            Debug.Log("JSON Formatado:\n" + formatado);
            if (textElement != null)
                textElement.text = formatado;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao processar JSON: " + e.Message);
        }
    }

    private string FormatJsonRecursivo(JToken token, int indentLevel)
    {
        StringBuilder sb = new StringBuilder();
        string indent = new string(' ', indentLevel * 4);

        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (JProperty prop in token.Children<JProperty>())
                {
                    sb.AppendLine($"{indent}{prop.Name}:");
                    sb.Append(FormatJsonRecursivo(prop.Value, indentLevel + 1));
                }
                break;

            case JTokenType.Array:
                foreach (var item in token.Children())
                {
                    sb.AppendLine($"{indent}-");
                    sb.Append(FormatJsonRecursivo(item, indentLevel + 1));
                }
                break;

            default:
                sb.AppendLine($"{indent}{token.ToString()}");
                break;
        }

        return sb.ToString();
    }
}
