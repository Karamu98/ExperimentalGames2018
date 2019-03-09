using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DMPlayerController : NetworkBehaviour {

    public float g_fPlayerSpeed;
    public float g_fBulletSpeed;
    public GameObject bulletPrefab;
    public Transform EndOfBarrelPosition;

    private void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        float iX = Input.GetAxis("Horizontal") * Time.deltaTime * g_fPlayerSpeed;
        float iZ = Input.GetAxis("Vertical") * Time.deltaTime * g_fPlayerSpeed;

        float iY = transform.position.y;
        transform.Translate(iX, iY, iZ);

        if(Input.GetButtonDown("Jump"))
        {
            Fire();
        }
        
    
    }
    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void Fire()
    {
        GameObject Bullet = (GameObject)Instantiate(bulletPrefab, EndOfBarrelPosition.position, EndOfBarrelPosition.rotation);
        Bullet.GetComponent<Rigidbody>().velocity = Bullet.transform.forward * g_fBulletSpeed;
        Destroy(Bullet, 2.0f);
    }
}
