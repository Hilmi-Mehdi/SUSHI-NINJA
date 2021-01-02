using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonedSushi : MonoBehaviour
{
    // Start is called before the first frame update
    private void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        DiasGames.ThirdPersonSystem.Health Mout = new DiasGames.ThirdPersonSystem.Health();
        if(collision.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            Mout.Die(); 
        }
    }

}
