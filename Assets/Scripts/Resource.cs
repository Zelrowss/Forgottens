using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{

    IEnumerator DestroyResource(){
        yield return new WaitForSeconds(60);
        Destroy(gameObject);
    }

    void Start(){
        StartCoroutine(DestroyResource());
    }

    void OnTriggerEnter(Collider other) {
        if (gameObject.CompareTag("Aurorium")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Iridium")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Biollumina")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Aurique")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Adamantite")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Prismatite")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }   
        else if (gameObject.CompareTag("Ether")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Arcanite")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Xénium")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Sélénium")) {
            if (!other.gameObject.CompareTag("Player")) return;

            Destroy(gameObject);
        }
    }
}
