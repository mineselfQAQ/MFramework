using UnityEngine;

public class PlayerStats : EntityStats<PlayerStats>
{
    [Header("General Stats")]
    public float pushForce = 4f;
    public float snapForce = 15f;
    public float slideForce = 10f;
    public float rotationSpeed = 970f;
    public float gravity = 38f;
    public float fallGravity = 65f;
    public float gravityTopSpeed = 50f;

    [Header("Pick'n Throw Stats")]
    public bool canPickUp = true;
    public bool canPickUpOnAir = false;
    public bool canJumpWhileHolding = true;
    public float pickDistance = 0.5f;
    public float throwVelocityMultiplier = 1.5f;

    [Header("Motion Stats")]
    public bool applySlopeFactor = true;
    public float acceleration = 13f;
    public float deceleration = 28f;
    public float friction = 28f;
    public float slopeFriction = 18f;
    public float topSpeed = 6f;
    public float turningDrag = 28f;
    public float airAcceleration = 32f;
    public float brakeThreshold = -0.8f;
    public float slopeUpwardForce = 25f;
    public float slopeDownwardForce = 28f;

    [Header("Running Stats")]
    public float runningAcceleration = 16f;
    public float runningTopSpeed = 7.5f;
    public float runningTurningDrag = 14f;

    [Header("Jump Stats")]
    public int multiJumps = 1;
    public float coyoteJumpThreshold = 0.15f;
    public float maxJumpHeight = 17f;
    public float minJumpHeight = 10f;

    [Header("Crouch Stats")]
    public float crouchHeight = 1f;
    public float crouchFriction = 10f;

    [Header("Crawling Stats")]
    public float crawlingAcceleration = 8f;
    public float crawlingFriction = 32f;
    public float crawlingTopSpeed = 2.5f;
    public float crawlingTurningSpeed = 3f;

    [Header("Wall Drag Stats")]
    public bool canWallDrag = true;
    public bool wallJumpLockMovement = true;
    public LayerMask wallDragLayers;
    public Vector3 wallDragSkinOffset;
    public float wallDragGravity = 12f;
    public float wallJumpDistance = 8f;
    public float wallJumpHeight = 15f;

    [Header("Pole Climb Stats")]
    public bool canPoleClimb = true;
    public Vector3 poleClimbSkinOffset;
    public float climbUpSpeed = 3f;
    public float climbDownSpeed = 8f;
    public float climbRotationSpeed = 2f;
    public float poleJumpDistance = 8f;
    public float poleJumpHeight = 15f;

    [Header("Swimming Stats")]
    public float waterConversion = 0.35f;
    public float waterRotationSpeed = 360f;
    public float waterUpwardsForce = 8f;
    public float waterJumpHeight = 15f;
    public float waterTurningDrag = 2.5f;
    public float swimAcceleration = 4f;
    public float swimDeceleration = 3f;
    public float swimTopSpeed = 4f;
    public float swimDiveForce = 15f;

    [Header("Spin Stats")]
    public bool canSpin = true;
    public bool canAirSpin = true;
    public float spinDuration = 0.5f;
    public float airSpinUpwardForce = 10f;
    public int allowedAirSpins = 1;

    [Header("Hurt Stats")]
    public float hurtUpwardForce = 10f;
    public float hurtBackwardsForce = 5f;

    [Header("Air Dive Stats")]
    public bool canAirDive = true;
    public bool applyDiveSlopeFactor = true;
    public float airDiveForwardForce = 16f;
    public float airDiveFriction = 32f;
    public float airDiveSlopeFriction = 12f;
    public float airDiveSlopeUpwardForce = 35f;
    public float airDiveSlopeDownwardForce = 40f;
    public float airDiveGroundLeapHeight = 10f;
    public float airDiveRotationSpeed = 45f;

    [Header("Stomp Attack Stats")]
    public bool canStompAttack = true;
    public float stompDownwardForce = 20f;
    public float stompAirTime = 0.8f;
    public float stompGroundTime = 0.5f;
    public float stompGroundLeapHeight = 10f;

    [Header("Ledge Hanging Stats")]
    public bool canLedgeHang = true;
    public LayerMask ledgeHangingLayers;
    public Vector3 ledgeHangingSkinOffset;
    public float ledgeMaxForwardDistance = 0.25f;
    public float ledgeMaxDownwardDistance = 0.25f;
    public float ledgeSideMaxDistance = 0.5f;
    public float ledgeSideHeightOffset = 0.15f;
    public float ledgeSideCollisionRadius = 0.25f;
    public float ledgeMovementSpeed = 1.5f;

    [Header("Ledge Climbing Stats")]
    public bool canClimbLedges = true;
    public LayerMask ledgeClimbingLayers;
    public Vector3 ledgeClimbingSkinOffset;
    public float ledgeClimbingDuration = 1f;

    [Header("Backflip Stats")]
    public bool canBackflip = true;
    public bool backflipLockMovement = true;
    public float backflipAirAcceleration = 12f;
    public float backflipTurningDrag = 2.5f;
    public float backflipTopSpeed = 7.5f;
    public float backflipJumpHeight = 23f;
    public float backflipGravity = 35f;
    public float backflipBackwardForce = 4f;
    public float backflipBackwardTurnForce = 8f;

    [Header("Gliding Stats")]
    public bool canGlide = true;
    public float glidingGravity = 10f;
    public float glidingMaxFallSpeed = 2f;
    public float glidingTurningDrag = 8f;

    [Header("Dash Stats")]
    public bool canAirDash = true;
    public bool canGroundDash = true;
    public float dashForce = 25f;
    public float dashDuration = 0.3f;
    public float groundDashCoolDown = 0.5f;
    public float allowedAirDashes = 1;

    [Header("Rail Grinding Stats")]
    public bool useCustomCollision = true;
    public float grindRadiusOffset = 0.26f;
    public float minGrindInitialSpeed = 10f;
    public float minGrindSpeed = 5f;
    public float grindTopSpeed = 25f;
    public float grindDownSlopeForce = 40f;
    public float grindUpSlopeForce = 30f;

    [Header("Rail Grinding Brake")]
    public bool canGrindBrake = true;
    public float grindBrakeDeceleration = 10;

    [Header("Rail Grinding Dash Stats")]
    public bool canGrindDash = true;
    public bool applyGrindingSlopeFactor = true;
    public float grindDashCoolDown = 0.5f;
    public float grindDashForce = 25f;
}