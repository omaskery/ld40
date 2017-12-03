using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public float UpdateFrequency = 10.0f; // Hz
    public float BirdSpawnPeriod = 1.0f; // seconds

    public CatBehaviour[] CatPrefabs;
    public BirdBehaviour[] BirdPrefabs;

    public static int ExpectedCatCountForGivenBirdCount(int birdCount)
    {
        int catCount = 0;
        if(birdCount > 0 && birdCount <= 2)
        {
            catCount = 1;
        }
        else if(birdCount > 2 && birdCount <= 5)
        {
            catCount = 2;
        }
        else if(birdCount > 5 && birdCount <= 10)
        {
            catCount = 4;
        }
        else if(birdCount > 10)
        {
            catCount = 6;
        }
        return catCount;
    }

	public void Start ()
    {
        CatExits = FindObjectsOfType<CatExit>();
        BirdExits = FindObjectsOfType<BirdExit>();

        m_BirdSpawnTimer = BirdSpawnPeriod;
	}
	
	public void Update ()
    {
        m_UpdatePeriod = 1.0f / UpdateFrequency;
        m_UpdateTimer -= Time.deltaTime;
        if(m_UpdateTimer <= 0.0f)
        {
            m_UpdateTimer += m_UpdatePeriod;

            TrySpawnCat();
        }

        TrySpawnBird();

        m_BirdSpawnTimer -= Time.deltaTime;
	}
    
    private void TrySpawnBird()
    {
        if (m_BirdSpawnTimer > 0.0f) return;

        var allBirds = FindObjectsOfType<BirdBehaviour>();
        var allBirdsPerched = allBirds
            .All(bird => bird.State == BirdBehaviour.States.EnjoyingNicePerch);
        var perchedCount = allBirds.Where(bird => bird.State == BirdBehaviour.States.EnjoyingNicePerch).Count();

        ScoreSystem.FindMe().RecordBirdCount(perchedCount);

        if (allBirdsPerched == false) return;

        m_BirdSpawnTimer += BirdSpawnPeriod;

        SpawnBird();
    }

    private void TrySpawnCat()
    {
        var birds = FindObjectsOfType<BirdBehaviour>();
        var cats = FindObjectsOfType<CatBehaviour>();

        var expectedCatCount = ExpectedCatCountForGivenBirdCount(birds.Length);
        if(cats.Length < expectedCatCount)
        {
            SpawnCat();
        }
    }

    private void SpawnCat()
    {
        if ((CatExits.Length < 1) || (CatPrefabs.Length < 1)) return;

        var spawn = CatExits[m_Rng.Next(CatExits.Length)];
        var catPrefab = CatPrefabs[m_Rng.Next(CatPrefabs.Length)];
        var newCat = Instantiate(catPrefab);
        newCat.transform.position = spawn.transform.position;
    }

    private void SpawnBird()
    {
        if ((BirdExits.Length < 1) || (BirdPrefabs.Length < 1)) return;

        var spawn = BirdExits[m_Rng.Next(BirdExits.Length)];
        var birdPrefab = BirdPrefabs[m_Rng.Next(BirdPrefabs.Length)];
        var newBird = Instantiate(birdPrefab);
        newBird.transform.position = spawn.transform.position;
    }

    private CatExit[] CatExits;
    private BirdExit[] BirdExits;

    private float m_UpdatePeriod;
    private float m_UpdateTimer = 0.0f;

    private float m_BirdSpawnTimer = 0.0f;

    private System.Random m_Rng = new System.Random();
}
