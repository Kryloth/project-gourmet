using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class MainGameController : MonoBehaviour
{
    public static MainGameController instance;
    public Sprite kotakNone;
    [Separator("Spell Database", true)]
    public List<Foogic> foogicDatabase = new List<Foogic>();
    [Separator("Prefab List", true)]
    public GameObject prefabDamagePopup;
    public GameObject prefabStatusPopup;
    public GameObject prefabItem;
    public GameObject enemyPrefab;

    [Separator("Text List", true)]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI wheatText;
    public TextMeshProUGUI meatText;
    public TextMeshProUGUI veggiesText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI movSpeedText;
    public TextMeshProUGUI enemiesLeftText;
    public Slider playerHP;
    
    [Separator("Audio List", true)]
    public AudioClip cookingDone;
    public AudioClip meteorFall;
    public int wheatCount;
    public int meatCount;
    public int veggiesCount;

    int minEnemy;
    int maxEnemy;
    int currRound = 0;
    List<int> selectedSpellSlot = new List<int>();
    public int currEnemies;
    public int currentSpell;
    public ItemObject[] itemIngredients;
    public Image[] ingredientsImage;
    public GameObject[] spellSlot;
    public List<ItemObject> cookingTable = new List<ItemObject>();
    

    Spawner[] spawners;

    private void Awake() {
        if(instance == null)
            instance = this;

        spawners = FindObjectsOfType<Spawner>();
        Time.timeScale = 1f;
    }
    private void Start()
    {
        currentSpell = 0;
        wheatCount = meatCount = veggiesCount = 3;
        currRound = -1;
        StartCoroutine(NextRound());
    }
    void SlotTween(){
        if(currentSpell == 0){
            GameObject uiSlot = spellSlot[0];
            LeanTween.scale(uiSlot.GetComponent<RectTransform>(), new Vector3(.5f, .5f, .5f), .5f);
            LeanTween.moveLocal(uiSlot, new Vector3(130f, 0f, 0f), 0.5f);
            
            uiSlot = spellSlot[1];
            LeanTween.scale(uiSlot.GetComponent<RectTransform>(), new Vector3(1f, 1f, 1f), .5f);
            LeanTween.moveLocal(uiSlot, new Vector3(0f, 0f, 0f), 0.5f);
            currentSpell = 1;
        }else if(currentSpell == 1){
            GameObject uiSlot = spellSlot[1];
            LeanTween.scale(uiSlot.GetComponent<RectTransform>(), new Vector3(.5f, .5f, .5f), .5f);
            LeanTween.moveLocal(uiSlot, new Vector3(130f, 0f, 0f), 0.5f);
            
            uiSlot = spellSlot[0];
            LeanTween.scale(uiSlot.GetComponent<RectTransform>(), new Vector3(1f, 1f, 1f), .5f);
            LeanTween.moveLocal(uiSlot, new Vector3(0f, 0f, 0f), 0.5f);
            currentSpell = 0;
        }
    }
    private void Update() {
        RefreshUI();
        PlayerInput();
    }
    void PlayerInput(){
        if(Player.instance != null){
            if(Input.GetKeyDown(KeyCode.Space) && FoogicInvoker.instance.foogicSpell[currentSpell] == null){
                Cook();
            }else if(Input.GetKeyDown(KeyCode.Space) && FoogicInvoker.instance.foogicSpell[currentSpell] != null){
                FoogicInvoker.instance.InvokeSpell();
                Image spellSlotImg = spellSlot[currentSpell].transform.GetChild(0).GetComponent<Image>();
                spellSlotImg.sprite = kotakNone;
                spellSlotImg.color = new Color32(255,255,255,255);
            }
            if(Input.GetKeyDown(KeyCode.R)){
                SlotTween();
            }
            if(Input.GetKeyDown(KeyCode.Alpha1)){
                if(wheatCount > 0){
                    wheatCount--;
                    cookingTable.Insert(0, itemIngredients[0]);
                    PrepareCooking();
                }
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)){
                if(meatCount > 0){
                    meatCount--;
                    cookingTable.Insert(0, itemIngredients[1]);
                    PrepareCooking();
                }
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)){
                if(veggiesCount > 0){
                    veggiesCount--;
                    cookingTable.Insert(0, itemIngredients[2]);
                    PrepareCooking();
                }
            }
        }
    }
    public void RefreshUI(){
        if(Player.instance == null){
            return;
        }
        playerHP.maxValue = Player.instance.MAX_HEALTH;
        playerHP.value = Player.instance.currentHealth;
        wheatText.SetText(wheatCount.ToString());
        meatText.SetText(meatCount.ToString());
        veggiesText.SetText(veggiesCount.ToString());
        attackText.SetText(Player.instance.projectileDamage.ToString());
        armorText.SetText(Player.instance.armor.ToString());
        movSpeedText.SetText(Player.instance.moveSpeed.ToString());
        waveText.SetText("Wave " + (currRound + 1).ToString());
        enemiesLeftText.SetText("Enemies Left: " + currEnemies.ToString());
        
    }
    void Cook(){
        if(selectedSpellSlot.Contains(currentSpell)){
            return;
        }
        if(FoogicInvoker.instance.foogicSpell[currentSpell] == null){
            Foogic foogic = RecipesCheck();
            GameObject spellSlotNow = spellSlot[currentSpell];
            if(foogic != null){
                selectedSpellSlot.Add(currentSpell);
                cookingTable.Clear();
                
                PrepareCooking();    
                StartCoroutine(CookingProcess(foogic, currentSpell, spellSlotNow));
            }else{
                Image spellSlotImg = spellSlotNow.transform.GetChild(0).GetComponent<Image>();
                spellSlotImg.sprite = kotakNone;
                spellSlotImg.color = new Color32(255,255,255,255);
            }
            return;
        }else{
            return;
        }
    }
    IEnumerator CookingProcess(Foogic foogic, int currentSpell, GameObject spellSlotNow){
        float cookingTime = foogic.cookingTime;
        Sprite sprite = foogic.spellSprite;
        Color32 color = new Color32(169, 169, 169, 255);
        Slider slider = spellSlotNow.GetComponent<Slider>();
        GameObject sliderArea = spellSlotNow.transform.GetChild(1).gameObject;
        TextMeshProUGUI durationText = spellSlotNow.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Image spellSlotImg = spellSlotNow.transform.GetChild(0).GetComponent<Image>();
        AudioSource audio = spellSlotNow.GetComponent<AudioSource>();

        spellSlotImg.color = color;
        spellSlotImg.sprite = sprite;
        sliderArea.SetActive(true);
        durationText.gameObject.SetActive(true);
        slider.value = 0f;
        slider.maxValue = foogic.cookingTime;

        while(cookingTime > 0){
            durationText.SetText((Mathf.FloorToInt(cookingTime)).ToString());
            yield return new WaitForSeconds(.1f);
            cookingTime -= 0.1f;
            slider.value += .1f;
        }
        sliderArea.SetActive(false);
        durationText.gameObject.SetActive(false);
        audio.PlayOneShot(cookingDone, .4f);

        FoogicInvoker.instance.foogicSpell[currentSpell] = foogic;
        selectedSpellSlot.Remove(currentSpell);
        spellSlotImg.color = new Color32(255,255,255,255);
    }
    Foogic RecipesCheck(){
        for (int i = 0; i < foogicDatabase.Count; i++)
        {
            List<ItemObject> foogicIngredients = new List<ItemObject>();
            Foogic foogic = foogicDatabase[i];
            for (int r = 0; r < foogic.recipes.Length; r++)
            {
                foogicIngredients.Add(foogic.recipes[r]);
            }
            bool existsCheck = ScrambledEquals(cookingTable, foogicIngredients);
            if(existsCheck){
                return foogic;
            }
        }
        return null;
    }
    void PrepareCooking(){
        if(cookingTable.IsNullOrEmpty()){
            for (int i = 0; i < ingredientsImage.Length; i++)
            {
                ingredientsImage[i].sprite = kotakNone;
            }
            return;
        }
        if(cookingTable.Count > 3){
            if(cookingTable[3].ingredient == Types.IngredientType.Wheat){
                wheatCount++;
            }else if(cookingTable[3].ingredient == Types.IngredientType.Meat){
                meatCount++;
            }else if(cookingTable[3].ingredient == Types.IngredientType.Veggies){
                veggiesCount++;
            }
            cookingTable.RemoveAt(cookingTable.Count - 1);
        }
        for (int i = 0; i < cookingTable.Count; i++)
        {
            ingredientsImage[i].sprite = cookingTable[i].sprite;
        }
    }
    void StartRound()
    {
        if(minEnemy < 8){
            minEnemy = 1 + currRound;
        }else{
            minEnemy = 8;
        }
        if(maxEnemy < 8){
            maxEnemy = 3 + currRound;
        }else{
            maxEnemy = 8;
        }

        Shuffle();
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(spawners[i].Spawn(minEnemy, maxEnemy, currRound));
        }
    }

    IEnumerator NextRound()
    {
        currRound += 1;
        yield return new WaitForSeconds(3f);
        StartRound();
    }

    public void CheckRound()
    {
        currEnemies--;
        if (currEnemies <= 0)
            StartCoroutine(NextRound());
    }
    private void Shuffle()
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, spawners.Length);
            Spawner temp;
            temp = spawners[rnd];
            spawners[rnd] = spawners[i];
            spawners[i] = temp;
        }
    }
    public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2) {
        var cnt = new Dictionary<T, int>();
        foreach (T s in list1) {
            if (cnt.ContainsKey(s)) {
            cnt[s]++;
            } else {
            cnt.Add(s, 1);
            }
        }
        foreach (T s in list2) {
            if (cnt.ContainsKey(s)) {
            cnt[s]--;
            } else {
            return false;
            }
        }
        return cnt.Values.All(c => c == 0);
    }
}
