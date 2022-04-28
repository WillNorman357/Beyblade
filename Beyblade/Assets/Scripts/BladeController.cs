using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BladeController : MonoBehaviour
{
    BladeSettings bSet;

    float health;
    int smallHitCount;

    Rigidbody rb;
    GameObject meshObject;
    Animator animator;

    Transform emptyObject;

    bool hitFloor = false;
    float floorHeight;

    float timer;

    public TextMeshProUGUI textMesh;

    float RelativeSpeed
    {
        get
        {
            float relSpeed = health / bSet.maxHealth;

            if (relSpeed <= bSet.relSpeedCap)
                return bSet.relSpeedCap;
            else
                return relSpeed;
        }
    }

    private void Start()
    {
        bSet = Resources.Load<BladeSettings>("BladeSettings");
        health = Random.Range(bSet.maxHealth - bSet.healthRange, bSet.maxHealth + bSet.healthRange);

        
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        meshObject = this.transform.GetChild(0).gameObject;
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        GameObject gameObject = new GameObject();
        Instantiate(gameObject, Vector3.zero, Quaternion.identity);
        emptyObject = gameObject.transform;

        rb.useGravity = false;
        GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        GetComponentInChildren<MeshRenderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    public void EnableBlade()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        rb.useGravity = true;

        Vector3 direction = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        rb.AddForce(direction.normalized * 3, ForceMode.Impulse);
    }


    void Update()
    {
        animator.speed = bSet.spinSpeed * RelativeSpeed;   
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*
        if (hitFloor == false)
        {
            hitFloor = true;
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        */

        if(hitFloor == false && collision.gameObject.tag == "Floor")
        {
            hitFloor = true;
            floorHeight = this.transform.position.y;

            //Vector3 forceDirection = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            //rb.AddForce(forceDirection.normalized * bSet.initialForce, ForceMode.Impulse);
        }

        Vector3 direction = this.transform.position - collision.GetContact(0).point;



        if (collision.gameObject.tag == "Blade")
        {
            smallHitCount--;
            if (smallHitCount <= 0)
            {
                smallHitCount = ArenaController.instance.ResetSmallHitCount;
                Debug.Log(smallHitCount);
                TakeDamage(bSet.bladeDmg);
                rb.AddForce(direction.normalized * bSet.bladeForce * RelativeSpeed, ForceMode.Impulse);
            }
            else
            {
                TakeDamage(bSet.smallHitDmg);
                rb.AddForce(direction.normalized * bSet.smallHitForce * RelativeSpeed, ForceMode.Impulse);
            }
        }
        if (collision.gameObject.tag == "Walls")
        {
            TakeDamage(bSet.wallDmg);
            rb.AddForce(direction.normalized * bSet.wallForce * RelativeSpeed, ForceMode.Impulse);
        }

        

        timer = 0f;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Blade")
        {
            timer += Time.deltaTime;
            Debug.Log(timer); 

            if(timer >= bSet.stayTime)
            {
                health -= bSet.bladeDmg;
                Vector3 direction = this.transform.position - collision.GetContact(0).point;
                rb.AddForce(direction.normalized * bSet.bladeForce * RelativeSpeed, ForceMode.Impulse);

                timer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = Vector3.zero - this.transform.position;
        direction = direction.normalized;

        emptyObject.transform.LookAt(this.transform.position);
        emptyObject.transform.localRotation = Quaternion.Euler(0, emptyObject.rotation.eulerAngles.y, emptyObject.rotation.eulerAngles.z);
        emptyObject.transform.Rotate(new Vector3(0, bSet.spinRotation, 0), Space.Self);

        direction =  emptyObject.transform.position - this.transform.position; //emptyObject.transform.right;
        direction = direction.normalized;
        float newForce = bSet.constForce * (Vector3.Distance(this.transform.position, emptyObject.position) / ArenaController.instance.radius);

        float speedMod;
        if (RelativeSpeed <= 0.80)
            speedMod = 0.80f;
        else
            speedMod = RelativeSpeed;

        if(hitFloor)
            rb.position = new Vector3(this.transform.position.x, floorHeight, this.transform.position.z);

        rb.AddForce((direction * newForce) * speedMod, ForceMode.Force);
    }

    void TakeDamage(float damage)
    {
        if(hitFloor != true)
        {
            return;
        }

        damage = Random.Range(damage * (1 + bSet.dmgRange), damage * (1 - bSet.dmgRange));
        health -= damage;

        if (health <= 0)
        {
            rb.constraints = RigidbodyConstraints.None;
            this.gameObject.tag = "Player";
            animator.speed = 0;

            Vector3 direction = meshObject.transform.up + meshObject.transform.right;
            rb.AddForce(direction.normalized * 30f, ForceMode.Impulse);
            GetComponent<Collider>().isTrigger = true;
            ArenaController.instance.UpdateScore(textMesh.text);
            Destroy(this);
        }
    }
}
