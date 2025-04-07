using System.Collections.Generic;

using PAC.Image;
using PAC.Image.Layers;
using PAC.Input;
using PAC.UndoRedo;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.Managers
{
    public class UndoRedoManager : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent onUndo = new UnityEvent();
        [SerializeField]
        private UnityEvent onRedo = new UnityEvent();
        [SerializeField]
        private UnityEvent onUndoOrRedo = new UnityEvent();

        private List<Stack<UndoRedoState>> undoStack = new List<Stack<UndoRedoState>>();
        private List<Stack<UndoRedoState>> redoStack = new List<Stack<UndoRedoState>>();

        private FileManager fileManager;
        private InputSystem inputSystem;

        private void Start()
        {
            fileManager = Finder.fileManager;
            inputSystem = Finder.inputSystem;

            inputSystem.SubscribeToGlobalKeyboard(KeyboardShortcut);

            undoStack.Add(new Stack<UndoRedoState>());
            redoStack.Add(new Stack<UndoRedoState>());
        }

        private void AddUndoStateNotDeleteRedoStack(UndoRedoState undoState, int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= fileManager.files.Count)
            {
                throw new System.Exception("File index not in valid range: " + fileIndex);
            }

            undoStack[fileIndex].Push(undoState);
        }
        public void AddUndoState(UndoRedoState undoState, int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= fileManager.files.Count)
            {
                throw new System.Exception("File index not in valid range: " + fileIndex);
            }

            AddUndoStateNotDeleteRedoStack(undoState, fileIndex);
            if (redoStack[fileIndex].Count > 0)
            {
                redoStack[fileIndex] = new Stack<UndoRedoState>();
            }
        }

        private void AddRedoState(UndoRedoState redoState, int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= fileManager.files.Count)
            {
                throw new System.Exception("File index not in valid range: " + fileIndex);
            }

            redoStack[fileIndex].Push(redoState);
        }

        private bool Undo(int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= fileManager.files.Count)
            {
                throw new System.Exception("File index not in valid range: " + fileIndex);
            }

            if (undoStack[fileIndex].Count == 0)
            {
                return false;
            }

            UndoRedoState state = undoStack[fileIndex].Pop();

            Layer[] affectedLayersCurrentState = new Layer[state.affectedLayers.Length];
            for (int i = 0; i < state.affectedLayers.Length; i++)
            {
                affectedLayersCurrentState[i] = fileManager.files[fileIndex].layers[state.affectedLayersIndices[i]];
            }
            UndoRedoState redoState = new UndoRedoState(state.action, affectedLayersCurrentState, state.affectedLayersIndices);
            AddRedoState(redoState, fileIndex);

            RestoreState(state, fileManager.files[fileIndex]);

            onUndo.Invoke();
            onUndoOrRedo.Invoke();

            return true;
        }

        private bool Redo(int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= fileManager.files.Count)
            {
                throw new System.Exception("File index not in valid range: " + fileIndex);
            }

            if (redoStack[fileIndex].Count == 0)
            {
                return false;
            }

            UndoRedoState state = redoStack[fileIndex].Pop();

            Layer[] affectedLayersCurrentState = new Layer[state.affectedLayers.Length];
            for (int i = 0; i < state.affectedLayers.Length; i++)
            {
                affectedLayersCurrentState[i] = fileManager.files[fileIndex].layers[state.affectedLayersIndices[i]];
            }
            UndoRedoState undoState = new UndoRedoState(state.action, affectedLayersCurrentState, state.affectedLayersIndices);
            AddUndoStateNotDeleteRedoStack(undoState, fileIndex);

            RestoreState(state, fileManager.files[fileIndex]);

            onRedo.Invoke();
            onUndoOrRedo.Invoke();

            return true;
        }

        private void RestoreState(UndoRedoState state, File file)
        {
            for (int i = 0; i < state.affectedLayers.Length; i++)
            {
                int layerIndex = state.affectedLayersIndices[i];
                if (layerIndex >= file.layers.Count)
                {
                    throw new System.Exception("Layer index too high for number of layers: " + layerIndex);
                }

                file.ReplaceLayer(layerIndex, state.affectedLayers[i]);
            }
        }

        private void KeyboardShortcut()
        {
            if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Z))
            {
                Undo(fileManager.currentFileIndex);
            }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Y))
            {
                Redo(fileManager.currentFileIndex);
            }
        }

        public void SubscribeToUndo(UnityAction call)
        {
            onUndo.AddListener(call);
        }
        public void SubscribeToRedo(UnityAction call)
        {
            onRedo.AddListener(call);
        }
        public void SubscribeToUndoOrRedo(UnityAction call)
        {
            onUndoOrRedo.AddListener(call);
        }
    }
}
