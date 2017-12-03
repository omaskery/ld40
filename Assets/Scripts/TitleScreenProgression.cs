using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenProgression : MonoBehaviour
{
    public float TitleDuration = 5.0f;
    public float UnskippableDuration = 1.0f;
    public string NextScene;
	
	void Update ()
    {
        m_TimeOnTitle += Time.deltaTime;
        if(m_TimeOnTitle >= TitleDuration)
        {
            m_ShouldSkip = true;
        }

        if(Input.anyKey)
        {
            m_ShouldSkip = true;
        }

        if (m_ShouldSkip && (m_TimeOnTitle >= UnskippableDuration))
        {
            SceneManager.LoadScene(NextScene, LoadSceneMode.Single);
        }
	}

    private bool m_ShouldSkip = false;
    private float m_TimeOnTitle = 0.0f;
}
