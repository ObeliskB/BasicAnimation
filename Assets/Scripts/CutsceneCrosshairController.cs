using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private GameObject canvas;

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

    /*public void PlayCutscene()
    {
        if (canvas != null) canvas.SetActive(false);
        director.Play();
    }*/

    private void OnCutsceneFinished(PlayableDirector pd)
    {
        // เมื่อ Timeline จบ ให้เปิด UI Canvas กลับมา
        if (canvas != null)
        {
            canvas.SetActive(true);
        }
    }
}
