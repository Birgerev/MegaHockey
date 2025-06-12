using UnityEngine;
using UnityEditor;

public static class AnimationCurveDebugVisualizer
{
    private static AnimationCurve curve;
    private static float timePointer;
    private static Vector3 worldPosition;
    private static Rect drawRect = new Rect(0, 0, 150, 100);
    private static Color curveColor = Color.green;
    private static Color pointerColor = Color.red;

    private static Camera cam;
    
    /*
    void OnDrawGizmos()
    {
        AnimationCurveDebugVisualizer.Visualise(turnSpeedOverVelocity, currentSpeed, transform.position + Vector3.up * 2f);
    }
    */
    
    public static void Visualise(AnimationCurve c, float pointerTime, Vector3 worldPos, Camera camera = null)
    {
        curve = c;
        timePointer = pointerTime;
        worldPosition = worldPos;
        cam = camera ?? Camera.main;

        SceneView.duringSceneGui -= Draw;
        SceneView.duringSceneGui += Draw;
    }

    private static void Draw(SceneView sceneView)
    {
        if (curve == null || cam == null)
            return;

        // Convert world position to screen space (GUI coordinates are y-inverted)
        Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);
        if (screenPos.z < 0) return; // Behind camera

        Vector2 guiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
        Rect rect = new Rect(guiPos.x, guiPos.y, drawRect.width, drawRect.height);

        Handles.BeginGUI();
        GUI.BeginGroup(rect);
        GUI.Box(new Rect(0, 0, drawRect.width, drawRect.height), GUIContent.none);

        // Draw curve
        Vector2 prev = EvaluatePoint(0);
        for (int i = 1; i < drawRect.width; i++)
        {
            float t = i / drawRect.width;
            Vector2 next = EvaluatePoint(t);
            Handles.color = curveColor;
            Handles.DrawLine(prev, next);
            prev = next;
        }

        // Draw pointer
        float normalizedTime = Mathf.InverseLerp(curve.keys[0].time, curve.keys[^1].time, timePointer);
        Vector2 pointer = EvaluatePoint(normalizedTime);
        Handles.color = pointerColor;
        Handles.DrawLine(new Vector2(pointer.x, drawRect.height), pointer);

        GUI.EndGroup();
        Handles.EndGUI();
    }

    private static Vector2 EvaluatePoint(float normalizedTime)
    {
        float curveTime = Mathf.Lerp(curve.keys[0].time, curve.keys[^1].time, normalizedTime);
        float value = curve.Evaluate(curveTime);
        float x = normalizedTime * drawRect.width;
        float y = drawRect.height - (value * drawRect.height);
        return new Vector2(x, y);
    }
}
