﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AsteroidInstanceController;

public class PlayerController : MonoBehaviour
{
    public EventManager eventManager;
    private Rigidbody2D rigidbody;
    [Range(0.0f, 10.0f)]
    public float acceleration;
    [Range(0.0f, 360.0f)]
    public float rotationSpeed;
    public float terminalVelocity;
    public PlayerLaserInstanceController laser;
    public float movementPowerUsage = 0.2f;
    public float rotationPowerUsage = 0.1f;
    public float laserPowerUsage = 0.5f;
    public float power = 100.0f;
    public int score = 0;
    public float remainingTime;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            Win();
        }
        Vector2 forward = transform.up;
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.rotation += rotationSpeed * Time.deltaTime;
            power -= rotationPowerUsage * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigidbody.rotation -= rotationSpeed * Time.deltaTime;
            power -= rotationPowerUsage * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            rigidbody.velocity += forward * acceleration * Time.deltaTime;
            power -= movementPowerUsage * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var laserInstance = Instantiate(laser).gameObject;
            laserInstance.transform.position = transform.position + (Vector3)forward * 0.2f;
            laserInstance.transform.rotation = transform.rotation;
            laserInstance.GetComponent<Rigidbody2D>().velocity = forward * 15;
            laserInstance.GetComponent<PlayerLaserInstanceController>().player = this;
            laserInstance.GetComponent<PlayerLaserInstanceController>().eventManager = eventManager;
            power -= laserPowerUsage;
        }
        rigidbody.drag = (float)Math.Exp(rigidbody.velocity.magnitude - terminalVelocity);
        if (power <= 0)
        {
            GameOver();
        }
    }

    private void Win()
    {
        eventManager.OnGameWon.Invoke();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            var size = collision.gameObject.GetComponent<AsteroidInstanceController>().size;
            switch (size)
            {
                case AsteroidSize.Big:
                    power -= 30;
                    break;
                case AsteroidSize.Medium:
                    power -= 25;
                    break;
                case AsteroidSize.Small:
                    power -= 20;
                    break;
                case AsteroidSize.Tiny:
                    power -= 15;
                    break;
            }
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Powerup"))
        {
            power += collision.gameObject.GetComponent<PowerupInstanceController>().powerRestoreAmount;
            Destroy(collision.gameObject);
        }
    }

    void GameOver()
    {
        eventManager.OnGameLost.Invoke();
        gameObject.SetActive(false);
    }
}
