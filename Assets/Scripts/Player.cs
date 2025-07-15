using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb; // the object which enables movement on the sprite
    public float movementSpeed; // a numerical value which determines how fast the sprite move on screen, could be used for gameplay/progression purposes

    private float minX, maxX, minY, maxY;
    private bool canMoveForward = true; // Flag to control forward movement

    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;
    // Variable for tracking how often the bullet it shot
    private int howOften = 0;
    // Variable for tracking when the trigger is on or off (pull trigger it fires)
    private bool trigger;

    public TMP_Text worldText;
    public List<TMP_Text> answers = new List<TMP_Text>();

    public int difficulty;
    private System.Random rand = new System.Random();

    // Difficulty can be between 1 and 4, 1 easiest, 4 hardest 
    private int[] sigFigs = { 1, 10, 100, 1000, 10000 };
    public int answer;
    public float puzzleDisplayDuration = 10f; // Duration to display the puzzle

    public GameObject healthBar;

    private float timer = 0;

    public TMP_Text gameOverTxt;

    public AudioClip collisionSound;
    public AudioClip correctAnswerSound;
    public AudioClip hoverSound;

    private AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        // Hide the text initially
        worldText.text = "";
        foreach (var answerText in answers)
        {
            answerText.text = "";
        }

        audioSource = GetComponent<AudioSource>();


        // Determine the screen boundaries in world space
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
    
        minX = bottomLeft.x;
        maxX = topRight.x;
        minY = bottomLeft.y + 45 ;
        maxY = topRight.y - 55;
    /*
        Debug.Log("minX " + minX);
        Debug.Log("maxX " + maxX);
        Debug.Log("minY " + minY);
        Debug.Log("maxY " + maxY);
*/
/*
        minX = 14.3488f;
        maxX = 9.8989f;
        minY = -239.34f;
        maxY = 225.8962f;
*/

        // Freeze the Z rotation
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized; // '.normalized' ensure that the movement speed is equal in all directions. Without the 'normalized' method, the sprite will move quicker when moving diagonally in comparison to other directions

        // Allow movement only if the player can move forward or if moving backward/sideways
        if (canMoveForward || playerInput.y <= 0)
        {
            rb.velocity = playerInput * movementSpeed; // taking the input from the user and multiplying it by the movementSpeed, controlled in the Unity Editor, and assigning it to the velocity of the rigidbody object
        }
        else
        {
            rb.velocity = new Vector2(playerInput.x * movementSpeed, 0); // Restrict forward movement but allow horizontal movement
        }

        // Clamp the position to keep the spaceship within the screen boundaries
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

        transform.position = clampedPosition;

        if (trigger)
        {
            howOften++;
            if (howOften % 120 == 0)
            {
                var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                bullet.GetComponent<Rigidbody2D>().velocity = bulletSpawnPoint.right * bulletSpeed;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Rock"))
        {
            // Stop forward movement if colliding with a rock
            canMoveForward = false;
            rb.velocity = Vector2.zero; // Immediately stop the player


            audioSource.PlayOneShot(collisionSound);
            healthBar.GetComponent<healthBar>().loseHealth(gameOverTxt);
            

        }

        if (collision.gameObject.CompareTag("CorrectAnswer"))
        {
            // Destroy the correct answer rock
            audioSource.PlayOneShot(correctAnswerSound);
            Destroy(collision.gameObject);
            float m = PlayerPrefs.GetFloat("SavedMultiplier", 1f);
            m += 0.5f;
            PlayerPrefs.SetFloat("SavedMultiplier", m);
            PlayerPrefs.Save();
          
            
            
            

        }

        if (collision.gameObject.CompareTag("PowerUp"))
        {
            float mult = PlayerPrefs.GetFloat("SavedMultiplier", 1f);
            mult += 0.1f;
            PlayerPrefs.SetFloat("SavedMultiplier", mult);
            PlayerPrefs.Save();
            Debug.Log($"[PowerUp] New multiplier: {mult}");

            // 2) destroy the orb
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.CompareTag("Asteroid"))
        {

            gameOverTxt.text = "GAME OVER!!";
            

            while (timer < 10.0)
            {
                timer += Time.deltaTime;
            }
            
            gameOverTxt.text = "";

            //timer += Time.deltaTime;


            audioSource.PlayOneShot(collisionSound);
            healthBar.GetComponent<healthBar>().loseHealth(gameOverTxt);



            
            

            



        }

    }

    private void DisplayPuzzleText()
    {
        // Display the puzzle text
        worldText.text = "Solve the puzzle:";
        foreach (var answerText in answers)
        {
            answerText.text = "Answer"; // Replace with actual answers
        }
    }

    private IEnumerator HidePuzzleTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePuzzleText();
    }

    private void HidePuzzleText()
    {
        worldText.text = "";
        foreach (var answerText in answers)
        {
            answerText.text = "";
        }
    }

    private void GeneratePuzzle()
    {
        var secondNumber = rand.Next(1, 10);
        var operation = rand.Next(0, 4);

        switch (operation)
        {
            case 0:
                addition(difficulty);
                break;
            case 1:
                subtraction(difficulty);
                break;
            case 2:
                multiplication(difficulty);
                break;
            case 3:
                division(difficulty);
                break;
        }

        var firstParam = -1;
        var secondParam = -1;
        if (answer >= 0 && answer < 10)
        {
            firstParam = 0;
            secondParam = 10;
        }
        else if (answer >= 10 && answer < 100)
        {
            firstParam = 10;
            secondParam = 100;
        }
        else if (answer >= 100 && answer < 1000)
        {
            firstParam = 1000;
            secondParam = 10000;
        }
        else if (answer >= 1000 && answer < 10000)
        {
            firstParam = 1000;
            secondParam = 10000;
        }
        else
        {
            firstParam = 10000;
            secondParam = 100000;
        }

        int[] allAnswers = new int[3];
        var correctAnswerPosition = rand.Next(0, 3);
        answers[correctAnswerPosition].text = answer + "";
        allAnswers[0] = answer;
        var count = 0;
        var allAnswerCount = 1;
        if (count == correctAnswerPosition)
        {
            count++;
        }
        while (count < 3)
        {
            if (count == correctAnswerPosition)
            {
                count++;
            }
            else
            {
                var notCopied = false;
                var falseAnswers = -1;
                while (notCopied == false)
                {
                    falseAnswers = rand.Next(firstParam, secondParam);
                    if (Array.IndexOf(allAnswers, falseAnswers) == -1)
                    {
                        answers[count].text = falseAnswers + "";
                        allAnswers[allAnswerCount] = falseAnswers;
                        allAnswerCount++;
                        notCopied = true;
                    }
                }
                count++;
            }
        }
    }

    int addition(int difficulty)
    {
        var firstNumber = rand.Next(sigFigs[difficulty - 1], sigFigs[difficulty]);
        var secondNumber = rand.Next(sigFigs[difficulty - 1], sigFigs[difficulty]);
        worldText.text = firstNumber + " + " + secondNumber;
        answer = firstNumber + secondNumber;
        return answer;
    }

    int subtraction(int difficulty)
    {
        var firstNumber = rand.Next(sigFigs[difficulty - 1], sigFigs[difficulty]);
        var secondNumber = -1;
        if (difficulty > 1)
        {
            secondNumber = rand.Next(sigFigs[difficulty - 1], firstNumber);
        }
        else
        {
            if (firstNumber != 0)
            {
                secondNumber = rand.Next(sigFigs[0], firstNumber);
            }
            else
            {
                secondNumber = 0;
            }
        }
        worldText.text = firstNumber + " - " + secondNumber;
        answer = firstNumber - secondNumber;
        return answer;
    }

    int multiplication(int difficulty)
    {
        if (difficulty >= 3)
        {
            difficulty = 3;
        }
        var secondDifficulty = 1;
        var firstNumber = rand.Next(sigFigs[difficulty - 1], sigFigs[difficulty]);
        var secondNumber = rand.Next(sigFigs[secondDifficulty - 1], sigFigs[secondDifficulty]);
        worldText.text = firstNumber + " * " + secondNumber;
        answer = firstNumber * secondNumber;
        return answer;
    }

    int division(int difficulty)
    {
        if (difficulty >= 3)
        {
            difficulty = 3;
        }
        var secondDifficulty = 1;
        var firstNumber = rand.Next(sigFigs[0], sigFigs[difficulty]);
        var noRemainder = false;
        while (!noRemainder)
        {
            var secondNumber = rand.Next(sigFigs[secondDifficulty - 1], sigFigs[secondDifficulty]);
            if (firstNumber % secondNumber == 0)
            {
                noRemainder = true;
                worldText.text = firstNumber + " / " + secondNumber;
                answer = firstNumber / secondNumber;
            }
        }
        return answer;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Rock") || collision.gameObject.CompareTag("CorrectAnswer"))
        {
            // Allow forward movement again when not colliding with a rock or correct answer rock
            canMoveForward = true;
        }
    }
}
