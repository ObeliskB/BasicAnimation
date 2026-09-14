using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] GameObject canvas;

    private void Awake()
    {
        if (canvas != null) canvas.SetActive(false);
    }

    private void OnEnable()
    {
        if (director != null)
        {
            director.stopped += OnCutsceneFinished;
        }
    }

    private void OnDisable()
    {
        if (director != null)
        {
            director.stopped -= OnCutsceneFinished;
        }
    }

    private void OnCutsceneFinished(PlayableDirector pd)
    {
        if (canvas != null)
        {
            canvas.SetActive(true);
        }
    }
}
