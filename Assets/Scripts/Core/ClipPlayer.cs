using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class ClipPlayer : MonoBehaviour
{
    PlayableGraph graph;
    AnimationClipPlayable playable;
    AnimationClip clip;

    public void Play(Animator animator, AnimationClip c)
    {
        clip = c;
        graph = PlayableGraph.Create("RunGraph");
        var output = AnimationPlayableOutput.Create(graph, "Anim", animator);
        playable = AnimationClipPlayable.Create(graph, clip);
        output.SetSourcePlayable(playable);
        graph.Play();
    }

    void Update()
    {
        // boucle l'animation
        if (playable.IsValid() && clip != null && clip.length > 0f)
            playable.SetTime(playable.GetTime() % clip.length);
    }

    void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }
}