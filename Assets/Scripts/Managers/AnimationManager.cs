using System.Collections.Generic;

using PAC.Maths;
using PAC.Input;
using PAC.Layers;
using UnityEngine;
using UnityEngine.Events;
using PAC.Colour.Compositing;
using PAC.Extensions.UnityEngine;
using PAC.Extensions.System.Collections;
using PAC.ImageEditing;
using PAC.UI.Components.General;
using PAC.UI.Components.Specialised.Animation;
using PAC.Config;
using PAC.Animation;

namespace PAC.Managers
{
    /// <summary>
    /// Handles the animation timeline and playback of animations.
    /// The animation system is still a work in progress.
    /// </summary>
    public class AnimationManager : MonoBehaviour
    {
        [Header("Current Frame")]
        [Min(0f)]
        [SerializeField]
        private int _currentFrameIndex = 0;
        /// <summary>The number of the frame currently being displayed.</summary>
        public int currentFrameIndex
        {
            get
            {
                return _currentFrameIndex;
            }
            set
            {
                SetFrameIndex(value);
            }
        }

        [Header("Frames")]
        [Min(1f)]
        [Tooltip("How many frames are shown per second.")]
        public int framerate = 12;
        [Tooltip("Whether to show a faint ghost of the previous/next frames.")]
        // This feature has not been fully added yet.
        public bool showOnionSkin = false;
        [Tooltip("The tint to give the onion skin.")]
        public Color onionSkinColour = new Color(1f, 1f, 1f, 0.3f);

        [Header("Animation Panel Settings")]
        [SerializeField]
        [Min(0f)]
        [Tooltip("How quickly you can scroll horizontally through the timeline.")]
        private float scrollSpeed = 1f;
        [SerializeField]
        [Min(0f)]
        [Tooltip("How far apart the frame 1, frame 2, etc markers are on the timeline. Changes when zooming in/out.")]
        private float frameNotchSpacing = 0.5f;
        [SerializeField]
        [Min(0f)]
        [Tooltip("How min distance between the frame 1, frame 2, etc markers on the timeline.")]
        private float minFrameNotchSpacing = 0.2f;
        [SerializeField]
        [Min(0f)]
        [Tooltip("How quickly you can scroll in/out on the timeline.")]
        private float frameNotchSpacingScrollSpeed = 0.1f;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onCurrentFrameIndexChange = new UnityEvent();
        [SerializeField]
        private UnityEvent onKeyFrameAdded = new UnityEvent();
        [SerializeField]
        private UnityEvent onKeyFrameDeleted = new UnityEvent();

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
            framerateTextbox.SetText(Preferences.startupAnimationFramerate.Get().ToString());
            OnFramerateChanged();
            framerateTextbox.SubscribeToFinishEvent(OnFramerateChanged);
            playPauseButton.SubscribeToLeftClick(OnPlayPause);
            playbackMode.SubscribeToOptionChanged(OnPlaybackModeChanged);

            layerManager.SubscribeToLayerChange(UpdateDisplay);
            fileManager.SubscribeToFileSwitched(UpdateDisplay);

            inputSystem.SubscribeToGlobalKeyboard(CheckKeyboardShortcuts);

            UpdateDisplay();
        }

        private void Update()
        {
            // Scrolling up/down, left/right or in/out on the timeline
            if (viewportInputTarget.mouseTarget.state == MouseTargetState.Hover && mouse.scrollDelta != 0f)
            {
                // Scroll in/out
                if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("scroll in/out timeline")))
                {
                    if (frameNotchSpacing >= minFrameNotchSpacing + frameNotchSpacingScrollSpeed || mouse.scrollDelta > 0f)
                    {
                        frameNotchSpacing += mouse.scrollDelta * frameNotchSpacingScrollSpeed;

                        UpdateDisplay();
                    }
                }
                // Scroll left/right
                else
                {
                    viewport.AddScrollAmount(-mouse.scrollDelta * scrollSpeed);
                }
            }

            // Change current frame by clicking
            if (viewportInputTarget.mouseTarget.state == MouseTargetState.Pressed)
            {
                currentFrameIndex = (int)Mathf.Max(0f, CoordsToFrameIndex(mouse.worldPos));
            }

            // Set frame needle to correct position
            needle.localPosition = new Vector3(-viewport.rectTransform.sizeDelta.x / 2f + frameNotchSpacing * currentFrameIndex, needle.transform.localPosition.z, needle.transform.localPosition.z);

            // Animation playback
            if (isPlaying)
            {
                frameTimer += Time.deltaTime;

                // Moving to next frame
                if (frameTimer >= 1f / framerate)
                {
                    frameTimer = 0f;

                    // Deal with reaching start/end of animation based on selected playback mode
                    if (playForwards && currentFrameIndex >= fileManager.currentFile.numOfFrames - 1 || !playForwards && currentFrameIndex <= 0)
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
                    // Move to next frame
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

            // Redisplay key frame icons if one has been added/removed
            // This is not the best way of checking for this, but it'll do for now
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

        /// <summary>
        /// Starts/resumes the animation playback.
        /// </summary>
        public void Play()
        {
            playPauseButton.SetOnOff(true);
            frameTimer = 0f;
        }
        /// <summary>
        /// Pauses the animation playback.
        /// </summary>
        public void Pause()
        {
            playPauseButton.SetOnOff(false);
            frameTimer = 0f;
        }
        /// <summary>
        /// Stops the animation playback and resets the current frame number back to the start.
        /// </summary>
        public void Stop()
        {
            Pause();
            currentFrameIndex = 0;
        }

        /// <summary>
        /// Changes which frame is currently displayed.
        /// </summary>
        private void SetFrameIndex(int frameIndex)
        {
            int previousFrameIndex = _currentFrameIndex;
            _currentFrameIndex = frameIndex;

            if (previousFrameIndex != _currentFrameIndex)
            {
                UpdateOnionSkin();
                onCurrentFrameIndexChange.Invoke();
            }
        }

        /// <summary>
        /// Redisplay the frame number markers and the keyframe icons.
        /// </summary>
        private void UpdateDisplay()
        {
            ClearFrameNotches();

            // Resize background to last the length of the animation
            RectTransform timelineRectTransform = timeline.GetComponent<RectTransform>();
            timelineRectTransform.sizeDelta = new Vector2(Mathf.Max(5f, (fileManager.currentFile.numOfFrames + 2) * frameNotchSpacing * 2f), timelineRectTransform.sizeDelta.y);

            // Display frame number markers
            for (int i = 0; i < fileManager.currentFile.numOfFrames; i++)
            {
                FrameNotch notch = Instantiate(frameNotchPrefab, timeline).GetComponent<FrameNotch>();
                notch.transform.localPosition = new Vector3(FrameIndexToXCoord(i), 0f, 0f);
                notch.frameNum = i + 1;

                frameNotches.Add(notch);
            }

            // Display key frame icons
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

                    KeyFrameIcon keyFrameButtonScript = keyFrameButton.GetComponent<KeyFrameIcon>();
                    keyFrameButtonScript.frameIndex = keyFrame.frame;
                    keyFrameButtonScript.layerIndex = i;
                }
            }

            UpdateOnionSkin();
        }

        /// <summary>
        /// Remove all frame markers from the timeline, ready to be redisplayed.
        /// </summary>
        private void ClearFrameNotches()
        {
            foreach (FrameNotch notch in frameNotches)
            {
                Destroy(notch.gameObject);
            }

            frameNotches = new List<FrameNotch>();
        }

        /// <summary>
        /// Update the onion skin to show the previous frame.
        /// </summary>
        private void UpdateOnionSkin()
        {
            if (!showOnionSkin)
            {
                onionSkin.sprite = Texture2DExtensions.Transparent(fileManager.currentFile.width, fileManager.currentFile.height).ToSprite();
            }
            else if (currentFrameIndex > 0)
            {
                onionSkin.sprite = BlendMode.Multiply.Blend(
                    onionSkinColour,
                    fileManager.currentFile.Render(currentFrameIndex - 1)
                    ).ToSprite();
            }
            else if (currentFrameIndex == 0 && playbackMode.selectedOption == "loop")
            {
                onionSkin.sprite = BlendMode.Multiply.Blend(
                    onionSkinColour,
                    fileManager.currentFile.Render(currentFrameIndex - 1)
                    ).ToSprite();
            }
            else
            {
                onionSkin.sprite = Texture2DExtensions.Transparent(fileManager.currentFile.width, fileManager.currentFile.height).ToSprite();
            }
        }

        /// <summary>
        /// Converts frame number to the local x coord of the corresponding frame marker.
        /// </summary>
        private float FrameIndexToXCoord(int frameIndex)
        {
            return -viewport.rectTransform.sizeDelta.x / 2f + frameNotchSpacing * frameIndex;
        }

        /// <summary>
        /// Converts local coords to the frame number of the closest frame marker.
        /// </summary>
        private int CoordsToFrameIndex(Vector2 coords)
        {
            return XCoordToFrameIndex(coords.x);
        }
        /// <summary>
        /// Converts a local x coord to the frame number of the closest frame marker.
        /// </summary>
        private int XCoordToFrameIndex(float xCoord)
        {
            return Mathf.RoundToInt((viewport.scrollingArea.InverseTransformPoint(new Vector3(xCoord, 0f, 0f)).x + viewport.rectTransform.sizeDelta.x / 2f) / frameNotchSpacing);
        }

        /// <summary>
        /// Extends the length of the animation by adding one frame to the end.
        /// </summary>
        public void AddKeyFrame()
        {
            fileManager.currentFile.numOfFrames++;
            Debug.Log("Added key frame.");

            UpdateDisplay();
            onKeyFrameAdded.Invoke();
        }

        /// <summary>
        /// Reduces the length of the animation by removing the final frame.
        /// </summary>
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

        /// <summary>
        /// Deletes the selected key frame.
        /// </summary>
        public void DeleteSelectedKeyFrame()
        {
            if (keyFrameToggleGroup.currentToggle)
            {
                KeyFrameIcon keyFrameButtonScript = keyFrameToggleGroup.currentToggle.GetComponent<KeyFrameIcon>();
                fileManager.currentFile.layers[keyFrameButtonScript.layerIndex].DeleteKeyFrame(keyFrameButtonScript.frameIndex);

                UpdateDisplay();
                onKeyFrameDeleted.Invoke();
            }
        }

        /// <summary>
        /// For debugging purposes. Prints the current frame number and the frame numbers of each keyframe.
        /// </summary>
        public void DebugLogKeyFrames()
        {
            Debug.Log("Current frame: " + currentFrameIndex + ". Key frames at: " + layerManager.selectedLayer.keyFrameIndices.ToPrettyString());
        }

        /// <summary>
        /// Handles checking keyboard shortcuts and enacting the relevant actions.
        /// </summary>
        private void CheckKeyboardShortcuts()
        {
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("play / pause")))
            {
                playPauseButton.Press();
            }
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("previous frame")))
            {
                currentFrameIndex = (currentFrameIndex - 1).Mod(fileManager.currentFile.numOfFrames);
            }
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("next frame")))
            {
                currentFrameIndex = (currentFrameIndex + 1).Mod(fileManager.currentFile.numOfFrames);
            }
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("first frame")))
            {
                currentFrameIndex = 0;
            }
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("last frame")))
            {
                currentFrameIndex = fileManager.currentFile.numOfFrames - 1;
            }
        }

        /// <summary>
        /// Called when the play/pause button is pressed.
        /// </summary>
        private void OnPlayPause()
        {
            isPlaying = playPauseButton.on;
            frameTimer = 0f;
        }

        /// <summary>
        /// Called when the framerate is changed.
        /// </summary>
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

        /// <summary>
        /// Called when the playback mode is changed.
        /// </summary>
        private void OnPlaybackModeChanged()
        {
            playForwards = true;
        }

        /// <summary>
        /// Called when a keyframe is added.
        /// </summary>
        private void OnKeyFrameAdded()
        {
            UpdateDisplay();
        }

        /// <summary>
        /// Event is invoked when the current frame number changes.
        /// </summary>
        public void SubscribeToOnCurrentFrameIndexChange(UnityAction call)
        {
            onCurrentFrameIndexChange.AddListener(call);
        }
        /// <summary>
        /// Event is invoked when a keyframe is added.
        /// </summary>
        public void SubscribeToOnKeyFrameAdded(UnityAction call)
        {
            onKeyFrameAdded.AddListener(call);
        }
        /// <summary>
        /// Event is invoked when a keyframe is deleted.
        /// </summary>
        public void SubscribeToOnKeyFrameDeleted(UnityAction call)
        {
            onKeyFrameDeleted.AddListener(call);
        }
    }
}
