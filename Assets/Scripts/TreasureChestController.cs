using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChestController : MonoBehaviour
{
    Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        Vector3 save = FindObjectOfType<GameController>().GetSavePoint();
        int contabilizaBonusSpeed = FindObjectOfType<GameController>().GetContabiliaBonusSpeed();
        if (save != Vector3.zero && transform.name.Equals("TreasureChestSpeed") && contabilizaBonusSpeed == 1)
        {
            ani.SetBool("isOpen", true);
            transform.GetChild(1).gameObject.SetActive(true);
            Destroy(transform.GetChild(1).gameObject);
        }
    }

    public void AnimationOpen()
    {
        ani.SetBool("isOpen", true);
        transform.GetChild(1).gameObject.SetActive(true);
    }
}
