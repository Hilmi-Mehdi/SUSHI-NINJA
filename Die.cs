using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    public GameObject canvas;
    public bool alive = true;
    private void Start()
    {
        Time.timeScale = 1f;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            alive = false;
            Destroy(gameObject.GetComponent<Rigidbody>());
            Destroy(gameObject.GetComponent<Animator>());
            Time.timeScale = 0f;
            Cursor.visible = true;
            canvas.SetActive(true);
            
        }
    }
}
