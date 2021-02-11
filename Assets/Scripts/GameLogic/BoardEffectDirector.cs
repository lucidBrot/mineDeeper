using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Assets.Scripts.Data;

using UnityEngine;

public class BoardEffectDirector : MonoBehaviour
{
    private static List<BoardEffectDirector> availableDirectors;

    private static GameObject directorGameObject;

    public static bool HasActiveEffect => activeEffects > 0;

    private static int activeEffects;

    private Action<BoardCell> effect;

    public static void PlayEffect(Action<BoardCell> effect)
    {
        var director = GetDirector();
        director.effect = effect;
        director.enabled = true;
    }

    private static BoardEffectDirector GetDirector()
    {
        if (availableDirectors == null)
        {
            availableDirectors = new List<BoardEffectDirector>();
        }

        BoardEffectDirector director = null;

        for (var i = availableDirectors.Count - 1; i >= 0; i--)
        {
            if (availableDirectors[i] != null)
            {
                director = availableDirectors[i];
                break;
            }

            availableDirectors.RemoveAt(i);
        }

        if (director == null)
        {
            var directorGo = GetDirectorGameObject();
            director = directorGo.AddComponent<BoardEffectDirector>();
            director.enabled = false;
        }

        return director;
    }

    private static GameObject GetDirectorGameObject()
    {
        if (directorGameObject == null)
        {
            directorGameObject = GameObject.Find("DirectorContainer");
        }

        if (directorGameObject == null)
        {
            directorGameObject = new GameObject("DirectorContainer");
        }

        return directorGameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
