using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor shapeeditor;
    Editor coloreditor;
    MaterialEditor colormaterialeditor;
    SerializedObject colorsettings;

    public override void OnInspectorGUI()
    {
        bool checkchanged = false;

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
                planet.GeneratePlanet();

            checkchanged = check.changed;
        }

        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        DrawSettingsEditor(planet.shapesettings, ref planet.shapesettingsfoldout, planet.OnShapeSettingsUpdate, ref shapeeditor);
        DrawSettingsEditor(planet.colorsettings, ref planet.colorsettingsfoldout, planet.OnColorSettingsUpdated, ref coloreditor);

        // -- draw material editor
        EditorGUILayout.PropertyField(colorsettings.FindProperty("planetmaterial"));
        if(checkchanged)
        {
            serializedObject.ApplyModifiedProperties();
            if (colormaterialeditor != null)
                DestroyImmediate(colormaterialeditor);

            if (planet.colorsettings.planetmaterial != null)
                colormaterialeditor = (MaterialEditor)CreateEditor(planet.colorsettings.planetmaterial);
        }

        if (planet.colorsettings.planetmaterial != null)
        {
            colormaterialeditor.DrawHeader();
            bool isdefaultmat = !AssetDatabase.GetAssetPath(planet.colorsettings.planetmaterial).StartsWith("Assets");
            using (new EditorGUI.DisabledGroupScope(isdefaultmat))
            {
                colormaterialeditor.OnInspectorGUI();
            }
        }
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
                if (editor == colormaterialeditor)
                {
                    CreateCachedEditor(settings, typeof(MaterialEditor), ref editor);
                    colormaterialeditor.DrawDefaultInspector();
                }
                else
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
        planet = target as Planet;
        colorsettings = new SerializedObject(planet.colorsettings);

        if (planet.colorsettings.planetmaterial != null)
            colormaterialeditor = (MaterialEditor)CreateEditor(planet.colorsettings.planetmaterial);
    }
}
