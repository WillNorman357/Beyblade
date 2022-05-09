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

    //List<BladeController> bladeControllers = new List<BladeController>();

    int bladesNum;
    int bladesIndex = 0;

    [SerializeField] TextMeshProUGUI scoreTextMesh;
    [SerializeField] TextMeshProUGUI inputTextMesh;
    [SerializeField] TMP_InputField inputFieldClass;
    [SerializeField] TextMeshProUGUI peopleTextMesh;
    [SerializeField] GameObject inputObject;

    bool gotBladeNum = false;
    bool bladesSpawned = false;

    private void Awake()
    {
        
        radius = GetComponent<SphereCollider>().radius;
        instance = this;
        currentHitCount = Random.Range(minSmallHit, maxSmallHit);

        Time.timeScale = 0f;


        SpawnPoint[] tempPoints = points.GetComponentsInChildren<SpawnPoint>();
        spawnPoints = new SpawnPoint[tempPoints.Length];

        List<int> nums = new List<int>();
        int tempNum = 0;
        for (int i = 0; i < tempPoints.Length; i++)
        {
            nums.Add(tempNum);
            tempNum++;
        }

        for (int i = 0; i < tempPoints.Length; i++)
        {
            /*
            int tempIndex = nums[Random.Range(0, nums.Count)];
            spawnPoints[i] = tempPoints[tempIndex];
            nums.RemoveAt(tempIndex);
            */
        }

        spawnPoints = points.GetComponentsInChildren<SpawnPoint>();
        bladeControllers = new BladeController[spawnPoints.Length];
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
        if (Input.GetButtonDown("Fire2") && bladesSpawned == false)
        {
            Time.timeScale = 1f;
            bladesSpawned = true;
            

            BladeController[] bladeControllers1 = FindObjectsOfType<BladeController>();

            foreach (BladeController item in bladeControllers1)
            {
                item.EnableBlade();
                //item.SetHealth();
            }

            inputObject.SetActive(false);
        }

        if(inputObject.activeSelf == true)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SpawnBlade();
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
        
        if(bladesSpawned == false)
        {
            //if(bladesIndex > 0)
                //bladeControllers[bladesIndex-1].textMesh.text = inputTextMesh.text;

            SpawnBlade();
            //scoreTextMesh.text = "";
            //gotBladeNum = true;
        }
        else
        {
            /*
            bladeControllers[bladesIndex].textMesh.text = inputTextMesh.text;
            bladesIndex++;
            inputTextMesh.text = "";

            if(bladesIndex >= bladeControllers.Length)
            {
                inputObject.SetActive(false);
            }
            */
        }

        inputTextMesh.text = "";
    }

    public void AddNum()
    {
        bladesNum++;

        scoreTextMesh.text = bladesNum.ToString();
    }

    void SpawnBlade()
    {
        
        if (bladesIndex >= spawnPoints.Length)
        {
            return;
        }
        else
        {
            //bladeControllers.Add(Instantiate<GameObject>(bladeObject, spawnPoints[bladesIndex].transform.position, Quaternion.identity).GetComponent<BladeController>());
            //bladeControllers[bladesIndex].textMesh.text = inputTextMesh.text;

            bladeControllers[bladesIndex] = Instantiate<GameObject>(bladeObject, spawnPoints[bladesIndex].transform.position, Quaternion.identity).GetComponent<BladeController>();
            //bladeControllers[bladesIndex].textMesh.text = inputTextMesh.text;

            GiveBladeName();
        }
    }

    void GiveBladeName()
    {
        bladeControllers[bladesIndex].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = inputTextMesh.text;
        peopleTextMesh.text = peopleTextMesh.text + "\n" + inputTextMesh.text;
        inputFieldClass.text = "";
        bladesIndex++;
    }

}
