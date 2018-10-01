using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using sonicKnuckles;

public class sonicKnucklesScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public Renderer background;

    public KMSelectable logo;
    public Renderer segaSurface;
    public Material splash;
    public Material white;
    public MonitorScreens[] monitors;
    public MonitorScreens[] badniks;
    public MonitorScreens[] heroes;
    public MonitorScreens boss;

    public Material[] backgroundOptions;
    public String[] levelNames;
    public Material[] monitorOptions;
    public Material[] badnikOptions;
    public Material bossBackground;
    public Material[] bossMaterials;
    public Renderer[] completeUI;
    public Renderer[] explosions;
    public Renderer[] explosions2;
    public Renderer[] dust;
    public Renderer[] sonicComplete;
    public Renderer[] knucklesComplete;
    public Renderer[] tailsComplete;
    public string[] monitorNames;
    public string[] badnikNames;
    public int[] monitorCodes;
    public int[] badnikCodes;
    public int[] heroCodes;
    private int backgroundIndex = 0;
    private int heroIndex = 0;
    private int monitorIndex = 0;
    private int badnikIndex = 0;
    private bool hitting;
    private int hitFlash = 0;

    public AudioClip[] mushroomSounds;
    public AudioClip[] noMushroomSounds;
    public AudioClip[] batterySounds;
    public AudioClip[] noBatterySounds;
    public AudioClip[] sandSounds;
    public AudioClip[] noSandSounds;
    public AudioClip[] lavaSounds;
    public AudioClip[] noLavaSounds;
    public AudioClip[] skySounds;
    public AudioClip[] noSkySounds;
    public AudioClip[] eggSounds;
    public AudioClip[] noEggSounds;

    public TextMesh[] HUD;
    public TextMesh minutesCountMesh;
    public Renderer UiImage;
    public Material[] UiImageOptions;
    public TextMesh UiText;
    public String[] UiTextOptions;
    public TextMesh livesCount;
    public TextMesh flashingTime;
    public TextMesh gameOver;
    public TextMesh timeOver;
    private int ringCount = 0;
    private int score = 0;
    private int secondsCount = 0;
    private int minutesCount = 0;
    private bool timerStarted;
    private bool rightTime;
    private int lives = 3;
    private bool flashing;
    private float delay = 0;

    private int correctBaseCode = 0;
    private int incorrectBaseCodeA = 0;
    private int incorrectBaseCodeB = 0;
    private int hitsRequired = 0;
    private int correctHitTime = 0;
    private GameObject lastPressed;
    private int explosionCount = 0;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    private bool buttonLock;
    private bool gameOverWaiting;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (MonitorScreens monitor in monitors)
        {
            MonitorScreens pressedMonitor = monitor;
            pressedMonitor.selectable.OnInteract += delegate () { OnMonitorPress(pressedMonitor); return false; };
        }
        foreach (MonitorScreens badnik in badniks)
        {
            MonitorScreens pressedBadnik = badnik;
            pressedBadnik.selectable.OnInteract += delegate () { OnBadnikPress(pressedBadnik); return false; };
        }
        foreach (MonitorScreens hero in heroes)
        {
            MonitorScreens pressedHero = hero;
            pressedHero.selectable.OnInteract += delegate () { OnHeroPress(pressedHero); return false; };
        }
        boss.selectable.OnInteract += delegate () { OnBossPress(); return false; };
        logo.OnInteract += delegate () { OnLogoPress(); return false; };
        GetComponent<KMBombModule>().OnActivate += OnLights;
    }

    void OnLights()
    {
        delay = UnityEngine.Random.Range(0, 2f);
        StartCoroutine(StingCoroutine());
    }

    IEnumerator StingCoroutine()
    {
        yield return new WaitForSeconds(delay);
        Audio.PlaySoundAtTransform("sega", transform);
        delay = 0;
    }

    void Start()
    {
        segaSurface.gameObject.SetActive(true);
        logo.gameObject.SetActive(true);
        gameOver.gameObject.SetActive(false);
        timeOver.gameObject.SetActive(false);
        foreach(Renderer sonic in sonicComplete)
        {
            sonic.gameObject.SetActive(false);
        }
        foreach(Renderer knuckles in knucklesComplete)
        {
            knuckles.gameObject.SetActive(false);
        }
        foreach(Renderer tails in tailsComplete)
        {
            tails.gameObject.SetActive(false);
        }
        foreach(Renderer explosion in explosions)
        {
            explosion.gameObject.SetActive(false);
        }
        foreach(Renderer explosion in explosions2)
        {
            explosion.gameObject.SetActive(false);
        }
        foreach(Renderer explosion in dust)
        {
            explosion.gameObject.SetActive(false);
        }
        foreach(Renderer display in completeUI)
        {
            display.gameObject.SetActive(false);
        }
        foreach(MonitorScreens button in heroes)
        {
            button.gameObject.SetActive(false);
        }
        foreach(MonitorScreens button in monitors)
        {
            button.gameObject.SetActive(false);
        }
        foreach(MonitorScreens button in badniks)
        {
            button.gameObject.SetActive(false);
        }
        boss.gObject.SetActive(false);
        lives = 3;
    }

    void OnLogoPress()
    {
        logo.AddInteractionPunch(1f);
        gameOverWaiting = false;
        logo.gameObject.SetActive(false);
        segaSurface.material = splash;
        StartCoroutine(preSetUp());
    }

    IEnumerator preSetUp()
    {
        Audio.PlaySoundAtTransform("mainTheme", transform);
        yield return new WaitForSeconds(6f);
        segaSurface.material = white;
        segaSurface.gameObject.SetActive(false);
        SetUp();
    }

    void SetUp()
    {
        gameOver.gameObject.SetActive(false);
        timeOver.gameObject.SetActive(false);
        backgroundIndex = UnityEngine.Random.Range(0,6);
        background.material = backgroundOptions[backgroundIndex];
        monitorIndex = UnityEngine.Random.Range(0,5);
        badnikIndex = UnityEngine.Random.Range(0,5);
        heroIndex = UnityEngine.Random.Range(0,3);
        int illegalBox = UnityEngine.Random.Range(0,3);
        int monSoundIndex = UnityEngine.Random.Range(0,4);
        int badSoundIndex = UnityEngine.Random.Range(0,4);
        while(badSoundIndex == monSoundIndex)
        {
            badSoundIndex = UnityEngine.Random.Range(0,4);
        }
        int heroSoundIndex = UnityEngine.Random.Range(0,4);
        while(heroSoundIndex == monSoundIndex || heroSoundIndex == badSoundIndex)
        {
            heroSoundIndex = UnityEngine.Random.Range(0,4);
        }
        foreach(Renderer sonic in sonicComplete)
        {
            sonic.gameObject.SetActive(false);
        }
        foreach(Renderer knuckles in knucklesComplete)
        {
            knuckles.gameObject.SetActive(false);
        }
        foreach(Renderer tails in tailsComplete)
        {
            tails.gameObject.SetActive(false);
        }
        foreach(Renderer explosion in explosions)
        {
            explosion.gameObject.SetActive(false);
        }
        foreach(Renderer explosion in explosions2)
        {
            explosion.gameObject.SetActive(false);
        }
        foreach(Renderer explosion in dust)
        {
            explosion.gameObject.SetActive(false);
        }
        foreach(Renderer display in completeUI)
        {
            display.gameObject.SetActive(false);
        }
        foreach(MonitorScreens monitor in monitors)
        {
            monitor.rend.material = monitorOptions[monitorIndex];
            monitor.label = monitorNames[monitorIndex];
            monitor.baseCode = monitorCodes[monitorIndex];
            monitor.gObject.SetActive(false);
            if(illegalBox == 0)
            {
                monitor.containsIllegalSound = true;
                int illegalSoundIndex = UnityEngine.Random.Range(0,20);
                if(backgroundIndex == 0)
                {
                    monitor.attachedSound = noMushroomSounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 1)
                {
                    monitor.attachedSound = noBatterySounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 2)
                {
                    monitor.attachedSound = noSandSounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 3)
                {
                    monitor.attachedSound = noLavaSounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 4)
                {
                    monitor.attachedSound = noSkySounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 5)
                {
                    monitor.attachedSound = noEggSounds[illegalSoundIndex];
                }
                correctBaseCode = monitor.baseCode;
            }
            else
            {
                monitor.containsIllegalSound = false;
                if(backgroundIndex == 0)
                {
                    monitor.attachedSound = mushroomSounds[monSoundIndex];
                }
                else if(backgroundIndex == 1)
                {
                    monitor.attachedSound = batterySounds[monSoundIndex];
                }
                else if(backgroundIndex == 2)
                {
                    monitor.attachedSound = sandSounds[monSoundIndex];
                }
                else if(backgroundIndex == 3)
                {
                    monitor.attachedSound = lavaSounds[monSoundIndex];
                }
                else if(backgroundIndex == 4)
                {
                    monitor.attachedSound = skySounds[monSoundIndex];
                }
                else if(backgroundIndex == 5)
                {
                    monitor.attachedSound = eggSounds[monSoundIndex];
                }
                incorrectBaseCodeA = monitor.baseCode;
                incorrectBaseCodeB = monitor.baseCode;
            }

        }
        foreach(MonitorScreens badnik in badniks)
        {
            badnik.rend.material = badnikOptions[badnikIndex];
            badnik.label = badnikNames[badnikIndex];
            badnik.baseCode = badnikCodes[badnikIndex];
            badnik.gObject.SetActive(false);
            if(illegalBox == 1)
            {
                badnik.containsIllegalSound = true;
                int illegalSoundIndex = UnityEngine.Random.Range(0,20);
                if(backgroundIndex == 0)
                {
                    badnik.attachedSound = noMushroomSounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 1)
                {
                    badnik.attachedSound = noBatterySounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 2)
                {
                    badnik.attachedSound = noSandSounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 3)
                {
                    badnik.attachedSound = noLavaSounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 4)
                {
                    badnik.attachedSound = noSkySounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 5)
                {
                    badnik.attachedSound = noEggSounds[illegalSoundIndex];
                }
                correctBaseCode = badnik.baseCode;
            }
            else
            {
                badnik.containsIllegalSound = false;
                if(backgroundIndex == 0)
                {
                    badnik.attachedSound = mushroomSounds[badSoundIndex];
                }
                else if(backgroundIndex == 1)
                {
                    badnik.attachedSound = batterySounds[badSoundIndex];
                }
                else if(backgroundIndex == 2)
                {
                    badnik.attachedSound = sandSounds[badSoundIndex];
                }
                else if(backgroundIndex == 3)
                {
                    badnik.attachedSound = lavaSounds[badSoundIndex];
                }
                else if(backgroundIndex == 4)
                {
                    badnik.attachedSound = skySounds[badSoundIndex];
                }
                else if(backgroundIndex == 5)
                {
                    badnik.attachedSound = eggSounds[badSoundIndex];
                }
                incorrectBaseCodeA = badnik.baseCode;
            }

        }
        foreach(MonitorScreens hero in heroes)
        {
            hero.baseCode = heroCodes[heroIndex];
            hero.gObject.SetActive(false);
            if(illegalBox == 2)
            {
                hero.containsIllegalSound = true;
                int illegalSoundIndex = UnityEngine.Random.Range(0,20);
                if(backgroundIndex == 0)
                {
                    hero.attachedSound = noMushroomSounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 1)
                {
                    hero.attachedSound = noBatterySounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 2)
                {
                    hero.attachedSound = noSandSounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 3)
                {
                    hero.attachedSound = noLavaSounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 4)
                {
                    hero.attachedSound = noSkySounds[illegalSoundIndex];
                }
                else if(backgroundIndex == 5)
                {
                    hero.attachedSound = noEggSounds[illegalSoundIndex];
                }
                correctBaseCode = hero.baseCode;
            }
            else
            {
                hero.containsIllegalSound = false;
                if(backgroundIndex == 0)
                {
                    hero.attachedSound = mushroomSounds[heroSoundIndex];
                }
                else if(backgroundIndex == 1)
                {
                    hero.attachedSound = batterySounds[heroSoundIndex];
                }
                else if(backgroundIndex == 2)
                {
                    hero.attachedSound = sandSounds[heroSoundIndex];
                }
                else if(backgroundIndex == 3)
                {
                    hero.attachedSound = lavaSounds[heroSoundIndex];
                }
                else if(backgroundIndex == 4)
                {
                    hero.attachedSound = skySounds[heroSoundIndex];
                }
                else if(backgroundIndex == 5)
                {
                    hero.attachedSound = eggSounds[heroSoundIndex];
                }
                incorrectBaseCodeB = hero.baseCode;
            }
        }
        boss.gObject.SetActive(false);
        monitors[backgroundIndex].gObject.SetActive(true);
        badniks[backgroundIndex].gObject.SetActive(true);
        heroes[heroIndex].gObject.SetActive(true);
        UiImage.material = UiImageOptions[heroIndex];
        UiText.text = UiTextOptions[heroIndex];
        livesCount.text = lives.ToString();

        if(minutesCount > 3)
        {
            minutesCount = 3;
            secondsCount = 30;
        }
        else if(minutesCount != 0)
        {
            if(secondsCount < 30)
            {
                minutesCount--;
                int randSeconds = UnityEngine.Random.Range(30,58);
                secondsCount = randSeconds;
            }
            else
            {
                int randSeconds = UnityEngine.Random.Range(0,30);
                secondsCount = randSeconds;
            }
        }
        else
        {
            secondsCount = 0;
        }
        minutesCountMesh.text = minutesCount.ToString() + ".";
        HUD[1].text = secondsCount.ToString("00");

        score = UnityEngine.Random.Range(150,2000);
        score = score * 10;
        HUD[0].text = score.ToString();
        if(!timerStarted)
        {
            timerStarted = true;
            StartCoroutine (Timer());
        }
        ringCount = UnityEngine.Random.Range(15,100);
        HUD[2].text = ringCount.ToString();
        Debug.LogFormat("[Sonic & Knuckles #{0}] The selected level is {1}.", moduleId, levelNames[backgroundIndex]);
        Debug.LogFormat("[Sonic & Knuckles #{0}] Your score is {1}.", moduleId, score);
        Debug.LogFormat("[Sonic & Knuckles #{0}] Your ring count is {1}.", moduleId, ringCount);
        Debug.LogFormat("[Sonic & Knuckles #{0}] Your hero is {1}. The base code is {2}. The contained sound is {3}.", moduleId, heroes[heroIndex].label, heroes[heroIndex].baseCode, heroes[heroIndex].attachedSound.name);
        Debug.LogFormat("[Sonic & Knuckles #{0}] Your badnik is {1}. The base code is {2}. The contained sound is {3}.", moduleId, badniks[badnikIndex].label, badniks[badnikIndex].baseCode, badniks[badnikIndex].attachedSound.name);
        Debug.LogFormat("[Sonic & Knuckles #{0}] Your monitor is {1}. The base code is {2}. The contained sound is {3}.", moduleId, monitors[monitorIndex].label, monitors[monitorIndex].baseCode, monitors[monitorIndex].attachedSound.name);
        if(heroes[heroIndex].containsIllegalSound)
        {
            Debug.LogFormat("[Sonic & Knuckles #{0}] The {1} button contains the illegal sound ({2}). Press it when the second timer is {3}, {4} or {5}.", moduleId, heroes[heroIndex].label, heroes[heroIndex].attachedSound.name, (ringCount % 20), (ringCount % 20)+20, (ringCount % 20)+40);
        }
        else if(badniks[heroIndex].containsIllegalSound)
        {
            Debug.LogFormat("[Sonic & Knuckles #{0}] The {1} button contains the illegal sound ({2}). Press it when the second timer is {3}, {4} or {5}.", moduleId, badniks[badnikIndex].label, badniks[badnikIndex].attachedSound.name, (ringCount % 20), (ringCount % 20)+20, (ringCount % 20)+40);
        }
        else
        {
            Debug.LogFormat("[Sonic & Knuckles #{0}] The {1} button contains the illegal sound ({2}). Press it when the second timer is {3}, {4} or {5}.", moduleId, monitors[monitorIndex].label, monitors[monitorIndex].attachedSound.name, (ringCount % 20), (ringCount % 20)+20, (ringCount % 20)+40);
        }
        calculateBossHits();
        Debug.LogFormat("[Sonic & Knuckles #{0}] The boss requires a total of {1} hits.", moduleId, hitsRequired);
        if(correctHitTime == 0)
        {
            Debug.LogFormat("[Sonic & Knuckles #{0}] Knuckles, ghost or running boots were not present. Perform the first {1} hits on even numbered seconds and the last one on an odd numbered second.", moduleId, hitsRequired - 1);
        }
        else
        {
            Debug.LogFormat("[Sonic & Knuckles #{0}] Knuckles, ghost or running boots were present. Perform the first {1} hits on odd numbered seconds and the last one on an even numbered second.", moduleId, hitsRequired - 1);
        }

    }

    IEnumerator Timer()
    {
        while(!moduleSolved && !gameOverWaiting)
        {
            yield return new WaitForSeconds(0.8f);
            secondsCount++;
            if(secondsCount == 60)
            {
                minutesCount++;
                secondsCount = 0;
                if(minutesCount == 9 && !flashing)
                {
                    StartCoroutine(TimerFlash());
                    flashing = true;
                }
            }
            HUD[1].text = secondsCount.ToString("00");
            minutesCountMesh.text = minutesCount.ToString() + ".";
            if(minutesCount == 9 && secondsCount == 59 && lives != 1)
            {
                buttonLock = true;
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Sonic & Knuckles #{0}] Strike! Time over!", moduleId);
                Audio.PlaySoundAtTransform("death", transform);
                lastPressed = null;
                timeOver.gameObject.SetActive(true);
                Audio.PlaySoundAtTransform("gameOver", transform);
                lives--;
                livesCount.text = lives.ToString();
                yield return new WaitForSeconds(10f);
                minutesCount = 0;
                secondsCount = 0;
                timeOver.gameObject.SetActive(false);
                buttonLock = false;
                SetUp();
            }
            else if(minutesCount == 9 && secondsCount == 59 && lives == 1)
            {
                GetComponent<KMBombModule>().HandleStrike();
                lastPressed = null;
                Debug.LogFormat("[Sonic & Knuckles #{0}] Strike! Time over!", moduleId);
                Audio.PlaySoundAtTransform("death", transform);
                buttonLock = true;
                gameOverWaiting = true;
                timerStarted = false;
                gameOver.gameObject.SetActive(true);
                Audio.PlaySoundAtTransform("gameOver", transform);
                lives--;
                livesCount.text = lives.ToString();
                yield return new WaitForSeconds(30f);
                buttonLock = false;
                minutesCount = 0;
                secondsCount = 0;
                gameOver.gameObject.SetActive(false);
                Start();
            }
        }
    }

    IEnumerator TimerFlash()
    {
        while(minutesCount == 9)
        {
            flashingTime.text = "";
            yield return new WaitForSeconds(0.4f);
            flashingTime.text = "TIME";
            yield return new WaitForSeconds(0.4f);
        }
        flashing = false;
    }

    void calculateBossHits()
    {
        hitsRequired = correctBaseCode*score;
        hitsRequired = ((hitsRequired - 1) % 9 )+ 1;
        hitsRequired = hitsRequired + incorrectBaseCodeA + incorrectBaseCodeB;
        hitsRequired = (hitsRequired % 10) + 1;
        if(monitors[monitorIndex].label == "running boots" || badniks[badnikIndex].label == "ghost" || heroes[heroIndex].label == "Knuckles")
        {
            correctHitTime = 1;
        }
        else
        {
            correctHitTime = 0;
        }
    }

    void SetUpBoss()
    {
        Audio.PlaySoundAtTransform("doomsday", transform);
        monitors[backgroundIndex].gObject.SetActive(false);
        badniks[backgroundIndex].gObject.SetActive(false);
        heroes[heroIndex].gObject.SetActive(false);
        boss.gObject.SetActive(true);
        background.material = bossBackground;
    }

    public void OnMonitorPress(MonitorScreens pressedMonitor)
    {
        if(buttonLock)
        {
            return;
        }
        pressedMonitor.selectable.AddInteractionPunch(0.5f);
        if(lastPressed == null)
        {
            lastPressed = pressedMonitor.gameObject;
            lastPressed.gameObject.SetActive(false);
        }
        else
        {
            lastPressed.gameObject.SetActive(true);
            lastPressed = pressedMonitor.gameObject;
            lastPressed.gameObject.SetActive(false);
        }
        if((secondsCount == (ringCount % 20)) || (secondsCount == (ringCount % 20) + 20) || (secondsCount == (ringCount % 20) + 40))
        {
            if(pressedMonitor.containsIllegalSound)
            {
                Debug.LogFormat("[Sonic & Knuckles #{0}] You pressed {1}. That is correct.", moduleId, pressedMonitor.label);
                SetUpBoss();
            }
            else
            {
                Debug.LogFormat("[Sonic & Knuckles #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedMonitor.label);
                Strike();
            }
        }
        else
        {
            Audio.PlaySoundAtTransform(pressedMonitor.attachedSound.name, transform);
        }
    }

    public void OnBadnikPress(MonitorScreens pressedBadnik)
    {
        if(buttonLock)
        {
            return;
        }
        pressedBadnik.selectable.AddInteractionPunch(0.5f);
        if(lastPressed == null)
        {
            lastPressed = pressedBadnik.gameObject;
            lastPressed.gameObject.SetActive(false);
        }
        else
        {
            lastPressed.gameObject.SetActive(true);
            lastPressed = pressedBadnik.gameObject;
            lastPressed.gameObject.SetActive(false);
        }
        if((secondsCount == (ringCount % 20)) || (secondsCount == (ringCount % 20) + 20) || (secondsCount == (ringCount % 20) + 40))
        {
            if(pressedBadnik.containsIllegalSound)
            {
                Debug.LogFormat("[Sonic & Knuckles #{0}] You pressed {1}. That is correct.", moduleId, pressedBadnik.label);
                SetUpBoss();
            }
            else
            {
                Debug.LogFormat("[Sonic & Knuckles #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedBadnik.label);
                Strike();
            }
        }
        else
        {
            Audio.PlaySoundAtTransform(pressedBadnik.attachedSound.name, transform);
        }
    }

    public void OnHeroPress(MonitorScreens pressedHero)
    {
        if(buttonLock)
        {
            return;
        }
        pressedHero.selectable.AddInteractionPunch(0.5f);
        if(lastPressed == null)
        {
            lastPressed = pressedHero.gameObject;
            lastPressed.gameObject.SetActive(false);
        }
        else
        {
            lastPressed.gameObject.SetActive(true);
            lastPressed = pressedHero.gameObject;
            lastPressed.gameObject.SetActive(false);
        }
        if((secondsCount == (ringCount % 20)) || (secondsCount == (ringCount % 20) + 20) || (secondsCount == (ringCount % 20) + 40))
        {
            if(pressedHero.containsIllegalSound)
            {
                Debug.LogFormat("[Sonic & Knuckles #{0}] You pressed {1}. That is correct.", moduleId, pressedHero.label);
                SetUpBoss();
            }
            else
            {
                Debug.LogFormat("[Sonic & Knuckles #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedHero.label);
                Strike();
            }
        }
        else
        {
            Audio.PlaySoundAtTransform(pressedHero.attachedSound.name, transform);
        }
    }

    public void OnBossPress()
    {
        if(buttonLock || moduleSolved || hitting)
        {
            return;
        }
        boss.selectable.AddInteractionPunch();
        if(correctHitTime == 1 && hitsRequired == 1)
        {
            correctHitTime = 0;
        }
        else if(correctHitTime == 0 && hitsRequired == 1)
        {
            correctHitTime = 1;
        }

        if((secondsCount % 2) == correctHitTime)
        {
            hitting = true;
            hitsRequired--;
            Debug.LogFormat("[Sonic & Knuckles #{0}] You hit the boss at {1}.{2}. {3} hits remaining.", moduleId, minutesCount, secondsCount, hitsRequired);
            StartCoroutine(bossHit());
        }
        else
        {
            if(correctHitTime == 0)
            {
                Debug.LogFormat("[Sonic & Knuckles #{0}] Strike! You hit the boss when the seconds timer was {1}. An even time was required.", moduleId, secondsCount);
            }
            else
            {
                Debug.LogFormat("[Sonic & Knuckles #{0}] Strike! You hit the boss when the seconds timer was {1}. An odd time was required.", moduleId, secondsCount);
            }
            Strike();
        }
    }

    IEnumerator bossHit()
    {
        if(hitsRequired == 0)
        {
            moduleSolved = true;
            while(explosionCount < 25)
            {
                int expIndex = UnityEngine.Random.Range(0,12);
                int expIndex2 = UnityEngine.Random.Range(0,12);
                int dustIndex = UnityEngine.Random.Range(0,12);
                explosions[expIndex].gameObject.SetActive(true);
                explosions2[expIndex2].gameObject.SetActive(true);
                dust[dustIndex].gameObject.SetActive(true);
                Audio.PlaySoundAtTransform("hitBoss", transform);
                yield return new WaitForSeconds(0.075f);
                explosions[expIndex].gameObject.SetActive(false);
                explosions2[expIndex2].gameObject.SetActive(false);
                dust[dustIndex].gameObject.SetActive(false);
                explosionCount++;
            }
            Audio.PlaySoundAtTransform("solved", transform);
            GetComponent<KMBombModule>().HandlePass();
            boss.gameObject.SetActive(false);
            if(heroIndex == 0)
            {
                sonicComplete[backgroundIndex].gameObject.SetActive(true);
                sonicComplete[backgroundIndex + 6].gameObject.SetActive(true);
            }
            else if(heroIndex == 1)
            {
                knucklesComplete[backgroundIndex].gameObject.SetActive(true);
                knucklesComplete[backgroundIndex + 6].gameObject.SetActive(true);
            }
            else if(heroIndex == 2)
            {
                tailsComplete[backgroundIndex].gameObject.SetActive(true);
                tailsComplete[backgroundIndex + 6].gameObject.SetActive(true);
            }
            background.material = backgroundOptions[backgroundIndex];
            completeUI[heroIndex].gameObject.SetActive(true);
            Debug.LogFormat("[Sonic & Knuckles #{0}] You defeated Dr. Robotnik. Module disarmed.", moduleId);
        }
        else
        {
            Audio.PlaySoundAtTransform("hitBoss", transform);
            while(hitFlash < 12)
            {
                boss.rend.material = bossMaterials[1];
                yield return new WaitForSeconds(0.025f);
                boss.rend.material = bossMaterials[0];
                yield return new WaitForSeconds(0.025f);
                hitFlash++;
            }
            hitFlash = 0;
            hitting = false;
        }
    }

    void Strike()
    {
        Audio.PlaySoundAtTransform("death", transform);
        GetComponent<KMBombModule>().HandleStrike();
        lastPressed = null;
        if(lives == 1)
        {
            lives--;
            livesCount.text = lives.ToString();
            StartCoroutine(GameOver());
        }
        else if(lives>0)
        {
            lives--;
            SetUp();
        }
        else
        {
            SetUp();
        }
    }

    IEnumerator GameOver()
    {
        gameOverWaiting = true;
        timerStarted = false;
        buttonLock = true;
        gameOver.gameObject.SetActive(true);
        Audio.PlaySoundAtTransform("gameOver", transform);
        yield return new WaitForSeconds(30f);
        buttonLock = false;
        StartCoroutine(Timer());
        minutesCount = 0;
        secondsCount = 0;
        gameOver.gameObject.SetActive(false);
        Start();
        StartCoroutine(StingCoroutine());
    }
}
