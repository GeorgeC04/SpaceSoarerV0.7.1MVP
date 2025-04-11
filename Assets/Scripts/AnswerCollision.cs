using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AnswerCollision : MonoBehaviour
{
    public TMP_Text thisText;
    public TMP_Text answerText;

    public GameObject playerObject; // Reference to the player GameObject

    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;
    private int howOften = 0;
    private bool trigger = false;

    private void OnTriggerEnter2D(Collider2D other)
    {

        int integerNumber = -1;

        if(thisText.text != ""){
            integerNumber = int.Parse(thisText.text + "");
        }

        // Ensure the playerObject has the Player component
        var playerComponent = playerObject.GetComponent<Player>();
        if (playerComponent != null && integerNumber == playerComponent.answer)
        {
            trigger = true; // Start shooting when the correct answer is detected
            HidePuzzleText(playerComponent);
        }
    }

    void Update()
    {
        if (trigger)
        {
            howOften++;
            if (howOften % 120 == 0) // Adjust this value as needed to control shooting frequency
            {
                ShootBullet();
            }
        }
    }

    void ShootBullet()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = bulletSpawnPoint.right * bulletSpeed;

        trigger = true;
    }

    void HidePuzzleText(Player playerComponent)
    {
        playerComponent.worldText.text = "";
        foreach (var answer in playerComponent.answers)
        {
            answer.text = "";
        }
    }
}
