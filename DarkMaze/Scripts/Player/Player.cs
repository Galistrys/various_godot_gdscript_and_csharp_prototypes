using Godot;
using Godot.Collections;
using System;

public class Player : KinematicBody
{
    public const string PLAYER_POSITION_KEY = "player_position";
    public const string PLAYER_ROTATION_KEY = "player_rotation";

    // Camera
    Spatial _headSpatial;
    Camera _playerCamera;
    RayCast _interactionRay;
    Position3D _movableGrabPosition;
    [Export]
    float _mouseSensitivity = 10;
    Vector2 _mouseRelative = Vector2.Zero;
    [Export]
    float _cameraFov = 70f;
    const float MAX_ANGLE = 85;
    const float MIN_ANGLE = -85f;

    // Movement
    Vector3 _velocity = Vector3.Zero;
    Vector3 _direction = Vector3.Zero;
    bool _canSprint = true;
    bool _sprinting = false;
    bool _crouching = false;
    float _pushFactor = 0.5f;

    // Walking
    [Export]
    float _gravity = 9.81f;
    [Export]
    float _walkSpeed = 5;
    [Export]
    float _sprintSpeed = 8;
    [Export]
    float _crouchSpeed = 2;
    [Export]
    float _acceleration = 8;
    [Export]
    float _deacceleration = 10;
    [Export]
    float _midAirAcceleration = 0.75f;
    [Export]
    float _jumpHeight = 10;
    bool _grounded = true;
    readonly Vector3 FLOOR_NORMAL = Vector3.Up;
    [Export]
    float _crouchHeightDiff = -1.0f;

    [Export]
    float _heightChangeSpeed = 10;
    float _initialHeadY;

    // Flying
    [Export]
    float _flySpeed = 10;
    [Export]
    float _flyAcceleration = 4;
    bool _flying = false;

    // Slopes
    [Export]
    float _floorMaxAngle = 45;

    const float VELOCITY_CLAMP = 0.25f;

    // Interaction
    IInteractable _interactionTarget = null;
    bool _canMove = true;
    bool _canWalk = true;
    bool _canCameraRotate = true;
    float _minimumDistance = 1;
    float _maximumDistance = 2.5f;
    float _moveInterDistance = 0.1f;
    float _rotationDiff = 0;
    // float _rotationMatchingFactor = 5;
    float _moveEntitySpeedFactor = 5;

    // Other
    CollisionShape _colShape;
    CollisionShape _crouchColShape;

    GameManager _gameManager;
    GUIManager _guiManager;
    Inventory _inventory;

    string _heldItemId = null;
    public bool IsHoldingItem { get { return _heldItemId != null; } }

    public override void _Ready()
    {
        _gameManager = Singletons.MainManager.GameManager;
        _guiManager = _gameManager.GUIManager;
        _inventory = _guiManager.Inventory;

        _headSpatial = GetNode<Spatial>("Head");
        _playerCamera = _headSpatial.GetNode<Camera>("Camera");
        _interactionRay = _playerCamera.GetNode<RayCast>("InteractionRay");
        _interactionRay.CastTo = new Vector3(0, 0, -_maximumDistance);
        _movableGrabPosition = _playerCamera.GetNode<Position3D>("MovableGrabPosition");
        _movableGrabPosition.Translation = _interactionRay.CastTo;

        _colShape = GetNode<CollisionShape>("Col");
        _crouchColShape = GetNode<CollisionShape>("CrouchCol");

        _initialHeadY = _headSpatial.Transform.origin.y;

        _playerCamera.Fov = _cameraFov;

        Translation = _gameManager.GetPlayerPositionFromSave();
        Rotation = _gameManager.GetPlayerRotationFromSave();

        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Process(float delta)
    {
        if (_guiManager.PeekState() is CrosshairGUIState)
        {
            if (_heldItemId == null)
            {
                if (Input.IsActionJustPressed("inventory"))
                {
                    Logger.Log("Requested to open Inventory.");
                    _guiManager.PushState(new InventoryGUIState());
                }
                else if (Input.IsActionJustPressed("journal"))
                {
                    Logger.Log("Requested to open Journal.");
                    _guiManager.PushState(new JournalGUIState());
                }
                else if (Input.IsActionJustPressed("ui_cancel"))
                {
                    Logger.Log("Requested to open Pause Menu.");
                    _guiManager.PushState(new PauseGUIState());
                }
            }
            else
            {
                if (Input.IsActionJustPressed("ui_cancel"))
                {
                    if (_heldItemId == null)
                    {

                    }
                    else
                    {
                        Logger.Log("Canceled item use.");
                        _heldItemId = null;
                    }
                }
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_canCameraRotate)
            CameraRotation(delta);
        
        Interaction(delta);

        if (_canMove)
        {
            if (_flying)
                Fly(delta);
            else
                Walk(delta);
        }

        ApplyImpulsesToRigidBodies();
    }

    public void GrabItem(string itemId)
    {
        Logger.Log("Grabbing Item: " + itemId);
        _heldItemId = itemId;
        HUD.Instance.ChangeCrosshair(itemId);
    }

    void Interaction(float delta)
    {
        if (_interactionTarget == null)
        {
            _canMove = true;
            _canCameraRotate = true;
            _canWalk = true;

            IInteractable rayTarget = _interactionRay.IsColliding() && _interactionRay.GetCollider() is IInteractable inter ? inter : null;
            if (rayTarget != null)
            {
                ViewInteractableInfo(rayTarget);

                if (_heldItemId == null)
                {
                    if (Input.IsActionJustPressed("interaction"))
                    {                    
                        if (rayTarget is ItemEntity ie)
                            PickUpItem(ie);
                        else if (rayTarget is StateEntity se)
                            BeginChangeStateOfEntityState(se);
                        else if (rayTarget is ExitDoor ed)
                        {
                            if (ed.TryToEnter())
                            {
                                
                            }
                            else
                            {
                                Logger.Log("Door is closed.");
                                HideInteractableInfo(false);
                            }
                        }
                        else
                        {
                            _interactionTarget = rayTarget;
                            if (_interactionTarget is MovableEntity me)
                            {
                                float zVal = Mathf.Clamp(-(_playerCamera.GlobalTransform.origin.DistanceTo(me.GlobalTransform.origin)), -_maximumDistance, -_minimumDistance);
                                _movableGrabPosition.Translation = new Vector3(0, 0, zVal);

                                _rotationDiff = Utils.Convert3DAngleY(me.GlobalTransform.basis.GetEuler(), this.Rotation);
                            }
                            else if (_interactionTarget is GrabbableEntity ge)
                            {
                                ge.BeginInteraction();
                            }
                            HideInteractableInfo(false);
                        }
                    }
                    else if (Input.IsActionJustReleased("interaction") && rayTarget is StateEntity se)
                    {
                        EndChangeStateOfEntityState(se);
                    }
                    else
                        _interactionTarget = null;
                }
                else
                {
                    if (Input.IsActionJustReleased("interaction"))
                    {
                        if (rayTarget.UseItem(_heldItemId))
                        { // Item has been used correctly
                            _inventory.RemoveOne(_heldItemId);
                            _heldItemId = null;
                        }
                        else
                        { // Item has not been used correctly
                            _heldItemId = null;
                        }
                        HideInteractableInfo();
                    }
                }
                
            }
            else
                HideInteractableInfo();
        }
        else
        {
            if (Input.IsActionJustReleased("interaction"))
            {
                if (_interactionTarget is GrabbableEntity ge)
                {
                    ge.EndedInteraction();
                }

                _interactionTarget = null;
            }
            else if (_interactionTarget is MovableEntity me)
            {
                _canMove = true;
                _canCameraRotate = true;
                _canWalk = true;
                
                float dist = _movableGrabPosition.GlobalTransform.origin.DistanceTo(me.GlobalTransform.origin);
                Vector3 dir = (_movableGrabPosition.GlobalTransform.origin - me.GlobalTransform.origin).Normalized();

                me.LinearVelocity = dir * dist * _moveEntitySpeedFactor + _velocity;

                // Vector3 tempAngular = me.AngularVelocity;
                // tempAngular.y = (Utils.Convert3DAngleY(me.GlobalTransform.basis.GetEuler(), this.Rotation) - _rotationDiff) * _rotationMatchingFactor;
                // me.AngularVelocity = tempAngular;

                if (Input.IsActionPressed("rotate"))
                {
                    _canCameraRotate = false;
                    
                    float mouseX = _mouseRelative.x * _mouseSensitivity * delta;
                    float mouseY = _mouseRelative.y * _mouseSensitivity * delta;

                    _mouseRelative = Vector2.Zero;

                    Vector3 posDif = me.GlobalTransform.origin - this.GlobalTransform.origin;
                    posDif.y = 0;
                    posDif = posDif.Normalized();

                    me.AngularVelocity = new Vector3(-posDif.z * mouseY, mouseX, posDif.x * mouseY);

                    _rotationDiff = Utils.Convert3DAngleY(me.GlobalTransform.basis.GetEuler(), this.Rotation);
                }

                if (Input.IsActionJustPressed("push"))
                    {
                        _canCameraRotate = true;
                        _interactionTarget = null;

                        me.ApplyCentralImpulse((_movableGrabPosition.GlobalTransform.origin - _playerCamera.GlobalTransform.origin).Normalized() * me.Weight);
                    }
            }
            else if (_interactionTarget is GrabbableEntity ge)
            {
                _canMove = true;
                _canCameraRotate = false;
                _canWalk = false;
                
                ge.MouseMove(this, _mouseRelative, _mouseSensitivity, delta);
                _mouseRelative = Vector2.Zero;
            }
            else
                _interactionTarget = null;
        }
    }

    void ViewInteractableInfo(IInteractable interactableObject)
    {
        HUD.Instance.ShowInteractableInfo(interactableObject);
    }

    void HideInteractableInfo(bool resetCross = true)
    {
        HUD.Instance.HideInteractableInfo(resetCross);
    }

    void PickUpItem(ItemEntity ie)
    {
        HideInteractableInfo();
        
        Item item = ie.PickUp();
        ItemData itemData = Database.GetItemData(item.ItemId);
        switch (itemData.Type)
        {
            case ItemType.Item:
                _inventory.AddItem(item);
                break;
            case ItemType.Note:
                Singletons.MainManager.GameManager.AddNote(item.ItemId);
                _guiManager.PushState(new ReaderGUIState(item.ItemId));
                break;
            case ItemType.Diary:
                Singletons.MainManager.GameManager.AddEntry(item.ItemId);
                _guiManager.PushState(new ReaderGUIState(item.ItemId));
                break;
        }
    }

    void BeginChangeStateOfEntityState(StateEntity se)
    {
        se.BeginChangeState();
    }

    void EndChangeStateOfEntityState(StateEntity se)
    {
        se.EndChangeState();
    }

    void ApplyImpulsesToRigidBodies()
    {
        for (int i = 0; i < GetSlideCount(); i++)
        {
            KinematicCollision collision = GetSlideCollision(i);
            if (collision.Collider is RigidBody rb)
                rb.ApplyCentralImpulse(-collision.Normal * _velocity.Length() * _pushFactor);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion iemm)
            _mouseRelative = iemm.Relative;
        else if (@event is InputEventKey iek)
        {
            // if (iek.Pressed && iek.Scancode == (int)KeyList.Escape)
            // {
            //     if (Input.GetMouseMode() == Input.MouseMode.Captured)
            //         Input.SetMouseMode(Input.MouseMode.Visible);
            //     else
            //         Input.SetMouseMode(Input.MouseMode.Captured);
            // }
        }
        else if (@event is InputEventMouseButton iemb)
        {
            if (iemb.ButtonIndex == (int)ButtonList.WheelUp)
            {
                if (_interactionTarget != null)
                {
                    Vector3 temp = new Vector3(0, 0, Mathf.Clamp(_movableGrabPosition.Translation.z - _moveInterDistance, -_maximumDistance, -_minimumDistance));
                    _movableGrabPosition.Translation = temp;
                }
            }
            else if (iemb.ButtonIndex == (int)ButtonList.WheelDown)
            {
                if (_interactionTarget != null)
                {
                    Vector3 temp = new Vector3(0, 0, Mathf.Clamp(_movableGrabPosition.Translation.z + _moveInterDistance, -_maximumDistance, -_minimumDistance));
                    _movableGrabPosition.Translation = temp;
                }
            }
        }
    }

    void CameraRotation(float delta)
    {
        if (Input.GetMouseMode() != Input.MouseMode.Captured)
            return;
        
        if (_mouseRelative.LengthSquared() > 0)
        {
            float mouseX = -_mouseRelative.x * _mouseSensitivity * delta;
            float mouseY = -_mouseRelative.y * _mouseSensitivity * delta;

            _mouseRelative = Vector2.Zero;

            RotateY(Mathf.Deg2Rad(mouseX));

            _headSpatial.RotateX(Mathf.Deg2Rad(mouseY));

            Vector3 tempRot = _headSpatial.RotationDegrees;
            tempRot.x = Mathf.Clamp(tempRot.x, MIN_ANGLE, MAX_ANGLE);
            _headSpatial.RotationDegrees = tempRot;
        }
    }

    void Fly(float delta)
    {
        _grounded = false;

        _direction = Vector3.Zero;
        var aim = _headSpatial.GlobalTransform.basis;

        if (Input.IsActionPressed("ui_up"))
            _direction -= aim.z;
        if (Input.IsActionPressed("ui_down"))
            _direction += aim.z;
        if (Input.IsActionPressed("ui_left"))
            _direction -= aim.x;
        if (Input.IsActionPressed("ui_right"))
            _direction += aim.x;
        _direction = _direction.Normalized();

        var target = _direction * _flySpeed;
        if (_sprinting)
            target *= 2;
        _velocity = _velocity.LinearInterpolate(target, _flyAcceleration * delta);

        

        _velocity = MoveAndSlide(_velocity);
    }

    void Walk(float delta)
    {
        var aim = _headSpatial.GlobalTransform.basis;

        Vector3 snap = Vector3.Zero;
        _grounded = IsOnFloor();

        _direction = Vector3.Zero;

        if (_canWalk)
        {
            _canSprint = true;
            if (Input.IsActionPressed("ui_up"))
                _direction -= aim.z;
            if (Input.IsActionPressed("ui_down"))
            {
                _direction += aim.z;
                _canSprint = false;   
            }
            if (Input.IsActionPressed("ui_left"))
                _direction -= aim.x;
            if (Input.IsActionPressed("ui_right"))
                _direction += aim.x;
            _direction.y = 0;
            _direction = _direction.Normalized();    
        }
        
        if (_grounded)
        {
            snap = new Vector3(0, -1, 0);
            if (Input.IsActionJustPressed("ui_select"))
            {
                snap = Vector3.Zero;
                _velocity.y = _jumpHeight;
            }
        }
        
        _velocity.y += -_gravity * 2 * delta;

        float speed = 0;
        if (Input.IsActionPressed("crouch") && _grounded)
        {
            speed = _crouchSpeed;
            _playerCamera.Fov = Mathf.Lerp(_playerCamera.Fov, _cameraFov, delta * 8);
            _sprinting = false;
            _crouching = true;

            _colShape.Disabled = true;
            _crouchColShape.Disabled = false;
        }
        else if (!_crouching && Input.IsActionPressed("sprint") && _canSprint)
        {
            speed = _sprintSpeed;
            _playerCamera.Fov = Mathf.Lerp(_playerCamera.Fov, _cameraFov * 1.05f, delta * 8);
            _sprinting = true;
            _crouching = false;

            _colShape.Disabled = false;
            _crouchColShape.Disabled = true;
        }
        else
        {
            speed = _walkSpeed;
            _playerCamera.Fov = Mathf.Lerp(_playerCamera.Fov, _cameraFov, delta * 8);
            _sprinting = false;
            _crouching = false;

            _colShape.Disabled = false;
            _crouchColShape.Disabled = true;
        }

        if (_crouching)
        {
            Transform headTransform = _headSpatial.Transform;
            headTransform.origin.y = Mathf.Lerp(headTransform.origin.y, _initialHeadY + _crouchHeightDiff, delta * _heightChangeSpeed);
            _headSpatial.Transform = headTransform;
        }
        else if (_headSpatial.Transform.origin.y != 0)
        {
            Transform headTransform = _headSpatial.Transform;
            headTransform.origin.y = Mathf.Lerp(headTransform.origin.y, _initialHeadY, delta * _heightChangeSpeed);
            _headSpatial.Transform = headTransform;
        }

        Vector3 tempVel = _velocity;
        tempVel.y = 0;
        Vector3 target = _direction * speed;
        float tempAccel = 0;
        if (!_grounded)
            tempAccel = _midAirAcceleration;
        else if (_direction.Dot(tempVel) > 0)
            tempAccel = _acceleration;
        else
            tempAccel = _deacceleration;
        
        tempVel = tempVel.LinearInterpolate(target, tempAccel * delta);
        _velocity.x = tempVel.x;
        _velocity.z = tempVel.z;

        if (_direction.Dot(_velocity) == 0)
        {
            if (_velocity.x < VELOCITY_CLAMP && _velocity.x > -VELOCITY_CLAMP)
                _velocity.x = 0;
            if (_velocity.z < VELOCITY_CLAMP && _velocity.z > -VELOCITY_CLAMP)
                _velocity.z = 0;
        }

        _velocity.y = MoveAndSlideWithSnap(_velocity, snap, FLOOR_NORMAL, true, 4, Mathf.Deg2Rad(_floorMaxAngle), false).y;
    }
}
