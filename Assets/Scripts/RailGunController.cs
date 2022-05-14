using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailGunController : Tower
{
    private GameObject plasmaBallPrefab;
    private GameObject bullet = null;
    private bool isArmed = false;
    private float reloadTime = .6f;
    private float bulletForce = 20f;

    protected override void Awake()
    {
        this.plasmaBallPrefab = Resources.Load("Projectile/PlasmaBall") as GameObject;
        base.SetRange(3);
        base.Awake();
    }

    protected override void Update()
    {
        if(!this.isArmed) Reload();
        base.Update();
    }

    protected override void TargetAction(GameObject target)
    {
        if(target == null) return;
        FaceTarget(target.transform.position);
        if(this.isArmed)
        {
            Shoot();
            this.isArmed = false;
        } 
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

    private void Shoot()
    {
        if(this.bullet == null)
        {
            Reload();
            return;
        }
        Rigidbody2D rb = this.bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(this.transform.up * this.bulletForce, ForceMode2D.Impulse);

        Destroy(this.bullet, 2f); //in case it doesn't hit anything
        this.bullet = null;
    }

    private void Reload()
    {
        if(this.bullet == null)
        {
            this.bullet = Instantiate(this.plasmaBallPrefab, this.transform.position, this.transform.rotation);
            this.bullet.transform.SetParent(this.transform);
        } 
        
        if(this.reloadTime > 0)
        {
            SpriteRenderer sprite = this.bullet.GetComponent<SpriteRenderer>();
            this.reloadTime -= Time.deltaTime;
            float opacity = 1f - (this.reloadTime / .6f);
            sprite.color = new Color(1f, 1f, 1f, opacity);
        }
        else
        {
            this.isArmed = true;
            this.reloadTime = .6f;
        }
    }
}
