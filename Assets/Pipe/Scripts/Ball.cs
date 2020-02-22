using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float powerjump = 8f;
    public float gravity = 20.0f;
    private Rigidbody rb;

    public bool onGround = false;
    private Vector3 currentEulerAngles;

    public InputPad ctrl;
    public AudioClip[] jumpAudio;
    public AudioClip gameoverAudio;
    public AudioClip mainMenuAudio;
    public AudioClip spinAudio;
    internal AudioSource audioS;
    private void Start()
    {
        audioS = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!GameManager.instance.isGameStart)
            return;

        Spin();
    }
    public void Jump()
    {
        if (!onGround)
            return;
        audioS.PlayOneShot(jumpAudio[Random.Range(0, jumpAudio.Length)]);
        rb.velocity += powerjump * Vector3.up;
        onGround = false;
    }
    private void LateUpdate()
    {
        if (!onGround && rb.velocity.y > -gravity)
        {
            Vector3 vel = rb.velocity;
            vel.y -= gravity * Time.deltaTime * 1.3f;
            rb.velocity = vel;
        }
    }
    private void Spin()
    {
        currentEulerAngles += transform.right * GameManager.instance.speed;
        transform.eulerAngles = currentEulerAngles;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground")
            onGround = true;
        if (collision.transform.tag == "Fail")
            GameManager.instance.GameOver();
    }
}
