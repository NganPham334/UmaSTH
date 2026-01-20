using UnityEngine;
using UnityEngine.UIElements;

public class SkewedRoundedBox : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SkewedRoundedBox, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlFloatAttributeDescription m_Skew = new UxmlFloatAttributeDescription { name = "skew-amount", defaultValue = 20f };
        UxmlColorAttributeDescription m_Color = new UxmlColorAttributeDescription { name = "fill-color", defaultValue = new Color(0.2f, 0.2f, 0.2f) };
        UxmlFloatAttributeDescription m_RadiusTL = new UxmlFloatAttributeDescription { name = "radius-top-left", defaultValue = 10f };
        UxmlFloatAttributeDescription m_RadiusTR = new UxmlFloatAttributeDescription { name = "radius-top-right", defaultValue = 10f };
        UxmlFloatAttributeDescription m_RadiusBL = new UxmlFloatAttributeDescription { name = "radius-bottom-left", defaultValue = 10f };
        UxmlFloatAttributeDescription m_RadiusBR = new UxmlFloatAttributeDescription { name = "radius-bottom-right", defaultValue = 10f };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as SkewedRoundedBox;
            ate.skewAmount = m_Skew.GetValueFromBag(bag, cc);
            ate.fillColor = m_Color.GetValueFromBag(bag, cc);
            ate.radiusTopLeft = m_RadiusTL.GetValueFromBag(bag, cc);
            ate.radiusTopRight = m_RadiusTR.GetValueFromBag(bag, cc);
            ate.radiusBottomLeft = m_RadiusBL.GetValueFromBag(bag, cc);
            ate.radiusBottomRight = m_RadiusBR.GetValueFromBag(bag, cc);
        }
    }

    public float skewAmount { get; set; }
    public Color fillColor { get; set; }
    public float radiusTopLeft { get; set; }
    public float radiusTopRight { get; set; }
    public float radiusBottomLeft { get; set; }
    public float radiusBottomRight { get; set; }

    public SkewedRoundedBox()
    {
        // Set this HERE, not in the USS
        this.pickingMode = PickingMode.Position;
    
        // The rest of your setup
        style.backgroundColor = Color.clear;
        generateVisualContent += OnGenerateVisualContent;
        RegisterCallback<CustomStyleResolvedEvent>(e => MarkDirtyRepaint());
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        
        // Use the USS tint color. If not hovered/active, it should be white (1,1,1)
        // Multiplying it ensures we keep our original fill color but apply the "tint"
        painter.fillColor = fillColor * resolvedStyle.unityBackgroundImageTintColor;

        float w = layout.width;
        float h = layout.height;
        if (w <= 0 || h <= 0) return;

        float s = skewAmount;
        float rTL = Mathf.Min(radiusTopLeft, h / 2f, w / 2f);
        float rTR = Mathf.Min(radiusTopRight, h / 2f, w / 2f);
        float rBL = Mathf.Min(radiusBottomLeft, h / 2f, w / 2f);
        float rBR = Mathf.Min(radiusBottomRight, h / 2f, w / 2f);
        float sm = 0.6f; 

        painter.BeginPath();
        // Top Right
        painter.MoveTo(new Vector2(w - rTR - (rTR * sm), 0)); 
        painter.BezierCurveTo(new Vector2(w - (rTR * sm), 0), new Vector2(w, rTR * sm), new Vector2(w, rTR + (rTR * sm)));
        // Bottom Right (Skewed)
        painter.LineTo(new Vector2(w - s, h - rBR - (rBR * sm)));
        painter.BezierCurveTo(new Vector2(w - s, h - (rBR * sm)), new Vector2(w - s - (rBR * sm), h), new Vector2(w - s - rBR - (rBR * sm), h));
        // Bottom Left
        painter.LineTo(new Vector2(rBL + (rBL * sm), h));
        painter.BezierCurveTo(new Vector2(rBL * sm, h), new Vector2(0, h - (rBL * sm)), new Vector2(0, h - rBL - (rBL * sm)));
        // Top Left (Skewed)
        painter.LineTo(new Vector2(s, rTL + (rTL * sm)));
        painter.BezierCurveTo(new Vector2(s, rTL * sm), new Vector2(s + (rTL * sm), 0), new Vector2(s + rTL + (rTL * sm), 0));
        painter.ClosePath();
        painter.Fill();
    }
}