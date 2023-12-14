/*
* FILE				: PlayerMovement.cs
* PROJECT			: SENG 2020 - Term Project
* PROGRAMMERS		: Cody Glanville ID: 8864645
* FIRST VERSION		: October(Generally), 2023
* DESCRIPTION		:
*	This file contains the script that will allow the player to move throughout our created game environments. As such, this file will
*	have methods pertaining to the calculation of velocities in order to move the player and also allow them to jump in the game's stages.
*	This file also contains the logic relating to the proper sounds being played when the player interacts with other objects in the game
*	such as enemies, collectables, and traps. Other logic is included to update the created tabulator when the player dies or collects an 
*	item during their gameplay.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   * PUBLIC CLASS        : PlayerMovement
   * DESCRIPTION	     :
   *	This class acts to in order to hold all the logic used for the movement of a players main character in our Pixel Andy platformer
   *	game. As such, the code pertains to the properties and class objects that can be applied to a player in order to give it physics,
   *	let it collide with its environment, and ultimately allow it to move through our created levels in a smooth and responsive fashion.
   */
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Tabulator tabulator; 

    public Rigidbody2D playerBody;                                          // Gives physics to our player's character

    public BoxCollider2D collision;                                         // Allows for the detection of collision with the player    

    public SpriteRenderer sprite;                                           // Used for flipping the sprite to opposite directions

    public Animator playerAnimation;                                        // Used for the player's animations

    public float dirX = 0f;                                                 // Used for horizontal speed, etc.
    bool doubleJump;                                                        // Boolean variable used to determine if a double-jump can be used

    public bool _Idle { get; set; }                                        // Used to test the UpdateAnimation method
    public bool _IsJumping { get; set; }                                   // Used to test the UpdateAnimation method

    public float DirX                                                      // Float used for the velocity of the characters x-axis movement
    {
        get { return dirX; }
        set { dirX = value; }
    }
   
    [SerializeField]private float moveSpeed = 7f;                           // Player movement speed   (SerializeField lets us change values in Player itslef)
    [SerializeField]private float jumpForce = 14f;                          // Player Jumping velocity (SerializeField lets us change values in Player itslef)
    [SerializeField]private LayerMask jumpableGround;                       // Determines if the player is touching jumpable ground

    [SerializeField] private AudioSource jumpSoundEffect;                   // Audio source object that will play for jumping
    [SerializeField] private AudioSource itemCollectEffect;                 // Audio source object that will play for collecting items
    [SerializeField] private AudioSource deathEffect;                       // Audio source object that will play upon a player dying
    [SerializeField] private AudioSource winEffect;                         // Audio source object that will play for a player completing a stage


    public enum MovementState { idle, running, jumping, falling }           // Enum used to determine the correct state of the movement animation

    private Vector3 playerOrigin;               // Record player starting position 
    public Vector3 RespawnPoint{get; set;}      // Property for if player resets to origin or a checkpoint 

    /*
    *	METHOD          : Start()
    *	DESCRIPTION		:
    *		This method is used technically as a type of constructor that will give the player object its components such as a Rigidbody2D, an
    *		Animator, a SpriteRenderer, and a BoxCollider2D in order to allow the player to interact properly with its environment. These objects 
    *		allow the player to have collision with other objects in their environment and also allow its animations to be updated properly.
    *	PARAMETERS:
    *		void               :    Void is used as there are no parameters
    *	RETURNS			:
    *		void	           :	Void is used as this method has no return values
    */
    private void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        collision = GetComponent<BoxCollider2D>();
        RespawnPoint = playerBody.position;             //record player's starting position 
    }

    /*
    *	METHOD          : Update()
    *	DESCRIPTION		:
    *		This method is called every frame that occurs in the gameplay of our game. As such, it will constantly determine the location of the
    *		player's movement by getting the raw input of their horizontal axis. Once the axis is determined, it is passed into the MovePlayer()
    *		method that determines the speed and velocity of the player. It will then check if the player jumps and no matter what it will 
    *		update the players animation state depending on if any actions occurred that may alter its state. 
    *	PARAMETERS:
    *		void               :    Void is used as there are no parameters
    *	RETURNS			:
    *		void	           :	Void is used as this method has no return values
    */
    private void Update()
    {
        float movementDirection = Input.GetAxisRaw("Horizontal");

        MovePlayer(movementDirection);

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        } 
        else if (Input.GetButtonDown("Cancel")) 
        {
            UIType? currentlyActive = UIManager.GetInstance().CurrentlyActive;

            if (currentlyActive == UIType.PauseMenu || currentlyActive == UIType.InLevelOverlay || currentlyActive == null ) 
            {
                UIManager.GetInstance().ToggleUIElement(UIType.PauseMenu);
                Time.timeScale = Time.timeScale != 1 ? 1 : 0;
            }
        }

        UpdateAnimationState();
    }

    /*
    *	METHOD          : MovePlayer(float movementDirection)
    *	DESCRIPTION		:
    *		This method is used for moving the player throughout our created environments. This method will first determine if the player has been properly
    *		instantiated with a playerBody and will use the horizontal axis value passed into the method to set a dirX float value to help calculate the velocity
    *		of the character's direction. Additional error checking was included during unit testing to set a log error if the RidigBody was not assigned to the 
    *		character object as well.
    *	PARAMETERS:
    *		float movementDirection         :   This is the value of the characters horizontal x-axis
    *	RETURNS			:
    *		void	                        :	Void is used as this method has no return values
    */
    public void MovePlayer(float movementDirection)
    {
        if (playerBody != null)
        {
            dirX = movementDirection;
            playerBody.velocity = new Vector2(dirX * moveSpeed, playerBody.velocity.y);
        }
        else
        {
            Debug.LogError("Rigidbody2D not assigned!");
            return;
        }
    }

    /*
    *	METHOD          : Jump()
    *	DESCRIPTION		:
    *		This method is used in order to determine if the character can execute a jump in game. First, a ground check is completed which ultimately allows the 
    *		jump logic to continue. If the player is grounded, then their vertical velocity will be increased shooting them into the air at a set velocity. If the
    *		user has also only jumped once, they will be able to execute a second jump mid-air which is decided by the doubleJump boolean value. This value is toggled
    *		to not allow the user to jump multiple times in the air and is also reset when the user touches the ground again. 
    *	PARAMETERS:
    *		void               :    Void is used as there are no parameters
    *	RETURNS			:
    *		void	           :	Void is used as this method has no return values
    */
    public void Jump()
    {
        if (IsGrounded())
        {
            jumpSoundEffect.Play();
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpForce);
            doubleJump = true;
        }
        else if (doubleJump)
        {
            jumpSoundEffect.Play();
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpForce);
            doubleJump = false;
        }
    }

    /*
    *	METHOD          : UpdateAnimationState()
    *	DESCRIPTION		:
    *		This method is used for determining the proper state the player's animation needs to be set as while moving around the game world. As such, an enum
    *		MovementState variable is used to switch the character between its 4 separate animation states depending on their horizontal (x) and vertical (y) axis 
    *		values. Each of these values will trigger a separate state for the character to be in. The _IsJumping and _Idle properties were used for testing purposes
    *		within the idle, jumping, and falling trigger states. 
    *	PARAMETERS:
    *		void               :    Void is used as there are no parameters
    *	RETURNS			:
    *		void	           :	Void is used as this method has no return values
    */
    public void UpdateAnimationState()
    {
        
        MovementState state;                    // Local movement state created

        if (dirX > 0f)                          // Left running animation determined by a negative x value (-x)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)                     // Right running animation state determined by a positive x value (+x)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;         // Idle state determined if the x axis value is neither positive or negative
            _Idle = true;
        }

        if (playerBody.velocity.y > .1f)        // If the players vertical y velocity is positive, the jumping animation will be toggled (+y)
        {
            state = MovementState.jumping;
            _IsJumping = true;
        }
        else if (playerBody.velocity.y < -.1f)  // If their vertical y velocity is negative, the falling animation will be toggled (-y)
        {
            state = MovementState.falling;
            _IsJumping = false;
        }

        playerAnimation.SetInteger("state", (int)state); 
    }

    /*
    *	METHOD          : IsGrounded()
    *	DESCRIPTION		:
    *		This method ultimately determines if the player is in a grounded state (not jumping or falling). The Physics2D.BoxCast will
    *		constantly move through the space of our player's environments and determine if any collisions have occurred. If any collision
    *		has been deteceted, the method will return true. Otherwise, it will return false and indicate that the player is either
    *		jumping or falling through space. 
    *	PARAMETERS:
    *		void               :    Void is used as there are no parameters
    *	RETURNS			:
    *		bool True	       :    True is retured if the player is currently on the ground
    *		bool False         :    False is returned if the player is currently NOT on the ground
    */
    public bool IsGrounded()
    {
        return Physics2D.BoxCast(collision.bounds.center, collision.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }


    /*
    *	METHOD          : OnCollisionEnter2D(Collision2D other)
    *	DESCRIPTION		:
    *		This method is used for when the player will interact with other objects in the game. The two if statements in the method regard
    *		the player interacting with both an enemy (which will reduce its health) and a portal (which acts as the goal for the level). As such, 
    *		this method will allow for the tabulator to determine life losing and win conditions for the player in the game.
    *	PARAMETERS:
    *		Collision2D         :    This will be a separate object other than the player (enemy, etc.)
    *	RETURNS			:
    *		void	            :	 Void is used as this method has no return values
    */
    private void OnCollisionEnter2D(Collision2D other) 
    {
        // If the player runs into an enemy, reduce their available lives 
        if (other.gameObject.tag == "Enemy")
        {
            deathEffect.Play();
            tabulator.Lives -= 1;                // update remaning lives in tabulator 
            Debug.Log("tabulator.lives: " + tabulator.Lives);
            playerBody.position = RespawnPoint;  // reset player back to starting position or last checkpoint encountered
            DynamicGameData gameData = GameDataManager.GetInstance();
            gameData.LivesRemaining = tabulator.Lives;
        }


        // If the player has reached the portal, they will win the specific level
        if (other.gameObject.tag == "Portal")
        {
            winEffect.Play();
            tabulator.Win = true; 
        }
    }

    /*
    *	METHOD          : OnTriggerEnter2D(Collider2D other)
    *	DESCRIPTION		:
    *		This method is used for altering the tabulator when a player has encountered a collectable object in the game. As such, upon
    *		the player interacting with a collectable (running into / colliding with it) the tabulator will be updated, the proper sound
    *		will be played for when a collectable is gathered, and the collectable itself will be deactived in order for the player to not
    *		encounter it multiple times.
    *	PARAMETERS:
    *		Collider2D other    :   This is an object separate from the player 
    *	RETURNS			:
    *		void	            :	Void is used as this method has no return values
    */
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the object is a collectable, update the tabulator and play the appropriate sound
        if (other.gameObject.tag == "Collectable")
        {
            itemCollectEffect.Play();
            tabulator.Collect++; 
            other.gameObject.SetActive(false);  // deactivate the collectable 
            DynamicGameData gameData = GameDataManager.GetInstance();
            gameData.CollectiblesFound = tabulator.Collect;
        }
    }
}
