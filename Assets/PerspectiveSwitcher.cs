using UnityEngine;
using System.Collections;
using UnityRawInput;

[RequireComponent(typeof(MatrixBlender))]
public class PerspectiveSwitcher : MonoBehaviour
{
    private Matrix4x4 ortho,
                        perspective;
    public float fov = 60f,
                        near = .3f,
                        far = 1000f,
                        orthographicSize = 50f,
                        orthoDuration = 1f,
                        persStartDuration = 1f,
                        persEndDuration = 1f;
    private float aspect;
    private MatrixBlender blender;
    private bool orthoOn;

    public Transform lerpTarget0;
    public Transform lerpTarget1;

    private bool rawInputAvailable = false;

    public Transform chuba;
    public Transform chubaLerpTarget0;
    public Transform chubaLerpTarget1;

    public Material streamBack;
    public Transform plugBack;

    public GameObject fakeLight;

    public Transform screenPullDown;
    public Transform screenLerpTarget1;
    public GameObject gameScreen;
    private bool gameCapTypeSwap = true;

    public Transform can;
    public Transform canTarget0;
    public Transform canTarget1;
    private bool drinking = false;

    public GameObject bottomGlow;

    public GameObject animCam;
    public GameObject gameCam;

    public Transform powerButton;
    public GameObject powerLight;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        aspect = (float)Screen.width / (float)Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        Camera.main.projectionMatrix = ortho;
        orthoOn = true;
        blender = (MatrixBlender)GetComponent(typeof(MatrixBlender));

        streamBack.color = new Color(0, 0, 0, 1);

        RawKeyInput.Start(true);
        RawKeyInput.OnKeyDown += HandleKeyDown;
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            orthoOn = !orthoOn;
            if (orthoOn)
                blender.BlendToMatrix(ortho, orthoDuration);
            else
                blender.BlendToMatrix(perspective, persDuration);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            blender.BlendToMatrix();
        }
        */
    }

    private void HandleKeyDown(RawKey key)
    {
        if(key == RawKey.P)
        {
            rawInputAvailable = !rawInputAvailable;
        }
        if (rawInputAvailable)
        {
            if (key == RawKey.Z)
            {
                orthoOn = !orthoOn;
                if (orthoOn)
                {
                    blender.BlendToMatrix(ortho, orthoDuration);
                    StopAllCoroutines();
                    StartCoroutine(lerpCam(transform, lerpTarget0, orthoDuration));
                }
                else
                {
                    blender.BlendToMatrix(perspective, persStartDuration, persEndDuration);
                    StopAllCoroutines();
                    StartCoroutine(lerpCam(transform, lerpTarget1, persStartDuration, persEndDuration));
                }
            }
            else if (key == RawKey.X)
            {
                blender.BlendToMatrix();
            }
            if(key == RawKey.Q)
            {
                StartCoroutine(lerpTransform(chuba, chubaLerpTarget1, 1));
                StartCoroutine(lerpColor(streamBack, Color.black));
            }
            if (key == RawKey.W)
            {
                StartCoroutine(lerpTransform(chuba, chubaLerpTarget0, 1));
                StartCoroutine(lerpColor(streamBack, Color.clear));
            }
            if (key == RawKey.E)
            {
                StartCoroutine(lerpTransform(screenPullDown, screenLerpTarget1, 1));
                StartCoroutine(lerpColor(streamBack, new Color(0,0,0,.5f)));
            }
            if(key == RawKey.D)
            {
                StopAllCoroutines();
                if (drinking)
                {
                    StartCoroutine(lerpTransform(can, canTarget1, 1));
                }
                else
                {
                    StartCoroutine(lerpTransform(can, canTarget0, 1));
                }
                drinking = !drinking;
            }
            if(key == RawKey.G)
            {
                gameScreen.SetActive(!bottomGlow.activeInHierarchy);
                bottomGlow.SetActive(!bottomGlow.activeInHierarchy);
            }
            if(key == RawKey.H)
            {
                gameCapTypeSwap = !gameCapTypeSwap;
                if (gameCapTypeSwap)
                {
                    gameScreen.GetComponent<Spout.SpoutReceiver>().sharingName = "gameCapture";
                }
                else
                {
                    gameScreen.GetComponent<Spout.SpoutReceiver>().sharingName = "windowCapture";
                }
            }
            if(key == RawKey.S)
            {
                StartCoroutine(playVideoAnim());
            }
            if(key == RawKey.O)
            {
                StartCoroutine(lerpTransform(plugBack, new Vector3(0.039f, -0.025f, -0.11f), 1, true, true));
            }
            if(key == RawKey.L)
            {
                StartCoroutine(lerpTransform(powerButton, new Vector3(powerButton.localPosition.x, powerButton.localPosition.y, -0.22f), 1, true, false, true));
            }
        }
    }

    private IEnumerator playVideoAnim()
    {
        animCam.SetActive(true);
        yield return new WaitForSeconds(11.867f);
        animCam.SetActive(false);
    }
    private IEnumerator lerpColor(Material src, Color dest, float duration = 1)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            src.color = Color.Lerp(src.color, dest, (Time.time - startTime) / duration);
            yield return 1;
        }
        src.color = dest;
    }
    private IEnumerator lerpTransform(Transform src, Vector3 dest, float duration = 1, bool localLerp = false, bool tempFakeLightSwitch = false, bool tempPowerButtonClick = false)
    {
        float startTime = Time.time;
        if (localLerp)
        {
            while (Time.time - startTime < duration)
            {
                src.localPosition = Vector3.Lerp(src.localPosition, dest, (Time.time - startTime) / duration);
                yield return 1;
            }
            src.localPosition = dest;
        }
        else
        {
            while (Time.time - startTime < duration)
            {
                src.position = Vector3.Lerp(src.position, dest, (Time.time - startTime) / duration);
                yield return 1;
            }
            src.position = dest;
        }
        if (tempFakeLightSwitch)
        {
            fakeLight.SetActive(true);
        }
        if (tempPowerButtonClick)
        {
            AudioSource t = GetComponent<AudioSource>();
            t.Play();
            powerLight.SetActive(true);
            yield return new WaitForSeconds(3);
            t.pitch = 0.75f;
            t.Play();
        }
    }
    private IEnumerator lerpTransform(Transform src, Transform dest, float duration = 1)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            src.position = Vector3.Lerp(src.position, dest.position, (Time.time - startTime) / duration);
            src.rotation = Quaternion.Lerp(src.rotation, dest.rotation, (Time.time - startTime) / duration);
            src.localScale = Vector3.Lerp(src.localScale, dest.localScale, (Time.time - startTime) / duration);
            yield return 1;
        }
        src.position = dest.position;
        src.rotation = dest.rotation;
        src.localScale = dest.localScale;
    }
    private IEnumerator lerpCam(Transform src, Transform dest, float duration)
    {
        float startTime = Time.time;
        while(Time.time - startTime < duration)
        {
            src.position = Vector3.Lerp(src.position, dest.position, (Time.time - startTime) / duration);
            src.rotation = Quaternion.Lerp(src.rotation, dest.rotation, (Time.time - startTime) / duration);
            yield return 1;
        }
        src.position = dest.position;
        src.rotation = dest.rotation;
    }
    private IEnumerator lerpCam(Transform src, Transform dest, float startDuration, float endDuration)
    {
        float startTime = Time.time;
        startDuration *= 4;
        while (Time.time - startTime < startDuration)
        {
            src.position = Vector3.Lerp(src.position, dest.position, (Time.time - startTime) / startDuration);
            src.rotation = Quaternion.Lerp(src.rotation, dest.rotation, (Time.time - startTime) / startDuration);
            startDuration = Mathf.Lerp(startDuration, endDuration, (Time.time - startTime) / 100f);
            yield return 1;
        }
        src.position = dest.position;
        src.rotation = dest.rotation;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        RawKeyInput.OnKeyDown -= HandleKeyDown;
        RawKeyInput.Stop();
    }
}
