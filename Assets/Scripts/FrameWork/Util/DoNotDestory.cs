using UnityEngine;

public class DoNotDestory : MonoBehaviour
{
    private void Awake() => DontDestroyOnLoad(gameObject);
};

