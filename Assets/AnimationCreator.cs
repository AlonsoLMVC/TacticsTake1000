using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AnimationCreator : MonoBehaviour
{
    public string characterName = "Hero"; // Character name
    public Texture2D spriteSheet; // Assign sprite sheet
    public float frameDuration = 0.10f; // Duration per frame

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

    private Sprite[] sprites;

    [ContextMenu("Create Animations")]
    public void CreateAndSaveAnimations()
    {
        if (string.IsNullOrEmpty(characterName))
        {
            Debug.LogError("Character name is empty!");
            return;
        }

        if (spriteSheet == null)
        {
            Debug.LogError("No sprite sheet assigned!");
            return;
        }

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
        sprites = spriteList.ToArray();

        if (sprites.Length == 0)
        {
            Debug.LogError("No sprites found in the assigned sprite sheet!");
            return;
        }

        Debug.Log($"Loaded {sprites.Length} sprites from {spriteSheet.name} (order preserved)");

        // Ensure folder exists
        string characterFolder = $"Assets/Animations/{characterName}/";
        if (!Directory.Exists(characterFolder))
        {
            Directory.CreateDirectory(characterFolder);
            AssetDatabase.Refresh();
        }

        // Generate animations
        GenerateAnimations(characterFolder, "Walk", walkFramesSE_SW, walkFramesNE_NW);
        GenerateAnimations(characterFolder, "Attack", attackFramesSE_SW, attackFramesNE_NW);
        GenerateAnimations(characterFolder, "Channel", channelFramesSE_SW, channelFramesNE_NW);
    }

    private void GenerateAnimations(string folderPath, string animationName, int[] framesSE_SW, int[] framesNE_NW)
    {
        CreateAnimation(folderPath, animationName, "SE", framesSE_SW);
        CreateAnimation(folderPath, animationName, "SW", framesSE_SW);
        CreateAnimation(folderPath, animationName, "NE", framesNE_NW);
        CreateAnimation(folderPath, animationName, "NW", framesNE_NW);
    }

    private void CreateAnimation(string folderPath, string animationName, string direction, int[] frames)
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 1f / frameDuration;

        List<ObjectReferenceKeyframe> spriteKeyframes = new List<ObjectReferenceKeyframe>();
        float totalTime = frames.Length * frameDuration;
        Vector3 scale = directionScales[direction];

        for (int i = 0; i < frames.Length; i++)
        {
            int index = frames[i];

            if (index < 0 || index >= sprites.Length)
            {
                Debug.LogWarning($"Index {index} is out of range of the sprites array.");
                continue;
            }

            float time = i * frameDuration;

            // Sprite keyframe
            ObjectReferenceKeyframe spriteKey = new ObjectReferenceKeyframe
            {
                time = time,
                value = sprites[index]
            };
            spriteKeyframes.Add(spriteKey);
        }

        // Set sprite animation
        AnimationUtility.SetObjectReferenceCurve(clip,
            new EditorCurveBinding { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" },
            spriteKeyframes.ToArray());

        // Scale animation (only two keyframes)
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

        // Save animation with characterName
        string fileName = $"{characterName}_{animationName}_{direction}.anim";
        string path = $"{folderPath}{fileName}";

        AssetDatabase.CreateAsset(clip, path);
        AssetDatabase.SaveAssets();

        Debug.Log($"Animation '{fileName}' created and saved at {path}");
    }
}
