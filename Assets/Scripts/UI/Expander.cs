// COGARCH COGARCH_UI Expander.cs
// Copyright © Jasper Ermatinger

#region usings

using Assets.Scripts;
using TMPro;
using Unity_Tools.Core;
using UnityEngine;
using UnityEngine.UI;

#endregion

/// <summary>
///     The expander.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class Expander : MonoBehaviour
{
    /// <summary>
    ///     The update interval.
    /// </summary>
    private const int UpdateInterval = 15;

    /// <summary>
    ///     The content.
    /// </summary>
    public GameObject Content;

    /// <summary>
    ///     The content background.
    /// </summary>
    public Sprite ContentBackground;

    /// <summary>
    ///     The expanded.
    /// </summary>
    public bool Expanded = true;

    /// <summary>
    ///     The header.
    /// </summary>
    public GameObject Header;

    /// <summary>
    ///     The header background.
    /// </summary>
    public Sprite HeaderBackground;

    /// <summary>
    ///     The header height.
    /// </summary>
    public float HeaderHeight = 50f;

    /// <summary>
    ///     The header text.
    /// </summary>
    public string HeaderText = "Header";

    /// <summary>
    ///     The update counter.
    /// </summary>
    private int updateCounter;

    /// <summary>
    ///     The on button click.
    /// </summary>
    private void OnButtonClick()
    {
        Expanded = !Expanded;

        if (Content != null)
        {
            Content.SetActive(Expanded);
        }
    }

    /// <summary>
    ///     The setup content.
    /// </summary>
    private void SetupContent()
    {
        Content = gameObject.GetOrCreateChild("Content");
        var contentTransform = Content.GetOrAddComponent<RectTransform>();
        contentTransform.localScale = Vector3.one;
        contentTransform.pivot = new Vector2(0, 1);
        contentTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        contentTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, HeaderHeight, -HeaderHeight);
        contentTransform.anchorMin = new Vector2(0, 0);
        contentTransform.anchorMax = new Vector2(1, 1);

        var contentPanel = Content.GetOrAddComponent<CanvasRenderer>();
        var contentBackground = Content.GetOrAddComponent<Image>();
        contentBackground.sprite = ContentBackground;
        contentBackground.color = new Color(1, 1, 1, 0.5f);

        if (Content != null)
        {
            Content.SetActive(Expanded);
        }
    }

    /// <summary>
    ///     The setup header.
    /// </summary>
    private void SetupHeader()
    {
        Header = gameObject.GetOrCreateChild("Header");
        var headerTransform = Header.GetOrAddComponent<RectTransform>();
        headerTransform.localScale = Vector3.one;
        headerTransform.pivot = new Vector2(0, 1);
        headerTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        headerTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, HeaderHeight);
        headerTransform.anchorMin = new Vector2(0, 1);
        headerTransform.anchorMax = new Vector2(1, 1);

        var headerImage = Header.GetOrAddComponent<Image>();
        headerImage.sprite = HeaderBackground;

        var headerButton = Header.GetOrAddComponent<Button>();
        headerButton.onClick.RemoveListener(OnButtonClick);
        headerButton.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    ///     The setup header text.
    /// </summary>
    private void SetupHeaderText()
    {
        var headerTextGo = Header.GetOrCreateChild("Text");
        var headerTextTransform = headerTextGo.GetOrAddComponent<RectTransform>();
        headerTextTransform.localScale = Vector3.one;
        headerTextTransform.pivot = new Vector2(0, 1);
        headerTextTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 30, -35);
        headerTextTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        headerTextTransform.anchorMin = new Vector2(0, 0);
        headerTextTransform.anchorMax = new Vector2(1, 1);

        var headerText = headerTextGo.GetOrAddComponent<TextMeshProUGUI>();
        headerText.text = HeaderText;
        headerText.fontSize = 24;
        headerText.alignment = TextAlignmentOptions.Center;
    }

    private void OnEnable()
    {
        UpdateLayout();
    }

    /// <summary>
    ///     The update.
    /// </summary>
    [ContextMenu("Update layout")]
    private void UpdateLayout()
    {
        SetupHeader();
        SetupHeaderText();
        SetupContent();
    }
}