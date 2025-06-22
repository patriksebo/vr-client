using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource audioOutput;
    private float[] samples;
    private bool isMicrophoneEnabled;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    
    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = device;
            UIManager.instance.microphones.options.Add(option);
        }
    }

    public void StartMicrophone()
    {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(Microphone.devices[UIManager.instance.microphones.value], true, 3, 44100);
        isMicrophoneEnabled = true;
        InvokeRepeating("StartSampleMicrophoneAudio", 3f, 3f);
    }
    
    private void StartSampleMicrophoneAudio()
    {
        if(isMicrophoneEnabled)
            StartCoroutine(SampleMicrophoneAudio());
    }
    
    private IEnumerator SampleMicrophoneAudio()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        while (audioSource.clip.loadState != AudioDataLoadState.Loaded)
        {
            yield return null;
        }
        samples = new float[audioSource.clip.samples * audioSource.clip.channels];
        audioSource.clip.GetData(samples, 0);
        for (int i = 0; i < samples.Length; ++i)
        {
            samples[i] = samples[i] * 0.5f;
        }
        Client.instance.SendVoiceMessage(samples);
    }

    public void MicrophoneEnable()
    {
        isMicrophoneEnabled = true;
    }

    public void MicrophoneDisable()
    {
        isMicrophoneEnabled = false;
    }
    
    public void ReceiveData(float[] _data)
    {
        AudioClip clip = AudioClip.Create("clip", 44100 * 3, 1, 44100, false);
        audioOutput.clip = clip;
        audioOutput.clip.SetData(_data, 0);
        audioOutput.Play();
    }
}
