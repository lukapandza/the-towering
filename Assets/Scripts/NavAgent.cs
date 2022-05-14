using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgent : MonoBehaviour
{
    public Animator animator;
    private List<Vector3> targets;
    private float moveDuration = .5f;
    private bool isMovingToTarget;
    private GameManager gameManager;

    void Awake()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(this.targets == null) this.targets = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.targets.Count > 0)
        {
            if(!this.isMovingToTarget)
            {
                this.animator.SetBool("IsWalking", true);
                StartCoroutine(SmoothMoveTo(this.targets[0]));
            }
        }
        else this.animator.SetBool("IsWalking", false);
        
    }

    private IEnumerator SmoothMoveTo(Vector3 target)
    {
        this.isMovingToTarget = true;

        float moveTime = 0;
        Vector3 start = this.gameObject.transform.position;

        //rotating towards next target:
        FaceTarget(target);

        while(moveTime < moveDuration)
        {
            moveTime += Time.deltaTime;
            this.gameObject.transform.position = Vector2.Lerp(start, target, moveTime / this.moveDuration);
            yield return null;
            //this.gameManager.RegisterLocation(this.gameObject);
        }

        this.isMovingToTarget = false;
        this.targets.RemoveAt(0);
        this.gameManager.RegisterLocation(this.gameObject);
    }

    private void FaceTarget(Vector3 target)
    {
        target.z = 0f;
 
        Vector3 objectPos = this.transform.position;
        target.x = target.x - objectPos.x;
        target.y = target.y - objectPos.y;
 
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    public void SetPath(List<Vector3> path)
    {
        this.targets = path;
    }

    public Vector3 GetDestination()
    {
        if(this.targets != null && this.targets.Count > 0) return this.targets[this.targets.Count - 1];
        else return this.gameObject.transform.position;
    } 
}
