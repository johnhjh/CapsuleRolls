using UnityEngine;

public class csAnimationSpin : MonoBehaviour
{

    Animation an;

    void Update()
    {
        an = gameObject.GetComponent<Animation>();
        an.Play();
    }
}
