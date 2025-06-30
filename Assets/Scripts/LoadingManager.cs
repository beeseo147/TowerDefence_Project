using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;
    
    [Header("UI Elements")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    [SerializeField] private Text tipText;
    
    [Header("Loading Tips")]
    [SerializeField] private string[] loadingTips = {
        "드론을 처치하면 아이템이 드롭됩니다!",
        "아이템을 사용하여 전투력을 향상시키세요!",
        "타워를 지켜 적의 공격을 막으세요!",
        "각 컨트롤러로 정확한 조준을 해보세요!"
    };
    
    private void Awake()
    {
        // Singleton 패턴
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
    
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }
    
    // Loading1 씬을 거쳐서 목표 씬으로 전환하는 메서드
    public void LoadSceneViaLoading(string targetSceneName)
    {
        StartCoroutine(LoadSceneViaLoadingAsync(targetSceneName));
    }
    
    public void LoadSceneViaLoading(int targetSceneIndex)
    {
        StartCoroutine(LoadSceneViaLoadingAsync(targetSceneIndex));
    }
    
    private void FindUIElements()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            var progressTextObj = canvas.transform.Find("progressText");
            if (progressTextObj != null)
                progressText = progressTextObj.GetComponent<Text>();

            var progressBarObj = canvas.transform.Find("LoadingSlider");
            if (progressBarObj != null)
                progressBar = progressBarObj.GetComponent<Slider>();

            var tipTextObj = canvas.transform.Find("tipText");
            if (tipTextObj != null)
                tipText = tipTextObj.GetComponent<Text>();
        }
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Loading Scene으로 전환
        SceneManager.LoadScene("Loading1");
        
        // Loading Scene이 완전히 로드될 때까지 대기
        yield return new WaitForSeconds(0.1f);
        
        // UI 요소들 찾기
        FindUIElements();
        
        // 랜덤 팁 표시
        if (tipText != null && loadingTips.Length > 0)
        {
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
        }
        
        // 실제 씬 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // 90%에서 멈춤
        
        while (!asyncLoad.isDone)
        {
            // 진행률 업데이트 (0.9까지)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            if (progressBar != null)
                progressBar.value = progress;
                
            if (progressText != null)
                progressText.text = $"{(progress * 100):F0}%";
            
            // 90% 이상이면 씬 활성화 허용
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
    
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        // Loading Scene으로 전환
        SceneManager.LoadScene("Loading1");
        
        yield return new WaitForSeconds(0.1f);
        
        // UI 요소들 찾기
        FindUIElements();
        
        // 랜덤 팁 표시
        if (tipText != null && loadingTips.Length > 0)
        {
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
        }
        
        // 실제 씬 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;
        
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            if (progressBar != null)
                progressBar.value = progress;
                
            if (progressText != null)
                progressText.text = $"{(progress * 100):F0}%";
            
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
    
    private IEnumerator LoadSceneViaLoadingAsync(string targetSceneName)
    {
        SceneManager.LoadScene("Loading1");
        yield return null; // 한 프레임 대기

        FindUIElements();

        if (tipText != null && loadingTips.Length > 0)
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;

        float timer = 0f;
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progress;
            if (progressText != null)
                progressText.text = $"{(progress * 100):F0}%";

            timer += Time.deltaTime;

            // 90% 이상 & 2초 이상 경과 시 씬 활성화
            if (asyncLoad.progress >= 0.9f && timer > 2f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    
    private IEnumerator LoadSceneViaLoadingAsync(int targetSceneIndex)
    {
        // Loading1 씬으로 전환
        SceneManager.LoadScene("Loading1");
        
        yield return new WaitForSeconds(0.1f);
        
        // UI 요소들 찾기
        FindUIElements();
        
        // 랜덤 팁 표시
        if (tipText != null && loadingTips.Length > 0)
        {
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
        }
        
        // 실제 목표 씬 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneIndex);
        asyncLoad.allowSceneActivation = false;
        
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            if (progressBar != null)
                progressBar.value = progress;
                
            if (progressText != null)
                progressText.text = $"{(progress * 100):F0}%";
            
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
    
} 