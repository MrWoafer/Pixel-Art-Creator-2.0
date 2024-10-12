using PAC.Layers;

namespace PAC.UndoRedo
{
    public enum UndoRedoAction
    {
        None = -1,
        Undefined = 0,
        Draw = 1,
        ReorderLayers = 2
    }

    public class UndoRedoState
    {
        public UndoRedoAction action { get; private set; } = UndoRedoAction.None;
        public Layer[] affectedLayers { get; private set; } = null;
        public int[] affectedLayersIndices { get; private set; } = null;

        public UndoRedoState(UndoRedoAction action, Layer layer, int layerIndex) : this(action, new Layer[] { layer }, new int[] { layerIndex }) { }

        public UndoRedoState(UndoRedoAction action, Layer[] affectedLayers, int[] affectedLayersIndices)
        {
            this.action = action;

            if (affectedLayers.Length != affectedLayersIndices.Length)
            {
                throw new System.Exception("The number of layers does not match the number of indices: " + affectedLayers.Length + " layers, " + affectedLayersIndices.Length + " indices.");
            }
            foreach(int index in affectedLayersIndices)
            {
                if (index < 0)
                {
                    throw new System.Exception("Layer index cannot be negative: " + index);
                }
            }

            this.affectedLayers = new Layer[affectedLayers.Length];
            for(int i = 0; i < affectedLayers.Length; i++)
            {
                this.affectedLayers[i] = affectedLayers[i].DeepCopy();
            }

            this.affectedLayersIndices = new int[affectedLayersIndices.Length];
            for (int i = 0; i < affectedLayersIndices.Length; i++)
            {
                this.affectedLayersIndices[i] = affectedLayersIndices[i];
            }
        }
    }
}