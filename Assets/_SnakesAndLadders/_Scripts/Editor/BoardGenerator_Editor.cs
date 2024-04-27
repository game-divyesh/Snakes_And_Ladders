using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(BoardGenerator))]
public class BoardGenerator_Editor : Editor
{
    public BoardGenerator boardGenerator;

    private bool isSnakeOrLadderBtnClick = false;

    private float disableTime = 0f;


    private void OnEnable()
    {
        boardGenerator = target as BoardGenerator;
    }


    public override void OnInspectorGUI()
    {
        // Call the default inspector GUI for the rest of the properties
        DrawDefaultInspector();

        // Add some vertical space
        EditorGUILayout.Space(20);

        DrawButtons();
    }

    private void DrawButtons()
    {
        GUIStyle globalButtonStyle = new GUIStyle(GUI.skin.button);
        globalButtonStyle.normal.textColor = Color.green;
        globalButtonStyle.fontSize = 12;
        globalButtonStyle.fontStyle = FontStyle.Bold;
        globalButtonStyle.alignment = TextAnchor.MiddleCenter;
        globalButtonStyle.fixedHeight = 30;



        if (GUILayout.Button("Create Board", globalButtonStyle))
        {
            boardGenerator.GenerateBoard();
        }

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Set Tiles Normal", globalButtonStyle))
        {
           boardGenerator.ResetAllTileType();
        }

        EditorGUILayout.Space(20);

        GUI.backgroundColor = Color.white;


        if (isSnakeOrLadderBtnClick)
        {
            if (Time.realtimeSinceStartup >= disableTime)
            {
                isSnakeOrLadderBtnClick = false;
                Repaint();
            }
        }

        GUI.backgroundColor = isSnakeOrLadderBtnClick ? Color.gray : Color.white;
        using (new EditorGUI.DisabledScope(isSnakeOrLadderBtnClick))
        {
            if (GUILayout.Button("Random Ladder Snake", globalButtonStyle))
            {
                isSnakeOrLadderBtnClick = true;
                disableTime = Time.realtimeSinceStartup + 4f; // Disable for 4 seconds
                boardGenerator.GenerateRandomLadderAndSnake();
            }
        }
    }

   
}// CLASS
