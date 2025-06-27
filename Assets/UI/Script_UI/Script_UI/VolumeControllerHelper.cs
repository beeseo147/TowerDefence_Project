using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VolumeControllerHelper : MonoBehaviour
{
    private VolumeController uiVolumeController;
    
    void Start()
    {
        uiVolumeController = VolumeController.Instance;
        if (uiVolumeController == null)
        {
            Debug.LogError("VolumeController를 찾을 수 없습니다!");
            return;
        }
        
        // 씬이 로드될 때마다 호출
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // 초기 BGM AudioSource들 찾기
        uiVolumeController.RefreshBGMAudioSources();
    }
    
    void OnDestroy()
    {
        // 이벤트 리스너 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene uiScene, LoadSceneMode uiMode)
    {
        // 씬이 로드된 후 잠시 기다린 다음 BGM AudioSource들 새로고침
        StartCoroutine(RefreshBGMAudioSourcesAfterDelay());
    }
    
    IEnumerator RefreshBGMAudioSourcesAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if (uiVolumeController != null)
        {
            uiVolumeController.RefreshBGMAudioSources();
        }
    }
    
    // 새로운 BGM AudioSource가 생성될 때 호출할 수 있는 public 메서드
    public void RegisterNewBGMAudioSource(AudioSource uiAudioSource)
    {
        if (uiVolumeController != null && uiAudioSource != null)
        {
            uiVolumeController.RegisterBGMAudioSource(uiAudioSource);
        }
    }
    
    // BGM GameObject를 찾아서 자동으로 등록하는 메서드
    public void FindAndRegisterBGMAudioSources()
    {
        if (uiVolumeController == null) return;
        
        // "BGM"이라는 이름의 GameObject들을 찾기
        GameObject[] uiAllGameObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject uiGameObject in uiAllGameObjects)
        {
            if (uiGameObject.name == "BGM")
            {
                AudioSource uiAudioSource = uiGameObject.GetComponent<AudioSource>();
                if (uiAudioSource != null)
                {
                    uiVolumeController.RegisterBGMAudioSource(uiAudioSource);
                }
            }
        }
    }
} 