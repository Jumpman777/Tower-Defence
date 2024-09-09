using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100;
    public float maxHealth = 100;

    private float healthperc;

    [Header("Offset for health bar. Increase this to increase distance between object and health bar")]
    public float offset;

    [Header("The object that will represent the health bar")]
    [SerializeField]
    private GameObject Healthbar;

    [Header("For extra effects")]
    [SerializeField]
    private bool Animation;
    [SerializeField]
    private float animSizeUpDuration = 0.1f;
    [SerializeField]
    private float animSizeDownDuration = 0.3f;
    [SerializeField]
    private float animAmmount = 1.0f;

    Vector3 animationAmmountChanged;
    Vector3 healthBarOriginScale;
    AudioSource healthBarAC;

    [Header("Sound to play when Healthbar takes damage, leave empty for no sound")]
    [SerializeField]
    private AudioClip hitSound;

    [Header("How much time it will take for the health bar to change")]
    public float Healthbarsmoothing = 1;

    [Header("How long the damage effect will last")]
    public float DamageDuration = 1;
    private GameObject healthBar;

    private GameObject HealthBarUI;
    private Image HealthBarImage;
    private Image damageEffectMask;
    private Image damageEffectImage;

    void moveHealthBar()
    {
        healthBar.transform.position = new Vector3(this.gameObject.transform.position.x, (this.gameObject.transform.position.y + ((this.transform.localScale.y / 2) + 0.5f)) + offset, this.gameObject.transform.position.z);
    }

    bool animRunning = false;
    private IEnumerator animCoroutine;
    IEnumerator AnimationCoroutine()
    {
        animRunning = true;

        float elapsedTime = 0;

        Vector3 preChangeScale = healthBar.transform.localScale;

        while (elapsedTime < animSizeUpDuration)
        {
            elapsedTime += Time.deltaTime;
            healthBar.transform.localScale = Vector3.Lerp(preChangeScale, new Vector3(Mathf.Clamp(preChangeScale.x + animAmmount, preChangeScale.x, animationAmmountChanged.x), Mathf.Clamp(preChangeScale.y + animAmmount, preChangeScale.y, animationAmmountChanged.y), Mathf.Clamp(preChangeScale.z + animAmmount, preChangeScale.z, animationAmmountChanged.z)), elapsedTime / animSizeUpDuration);
            yield return null;
        }

        Vector3 afterChangeScale = healthBar.transform.localScale;
        elapsedTime = 0;

        while (elapsedTime < animSizeDownDuration)
        {
            elapsedTime += Time.deltaTime;
            healthBar.transform.localScale = Vector3.Lerp(afterChangeScale, healthBarOriginScale, elapsedTime / animSizeDownDuration);
            yield return null;
        }

        healthBar.transform.localScale = healthBarOriginScale;
        animRunning = false;
    }

    bool isRunning = false;
    private IEnumerator healthbarcoroutine;
    IEnumerator MoveHealthBarCoroutine()
    {
        isRunning = true;

        float elapsedTime = 0;
        float preChangePercent = HealthBarImage.fillAmount;
        while (elapsedTime < Healthbarsmoothing)
        {

            elapsedTime += Time.deltaTime;
            HealthBarImage.fillAmount = Mathf.Lerp(preChangePercent, healthperc, elapsedTime / Healthbarsmoothing);
            yield return null;
        }

        HealthBarImage.fillAmount = healthperc;
        isRunning = false;
    }

    bool fadeEffectInProgress = false;
    float lastDamage = 0;
    private IEnumerator damageBarEffectCoroutine;
    IEnumerator DamageBarEffect()
    {
        fadeEffectInProgress = true;
        damageEffectImage.fillAmount = (1 - healthperc) + (lastDamage / maxHealth);

        float elapsedTime = 0;
        damageEffectImage.color = new Color32((byte)damageEffectImage.color.r, (byte)damageEffectImage.color.g, (byte)damageEffectImage.color.b, (byte)0.5f);

        while (elapsedTime < DamageDuration)
        {
            elapsedTime += Time.deltaTime;
            damageEffectImage.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(200, 0.0f, elapsedTime / DamageDuration));
            yield return null;
        }

        damageEffectImage.color = new Color32((byte)damageEffectImage.color.r, (byte)damageEffectImage.color.g, (byte)damageEffectImage.color.b, (byte)0.0f);
        fadeEffectInProgress = false;
    }

    void moveHealthBarFill()
    {
        if (isRunning == false)
        {
            StartCoroutine(healthbarcoroutine);
        }
        if (fadeEffectInProgress == false)
        {
            StartCoroutine(damageBarEffectCoroutine);
        }
    }

    float calculateHealthBarPercent()
    {
        return health / maxHealth;
    }

    public void TakeDamage(float Amount)
    {
        health -= Amount;
        lastDamage = Amount;
        moveHealthBarFill();
        if (animRunning == false && Animation == true)
        {
            StartCoroutine(animCoroutine);
        }
        else if (animRunning == true && Animation == true)
        {
            StopCoroutine(animCoroutine);
            StartCoroutine(animCoroutine);
        }
        if (hitSound != null)
        {
            healthBarAC.Stop();
            healthBarAC.Play();
        }

        // Destroy object when health is zero
        if (health <= 0)
        {
            Destroy(healthBar);
            Destroy(gameObject);
        }
    }

    void Start()
    {

        healthBar = Instantiate(Healthbar);
        moveHealthBar();

        HealthBarUI = healthBar.transform.GetChild(0).gameObject;
        HealthBarImage = HealthBarUI.transform.GetChild(0).transform.GetComponent<Image>();
        damageEffectMask = HealthBarUI.transform.GetChild(HealthBarUI.transform.childCount - 1).transform.GetComponent<Image>();
        damageEffectImage = damageEffectMask.transform.GetChild(0).GetComponent<Image>();

        animationAmmountChanged = new Vector3(healthBar.transform.localScale.x + animAmmount, healthBar.transform.localScale.y + animAmmount, healthBar.transform.localScale.z + animAmmount);
        healthBarOriginScale = healthBar.transform.localScale;

        healthBarAC = healthBar.GetComponent<AudioSource>();
        if (hitSound != null)
        {
            healthBarAC.clip = hitSound;
        }
    }

    void Update()
    {
        healthbarcoroutine = MoveHealthBarCoroutine();
        damageBarEffectCoroutine = DamageBarEffect();
        animCoroutine = AnimationCoroutine();

        if (fadeEffectInProgress == false)
        {
            damageEffectImage.color = new Color32((byte)damageEffectImage.color.r, (byte)damageEffectImage.color.g, (byte)damageEffectImage.color.b, (byte)0);
        }

        damageEffectMask.fillAmount = HealthBarImage.fillAmount;

        moveHealthBar();
        float clampedHealth = Mathf.Clamp(health, 0, maxHealth);
        health = clampedHealth;

        healthperc = calculateHealthBarPercent();
    }
}

