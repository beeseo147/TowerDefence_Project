using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class VolumeController : MonoBehaviour
{
    [Header("UI References")]
    public Button uiVolumeUpButton;
    public Button uiVolumeDownButton;
    
    [Header("Volume Settings")]
    public float uiVolumeChangeSpeed = 1f;
    public float uiMaxVolume = 1f;
    public float uiMinVolume = 0f;
    
    [Header("Current Volume")]
    [SerializeField] private float uiCurrentVolume = 1f;
    
    private bool uiIsVolumeUpPressed = false;
    private bool uiIsVolumeDownPressed = false;
    private List<AudioSource> uiBGMAudioSources = new List<AudioSource>();
    
    public static VolumeController Instance { get; private set; }
    
    public float UICurrentVolume 
    { 
        get { return uiCurrentVolume; }
        set 
        { 
            uiCurrentVolume = Mathf.Clamp(value, uiMinVolume, uiMaxVolume);
            UpdateBGMAudioSourcesVolume();
        }
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SetupUIButtons();
        FindBGMAudioSources();
    }
    void OnEnable()
    {
        FindBGMAudioSources();
    }
    void Update()
    {
        HandleContinuousVolumeChange();
    }
    
    void SetupUIButtons()
    {
        if (uiVolumeUpButton != null)
        {
            uiVolumeUpButton.onClick.AddListener(OnVolumeUpClick);
            
            // 버튼을 꾹 누르고 있을 때의 이벤트
            EventTrigger uiVolumeUpTrigger = uiVolumeUpButton.gameObject.GetComponent<EventTrigger>();
            if (uiVolumeUpTrigger == null)
                uiVolumeUpTrigger = uiVolumeUpButton.gameObject.AddComponent<EventTrigger>();
            
            EventTrigger.Entry uiPointerDown = new EventTrigger.Entry();
            uiPointerDown.eventID = EventTriggerType.PointerDown;
            uiPointerDown.callback.AddListener((data) => { uiIsVolumeUpPressed = true; });
            uiVolumeUpTrigger.triggers.Add(uiPointerDown);
            
            EventTrigger.Entry uiPointerUp = new EventTrigger.Entry();
            uiPointerUp.eventID = EventTriggerType.PointerUp;
            uiPointerUp.callback.AddListener((data) => { uiIsVolumeUpPressed = false; });
            uiVolumeUpTrigger.triggers.Add(uiPointerUp);
        }
        
        if (uiVolumeDownButton != null)
        {
            uiVolumeDownButton.onClick.AddListener(OnVolumeDownClick);
            
            // 버튼을 꾹 누르고 있을 때의 이벤트
            EventTrigger uiVolumeDownTrigger = uiVolumeDownButton.gameObject.GetComponent<EventTrigger>();
            if (uiVolumeDownTrigger == null)
                uiVolumeDownTrigger = uiVolumeDownButton.gameObject.AddComponent<EventTrigger>();
            
            EventTrigger.Entry uiPointerDown = new EventTrigger.Entry();
            uiPointerDown.eventID = EventTriggerType.PointerDown;
            uiPointerDown.callback.AddListener((data) => { uiIsVolumeDownPressed = true; });
            uiVolumeDownTrigger.triggers.Add(uiPointerDown);
            
            EventTrigger.Entry uiPointerUp = new EventTrigger.Entry();
            uiPointerUp.eventID = EventTriggerType.PointerUp;
            uiPointerUp.callback.AddListener((data) => { uiIsVolumeDownPressed = false; });
            uiVolumeDownTrigger.triggers.Add(uiPointerUp);
        }
    }
    
    void HandleContinuousVolumeChange()
    {
        if (uiIsVolumeUpPressed)
        {
            UICurrentVolume += uiVolumeChangeSpeed * Time.deltaTime;
        }
        
        if (uiIsVolumeDownPressed)
        {
            UICurrentVolume -= uiVolumeChangeSpeed * Time.deltaTime;
        }
    }
    
    public void OnVolumeUpClick()
    {
        UICurrentVolume += 0.1f;
    }
    
    public void OnVolumeDownClick()
    {
        UICurrentVolume -= 0.1f;
    }
    
    void FindBGMAudioSources()
    {
        uiBGMAudioSources.Clear();
        
        // "BGM"이라는 이름의 GameObject들을 찾기
        GameObject[] uiAllGameObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject uiGameObject in uiAllGameObjects)
        {
            if (uiGameObject.name == "BGM")
            {
                AudioSource uiAudioSource = uiGameObject.GetComponent<AudioSource>();
                if (uiAudioSource != null)
                {
                    uiBGMAudioSources.Add(uiAudioSource);
                    Debug.Log($"BGM AudioSource 발견: {uiGameObject.name}");
                }
            }
        }
        
        // 찾은 BGM AudioSource들의 볼륨 설정
        UpdateBGMAudioSourcesVolume();
    }
    
    void UpdateBGMAudioSourcesVolume()
    {
        foreach (AudioSource uiAudioSource in uiBGMAudioSources)
        {
            if (uiAudioSource != null)
            {
                uiAudioSource.volume = uiCurrentVolume;
            }
        }
    }
    
    // 새로운 BGM AudioSource가 생성될 때 호출
    public void RegisterBGMAudioSource(AudioSource uiAudioSource)
    {
        if (uiAudioSource != null && !uiBGMAudioSources.Contains(uiAudioSource))
        {
            uiBGMAudioSources.Add(uiAudioSource);
            uiAudioSource.volume = uiCurrentVolume;
            Debug.Log("새로운 BGM AudioSource 등록됨");
        }
    }
    
    // BGM AudioSource가 제거될 때 호출
    public void UnregisterBGMAudioSource(AudioSource uiAudioSource)
    {
        if (uiAudioSource != null && uiBGMAudioSources.Contains(uiAudioSource))
        {
            uiBGMAudioSources.Remove(uiAudioSource);
            Debug.Log("BGM AudioSource 제거됨");
        }
    }
    
    // 씬이 변경될 때 새로운 BGM AudioSource들을 찾기 위해 호출
    public void RefreshBGMAudioSources()
    {
        FindBGMAudioSources();
    }
    
    // 현재 등록된 BGM AudioSource 개수 반환
    public int GetBGMAudioSourceCount()
    {
        return uiBGMAudioSources.Count;
    }
} 