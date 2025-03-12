using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePictureUIManager : MonoBehaviour
{
    [SerializeField] private Image backgroundImage; // Background image
    [SerializeField] private Image profileImage; // Profile image

    [SerializeField] private Slider expSlider; // EXP bar

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI queuePositionText;

    // Method to set the background image
    public void SetBackgroundImage(Sprite newBackground)
    {
        if (backgroundImage != null)
        {
            backgroundImage.sprite = newBackground;
        }
    }

    // Method to set the profile picture
    public void SetProfileImage(Sprite newProfileImage)
    {
        if (profileImage != null)
        {
            profileImage.sprite = newProfileImage;
        }
        else
        {
            Debug.Log("SPRITE IS NULL PROBLEM HERE");
        }
    }

    // Method to set EXP slider value (0 to 1)
    public void SetExpSlider(float value)
    {
        if (expSlider != null)
        {
            expSlider.value = Mathf.Clamp01(value); // Ensures value stays between 0 and 1
        }
    }

    // Method to update level text
    public void SetLevelText(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Lv." + level;
        }
    }

    // Method to update EXP text
    public void SetExpText(int currentExp, int maxExp)
    {
        if (expText != null)
        {
            expText.text = $"{currentExp} / {maxExp} EXP";
        }
    }

    // Method to update queue position text
    public void SetQueuePositionText(int position)
    {
        if (queuePositionText != null)
        {
            queuePositionText.text = "Queue Position: " + position;
        }
    }
}
