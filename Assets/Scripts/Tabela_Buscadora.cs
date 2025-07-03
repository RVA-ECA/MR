using System.Collections.Generic;
using OVRSimpleJSON;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tabela_Buscadora : MonoBehaviour
{

    private TextMeshProUGUI texto; // Referência ao componente TMP

    [Header("Arquivo JSON (array de objetos)")]
    public TextAsset arquivoJson;

    [Header("Tamanho da célula")]
    public Vector2 tamanhoCelula = new Vector2(300, 80);

    [Header("Tamanho máximo da Tabela (ScrollView)")]
    public Vector2 tamanhoMaximoTabela = new Vector2(1600, 800);

    [Header("Tamanho da fonte")]
    public int tamanhoFonte = 20;

    [Header("Cores")]
    public Color corBorda = Color.black;
    public Color corTexto = Color.white;
    public Color corFundoTabela = new Color(0.1f, 0.1f, 0.1f);

    [Header("Cabeçalhos (chaves do JSON)")]
    public string[] cabecalhos;

    void Start()
    {
        
        // Pegar automaticamente o componente TextMeshPro do GameObject
        texto = GetComponent<TextMeshProUGUI>();

        // Verificar se o componente TextMeshPro foi encontrado e então obter suas propriedades
        if (texto != null)
        {
            string fontName = texto.font.name;
            float fontSize = texto.fontSize;
            bool autoSize = texto.enableAutoSizing;
            Vector4 color = texto.color;
            TextAlignmentOptions alignment = texto.alignment;
            bool wrappingEnabled = texto.enableWordWrapping;
        }
        else
        {
            Debug.LogError("Componente TextMeshPro não encontrado!");
        }

        if (arquivoJson == null || cabecalhos == null || cabecalhos.Length == 0)
        {
            Debug.LogError("JSON ou cabeçalhos não atribuídos.");
            return;
        }

        JSONNode root;
        try
        {
            root = JSON.Parse(arquivoJson.text);
            //Debug.Log(root.ToString(2)); // Log para debug
        }
        catch
        {
            Debug.LogError("Erro ao fazer parse do JSON.");
            return;
        }

        if (root == null || !root.IsArray)
        {
            Debug.LogError("JSON inválido: deve ser um array de objetos.");
            return;
        }

        var linhasJson = root.AsArray;
        //Debug.Log($"Linhas encontradas: {linhasJson.Count}");
        int colunas = cabecalhos.Length;
        //Debug.Log($"Colunas definidas: {colunas}");
        int linhas = linhasJson.Count + 1;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // ScrollView vertical principal
        GameObject scrollGO = new GameObject("ScrollView", typeof(RectTransform), typeof(Image), typeof(ScrollRect), typeof(Mask));
        scrollGO.transform.SetParent(canvas.transform, false);
        var scrollRT = scrollGO.GetComponent<RectTransform>();
        scrollRT.sizeDelta = tamanhoMaximoTabela;
        scrollGO.GetComponent<Image>().color = corTexto;
        scrollGO.GetComponent<Mask>().showMaskGraphic = false;

        // Content da ScrollView
        GameObject contentGO = new GameObject("Content", typeof(RectTransform), typeof(GridLayoutGroup));
        contentGO.transform.SetParent(scrollGO.transform, false);
        var contentRT = contentGO.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(0, 1);
        contentRT.pivot = new Vector2(0, 1);
        contentRT.anchoredPosition = Vector2.zero;

        // Scroll settings
        var scroll = scrollGO.GetComponent<ScrollRect>();
        scroll.content = contentRT;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.viewport = scrollRT;

        // Grid para organização das células
        var grid = contentGO.GetComponent<GridLayoutGroup>();
        grid.cellSize = tamanhoCelula;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = colunas;
        grid.spacing = new Vector2(1, 1);

        contentGO.AddComponent<Image>().color = corFundoTabela;

        // Cabeçalho
        foreach (var key in cabecalhos)
        {
            var cel = CriarCelula(key.ToUpper(), true);
            cel.transform.SetParent(contentGO.transform, false);
        }

        // Dados
        foreach (JSONNode node in linhasJson)
        {
            string linhaCompleta = "";
            foreach (var header in cabecalhos)
            {
                string chave = header;
                string valor = node[chave] != null ? node[chave].ToString().Trim('"') : "";
                linhaCompleta += $"{chave}: {valor} | ";

                var cel = CriarCelula(valor, false);
                cel.transform.SetParent(contentGO.transform, false);
            }

            // Exibe toda a linha no console
            Debug.Log($"Linha JSON -> {linhaCompleta}");
        }
    }

    GameObject CriarCelula(string texto, bool isHeader)
    {

        // Cria GameObject para célula
        Debug.Log($"Criando célula: {texto} (Header: {isHeader})");
        var cel = new GameObject("Celula", typeof(RectTransform), typeof(Image));
        var img = cel.GetComponent<Image>();
        img.color = isHeader ? corBorda : corBorda * 0.8f;

        if (!isHeader)
        {
            // Scroll horizontal para célula de dados
            var scroll = cel.AddComponent<ScrollRect>();
            scroll.horizontal = true;
            scroll.vertical = false;

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewport.transform.SetParent(cel.transform, false);
            var rtViewport = viewport.GetComponent<RectTransform>();
            rtViewport.anchorMin = Vector2.zero;
            rtViewport.anchorMax = Vector2.one;
            rtViewport.offsetMin = Vector2.zero;
            rtViewport.offsetMax = Vector2.zero;
            viewport.GetComponent<Image>().color = corTexto;
            viewport.GetComponent<Mask>().showMaskGraphic = false;
            scroll.viewport = rtViewport;

            var content = new GameObject("ContentTexto", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            var rtContent = content.GetComponent<RectTransform>();
            rtContent.anchorMin = new Vector2(0, 0);
            rtContent.anchorMax = new Vector2(0, 1);
            rtContent.pivot = new Vector2(0, 0.5f);
            rtContent.anchoredPosition = Vector2.zero;
            scroll.content = rtContent;

            var txt = CriarTexto(texto, false);
            txt.transform.SetParent(rtContent, false);

            // Ajusta largura conforme conteúdo
            float larguraTexto = Mathf.Max(txt.preferredWidth + 40, tamanhoCelula.x);
            rtContent.sizeDelta = new Vector2(larguraTexto, tamanhoCelula.y);
        }
        else
        {
            var txt = CriarTexto(texto, true);
            txt.transform.SetParent(cel.transform, false);
        }

        return cel;
    }

    TextMeshProUGUI CriarTexto(string texto, bool isHeader)
    {
        var txtObj = new GameObject("Texto", typeof(TextMeshProUGUI));
        var txt = txtObj.GetComponent<TextMeshProUGUI>();
        int tamanhoFonteH = (int)(tamanhoFonte * 1.1); // Aumenta um pouco o tamanho da fonte do cabeçalho
        txt.text = texto;
        txt.color = corTexto;
        txt.fontSize = isHeader ? tamanhoFonteH : tamanhoFonte;
        txt.alignment = TextAlignmentOptions.Left;
        txt.enableAutoSizing = false;

        var rtTxt = txt.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.offsetMin = Vector2.zero;
        rtTxt.offsetMax = Vector2.zero;

        return txt;
    }
}
