using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreSystem : MonoBehaviour
{
    public int CatsInOrbit = 0;
    public int BirdsLost = 0;
    public int MostBirds = 0;

    public static ScoreSystem FindMe()
    {
        return FindObjectOfType<ScoreSystem>();
    }

	public void Start ()
    {
        m_Text = GetComponent<Text>();
	}

    public void CatLaunchedToOrbit()
    {
        CatsInOrbit++;
        UpdateText();
    }

    public void BirdLost()
    {
        BirdsLost++;
        UpdateText();
    }

    public void RecordBirdCount(int count)
    {
        if(count > MostBirds)
        {
            MostBirds = count;
            UpdateText();
        }
    }
	
	private void UpdateText ()
    {
        var builder = new StringBuilder();
        builder.AppendFormat("Most Birds: {0}", MostBirds);
        if(BirdsLost > 0)
        {
            builder.AppendFormat("\nBirds Lost: {0}", BirdsLost);
        }
        if(CatsInOrbit > 0)
        {
            builder.AppendFormat("\nCats In Orbit: {0}", CatsInOrbit);
        }
        m_Text.text = builder.ToString();
	}

    private Text m_Text;
}
