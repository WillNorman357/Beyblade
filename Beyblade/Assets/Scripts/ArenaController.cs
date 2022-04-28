using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ArenaController : MonoBehaviour
{
    [HideInInspector] public float radius;
    [SerializeField] int minSmallHit, maxSmallHit;
    int currentHitCount;
    int resetCount = 2;

    [SerializeField] GameObject points;
    SpawnPoint[] spawnPoints;

    public static ArenaController instance;

    [SerializeField] GameObject bladeObject;
    BladeController[] bladeControllers;

    int bladesNum;
    int bladesIndex = 0;

    [SerializeField] TextMeshProUGUI scoreTextMesh;
    [SerializeField] TextMeshProUGUI inputTextMesh;
    [SerializeField] GameObject inputObject;

    bool gotBladeNum = false;
    bool bladesSpawned = false;

    private void Awake()
    {
        radius = GetComponent<SphereCollider>().radius;
        instance = this;
        currentHitCount = Random.Range(minSmallHit, maxSmallHit);

        Time.timeScale = 0f;

        spawnPoints = points.GetComponentsInChildren<SpawnPoint>();
    }

    public int ResetSmallHitCount
    {
        get
        {
            if(resetCount <= 0)
            {
                currentHitCount = Random.Range(minSmallHit, maxSmallHit);
                resetCount = 2;
            }

            resetCount--;

            return currentHitCount;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Time.timeScale = 1f;

            BladeController[] bladeControllers = FindObjectsOfType<BladeController>();

            foreach (BladeController item in bladeControllers)
            {
                item.EnableBlade();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
        }
    }

    public void UpdateScore(string name)
    {
        scoreTextMesh.text =  name + " is out!" + "\n" + scoreTextMesh.text;
    }

    public void InputText()
    {
        if(gotBladeNum == false)
        {          
            SpawnBlades();
            gotBladeNum = true;
            scoreTextMesh.text = "";
        }
        else if(bladesSpawned == false)
        {
            bladeControllers[bladesIndex].textMesh.text = inputTextMesh.text;
            bladesIndex++;
            inputTextMesh.text = "";

            if(bladesIndex >= bladeControllers.Length)
            {
                inputObject.SetActive(false);
            }
        }

        inputTextMesh.text = "";
    }

    public void AddNum()
    {
        bladesNum++;

        scoreTextMesh.text = bladesNum.ToString();
    }

    void SpawnBlades()
    {
        bladeControllers = new BladeController[bladesNum];

        for (int i = 0; i < bladeControllers.Length; i++)
        {
            bladeControllers[i] = Instantiate(bladeObject, spawnPoints[i].transform.position, Quaternion.identity).GetComponent<BladeController>();
        }
    }
}
