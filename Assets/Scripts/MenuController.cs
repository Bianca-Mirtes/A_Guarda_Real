using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MenuController : MonoBehaviour
{
    //Constantes de menu
    public int play = 3;

    public static MenuController instance = null;
    private int previousSceneIndex;
    private string sceneName;
    private bool isPause = false; 

    private Button btnPlay;
    private Button btnMultiplayer;
    private Button btnDelSave;
    private Button btnCreditos;
    private Button btnQuit;

    private Button avancar;
    private Button sair;

    private Button continuarBtn;
    private Button configuracoesBtn;
    private Button menuInicialBtn;
    private Button menuDePauseBtn;

    private GameObject configMenuPausa;
    private GameObject menuPausa;
    private GameObject buttonContinuar;

    public GameObject pause;

    public GameObject gm;
    private AudioController controleDeAudio;
    public int indexCena;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        menuDePauseBtn = GameObject.Find("VoltarBtn").GetComponent<Button>();
        continuarBtn = GameObject.Find("ContinuarBtn").GetComponent<Button>();
        configuracoesBtn = GameObject.Find("ConfigBtn").GetComponent<Button>();
        menuInicialBtn = GameObject.Find("MenuBtn").GetComponent<Button>();

        configMenuPausa = GameObject.Find("configMenu");
        menuPausa = GameObject.Find("pauseMenu");
        buttonContinuar = GameObject.Find("ContinuarBtn");

        pause.SetActive(false);

        indexCena = 2;
        if (PlayerPrefs.GetInt("FaseAtual") != 0)
        {
            indexCena = 11;
        }
    }

    private void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void updateCombo()
    {
        TextMeshProUGUI comboTxt = GameObject.Find("comboTxt").GetComponent<TextMeshProUGUI>();
        PlayerController player = GameObject.Find("Simetra").GetComponent<PlayerController>();
        if (player.getCombo() > 1)
        {
            string old = comboTxt.text;
            comboTxt.text = "x" + player.getCombo();
            if (old != comboTxt.text)
                comboTxt.fontSize = comboTxt.fontSize + 22;
        }
        else
        {
            comboTxt.text = "";
            comboTxt.fontSize = 80;
        }
    }

    private void changeHudInfos()
    {
        PlayerController player = GameObject.Find("Simetra").GetComponent<PlayerController>();
        GameObject.Find("danoDeAtaqueTxt").GetComponent<TextMeshProUGUI>().text = "" + player.GetDano();
        GameObject.Find("velocidadeTxt").GetComponent<TextMeshProUGUI>().text = "" + player.GetSpeed();
        updateCombo();
    }

    //Define coisas para cenas especificas quando estas sao carregadas - util demais
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (scene.name == "Derrota" || scene.name == "Vitoria" || scene.name == "Placar")
        {
            // Encontra o bot�o pelo nome ou atrav�s de uma busca na hierarquia
            avancar = GameObject.Find("btn1").GetComponent<Button>();
            sair = GameObject.Find("btn2").GetComponent<Button>();
            avancar.onClick.AddListener(loadScene);
            sair.onClick.AddListener(mainMenu);

            if(scene.name == "Placar" && previousSceneIndex == 17)
            {
                GameObject.Find("btn1").SetActive(false);
                GameObject buttonSair = GameObject.Find("btn2");
                EventSystem.current.SetSelectedGameObject(buttonSair);
            }
        }

        if (scene.name == "FimDeJogo")
        {
            // Encontra o bot�o pelo nome ou atrav�s de uma busca na hierarquia
            sair = GameObject.Find("btn2").GetComponent<Button>();
            sair.onClick.AddListener(mainMenu);
        }

        if (scene.name == "MenuInicial"){
            btnPlay = GameObject.Find("PlayBtn").GetComponent<Button>();
            //PlayerPrefs.SetInt("FaseAtual", 0);
            int save = PlayerPrefs.GetInt("FaseAtual");
            if (save != 0){
                play = save;
                btnPlay.GetComponentInChildren<TextMeshProUGUI>().text = "Continuar";
                indexCena = 11;
            }
            else
            {
                btnPlay.GetComponentInChildren<TextMeshProUGUI>().text = "Novo Jogo";
            }
            btnPlay.onClick.AddListener(FadeOut);
            btnMultiplayer = GameObject.Find("MultiplayerBtn").GetComponent<Button>();
            btnMultiplayer.onClick.AddListener(PlayMultiplayer);
            btnDelSave = GameObject.Find("btn2").GetComponent<Button>();
            btnDelSave.onClick.AddListener(DelSave);
            btnQuit = GameObject.Find("ExitBtn").GetComponent<Button>();
            btnQuit.onClick.AddListener(QuitGame);
            btnCreditos = GameObject.Find("CreditosBtn").GetComponent<Button>();
            btnCreditos.onClick.AddListener(RunCredits);
        }


        //setar menu de pause
        if (sceneName != "MenuInicial" && sceneName != "Derrota" && sceneName != "Vitoria" && sceneName != "Placar")
        {
            //Salva fase atual pra continuar
            if(sceneName != "MLevelOne" && sceneName != "MLevelTwo" && sceneName != "MLevelTree" && sceneName != "MLevelFour")
            {
                PlayerPrefs.SetInt("FaseAtual", SceneManager.GetActiveScene().buildIndex);
            }
            controleDeAudio = GameObject.Find("AudioController").GetComponent<AudioController>();
            pause.SetActive(true);
            configMenuPausa.SetActive(true);
            GameObject volume = GameObject.Find("SliderVolume");
            Slider slider = volume.GetComponent<Slider>();
            slider.value = controleDeAudio.GetVolume();

            while (!pause.activeSelf && !buttonContinuar.activeSelf)
            { // Aguarda ate que o menu de pause esteja ativo
                
            }
            menuPause(buttonContinuar);
            continuarBtn.onClick.AddListener(resume);
            configuracoesBtn.onClick.AddListener(configPause);
            menuInicialBtn.onClick.AddListener(mainMenu);
            menuDePauseBtn.onClick.AddListener(menuPause);
            configMenuPausa.SetActive(false);
            pause.SetActive(false);
        }

        /*if (sceneName == "LevelOne" || sceneName == "LevelTwo" || sceneName == "LevelThree" || sceneName == "LevelFour" || sceneName == "LevelFive")
        { 
            changeHudInfos();
        }*/
    }
    

    // Update is called once per frame
    void Update(){
        if (sceneName == "LevelOne" || sceneName == "LevelTwo" || sceneName == "LevelTree" || sceneName == "LevelFour" || sceneName == "LevelFive" || sceneName == "Tutorial" || sceneName == "Acidron1")
        {
            changeHudInfos();
        }
        

        //Coloca o jogo em pause
        if (sceneName != "MenuInicial" && sceneName != "Derrota" && sceneName != "Vitoria" && sceneName != "Cutscene1" && sceneName != "Cutscene2")
        {
            float pausar = Input.GetAxisRaw("Pause");
            if (pausar>0 && !gm.activeSelf){ //REMOVER JUNTO COM O GM
                pause.SetActive(true);
                EventSystem.current.SetSelectedGameObject(buttonContinuar);
                Time.timeScale = 0f;
                isPause = true;
                menuPause();
            }
            //B pra voltar
            if(pause.activeSelf == true){
                float back = Input.GetAxisRaw("Fire2");
                if (back > 0){
                    GameObject configDePausa = GameObject.Find("configMenu");
                    if(configDePausa != null){
                        configMenuPausa.SetActive(false);
                        menuPausa.SetActive(true);
                    }
                    resume();
                }
            }
            
        }

        if (sceneName == "MenuInicial"){
            //B pra voltar
            float back = Input.GetAxisRaw("Fire2");
            if (back > 0){
                // && ConfiguracoesMenu.activeSelf
                GameObject menuPrincipal = GameObject.Find("UIController");
                UIController uiController = menuPrincipal.GetComponent<UIController>();
                uiController.menu();
                uiController.selectMain();
            }
        }

        if (sceneName == "Cutscene1"){
            GameObject videoPanel = GameObject.Find("VideoPanel");
            VideoPlayer videoPlayer = videoPanel.GetComponent<VideoPlayer>();
            //Debug.Log(videoPlayer.clip.name);

            //A/ENTER pra avancar
            bool forward1 = Input.GetButtonDown("Fire1");
            bool forward2 = Input.GetKeyDown(KeyCode.Return);
            if (forward1 || forward2){
                if(indexCena == 11){
                    /*indexCena = 2;*/
                    SceneManager.LoadScene(play+1);
                }

                Animator animator = GameObject.Find("TransicaoCutscene").GetComponent<Animator>();
                animator.Play("Cutscene");
                string cena = "Cena" + (indexCena++);
                videoPlayer.clip = (VideoClip)Resources.Load(cena);
                videoPlayer.isLooping = true;

                GameObject buttonNext = GameObject.Find("btnProximo");
                Vector2 newPosition = new Vector2(787f, -207.104f);
                buttonNext.GetComponent<Transform>().localPosition = newPosition;
            }
        }

        if (sceneName == "Cutscene2")
        {
            GameObject videoPanel = GameObject.Find("VideoPanel");
            VideoPlayer videoPlayer = videoPanel.GetComponent<VideoPlayer>();
            //Debug.Log(videoPlayer.clip.name);

            //A/ENTER pra avancar
            bool forward1 = Input.GetButtonDown("Fire1");
            bool forward2 = Input.GetKeyDown(KeyCode.Return);
            if (forward1 || forward2)
            {
                if (indexCena == 16)
                {
                    indexCena = 2;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
                }

                Animator animator = GameObject.Find("TransicaoCutscene").GetComponent<Animator>();
                animator.Play("Cutscene");
                string cena = "Cena" + (indexCena++);
                videoPlayer.clip = (VideoClip)Resources.Load(cena);
                videoPlayer.isLooping = true;
            }
        }

        if (sceneName == "Derrota" || sceneName == "Vitoria" || sceneName == "Placar")
        {
            if (Input.GetKey(KeyCode.LeftArrow)){
                GameObject button = GameObject.Find("btn1");
                EventSystem.current.SetSelectedGameObject(button);
            }
            else if (Input.GetKey(KeyCode.RightArrow)){
                GameObject button = GameObject.Find("btn2");
                EventSystem.current.SetSelectedGameObject(button);
            }

            float A = Input.GetAxisRaw("Fire1");
            float B = Input.GetAxisRaw("Fire2");

            if (A > 0){
                //Reinicia ou avanca fase
                loadScene();
            }
            else if (B > 0){
                //Vai pro menu
                mainMenu();
            }
        }

        if (sceneName == "FimDeJogo"){
            GameObject select = GameObject.Find("btn2");
            EventSystem.current.SetSelectedGameObject(select);
            float B = Input.GetAxisRaw("Fire2");
            if (B > 0){
                //Vai pro menu
                mainMenu();
            }
            PlayerPrefs.SetInt("FaseAtual", 0);

            indexCena = 2;
            play = 3;
        }
    }
    public void loadScene()
    {
        if (SceneManager.GetActiveScene().name.Equals("Derrota"))
        {
            string nameScene = SceneUtility.GetScenePathByBuildIndex(previousSceneIndex);
            SceneManager.LoadScene(nameScene);
        }
        if (SceneManager.GetActiveScene().name.Equals("Vitoria"))
        {
            SceneManager.LoadScene(previousSceneIndex + 1);
        }
        if (SceneManager.GetActiveScene().name.Equals("Placar"))
        {
            SceneManager.LoadScene(previousSceneIndex + 1);
        }

        //Debug.Log("Carregando cena (repetindo/avancando)");
    }

    public void PreviousScene(int index)
    {
        previousSceneIndex = index;
    }

    public int GetPreviusSceneIndex()
    {
        return previousSceneIndex;
    }

    public void mainMenu()
    {
        if (SceneManager.GetActiveScene().name.Equals("Vitoria"))
        {
            PlayerPrefs.SetInt("FaseAtual", PlayerPrefs.GetInt("FaseAtual")+1);
        }

        if (!SceneManager.GetActiveScene().name.Equals("Vitoria") && !SceneManager.GetActiveScene().name.Equals("Derrota"))
        {
            GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
            if (gameController != null)
            {
                gameController.ResetLifes();
            }
        }
        
        if (pause.activeSelf)
        {
            resume();
            Time.timeScale = 1f;
        }
        Debug.Log("Voltando pro menu inicial");
        SceneManager.LoadScene(0);
    }

    public void resume()
    {
        PlayerController player = GameObject.Find("Simetra").GetComponent<PlayerController>();
        player.resetaInputPulo();
        pause.SetActive(false);
        Time.timeScale = 1f;
        isPause = false;
    }

    public void configPause(){
        PlayerController player = GameObject.Find("Simetra").GetComponent<PlayerController>();
        player.resetaInputPulo();
        GameObject button = GameObject.Find("VolumeBtn");
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void menuPause(GameObject buttonContinuar)
    {
        GameObject button = GameObject.Find("ContinuarBtn");
        EventSystem.current.SetSelectedGameObject(buttonContinuar);
    }

    public void menuPause()
    {
        GameObject button = GameObject.Find("ContinuarBtn");
        EventSystem.current.SetSelectedGameObject(button);
    }

    public async void FadeOut()
    {
        GameObject.Find("CanvasFade").GetComponent<Canvas>().sortingOrder = 5;
        Animator animator = GameObject.Find("ImageFadeOut").GetComponent<Animator>();
        animator.SetBool("isFadeOut", true);
        await Task.Delay(1000);
        PlayGame();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(play);
    }

    public void PlayMultiplayer()
    {
        SceneManager.LoadScene(14);
    }
    public void RunCredits()
    { 
    }

    public void DelSave()
    {
        PlayerPrefs.SetInt("FaseAtual", 0);
        PlayerPrefs.SetFloat("saveX", 0f);
        PlayerPrefs.SetFloat("saveY", 0f);

        indexCena = 2;
        play = 3;

        btnPlay.GetComponentInChildren<TextMeshProUGUI>().text = "Novo Jogo";
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
