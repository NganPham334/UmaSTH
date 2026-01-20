using UnityEngine;
using UnityEngine.UIElements;

public class LauncherThing : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // 1. Query elements by their UXML names
        var loadBtn = root.Q<VisualElement>("loadgame");
        var exitBtn = root.Q<VisualElement>("exit");

        // 2. Check if a save exists using the static helper from your Save script
        bool hasSave = SaveRunButton.HasSavedGame();

        // 3. Apply state-based logic
        if (!hasSave)
        {
            // Disable the load button visually and functionally
            SetElementEnabled(loadBtn, false);

            // Flip the exit button's skew and position via USS
            ApplyMirroredState(exitBtn, true);
        }
        else
        {
            SetElementEnabled(loadBtn, true);
            ApplyMirroredState(exitBtn, false);
        }
    }

    private void SetElementEnabled(VisualElement element, bool enabled)
    {
        if (element == null) return;

        // SetEnabled handles 'pickingMode' automatically and allows :disabled pseudo-states
        element.SetEnabled(enabled);

        if (enabled)
            element.RemoveFromClassList("button--disabled");
        else
            element.AddToClassList("button--disabled");
    }
    
    private void ApplyMirroredState(VisualElement element, bool isMirrored)
    {
        if (element == null) return;

        // Fix: If the layout isn't calculated yet, wait for it using a proper reference
        if (element.layout.width <= 0 || float.IsNaN(element.layout.width))
        {
            EventCallback<GeometryChangedEvent> callback = null;
            callback = (evt) =>
            {
                element.UnregisterCallback(callback); // No longer null, so no exception
                ExecuteMirrorLogic(element, isMirrored);
            };
            element.RegisterCallback(callback);
        }
        else
        {
            ExecuteMirrorLogic(element, isMirrored);
        }
    }

    private void ExecuteMirrorLogic(VisualElement element, bool isMirrored)
    {
        // 1. Standard VisualElement properties
        element.style.alignSelf = isMirrored ? Align.FlexEnd : Align.FlexStart;
        
        // Applying the specific tilt seen in your screenshot
        element.style.rotate = isMirrored ? new Rotate(-6) : new Rotate(6);

        if (element is SkewedRoundedBox box)
        {
            // 2. Set explicit values to avoid "swapping" 0 or unitialized values
            float skewValue = -20f; 
            float radiusValue = 30f;

            if (isMirrored)
            {
                // MIRRORED (e.g. Right side of horseshoe)
                box.skewAmount = -skewValue;
                
                // Round the outside corners (Right side)
                box.radiusTopLeft = radiusValue;
                box.radiusBottomLeft = 0f;
                box.radiusTopRight = 0f;
                box.radiusBottomRight = radiusValue;
            }
            else
            {
                // DEFAULT (e.g. Left side, Exit Game)
                box.skewAmount = skewValue;
                
                // Round the outside corners (Left side)
                box.radiusTopLeft = 0f;
                box.radiusBottomLeft = radiusValue;
                box.radiusTopRight = radiusValue;
                box.radiusBottomRight = 0f;
            }

            // 3. Force the custom Mesh to redraw with these new C# values
            box.MarkDirtyRepaint();
        }
    }
}

