using UnityEngine;
using UnityEngine.UI;

public class CartoonGIFPlayer : MonoBehaviour
{
    public Sprite[] frames;
    public float frameRate = 0.1f;
    public bool playOnStart = true;

    private Image spriteRenderer;
    private int currentFrameIndex = 0;
    private float timer = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<Image>();
    }

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    private void Update()
    {
        if (frames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;

            // Update the sprite with the next frame
            currentFrameIndex = (currentFrameIndex + 1) % frames.Length;
            spriteRenderer.sprite = frames[currentFrameIndex];
        }
    }

    public void Play()
    {
        if (frames.Length > 0)
        {
            // Start playing the animation from the first frame
            currentFrameIndex = 0;
            spriteRenderer.sprite = frames[currentFrameIndex];
        }
    }

    public void Stop()
    {
        // Stop the animation and reset the frame index
        currentFrameIndex = 0;
        spriteRenderer.sprite = null;
    }
}
