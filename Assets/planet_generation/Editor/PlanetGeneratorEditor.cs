using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor
{
    PlanetGenerator planetgenerator;
    Editor planeteditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
        }

        DrawSettingsEditor(planetgenerator.planetsettings, ref planetgenerator.planetsettingsfoldout, null, ref planeteditor);
    }

    void DrawSettingsEditor(Object settings, ref bool foldout, System.Action onsettingsupdated, ref Editor editor)
    {
        if (settings == null)
            return;

        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if (foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();

                if (check.changed)
                {
                    if (onsettingsupdated != null)
                    {
                        onsettingsupdated();
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        planetgenerator = target as PlanetGenerator;
    }
}
