
using UnityEngine.SceneManagement;
public class LoadSceneTrigger : TriggerBase
{
    public string _SceneName;

    protected override void OnGameTrigger()
    {
        if (string.IsNullOrEmpty(_SceneName))
            return;
           
            
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.UnloadSceneAsync(sceneIndex);
        SceneManager.LoadSceneAsync(_SceneName, LoadSceneMode.Additive);
    }
}

