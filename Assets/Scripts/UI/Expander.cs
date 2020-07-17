// COGARCH COGARCH_UI Expander.cs
// Copyright © Jasper Ermatinger

#region usings

using System.Threading.Tasks;
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
    ///     The content.
    /// </summary>
    public GameObject Content;

    /// <summary>
    ///     The content background.
    /// </summary>
    public Sprite ContentBackground;

    public Color ContentBackgroundColor = new Color(1, 1, 1, 0.2f);

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

    public Color HeaderBackgroundColor = new Color(1, 1, 1, 0.3f);

    /// <summary>
    ///     The header height.
    /// </summary>
    public float HeaderHeight = 15f;

    public float HeaderFontSize = 14;

    public TextAlignmentOptions HeaderAlignment = TextAlignmentOptions.Center;

    /// <summary>
    ///     The header text.
    /// </summary>
    public string HeaderText = "Header";

    public GameObject Icon;

    public Sprite IconSprite;

    public Color IconColor = new Color(1, 1, 1, 1);

    public float IconSize = 12f;
    
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
        contentBackground.color = ContentBackgroundColor;

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

        var headerLayout = this.Header.GetOrAddComponent<LayoutElement>();
        headerLayout.preferredHeight = this.HeaderHeight;

        var headerImage = Header.GetOrAddComponent<Image>();
        headerImage.sprite = HeaderBackground;
        headerImage.color = this.HeaderBackgroundColor;

        var headerButton = Header.GetOrAddComponent<Button>();
        headerButton.onClick.RemoveListener(OnButtonClick);
        headerButton.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    ///     The setup header text.
    /// </summary>
    private void SetupHeaderText()
    {
        var hOffset = (this.IconSize <= 1e-5) ? 0f : this.HeaderHeight;

        var headerTextGo = Header.GetOrCreateChild("Text");
        var headerTextTransform = headerTextGo.GetOrAddComponent<RectTransform>();
        headerTextTransform.localScale = Vector3.one;
        headerTextTransform.pivot = new Vector2(0, 1);
        headerTextTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, hOffset, -hOffset);
        headerTextTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        headerTextTransform.anchorMin = new Vector2(0, 0);
        headerTextTransform.anchorMax = new Vector2(1, 1);

        var headerText = headerTextGo.GetOrAddComponent<TextMeshProUGUI>();
        headerText.text = HeaderText;
        headerText.fontSize = this.HeaderFontSize;
        headerText.alignment = this.HeaderAlignment;
    }

    private void SetupIcon()
    {
        this.Icon = this.Header.GetOrCreateChild("Icon");
        var iconTransform = this.Icon.GetOrAddComponent<RectTransform>();
        iconTransform.localScale = Vector3.one;
        iconTransform.localPosition =
            new Vector3((this.HeaderHeight - this.IconSize) / 2f, -(this.HeaderHeight / 2f), 0);
        iconTransform.anchorMin = new Vector2(0, 0.5f);
        iconTransform.anchorMax = new Vector2(0, 0.5f);
        iconTransform.anchoredPosition = new Vector2((this.HeaderHeight - this.IconSize) / 2f, 0);
        iconTransform.sizeDelta = new Vector2(this.IconSize, this.IconSize);
        iconTransform.pivot = new Vector2(0, 0.5f);

        var iconLayout = this.Icon.GetOrAddComponent<LayoutElement>();
        iconLayout.preferredHeight = this.IconSize;
        iconLayout.preferredWidth = this.IconSize;

        var iconImage = this.Icon.GetOrAddComponent<Image>();
        iconImage.sprite = this.IconSprite;
        iconImage.color = this.IconColor;
        iconImage.raycastTarget = false;
    }
    
    private void OnEnable()
    {
        UpdateLayout();
    }

    #if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        this.UpdateLayout();
    }
    #endif

    /// <summary>
    ///     The update.
    /// </summary>
    private void UpdateLayout()
    {
        SetupHeader();
        SetupHeaderText();
        SetupContent();
        this.SetupIcon();
    } 
}