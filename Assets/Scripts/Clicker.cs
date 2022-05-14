using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    private GameManager gameManager;
    private Camera cam;
    private bool isTowerSelected = false;
    private bool isPlaceable = false;
    private GameObject railGunPreview;

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.cam = this.gameObject.GetComponent<Camera>();

        //create ghostly tower and set it to inactive
        Object railGunPrefab = Resources.Load("Tower/RailGun");
        this.railGunPreview = (GameObject) Instantiate(railGunPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        SpriteRenderer[] renderers = this.railGunPreview.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer renderer in renderers)
        {
            renderer.color = new Color(1f, 1f, 1f, .6f);
        }
        this.railGunPreview.SetActive(this.isPlaceable);

        //for testing purposes:
        this.isTowerSelected = false;
    }

    void Update()
    {
        if(this.isTowerSelected)
        {
            Vector3 mousePos = this.cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0f);

            GraphNode nearestNode = this.gameManager.GetTileAt(mousePos);
            if(nearestNode.GetIsFree())
            {
                this.railGunPreview.transform.position = nearestNode.GetPosition();
                this.isPlaceable = true;
            }
            else this.isPlaceable = false;

            this.railGunPreview.SetActive(this.isPlaceable);

            if(Input.GetButtonDown("Fire1") && this.isPlaceable) this.gameManager.SpawnRailGunOnTile(nearestNode);
        }
    }

    public void ToggleIsPlaceable()
    {
        this.isTowerSelected = !this.isTowerSelected;
    }
}
