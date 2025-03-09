using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AnimationCreator : MonoBehaviour
{
    public string spritesheetFolder = "Assets/Spritesheets/"; // Folder containing spritesheets
    public float frameDuration = 0.10f; // Duration per frame
    public RuntimeAnimatorController baseAnimatorController; // Assign in Inspector

    // Separate frame indices for SE/SW and NE/NW per animation type
    public int[] walkFramesSE_SW = { 0, 1, 2, 3, 1 };
    public int[] walkFramesNE_NW = { 4, 5, 6, 7, 5 };

    public int[] attackFramesSE_SW = { 8, 9, 10, 11, 9 };
    public int[] attackFramesNE_NW = { 12, 13, 14, 15, 13 };

    public int[] channelFramesSE_SW = { 16, 17, 18, 19, 17 };
    public int[] channelFramesNE_NW = { 20, 21, 22, 23, 21 };

    private readonly Dictionary<string, Vector3> directionScales = new Dictionary<string, Vector3>
    {
        { "SE", new Vector3(-5, 5, 5) },
        { "SW", new Vector3(5, 5, 5) },
        { "NE", new Vector3(-5, 5, 5) },
        { "NW", new Vector3(5, 5, 5) }
    };

    private readonly string[] baseAnimationNames = new string[]
    {
        "Player_Attack_NE", "Player_Attack_NW", "Player_Attack_SE", "Player_Attack_SW",
        "Player_Channel_NE", "Player_Channel_NW", "Player_Channel_SE", "Player_Channel_SW",
        "Player_Walk_NE", "Player_Walk_NW", "Player_Walk_SE", "Player_Walk_SW"
    };

    [ContextMenu("Create Animations From Folder")]
    public void CreateAnimationsFromFolder()
    {
        if (!Directory.Exists(spritesheetFolder))
        {
            Debug.LogError($"Spritesheet folder does not exist: {spritesheetFolder}");
            return;
        }

        if (baseAnimatorController == null)
        {
            Debug.LogError("No base Animator Controller assigned!");
            return;
        }

        string[] spritesheetPaths = Directory.GetFiles(spritesheetFolder, "*.png"); // Adjust if needed for other formats

        foreach (string path in spritesheetPaths)
        {
            Texture2D spriteSheet = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (spriteSheet == null)
            {
                Debug.LogWarning($"Could not load spritesheet at {path}");
                continue;
            }

            string characterName = Path.GetFileNameWithoutExtension(path); // Use filename as character name
            ProcessSpritesheet(characterName, spriteSheet);
        }
    }

    private void ProcessSpritesheet(string characterName, Texture2D spriteSheet)
    {
        // Load sprites from sprite sheet
        string spriteSheetPath = AssetDatabase.GetAssetPath(spriteSheet);
        Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath);

        // Manually filter sprites
        List<Sprite> spriteList = new List<Sprite>();
        foreach (Object asset in allAssets)
        {
            if (asset is Sprite)
                spriteList.Add((Sprite)asset);
        }
        Sprite[] sprites = spriteList.ToArray();

        if (sprites.Length == 0)
        {
            Debug.LogError($"No sprites found in {spriteSheet.name}");
            return;
        }

        Debug.Log($"Processing {characterName}: {sprites.Length} sprites found");

        // Ensure folder exists
        string characterFolder = $"Assets/Animations/{characterName}/";
        if (!Directory.Exists(characterFolder))
        {
            Directory.CreateDirectory(characterFolder);
            AssetDatabase.Refresh();
        }

        // Generate animations
        GenerateAnimations(characterFolder, characterName, "Walk", sprites, walkFramesSE_SW, walkFramesNE_NW, true, false);
        GenerateAnimations(characterFolder, characterName, "Attack", sprites, attackFramesSE_SW, attackFramesNE_NW, false, true);
        GenerateAnimations(characterFolder, characterName, "Channel", sprites, channelFramesSE_SW, channelFramesNE_NW, false, true);

        // Create the animation override controller
        CreateAnimatorOverrideController(characterFolder, characterName);
    }

    private void GenerateAnimations(string folderPath, string characterName, string animationName, Sprite[] sprites, int[] framesSE_SW, int[] framesNE_NW, bool shouldLoop, bool addAnimationEvent)
    {
        Debug.Log($" Generating `{animationName}` animations for {characterName}...");

        CreateAnimation(folderPath, characterName, animationName, "SE", sprites, framesSE_SW, shouldLoop, addAnimationEvent);
        CreateAnimation(folderPath, characterName, animationName, "SW", sprites, framesSE_SW, shouldLoop, addAnimationEvent);
        CreateAnimation(folderPath, characterName, animationName, "NE", sprites, framesNE_NW, shouldLoop, addAnimationEvent);
        CreateAnimation(folderPath, characterName, animationName, "NW", sprites, framesNE_NW, shouldLoop, addAnimationEvent);

        Debug.Log($" `{animationName}` animations generated for {characterName}.");
    }


    private void CreateAnimation(string folderPath, string characterName, string animationName, string direction, Sprite[] sprites, int[] frames, bool shouldLoop, bool addAnimationEvent)
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 1f / frameDuration;

        if (shouldLoop)
        {
            AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
        }

        List<ObjectReferenceKeyframe> spriteKeyframes = new List<ObjectReferenceKeyframe>();
        float totalTime = frames.Length * frameDuration;
        Vector3 scale = directionScales[direction];

        for (int i = 0; i < frames.Length; i++)
        {
            int index = frames[i];

            if (index < 0 || index >= sprites.Length)
            {
                Debug.LogWarning($"Index {index} is out of range of {characterName}'s sprite array.");
                continue;
            }

            float time = i * frameDuration;

            ObjectReferenceKeyframe spriteKey = new ObjectReferenceKeyframe
            {
                time = time,
                value = sprites[index]
            };
            spriteKeyframes.Add(spriteKey);
        }

        // Set sprite animation keyframes
        AnimationUtility.SetObjectReferenceCurve(clip,
            new EditorCurveBinding { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" },
            spriteKeyframes.ToArray());

        // ** Restore Scale Keyframes**
        Keyframe[] scaleKeyframesX =
        {
        new Keyframe(0f, scale.x),
        new Keyframe(totalTime, scale.x)
    };
        Keyframe[] scaleKeyframesY =
        {
        new Keyframe(0f, scale.y),
        new Keyframe(totalTime, scale.y)
    };
        Keyframe[] scaleKeyframesZ =
        {
        new Keyframe(0f, scale.z),
        new Keyframe(totalTime, scale.z)
    };

        AnimationUtility.SetEditorCurve(clip,
            new EditorCurveBinding { path = "", type = typeof(Transform), propertyName = "m_LocalScale.x" },
            new AnimationCurve(scaleKeyframesX));

        AnimationUtility.SetEditorCurve(clip,
            new EditorCurveBinding { path = "", type = typeof(Transform), propertyName = "m_LocalScale.y" },
            new AnimationCurve(scaleKeyframesY));

        AnimationUtility.SetEditorCurve(clip,
            new EditorCurveBinding { path = "", type = typeof(Transform), propertyName = "m_LocalScale.z" },
            new AnimationCurve(scaleKeyframesZ));

        // Add Animation Event (if applicable)
        if (addAnimationEvent)
        {
            AnimationEvent animEvent = new AnimationEvent();
            animEvent.functionName = "notifyEndOfAttack";
            animEvent.time = totalTime - (frameDuration * 0.5f);

            AnimationEvent[] events = new AnimationEvent[] { animEvent };
            AnimationUtility.SetAnimationEvents(clip, events);
        }

        string fileName = $"{characterName}_{animationName}_{direction}.anim";
        string path = $"{folderPath}{fileName}";

        AssetDatabase.CreateAsset(clip, path);
        AssetDatabase.SaveAssets();

        Debug.Log($" Created animation with scale keyframes: {fileName}");
    }


    private void CreateAnimatorOverrideController(string folderPath, string characterName)
    {
        AnimatorOverrideController overrideController = new AnimatorOverrideController(baseAnimatorController);
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        Debug.Log($"Creating override controller for {characterName}...");

        bool foundChannelNW = false;

        foreach (var baseAnimName in baseAnimationNames)
        {
            AnimationClip baseClip = overrideController[baseAnimName];

            if (baseClip != null)
            {
                string newClipName = $"{characterName}_{baseAnimName.Replace("Player_", "")}.anim";
                string newClipPath = $"{folderPath}{newClipName}";

                Debug.Log($" Looking for `{newClipPath}` in Unity...");
                AnimationClip newClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(newClipPath);

                if (newClip != null)
                {
                    overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(baseClip, newClip));
                    Debug.Log($" Overriding {baseAnimName} Å® {newClipPath}");

                    if (baseAnimName == "Player_Channel_NW")
                    {
                        foundChannelNW = true;
                    }
                }
                else
                {
                    Debug.LogError($" MISSING FILE: `{newClipPath}` NOT FOUND! Expected override for {baseAnimName}");
                }
            }
            else
            {
                Debug.LogWarning($" Base animation {baseAnimName} is missing in the Animator Controller.");
            }
        }

        overrideController.ApplyOverrides(overrides);

        string overridePath = $"{folderPath}{characterName}_OverrideController.overrideController";
        AssetDatabase.CreateAsset(overrideController, overridePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (!foundChannelNW)
        {
            Debug.LogError($" ERROR: `Player_Channel_NW` was NOT overridden for {characterName}! Check if `{folderPath}{characterName}_Channel_NW.anim` exists.");
        }
        else
        {
            Debug.Log($" SUCCESS: `Player_Channel_NW` successfully overridden for {characterName}.");
        }
    }





}
