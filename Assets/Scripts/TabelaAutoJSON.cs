using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TabelaAutoJSON : MonoBehaviour
{
    [Header("Arquivo JSON")]
    public TextAsset arquivoJson;

    [Header("Tamanho da célula")]
    public Vector2 tamanhoCelula = new Vector2(320, 80);

    [Header("Cores")]
    public Color corBorda = Color.white;
    public Color corTexto = Color.black;
    public Color corFundoTabela = new Color(0.1f, 0.1f, 0.1f);

    [Serializable]
    public class Linha
    {
        public List<string> valores;
    }

    [Serializable]
    public class Tabela
    {
        public List<Linha> linhas;
    }

    void Start()
    {
        if (arquivoJson == null)
        {
            Debug.LogError("JSON não atribuído.");
            return;
        }

        Tabela tabela = JsonUtility.FromJson<Tabela>(arquivoJson.text);
        if (tabela == null || tabela.linhas == null || tabela.linhas.Count == 0)
        {
            Debug.LogError("JSON inválido ou vazio.");
            return;
        }

        int colunas = tabela.linhas[0].valores.Count;
        int linhas = tabela.linhas.Count;

        // Cria Canvas se não houver
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // Cria painel com Grid
        GameObject painel = new GameObject("Tabela", typeof(RectTransform), typeof(Image), typeof(GridLayoutGroup));
        painel.transform.SetParent(canvas.transform, false);

        RectTransform rt = painel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(tamanhoCelula.x * colunas + 10, tamanhoCelula.y * linhas + 10);

        painel.GetComponent<Image>().color = corFundoTabela;

        GridLayoutGroup grid = painel.GetComponent<GridLayoutGroup>();
        grid.cellSize = tamanhoCelula;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = colunas;
        grid.spacing = new Vector2(1, 1);

        // Gera células
        foreach (var linha in tabela.linhas)
        {
            foreach (var valor in linha.valores)
            {
                GameObject celula = CriarCelula(valor);
                celula.transform.SetParent(painel.transform, false);
            }
        }
    }

    GameObject CriarCelula(string texto)
    {
        GameObject cel = new GameObject("Celula", typeof(RectTransform), typeof(Image));
        cel.GetComponent<Image>().color = corBorda;

        GameObject txtObj = new GameObject("Texto", typeof(TextMeshProUGUI));
        txtObj.transform.SetParent(cel.transform, false);

        TextMeshProUGUI txt = txtObj.GetComponent<TextMeshProUGUI>();
        txt.text = texto;
        txt.color = corTexto;
        txt.fontSize = 20;
        txt.alignment = TextAlignmentOptions.Center;
        txt.enableAutoSizing = true;

        RectTransform rtTxt = txtObj.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.offsetMin = Vector2.zero;
        rtTxt.offsetMax = Vector2.zero;

        return cel;
    }
}
