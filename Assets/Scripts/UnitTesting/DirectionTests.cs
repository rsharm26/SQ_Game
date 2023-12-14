//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using NUnit.Framework;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using UnityEngine.TestTools;

//public class DirectionTests
//{
//    [Test]
//    public void PlayerVelocity_Positive()
//    {
//        GameObject playerObject = new GameObject();
//        Rigidbody2D rb = playerObject.AddComponent<Rigidbody2D>();

//        // Set the velocity of the Rigidbody2D directly
//        rb.velocity = new Vector2(2f, 3f);

//        // Access and verify the velocity values
//        Assert.AreEqual(2f, rb.velocity.x);
//        Assert.AreEqual(3f, rb.velocity.y);

//        // Clean up the object
//        Object.DestroyImmediate(playerObject);
//    }

//    [Test]
//    public void PlayerVelocity_Negative()
//    {
//        GameObject playerObject = new GameObject();
//        Rigidbody2D rb = playerObject.AddComponent<Rigidbody2D>();

//        // Set the velocity of the Rigidbody2D directly
//        rb.velocity = new Vector2(-2f, -3f);

//        // Access and verify the velocity values
//        Assert.AreEqual(-2f, rb.velocity.x);
//        Assert.AreEqual(-3f, rb.velocity.y);

//        // Clean up the object
//        Object.DestroyImmediate(playerObject);
//    }



//    [Test]
//    public void UpdateAnimationState_MoveLeft()
//    {
//        GameObject playerObject = new GameObject();
//        PlayerMovement testPlayer = playerObject.AddComponent<PlayerMovement>();
//        Rigidbody2D rb = playerObject.AddComponent<Rigidbody2D>();
//        testPlayer.playerBody = rb;
//        BoxCollider2D collider = playerObject.AddComponent<BoxCollider2D>();
//        testPlayer.collision = collider;
//        testPlayer.sprite = playerObject.AddComponent<SpriteRenderer>();
//        testPlayer.playerAnimation = playerObject.AddComponent<Animator>();

//        testPlayer.DirX = -0.5f;
//        testPlayer.UpdateAnimationState();

//        Assert.IsTrue(testPlayer.sprite.flipX);

//        Object.DestroyImmediate(playerObject);
//    }

//    [Test]
//    public void UpdateAnimationState_MoveRight()
//    {
//        GameObject playerObject = new GameObject();
//        PlayerMovement testPlayer = playerObject.AddComponent<PlayerMovement>();
//        Rigidbody2D rb = playerObject.AddComponent<Rigidbody2D>();
//        testPlayer.playerBody = rb;
//        BoxCollider2D collider = playerObject.AddComponent<BoxCollider2D>();
//        testPlayer.collision = collider;
//        testPlayer.sprite = playerObject.AddComponent<SpriteRenderer>();
//        testPlayer.playerAnimation = playerObject.AddComponent<Animator>();

//        testPlayer.DirX = 0.5f;
//        testPlayer.UpdateAnimationState();

//        Assert.IsFalse(testPlayer.sprite.flipX);

//        Object.DestroyImmediate(playerObject);
//    }

//    [Test]
//    public void UpdateAnimationState_Idle()
//    {
//        GameObject playerObject = new GameObject();
//        PlayerMovement testPlayer = playerObject.AddComponent<PlayerMovement>();
//        Rigidbody2D rb = playerObject.AddComponent<Rigidbody2D>();
//        testPlayer.playerBody = rb;
//        BoxCollider2D collider = playerObject.AddComponent<BoxCollider2D>();
//        testPlayer.collision = collider;
//        testPlayer.sprite = playerObject.AddComponent<SpriteRenderer>();
//        testPlayer.playerAnimation = playerObject.AddComponent<Animator>();

//        testPlayer.DirX = 0.0f;
//        testPlayer.UpdateAnimationState();

//        Assert.IsTrue(testPlayer._Idle);

//        Object.DestroyImmediate(playerObject);
//    }

//    [Test]
//    public void UpdateAnimationState_Jumping()
//    {
//        GameObject playerObject = new GameObject();
//        PlayerMovement testPlayer = playerObject.AddComponent<PlayerMovement>();
//        Rigidbody2D rb = playerObject.AddComponent<Rigidbody2D>();
//        testPlayer.playerBody = rb;
//        BoxCollider2D collider = playerObject.AddComponent<BoxCollider2D>();
//        testPlayer.collision = collider;
//        testPlayer.sprite = playerObject.AddComponent<SpriteRenderer>();
//        testPlayer.playerAnimation = playerObject.AddComponent<Animator>();

//        rb.velocity = new Vector2(0, 7f);

//        testPlayer.UpdateAnimationState();

//        Assert.IsTrue(testPlayer._IsJumping);

//        Object.DestroyImmediate(playerObject);
//    }

//    [Test]
//    public void UpdateAnimationState_Falling()
//    {
//        GameObject playerObject = new GameObject();
//        PlayerMovement testPlayer = playerObject.AddComponent<PlayerMovement>();
//        Rigidbody2D rb = playerObject.AddComponent<Rigidbody2D>();
//        testPlayer.playerBody = rb;
//        BoxCollider2D collider = playerObject.AddComponent<BoxCollider2D>();
//        testPlayer.collision = collider;
//        testPlayer.sprite = playerObject.AddComponent<SpriteRenderer>();
//        testPlayer.playerAnimation = playerObject.AddComponent<Animator>();

//        rb.velocity = new Vector2(0, -7f);

//        testPlayer.UpdateAnimationState();

//        Assert.IsFalse(testPlayer._IsJumping);

//        Object.DestroyImmediate(playerObject);
//    }
//}
