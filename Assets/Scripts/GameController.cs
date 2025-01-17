using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    // player
    private int playerLifes = 5;
    private float maxSpeed = 11f;
    private int contabilizaMorte = 0;
    private int contabilizaBonusSpeed = 0;
    private int contabilizaBonusForce = 0;
    private bool saveProgress = false;

    //private Vector3 savePoint = Vector3.zero;

    // enemies
    public GameObject[] enemiesLevelOne;
    public GameObject[] enemiesLevelTwo;
    public GameObject[] enemiesLevelTree;
    public GameObject[] enemiesLevelFour;
    public GameObject[] enemiesLevelFive;
    public GameObject[] enemiesAcidron1;

    public static GameController instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name.Equals("LevelTree") ||
            SceneManager.GetActiveScene().name.Equals("LevelFour") || SceneManager.GetActiveScene().name.Equals("LevelFive"))
        {
            Destroy(GameObject.Find("BG"));
            GameObject.Find("numInfos").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("numInfos").transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void verifCollectibles()
    {
        if (saveProgress)
        {
            GameObject hud = GameObject.Find("HUD");
            if (hud != null && !SceneManager.GetActiveScene().name.Equals("Tutorial"))
            {
                hud.transform.GetChild(4).gameObject.SetActive(false);
            }
        }
        GameObject[] coletaveis = GameObject.FindGameObjectsWithTag("treasureChest");
        GameObject player = GameObject.Find("Simetra");
        if (coletaveis != null && player != null)
        {
            foreach (GameObject elem in coletaveis)
            {
                if (elem.transform.childCount == 2)
                {
                    float distance = Vector3.Distance(elem.transform.position, player.transform.position);
                    if (elem.transform.position.x < player.transform.position.x)
                    {
                        if (distance >= 6f)
                        {
                            GameObject hud = GameObject.Find("HUD");
                            if (hud != null && !SceneManager.GetActiveScene().name.Equals("Tutorial"))
                            {
                                float value = Mathf.PingPong(Time.time, 1f);
                                Color color = hud.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().color;
                                hud.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().color = new Vector4(color.r, color.g, color.b, value);
                                hud.transform.GetChild(4).gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        if (distance <= 20f)
                        {
                            if (elem.transform.parent != null)
                            {
                                if (elem.transform.parent.localPosition.x >= player.transform.position.x)
                                {
                                    GameObject hud = GameObject.Find("HUD");
                                    if (hud != null)
                                    {
                                        hud.transform.GetChild(4).gameObject.SetActive(false);
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log(elem.transform.localPosition.x);
                                if (elem.transform.localPosition.x >= player.transform.position.x)
                                {
                                    GameObject hud = GameObject.Find("HUD");
                                    if (hud != null)
                                    {
                                        hud.transform.GetChild(4).gameObject.SetActive(false);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
    }

    private void Update()
    {
        GameObject hud = GameObject.Find("HUD");
        if (hud != null && !SceneManager.GetActiveScene().name.Equals("Tutorial"))
        {
            verifCollectibles();
        }
    }

    public void SetContabilizaBonusForce()
    {
        contabilizaBonusForce = 0;
    } 

    public int GetContabiliaBonusSpeed()
    {
        return contabilizaBonusSpeed;
    }

    public GameObject[] GetEnemies(int level)
    {
        if(level == 1)
        {
            FindObjectOfType<LifeBarController>().InitSlider(1);
            return enemiesLevelOne;
        }else if (level == 2)
        {
            FindObjectOfType<LifeBarController>().InitSlider(2);
            return enemiesLevelTwo;
        } else if (level == 3)
        {
            FindObjectOfType<LifeBarController>().InitSlider(3);
            return enemiesLevelTree;
        } else if(level == 4)
        {
            FindObjectOfType<LifeBarController>().InitSlider(4);
            return enemiesLevelFour;
        }else if (level == 5)
        {
            return enemiesLevelFive;
        }
        else
        {
            return enemiesAcidron1;
        }
    }

    public void HurtPlayer(int danoEnemy)
    {        
        GameObject.Find("Simetra").GetComponent<PlayerController>().AnimationHurtPlayer();
        if(danoEnemy > 1)
        {
            while(danoEnemy > 0)
            {
                playerLifes--;
                if (playerLifes >= 0)
                {
                    Destroy(GameObject.Find("LifePlayer").transform.GetChild(playerLifes).gameObject);
                }
                danoEnemy--;
            }
        }
        else
        {
            playerLifes--;
            if(playerLifes >= 0)
            {
                Destroy(GameObject.Find("LifePlayer").transform.GetChild(playerLifes).gameObject);
            }

        }

        if (playerLifes <= 0)
        {
            GameObject.Find("Simetra").GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
            GameObject.Find("Simetra").GetComponent<PlayerController>().AnimationDeadPlayer();
            GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemys)
            {
                if (enemy.layer == 6)
                {
                    enemy.GetComponent<Enemy1Controller>().SetPlayerCheck(false);
                }

                if (enemy.layer == 7)
                {
                    enemy.GetComponent<Enemy2Controller>().SetPlayerCheck(false);
                }

                if (enemy.layer == 8)
                {
                    enemy.GetComponent<Enemy3Controller>().SetPlayerCheck(false);
                }

                if (enemy.layer == 9)
                {
                    enemy.GetComponent<Enemy4Controller>().SetPlayerCheck(false);
                }

                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    enemy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                }
                enemy.GetComponent<BoxCollider2D>().isTrigger = true;

            }
            GameObject boss = GameObject.FindWithTag("Boss");
            if(boss != null)
            {
                Rigidbody2D bossRigidbody = boss.GetComponent<Rigidbody2D>();
                if (bossRigidbody != null)
                {
                    boss.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    boss.GetComponent<CapsuleCollider2D>().isTrigger = true;
                }
            }
            Invoke("DeadPlayer", 1f);
            return;
        }
    }

    public void DeadPlayer()
    {
        if (contabilizaMorte == 0)
        {
            Invoke("GameOver", 2f);
            contabilizaMorte = 1;
        }
    }

    public bool isPlayerAlive()
    {
        return playerLifes > 0;
    }

    public void SetSafePoint(GameObject savepoint)
    {
        PlayerPrefs.SetFloat("saveX", savepoint.transform.position.x);
        PlayerPrefs.SetFloat("saveY", savepoint.transform.position.y);
    }

    public Vector3 GetSavePoint()
    {
        Vector3 savePointVector = new Vector3(PlayerPrefs.GetFloat("saveX"), PlayerPrefs.GetFloat("saveY"), 0f);
        return savePointVector;
    }

    public void HurtEnemy(GameObject enemy)
    {
        if (enemy.layer == 6)
        {
            enemy.GetComponent<Enemy1Controller>().Hurt(GameObject.Find("Simetra").GetComponent<PlayerController>().GetDano());
        }
        if (enemy.layer == 7)
        {
            enemy.GetComponent<Enemy2Controller>().Hurt(GameObject.Find("Simetra").GetComponent<PlayerController>().GetDano());
        }

        if (enemy.layer == 8)
        {
            enemy.GetComponent<Enemy3Controller>().Hurt(GameObject.Find("Simetra").GetComponent<PlayerController>().GetDano());
        }

        if (enemy.layer == 9)
        {
            enemy.GetComponent<Enemy4Controller>().Hurt(GameObject.Find("Simetra").GetComponent<PlayerController>().GetDano());
        }

        if (enemy.layer == 15)
        {
            enemy.GetComponent<enemyTarget>().Hurt(GameObject.Find("Simetra").GetComponent<PlayerController>().GetDano());
        }
    }

    public void GetCollectibles(GameObject collectable)
    {
        if(collectable.layer == 10) // force
        {
            if (contabilizaBonusForce == 0)
            {
                GameObject hud = GameObject.Find("HUD");
                if (hud != null)
                {
                    hud.transform.GetChild(4).gameObject.SetActive(false);
                }
                GameObject.Find("Simetra").GetComponent<PlayerController>().SetDanoPlayer(1);
                contabilizaBonusForce = 1;
            }
        }
        if(collectable.layer == 11) // speed
        {
            if(GameObject.Find("Simetra").GetComponent<PlayerController>().GetSpeed() <= maxSpeed)
            {
                if(contabilizaBonusSpeed == 0)
                {
                    GameObject hud = GameObject.Find("HUD");
                    if (hud != null)
                    {
                        hud.transform.GetChild(4).gameObject.SetActive(false);
                    }
                    GameObject.Find("Simetra").GetComponent<PlayerController>().SetSpeed(1);
                    contabilizaBonusSpeed = 1;
                }
                
            }
            
        }
    }

    public void ResetLifes()
    {
        playerLifes = 5;
    }

    void GameOver()
    {
        playerLifes = 5;
        if ((PlayerPrefs.GetFloat("saveX") != 0) && (PlayerPrefs.GetFloat("saveY") != 0))
        {
            saveProgress = true;
            if (contabilizaBonusForce != 0)
            {
                contabilizaBonusForce = 0;
            }  
        }
        else
        {
            saveProgress = false;
            contabilizaBonusSpeed = 0;
        }
        string derrota = SceneUtility.GetScenePathByBuildIndex(2);
        GameObject.Find("MenuController").GetComponent<MenuController>().PreviousScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(derrota);
        contabilizaMorte = 0;
    }

    public void LevelEnd()
    {
        playerLifes = 5;
        string vitoria = SceneUtility.GetScenePathByBuildIndex(1);
        string final = SceneUtility.GetScenePathByBuildIndex(13);

        saveProgress = false;
        PlayerPrefs.SetFloat("saveX", 0f);
        PlayerPrefs.SetFloat("saveY", 0f);

        GameObject.Find("MenuController").GetComponent<MenuController>().PreviousScene(SceneManager.GetActiveScene().buildIndex);

        //Index da ultima fase
        if (SceneManager.GetActiveScene().name.Equals("Tutorial") || SceneManager.GetActiveScene().name.Equals("Acidron1") || SceneManager.GetActiveScene().name.Equals("Acidron2"))
        {
            if(SceneManager.GetActiveScene().buildIndex == 12){
                SceneManager.LoadScene(final);
            }
            else{
                SceneManager.LoadScene(vitoria);
            }
        }
        else
        {
            if(contabilizaBonusForce == 1 && contabilizaBonusSpeed == 1)
            {
                if (SceneManager.GetActiveScene().buildIndex == 12)
                {
                    SceneManager.LoadScene(final);
                }
                else
                {
                    SceneManager.LoadScene(vitoria);
                }
                contabilizaBonusSpeed = 0;
                contabilizaBonusForce = 0;
            }
        }
    }
}
