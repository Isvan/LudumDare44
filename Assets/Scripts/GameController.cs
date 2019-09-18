using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public blinder fullBlind;
    public blinder partBlind;


    public GameObject player;
    public List<GameObject> baddiesList;
    public GameObject[] baddieRef;
    public List<GameObject> spawnPoints;
    public float cameraHeight = -30;

    public Button[] btns;

    private float enemyMoveSpeedMod = 1.0f;

    public GameObject[] weapons;

    public bool paused;
    private bool wasHealding = false;

    public bool waitingForWave = true;
    public Canvas pauseCanvas;
    public Canvas sacrificeCanvas;
    public Canvas buffCanvas;
    public Canvas HUDCanvas;
    public Canvas IntroCanvas;
    public Canvas OutroCanvas;

    private int gameState = 0;

    private CanvasGroup pauseMenu;
    private CanvasGroup sacrifceMenu;
    private CanvasGroup buffMenu;
    private CanvasGroup hudGroup;
    private CanvasGroup IntroMenu;



    public Text sacrificeDesc;
    public Text BuffDesc;
    public Text waveText;

    public CanvasGroup[] icons;

    private int selectedBuffPicked;
    private int selectedSacrificePicked;
    private int[] pickedSacrifices;

    public int wave;
    // Use this for initialization
    void Start()
    {
        paused = true;
        pauseMenu = pauseCanvas.GetComponent<CanvasGroup>();
        sacrifceMenu = sacrificeCanvas.GetComponent<CanvasGroup>();
        buffMenu = buffCanvas.GetComponent<CanvasGroup>();
        IntroMenu = IntroCanvas.GetComponent<CanvasGroup>();
    
        hudGroup = HUDCanvas.GetComponent<CanvasGroup>();
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i] = GameObject.Instantiate(weapons[i]);
        }

        mainChar playerS = player.GetComponent<mainChar>();
        playerS.updateWeapon(0, weapons[0]);
        playerS.updateWeapon(1, weapons[1]);
        playerS.updateWeapon(2, weapons[2]);


        pickedSacrifices = new int[4];
        pickedSacrifices[0] = 0;
        pickedSacrifices[1] = 0;
        pickedSacrifices[2] = 0;
        pickedSacrifices[3] = 0;

    }

    public void startGame()
    {
        hideUI(IntroMenu);
        ShowUI(hudGroup);

    }

    public void playAgain()
    {

        resetGame();
   
    }

    // Update is called once per frame
    void Update()
    {

        if (player.GetComponent<mainChar>().dead)
        {
         
            foreach (GameObject bad in baddiesList)
            {
                Destroy(bad);
            }

            baddiesList.Clear();
            resetGame();

        }

        int dead = 0;

        foreach (GameObject bad in baddiesList)
        {
            if (bad == null)
            {
                dead++;
            }
        }

        if (dead == baddiesList.Count && !waitingForWave)
        {
            baddiesList.Clear();
            waveOver();     
        }



        Vector3 newPos = player.transform.position;
        newPos.z += cameraHeight;

        transform.position = newPos;

    }

    void SpawnBaddie()
    {
        Debug.Log("Spawn called");
        int pick = Random.Range(0, spawnPoints.Count);

        Vector3 spawnPos = spawnPoints[pick].transform.position;
        spawnPos.x += Random.Range(-2f, 2f);
        spawnPos.y += Random.Range(-2f, 2f);

        pick = Random.Range(0, baddieRef.Length);
        GameObject newBad = Instantiate(baddieRef[pick], spawnPos, new Quaternion());

        newBad.GetComponent<Baddie>().targetObj = player;
        newBad.GetComponent<Baddie>().speed *= enemyMoveSpeedMod;

        baddiesList.Add(newBad);
    }

    void nextWave()
    {

        Debug.Log("Next Wave Called");
        mainChar p = player.GetComponent<mainChar>();
        if (wasHealding) {

            p.hpRegen = true;

            } 

        wave++;
        waveText.text = "Wave : " + wave;
        //Spawn All the new Baddies

        //Pop Up Menu to sacrifce stuff
        for (int i = 0; i < wave * 5 + 10; i++)
        {
            SpawnBaddie();
        }
        //Start next wave
    }

    void resetGame()
    {

      
        ShowUI(IntroMenu);

        foreach(Button btn in btns)
        {
            if(btn != null)
            {
                btn.interactable = true;
            }
        }

        foreach (GameObject bad in baddiesList)
        {
            Destroy(bad);
        }

        baddiesList.Clear();
        waveOver();
        wave = 0;
        waveText.text = "Wave : " + wave;
        mainChar playerS = player.GetComponent<mainChar>();
        playerS.currentHp = playerS.hp;
        playerS.dead = false;

        foreach(CanvasGroup C in icons)
        {
            C.alpha = 0;
        }

        for(int i = 0;i < pickedSacrifices.Length; i++)
        {
            pickedSacrifices[i] = 0;
        }

        fullBlind.hide();
        partBlind.hide();

        //Reduce attack speed my 1/2
        for (int i = 0; i < weapons.Length; i++)
        {

            if (weapons[i].tag.Equals("Melee"))
            {
                MeleeAttack mA = weapons[i].GetComponent<MeleeAttack>();
                
                    mA.attackSpeedMod = 1.0f;
                


            }
            else
            {
                RangedAttack mA = weapons[i].GetComponent<RangedAttack>();
                
              
                    mA.attackSpeed = 1.0f;
            }
            

        }


        mainChar p = player.GetComponent<mainChar>();
        p.slowDown = 1.0f;
        p.speedMod = 1.0f;
        p.hpRegen = false;
        wasHealding = false;
        p.canEat = true;
    }

    void waveOver()
    {
        gameState = 2;
        mainChar p = player.GetComponent<mainChar>();
        waitingForWave = true;
        if (p.hpRegen)
        {
            wasHealding = true;
            p.hpRegen = false;
        }
        ShowUI(pauseMenu);
        //Reset HP of Char

        //Show pop up for sacrifice options


    }

    void hideUI(CanvasGroup can)
    {
        can.alpha = 0f;
        can.blocksRaycasts = false;

    }

    void ShowUI(CanvasGroup can)
    {
        can.alpha = 1f;
        can.blocksRaycasts = true;
    }

    public void backToPauseMenu()
    {
        hideUI(sacrifceMenu);
        ShowUI(pauseMenu);

    }

    public void goToSacrifceMenu()
    {

        hideUI(pauseMenu);
        ShowUI(sacrifceMenu);

    }

    public void readyForNextWave()
    {
        waitingForWave = false;
        //Debug.Log("Clicked");
        hideUI(pauseMenu);
        nextWave();


    }

     
    public void activateSacrifice()
    {

        hideUI(sacrifceMenu);
        ShowUI(buffMenu);
   

        mainChar p = player.GetComponent<mainChar>();
        switch (selectedSacrificePicked)
        {
            case 1:

                //Reduce attack speed my 1/2
                for(int i = 0;i < weapons.Length; i++)
                {

                    if (weapons[i].tag.Equals("Melee"))
                    {
                        MeleeAttack mA = weapons[i].GetComponent<MeleeAttack>();
                        if (pickedSacrifices[0] == 0)
                        {
                            mA.attackSpeedMod = 2.0f;
                            
                                

                        }
                        else
                        {
                            mA.attackSpeedMod = 200.0f;
                            
                        }

                          
                        
                    }else{
                        RangedAttack mA = weapons[i].GetComponent<RangedAttack>();
                       

                        if (pickedSacrifices[0] == 0){
                            mA.attackSpeedMod = 2.0f;
                           
                        }
                        else
                        {
                            
                            mA.attackSpeed = 200.0f;
                        }
                    }

                }

                pickedSacrifices[0]++;

                if(pickedSacrifices[0] == 1)
                {
                    icons[1].alpha = 1.0f;
                }else
                {
                    btns[10].interactable = false;
                    icons[5].alpha = 1.0f;
                }

                break;

            case 2:
                //Reduce Vision by half

                if(pickedSacrifices[1] == 0)
                {
                    pickedSacrifices[1]++;
                    icons[0].alpha = 1.0f;
                    partBlind.show();
                }
                else
                {
                    icons[4].alpha = 1.0f;
                    fullBlind.show();
                    partBlind.hide();
                    btns[9].interactable = false;
                }

                break;
            case 3:
                //Reduce Movement Speed by half

                if (pickedSacrifices[2] == 0)
                {
                    p.speedMod *= 0.5f;
                    pickedSacrifices[2]++;
                    icons[2].alpha = 1.0f;
                }
                else
                {
                    p.speedMod = 0.0f;
                    icons[6].alpha = 1.0f;
                    btns[11].interactable = false;
                }
               
                break;
            case 4:
                //Remove hp gain from drops
                icons[3].alpha = 1.0f;
                p.canEat = false;
                btns[8].interactable = false;
                break;
        }

        selectedSacrificePicked = -1;

    }   

    public void selectedSacrifice(int option)
    {
        //  Debug.Log("Option :" + option + " was presed");

        selectedSacrificePicked = option;

        switch (option)
        {
           

            case 1:
               // Debug.Log("Arm Selected");
                if(pickedSacrifices[0] == 0)
                {

                    sacrificeDesc.text = "Sacrifice one of your arms. Reduce your attack speed by half.";
                }
                else
                {

                    sacrificeDesc.text = "Sacrifice your other arm. Remove your ability to use weapons.";
                }
                break;

            case 2:


                //Debug.Log("Eye Selected");
                if (pickedSacrifices[1] == 0)
                {
                    sacrificeDesc.text = "Sacrifice one of your eyes. Reduce your field of view by half.";
                }
                else
                {

                    sacrificeDesc.text = "Sacrifice your other eye. Reduce your field of view to only your immediate surroundings.";
                }
                    break;

            case 3:
                //Debug.Log("Leg Selected");
                if (pickedSacrifices[2] == 0)
                {
                    sacrificeDesc.text = "Sacrifice one of your legs. Reduce your movement speed by half.";
                }
                else
                {

                    sacrificeDesc.text = "Sacrifice your other leg. Remove your ability to walk.";
                }
               
                    break;

            case 4:
                //Debug.Log("Tongue Selected");
                if (pickedSacrifices[3] == 0)
                {
                    sacrificeDesc.text = "Sacrifice your tongue. Remove your ability to consume health potions.";
                }else
                {

                }
                    break;
        }
    }

    public void selectedBuff(int option)
    {
        selectedBuffPicked = option;
        switch (option)
        {
            case 1: //Attack speed
                BuffDesc.text = "Double your attack speed with all weapons.";
                
                break;

            case 2: //Healing
                BuffDesc.text = "Regenerate health over time.";
               
                break;

            case 3: ///Power Strike
                BuffDesc.text = "Increase weapon damage. Arrows can pierce through demons, and melee weapons can cleave multiple demons.";
                
                break;

            case 4: //Teleportation
                BuffDesc.text = "Gain the ability to teleport a short distance once every two seconds.";
                
                break;

            case 5: //More HP
                BuffDesc.text = "Double your health pool and reset your health to full.";
                
                break;

            case 6: //FireBall
                BuffDesc.text = "Gain the ability to cast bursts of flame once every two seconds.";
           
                break;

            case 7: //Enemy Slow
                BuffDesc.text = "Reduce demon movement speed by half.";
                
                break;


        }
    }


    public void aquireBuff()
    {


        mainChar p = player.GetComponent<mainChar>();
        //Apply selected buff
        switch (selectedBuffPicked)
        {
            case 1: //Attack speed
                btns[1].interactable = false;
                icons[7].alpha = 1.0f;

                //Reduce attack speed my 1/2
                for (int i = 0; i < weapons.Length; i++)
                {

                    if (weapons[i].tag.Equals("Melee"))
                    {
                        MeleeAttack mA = weapons[i].GetComponent<MeleeAttack>();
                       
                            mA.attackSpeedMod = 0.5f;
                        
                    }
                    else
                    {
                        RangedAttack mA = weapons[i].GetComponent<RangedAttack>();
                        
                            mA.attackSpeed = 0.5f;
                        
                    }

                }

                break;

            case 2: //Healing
                btns[2].interactable = false;
                icons[9].alpha = 1.0f;
                p.hpRegen = true;
                break;

            case 3: ///Power Strike
                btns[3].interactable = false;
                icons[12].alpha = 1.0f;

                //Reduce attack speed my 1/2
                for (int i = 0; i < weapons.Length; i++)
                {

                    if (weapons[i].tag.Equals("Melee"))
                    {
                        MeleeAttack mA = weapons[i].GetComponent<MeleeAttack>();

                        mA.cleaveNumber *= 2;
                        mA.attackDamage *= 2;

                    }
                    else
                    {
                        RangedAttack mA = weapons[i].GetComponent<RangedAttack>();

                        mA.peirceMod = 2;

                    }

                }



                break;

            case 4: //Teleportation
                btns[4].interactable = false;
                icons[10].alpha = 1.0f;
                p.activeSpells[0] = 1;

                break;

            case 5: //More HP
                btns[5].interactable = false;
                icons[13].alpha = 1.0f;
                p.hp = 200;
                p.currentHp = 200;
                p.updateHP();
                break;

            case 6: //FireBall
                btns[6].interactable = false;
                icons[11].alpha = 1.0f;
                p.activeSpells[1] = 1;

                break;

            case 7: //Enemy Slow
                btns[7].interactable = false;
                enemyMoveSpeedMod = 0.5f;
                icons[8].alpha = 1.0f;
                break;


        }

        hideUI(buffMenu);
        readyForNextWave();
    }
}
