using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationManager : MonoBehaviour
{
    [Header("Current Frame")]
    [Min(0f)]
    [SerializeField]
    private int _currentFrameIndex = 0;
    public int currentFrameIndex
    {
        get
        {
            return _currentFrameIndex;
        }
        set
        {
            int previousFrameIndex = _currentFrameIndex;
            _currentFrameIndex = value;
            if (previousFrameIndex != _currentFrameIndex)
            {
                onCurrentFrameIndexChange.Invoke();
            }
        }
    }

    [Header("Frames")]
    [Min(1f)]
    public int framerate = 12;
    public bool showOnionSkin = false;
    public Color onionSkinColour = new Color(1f, 1f, 1f, 0.3f);

    [Header("Animation Panel Settings")]
    [SerializeField]
    [Min(0f)]
    private float scrollSpeed = 1f;
    [SerializeField]
    [Min(0f)]
    private float frameNotchSpacing = 0.5f;
    [SerializeField]
    [Min(0f)]
    private float minFrameNotchSpacing = 0.2f;
    [SerializeField]
    [Min(0f)]
    private float frameNotchSpacingScrollAmount = 0.1f;

    [Header("References")]
    [SerializeField]
    private GameObject frameNotchPrefab;
    [SerializeField]
    private GameObject keyFramePrefab;

    private List<FrameNotch> frameNotches = new List<FrameNotch>();

    private Mouse mouse;
    private UIViewport viewport;
    private InputTarget viewportInputTarget;
    private Transform needle;
    private Transform timeline;

    private UIToggleButton playPauseButton;
    private UITextbox framerateTextbox;
    private UIDropdownChoice playbackMode;

    private LayerManager layerManager;
    private FileManager fileManager;
    private InputSystem inputSystem;

    private UIToggleGroup keyFrameToggleGroup;

    private SpriteRenderer onionSkin;

    private bool isPlaying = false;
    private float frameTimer = 0f;
    private bool playForwards = true;

    private UnityEvent onCurrentFrameIndexChange = new UnityEvent();
    private UnityEvent onKeyFrameDeleted = new UnityEvent();

    private void Awake()
    {
        mouse = Finder.mouse;
        viewport = transform.Find("Viewport").GetComponent<UIViewport>();
        viewportInputTarget = viewport.GetComponent<InputTarget>();
        needle = viewport.scrollingArea.Find("Needle");
        timeline = viewport.scrollingArea.Find("Timeline");

        framerateTextbox = transform.Find("Framerate").GetComponent<UITextbox>();
        playPauseButton = transform.Find("Play Pause").GetComponent<UIToggleButton>();
        playbackMode = transform.Find("Playback Mode").GetComponent<UIDropdownChoice>();

        layerManager = Finder.layerManager;
        fileManager = Finder.fileManager;
        inputSystem = Finder.inputSystem;

        keyFrameToggleGroup = viewport.scrollingArea.Find("Toggle Group").GetComponent<UIToggleGroup>();

        onionSkin = GameObject.Find("Onion Skin").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        OnFramerateChanged();
        framerateTextbox.SubscribeToFinishEvent(OnFramerateChanged);
        playPauseButton.SubscribeToLeftClick(OnPlayPause);
        playbackMode.SubscribeToOptionChanged(OnPlaybackModeChanged);

        layerManager.SubscribeToLayerChange(UpdateDisplay);
        fileManager.SubscribeToFileSwitched(UpdateDisplay);

        inputSystem.SubscribeToGlobalKeyboard(KeyboardShortcut);

        UpdateDisplay();
    }

    private void Update()
    {
        if (viewportInputTarget.mouseTarget.state == MouseTargetState.Hover && mouse.scrollDelta != 0f)
        {
            if (inputSystem.globalKeyboardTarget.IsHeld(KeyCode.LeftControl) || inputSystem.globalKeyboardTarget.IsHeld(KeyCode.RightControl))
            {
                if (frameNotchSpacing >= minFrameNotchSpacing + frameNotchSpacingScrollAmount || mouse.scrollDelta > 0f)
                {
                    frameNotchSpacing += mouse.scrollDelta * frameNotchSpacingScrollAmount;

                    UpdateDisplay();
                }
            }
            else
            {
                viewport.AddScrollAmount(-mouse.scrollDelta * scrollSpeed);
            }
        }

        if (viewportInputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            currentFrameIndex = (int)Mathf.Max(0f, CoordsToFrameIndex(mouse.worldPos));
        }

        needle.localPosition = new Vector3(-viewport.rectTransform.sizeDelta.x / 2f + frameNotchSpacing * currentFrameIndex, needle.transform.localPosition.z, needle.transform.localPosition.z);

        if (isPlaying)
        {
            frameTimer += Time.deltaTime;

            if (frameTimer >= 1f / framerate)
            {
                frameTimer = 0f;

                if ((playForwards && currentFrameIndex >= fileManager.currentFile.numOfFrames - 1) || (!playForwards && currentFrameIndex <= 0))
                {
                    if (playbackMode.selectedOption == "once")
                    {
                        Stop();
                    }
                    else if (playbackMode.selectedOption == "loop")
                    {
                        currentFrameIndex = 0;
                    }
                    else if (playbackMode.selectedOption == "ping-pong")
                    {
                        if (fileManager.currentFile.numOfFrames <= 1)
                        {
                            currentFrameIndex = 0;
                            playForwards = !playForwards;
                        }
                        else
                        {
                            if (playForwards)
                            {
                                currentFrameIndex -= 1;
                                playForwards = false;
                            }
                            else
                            {
                                currentFrameIndex += 1;
                                playForwards = true;
                            }
                        }
                    }
                    else
                    {
                        throw new System.Exception("Unknown / unimplemented playback mode: " + playbackMode.selectedOption);
                    }
                }
                else
                {
                    if (playForwards)
                    {
                        currentFrameIndex++;
                    }
                    else
                    {
                        currentFrameIndex--;
                    }
                }
            }
        }

        int numOfCurrentKeyFrames = 0;
        foreach (Layer layer in fileManager.currentFile.layers)
        {
            numOfCurrentKeyFrames += layer.keyFrames.Count;
        }

        if (numOfCurrentKeyFrames != keyFrameToggleGroup.Count)
        {
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        ClearFrameNotches();

        RectTransform timelineRectTransform = timeline.GetComponent<RectTransform>();
        timelineRectTransform.sizeDelta = new Vector2(Mathf.Max(5f, (fileManager.currentFile.numOfFrames + 2) * frameNotchSpacing * 2f), timelineRectTransform.sizeDelta.y);

        for (int i = 0; i < fileManager.currentFile.numOfFrames; i++)
        {
            FrameNotch notch = Instantiate(frameNotchPrefab, timeline).GetComponent<FrameNotch>();
            notch.transform.localPosition = new Vector3(FrameIndexToXCoord(i), 0f, 0f);
            notch.frameNum = i + 1;

            frameNotches.Add(notch);
        }

        keyFrameToggleGroup.DestroyToggles();

        for (int i = 0; i < fileManager.currentFile.layers.Count; i++)
        {
            Layer layer = fileManager.currentFile.layers[i];
            float y = layerManager.WorldYCoordOfLayerTile(i);

            foreach (AnimationKeyFrame keyFrame in layer.keyFrames)
            {
                UIToggleButton keyFrameButton = Instantiate(keyFramePrefab, viewport.scrollingArea).GetComponent<UIToggleButton>();
                keyFrameToggleGroup.Add(keyFrameButton);

                keyFrameButton.transform.position = new Vector3(0f, y, 0f);
                keyFrameButton.transform.localPosition = new Vector3(FrameIndexToXCoord(keyFrame.frame), keyFrameButton.transform.localPosition.y, -0.1f);

                KeyFrameButton keyFrameButtonScript = keyFrameButton.GetComponent<KeyFrameButton>();
                keyFrameButtonScript.frameIndex = keyFrame.frame;
                keyFrameButtonScript.layerIndex = i;
            }
        }

        if (showOnionSkin && currentFrameIndex > 0)
        {
            onionSkin.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.Multiply(fileManager.currentFile.Render(currentFrameIndex - 1), onionSkinColour));
        }
        else
        {
            onionSkin.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.BlankTexture(fileManager.currentFile.width, fileManager.currentFile.height));
        }
    }

    private void ClearFrameNotches()
    {
        foreach (FrameNotch notch in frameNotches)
        {
            Destroy(notch.gameObject);
        }

        frameNotches = new List<FrameNotch>();
    }

    private float FrameIndexToXCoord(int frameIndex)
    {
        return -viewport.rectTransform.sizeDelta.x / 2f + frameNotchSpacing * frameIndex;
    }

    private int CoordsToFrameIndex(Vector2 coords)
    {
        return XCoordToFrameIndex(coords.x);
    }
    private int XCoordToFrameIndex(float xCoord)
    {
        return Mathf.RoundToInt((viewport.scrollingArea.InverseTransformPoint(new Vector3(xCoord, 0f, 0f)).x + viewport.rectTransform.sizeDelta.x / 2f) / frameNotchSpacing);
    }

    public void AddKeyFrame()
    {
        //layerManager.selectedLayer.animation.AddKeyFrame(currentFrameIndex);
        fileManager.currentFile.numOfFrames++;
        Debug.Log("Added key frame.");

        UpdateDisplay();
    }

    public void RemoveKeyFrame()
    {
        if (fileManager.currentFile.numOfFrames > 1)
        {
            fileManager.currentFile.numOfFrames--;
            Debug.Log("Removed key frame.");

            if (currentFrameIndex >= fileManager.currentFile.numOfFrames)
            {
                currentFrameIndex = fileManager.currentFile.numOfFrames - 1;
            }

            UpdateDisplay();
            onKeyFrameDeleted.Invoke();
        }
    }

    public void DeleteSelectedKeyFrame()
    {
        if (keyFrameToggleGroup.currentToggle)
        {
            KeyFrameButton keyFrameButtonScript = keyFrameToggleGroup.currentToggle.GetComponent<KeyFrameButton>();
            fileManager.currentFile.layers[keyFrameButtonScript.layerIndex].DeleteKeyFrame(keyFrameButtonScript.frameIndex);

            UpdateDisplay();
            onKeyFrameDeleted.Invoke();
        }
    }

    public void DebugLogKeyFrames()
    {
        Debug.Log("Current frame: " + currentFrameIndex + ". Key frames at: " + Functions.ArrayToString(layerManager.selectedLayer.keyFrameIndices));
    }

    private void OnFramerateChanged()
    {
        if (framerateTextbox.text == "")
        {
            framerateTextbox.SetText(framerate.ToString());
        }
        else
        {
            framerate = int.Parse(framerateTextbox.text);
        }
    }

    public void Play()
    {
        playPauseButton.SetOnOff(true);
        frameTimer = 0f;
    }
    public void Pause()
    {
        playPauseButton.SetOnOff(false);
        frameTimer = 0f;
    }
    public void Stop()
    {
        Pause();
        currentFrameIndex = 0;
    }
    private void OnPlayPause()
    {
        isPlaying = playPauseButton.on;
        frameTimer = 0f;
    }

    private void OnKeyFrameAdded()
    {
        UpdateDisplay();
    }

    private void OnPlaybackModeChanged()
    {
        playForwards = true;
    }

    private void KeyboardShortcut()
    {
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("play / pause")))
        {
            playPauseButton.Press();
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("previous frame")))
        {
            currentFrameIndex = Functions.Mod(currentFrameIndex - 1, fileManager.currentFile.numOfFrames);
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("next frame")))
        {
            currentFrameIndex = Functions.Mod(currentFrameIndex + 1, fileManager.currentFile.numOfFrames);
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("first frame")))
        {
            currentFrameIndex = 0;
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("last frame")))
        {
            currentFrameIndex = fileManager.currentFile.numOfFrames - 1;
        }
    }

    public void SubscribeToCurrentFrameIndexChange(UnityAction call)
    {
        onCurrentFrameIndexChange.AddListener(call);
    }

    public void SubscribeToKeyFrameDeletion(UnityAction call)
    {
        onKeyFrameDeleted.AddListener(call);
    }
}
