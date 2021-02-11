using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;
using JetBrains.Annotations;
using UnityEngine;

public class BoardEffectOrchestrator : MonoBehaviour
{
    private static List<BoardEffectOrchestrator> availableOrchestrators;

    private static List<BoardEffectOrchestrator> activeOrchestrators = new List<BoardEffectOrchestrator>();

    private static GameObject directorGameObject;

    public static bool HasActiveEffect => activeOrchestrators.Count > 0;

    private IList source;

    private Func<object, Vector3> objectToPosition;

    private Action<object> effect;

    private Action continueWith;

    private IEffectTimeline timeline;

    private float startTime;

    private int currentIndex;

    private readonly List<(object item, float time)> sortedItems = new List<(object, float)>();

    public static void AbortEffects()
    {
        if (!HasActiveEffect)
        {
            return;
        }

        for (var i = activeOrchestrators.Count - 1; i >= 0 ; i--)
        {
            var orchestrator = activeOrchestrators[i];
            orchestrator.Finish();
        }

        activeOrchestrators.Clear();
    }

    public static void PlayEffect(IList source, [NotNull]IEffectTimeline timeline, Func<object, Vector3> objectToPosition, [NotNull]Action<object> effect, Action continueWith = null)
    {
        var orchestrator = GetDirector();
        orchestrator.source = source;
        orchestrator.timeline = timeline;
        orchestrator.objectToPosition = objectToPosition;
        orchestrator.effect = effect;
        orchestrator.continueWith = continueWith;
        orchestrator.Initialize();
        orchestrator.enabled = true;
    }

    private static BoardEffectOrchestrator GetDirector()
    {
        if (availableOrchestrators == null)
        {
            availableOrchestrators = new List<BoardEffectOrchestrator>();
        }

        BoardEffectOrchestrator orchestrator = null;

        for (var i = availableOrchestrators.Count - 1; i >= 0; i--)
        {
            orchestrator = availableOrchestrators[i];
            availableOrchestrators.RemoveAt(i);

            if (orchestrator != null)
            {
                break;
            }
        }

        if (orchestrator == null)
        {
            var orchestratorGo = GetDirectorGameObject();
            orchestrator = orchestratorGo.AddComponent<BoardEffectOrchestrator>();
            orchestrator.enabled = false;
        }

        return orchestrator;
    }

    private static GameObject GetDirectorGameObject()
    {
        if (directorGameObject == null)
        {
            directorGameObject = GameObject.Find("OrchestratorContainer");
        }

        if (directorGameObject == null)
        {
            directorGameObject = new GameObject("OrchestratorContainer");
        }

        return directorGameObject;
    }

    private void Initialize()
    {
        this.startTime = Time.time;
        this.currentIndex = 0;

        this.sortedItems.Clear();

        if (this.sortedItems.Capacity < this.source.Count)
        {
            this.sortedItems.Capacity = this.source.Count;
        }

        foreach (var item in this.source)
        {
            var pos = this.objectToPosition(item);
            this.sortedItems.Add((item, this.timeline.GetEffectTime(pos)));
        }

        this.sortedItems.Sort((a, b) => a.time.CompareTo(b.time));
        activeOrchestrators.Add(this);
    }

    private void Finish()
    {
        this.source = null;
        this.objectToPosition = null;
        this.effect = null;
        this.timeline = null;
        this.continueWith = null;
        this.sortedItems.Clear();
        this.enabled = false;

        if (availableOrchestrators == null)
        {
            availableOrchestrators = new List<BoardEffectOrchestrator>();
        }
        
        availableOrchestrators.Add(this);
    }

    // Update is called once per frame
    private void Update()
    {
        var tMax = Time.time - this.startTime;

        for (; this.currentIndex < this.sortedItems.Count; this.currentIndex++)
        {
            var (item, time) = this.sortedItems[this.currentIndex];

            if (time > tMax)
            {
                break;
            }

            this.effect(item);
        }

        if (this.currentIndex >= this.sortedItems.Count)
        {
            activeOrchestrators.Remove(this);
            this.continueWith?.Invoke();
            this.Finish();
        }
    }
}