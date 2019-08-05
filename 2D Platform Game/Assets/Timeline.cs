using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Timeline : MonoBehaviour {

    private PlayableDirector playableDirector;

    private Dictionary<string, PlayableBinding> bindingDictionary = new Dictionary<string, PlayableBinding>();

    public UnityEvent OnOver = new UnityEvent();

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        foreach (PlayableBinding binding in playableDirector.playableAsset.outputs)
        {
            if (!bindingDictionary.ContainsKey(binding.streamName))
            {
                bindingDictionary.Add(binding.streamName, binding);
            }
        }
    }

    public void SetBinding()
    {
        playableDirector.SetGenericBinding(bindingDictionary["Player"].sourceObject, Player.instance.gameObject);
    }

    public void Play()
    {
        SetBinding();
        playableDirector.Play();
    }

    void OnEnable()
    {
        playableDirector.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        OnOver.Invoke();
    }

    void OnDisable()
    {
        playableDirector.stopped -= OnPlayableDirectorStopped;
    }
}
