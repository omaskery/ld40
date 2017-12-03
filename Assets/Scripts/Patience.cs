using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Text))]
public class Patience : MonoBehaviour
{
    public float StartingPatience = 30.0f;
    public float CurrentPatience;
    public float PatienceRecoveryRatePerBird = 0.0f;

	public void Start ()
    {
        m_Text = GetComponent<Text>();
        CurrentPatience = StartingPatience;
	}
	
	public void Update ()
    {
        var perchedBirdCount = FindObjectsOfType<BirdBehaviour>()
            .Where(bird => bird.State == BirdBehaviour.States.EnjoyingNicePerch)
            .Count();

        if(perchedBirdCount < 1)
        {
            CurrentPatience -= Time.deltaTime;
            if(CurrentPatience <= 0.0f)
            {
                SceneManager.LoadScene("lose", LoadSceneMode.Single);
            }
        }

        CurrentPatience += (PatienceRecoveryRatePerBird * perchedBirdCount) * Time.deltaTime;

        m_Text.text = string.Format("Patience: {0:00.00} seconds", CurrentPatience);
	}

    private Text m_Text;
}
