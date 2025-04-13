/*
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PokemonBase))]
public class PokemonBaseEditor : Editor
{
    private SerializedProperty nameProp, speciesProp, heightProp, weightProp, type1Prop, type2Prop, dexProp, hpProp, attackProp, defenseProp, spAttackProp, spDefenseProp, speedProp, minLevelProp, evYieldProp, catchRateProp, maleRatioProp, femaleRatioProp, learnableMovesProp, machineMovesProp, frontSpriteProp, backSpriteProp;

    private void OnEnable()
    {
        // Cache all the serialized properties
        nameProp = serializedObject.FindProperty("name");
        speciesProp = serializedObject.FindProperty("species");
        heightProp = serializedObject.FindProperty("height");
        weightProp = serializedObject.FindProperty("weight");
        type1Prop = serializedObject.FindProperty("type1");
        type2Prop = serializedObject.FindProperty("type2");
        dexProp = serializedObject.FindProperty("dex");
        hpProp = serializedObject.FindProperty("hp");
        attackProp = serializedObject.FindProperty("attack");
        defenseProp = serializedObject.FindProperty("defense");
        spAttackProp = serializedObject.FindProperty("spAttack");
        spDefenseProp = serializedObject.FindProperty("spDefense");
        speedProp = serializedObject.FindProperty("speed");
        minLevelProp = serializedObject.FindProperty("minLevel");
        evYieldProp = serializedObject.FindProperty("evYield");
        catchRateProp = serializedObject.FindProperty("catchRate");
        maleRatioProp = serializedObject.FindProperty("maleRatio");
        femaleRatioProp = serializedObject.FindProperty("femaleRatio");
        learnableMovesProp = serializedObject.FindProperty("learnableMoves");
        machineMovesProp = serializedObject.FindProperty("machineMoves");
        frontSpriteProp = serializedObject.FindProperty("frontSprite");
        backSpriteProp = serializedObject.FindProperty("backSprite");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Load the current values from the real script into the serialized copy

        // Manually draw each property in desired order
        EditorGUILayout.PropertyField(nameProp);
        EditorGUILayout.PropertyField(speciesProp);
        EditorGUILayout.PropertyField(heightProp);
        EditorGUILayout.PropertyField(weightProp);
        EditorGUILayout.PropertyField(type1Prop);
        EditorGUILayout.PropertyField(type2Prop);
        EditorGUILayout.PropertyField(dexProp);
        EditorGUILayout.PropertyField(hpProp);
        EditorGUILayout.PropertyField(attackProp);
        EditorGUILayout.PropertyField(defenseProp);
        EditorGUILayout.PropertyField(spAttackProp);
        EditorGUILayout.PropertyField(spDefenseProp);
        EditorGUILayout.PropertyField(speedProp);
        EditorGUILayout.PropertyField(minLevelProp);
        EditorGUILayout.PropertyField(evYieldProp);
        EditorGUILayout.PropertyField(catchRateProp);

        // Custom handling for male and female ratios
        var maleRatio = EditorGUILayout.Slider("Male Ratio", maleRatioProp.floatValue, 0f, 100f);
        EditorGUILayout.Slider("Female Ratio", 100f - maleRatio, 0f, 100f);
        if (!Mathf.Approximately(maleRatioProp.floatValue, maleRatio))
        {
            maleRatioProp.floatValue = maleRatio;
            femaleRatioProp.floatValue = 100f - maleRatio;
        }

        // Continue drawing other properties
        EditorGUILayout.PropertyField(learnableMovesProp);
        EditorGUILayout.PropertyField(machineMovesProp);
        EditorGUILayout.PropertyField(frontSpriteProp);
        EditorGUILayout.PropertyField(backSpriteProp);

        serializedObject.ApplyModifiedProperties(); // Apply changes to the real script
    }
}
*/

