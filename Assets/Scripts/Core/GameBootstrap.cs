using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    void Awake()
    {
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1.1f, 0f);
        var runner = player.AddComponent<RunnerController>();

        // Vrai personnage : fichier "Runner" dans Assets/Resources (sinon on garde la capsule)
        var model = Resources.Load<GameObject>("Runner");
        if (model != null)
        {
            player.GetComponent<MeshRenderer>().enabled = false;
            var visual = Instantiate(model, player.transform);
            visual.transform.localPosition = new Vector3(0f, -1f, 0f);
            visual.transform.localRotation = Quaternion.identity;
            foreach (var c in visual.GetComponentsInChildren<Collider>()) Destroy(c);

            // animation de course
            AnimationClip run = null;
            foreach (var c in Resources.LoadAll<AnimationClip>("Runner"))
            {
                if (!c.name.StartsWith("__preview__")) { run = c; break; }
            }
            var animator = visual.GetComponent<Animator>();
            if (animator == null) animator = visual.AddComponent<Animator>();
            animator.applyRootMotion = false;
            if (run != null) visual.AddComponent<ClipPlayer>().Play(animator, run);
        }

        var follow = Camera.main.gameObject.AddComponent<CameraFollow>();
        follow.target = player.transform;

        var gm = new GameObject("GameManager").AddComponent<GameManager>();
        gm.player = runner;

        var sp = new GameObject("Spawner").AddComponent<Spawner>();
        sp.player = player.transform;

        new GameObject("Scenery").AddComponent<Scenery>();
    }
}