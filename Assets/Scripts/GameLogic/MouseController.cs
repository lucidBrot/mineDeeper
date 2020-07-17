using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Frontend;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Solver;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private readonly KeyCode[] focusInputs =
    {
        KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, 
        KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
    };

    public Camera Camera;

    public CameraBehaviour CameraBehaviour;

    public CameraMovement CameraMovement;

    public KeyCode ActivateKey;

    public KeyCode MarkKey;

    public KeyCode FocusKey;

    public LayerMask LayerMask;

    public float RotationTriggerMarginPixels = 8f;

    private KeyDownState keyState;

    private GameObject keyDownObject;

    private Vector3 keyDownPosition;

    private Revelation activeRevelation;

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (keyState == KeyDownState.None)
        {
            if (Input.GetKeyDown(ActivateKey))
            {
                keyState = KeyDownState.ActivationKey;
                HandleKeyDown();
            }
            else if (Input.GetKeyDown(MarkKey))
            {
                keyState = KeyDownState.MarkKey;
                HandleKeyDown();
            }
            else
            {
                this.HandleFocusInput();
            }
        }
        else
        {
            if (activeRevelation != null)
            {
                // Can't accept clicks while we are given a revelation...
                if (!activeRevelation.IsFinished)
                {
                    return;
                }

                activeRevelation = null;
            }

            if (keyState == KeyDownState.ActivationKey && Input.GetKeyUp(ActivateKey) || keyState == KeyDownState.MarkKey && Input.GetKeyUp(MarkKey))
            {
                var ray = Camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, LayerMask))
                {
                    if (keyDownObject != null && keyDownObject == hit.collider.gameObject)
                    {
                        var field = keyDownObject.GetComponentInParent<FieldVisual>();
                        if (field != null)
                        {
                            if (keyState == KeyDownState.ActivationKey && field.BoardCell.State == CellState.Default)
                            {
                                activeRevelation = RevealCellsAnimated(field.BoardCell);
                            }
                            else if (keyState == KeyDownState.MarkKey)
                            {
                                Game.Instance.ToggleMarking(field.BoardCell);
                            }
                        }
                    }
                }

                HandleKeyUp();
            }

            if (keyState == KeyDownState.MarkKey)
            {
                var currentMousePos = Input.mousePosition;
                if ((keyDownPosition - currentMousePos).magnitude >= RotationTriggerMarginPixels)
                {
                    HandleKeyUp();
                }
            }
        }
    }

    private void HandleKeyDown()
    {
        keyDownPosition = Input.mousePosition;
        SetCameraMovementActive(false);

        var ray = Camera.ScreenPointToRay(keyDownPosition);
        if (Physics.Raycast(ray, out var hit, LayerMask))
        {
            keyDownObject = hit.collider.gameObject;
        }
    }

    private void HandleKeyUp()
    {
        keyState = KeyDownState.None;
        keyDownObject = null;
        SetCameraMovementActive(true);
    }

    private void SetCameraMovementActive(bool active)
    {
        if (CameraBehaviour != null)
        {
            CameraBehaviour.enabled = active;
        }

        if (CameraMovement != null)
        {
            CameraMovement.enabled = active;
        }
    }

    private Revelation RevealCellsAnimated(BoardCell cell)
    {
        var revelation = Game.Instance.RevealSlow(cell);
        revelation.FollowWith(Game.Instance.CheckIfGameFinished);
        return revelation;
    }

    private void HandleFocusInput()
    {
        for (var i = 0; i < this.focusInputs.Length; i++)
        {
            var focusInput = this.focusInputs[i];
            if (Input.GetKeyDown(focusInput))
            {
                var ray = this.Camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, this.LayerMask))
                {
                    var field = hit.collider.GetComponentInParent<FieldVisual>();
                    if (field != null)
                    {
                        var cell = field.BoardCell;

                        if (!cell.Focused || cell.FocusId != i)
                        {
                            cell.Focused = true;
                            cell.FocusId = i;
                        }
                        else
                        {
                            cell.Focused = false;
                        }
                    }
                }
            }
        }
    }
    
    private enum KeyDownState
    {
        None,

        ActivationKey,

        MarkKey
    }
}
