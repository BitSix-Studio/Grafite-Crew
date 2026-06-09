using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private AudioSource musicSource;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);

        musicSlider.value = savedVolume;
        musicSource.volume = savedVolume;

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
    }

    private void ChangeMusicVolume(float volume)
    {
        musicSource.volume = volume;

        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
}