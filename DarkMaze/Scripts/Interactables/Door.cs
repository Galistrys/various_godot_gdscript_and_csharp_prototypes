using Godot;
using Godot.Collections;
using System;

public class Door : GrabbableEntity
{
    [Export]
    public bool _isLocked = false;
    public bool IsLocked
    {
        get
        {
            return _isLocked;
        }
        set
        {
            Logger.Log($"Setting IsLocked to \"{value}\"");
            _isLocked = value;
        }
    }
    [Export]
    string itemIdToUnlock = "";

    [Signal]
    public delegate void HandlePressed(string eventId);
    [Signal]
    public delegate void HandleReleased(string eventId);
    [Signal]
    public delegate void Opened(string eventId);
    [Signal]
    public delegate void Closed(string eventId);
    [Signal]
    public delegate void Unlocked(string eventId);

    [Export]
    private string _onDoorLockedEvent = "";
    [Export]
    private string _onHandlePressEvent = "";
    [Export]
    private string _onHandleReleaseEvent = "";
    [Export]
    private string _onOpenEvent = "";
    [Export]
    private string _onCloseEvent = "";
    [Export]
    private string _onUnlockEvent = "";

    [Export]
    private float _mouseSensitivityMod = 1f;
    public float MouseSensitivityMod { get { return _mouseSensitivityMod; } }

    [Export]
    private Vector3 _rotationMaxValues;
    private Vector3 RotationMaxValues { get { return _rotationMaxValues; } }

    [Export]
    private Vector3 _rotationMinValues;
    private Vector3 RotationMinValues { get { return _rotationMinValues; } }

    private bool _isClosed = false;
    public bool IsClosed
    {
        get
        {
            return _isClosed;
        }
        set
        {
            _isClosed = value;

            Logger.Log($"Setting IsClosed to \"{_isClosed}\"");
            
            if (_isClosed)
                EmitSignal(nameof(Closed), _onOpenEvent);
            else
                EmitSignal(nameof(Opened), _onCloseEvent);
        }
    }

    public override void MouseMove(Spatial caller, Vector2 mouse, float sensitivity, float delta)
    {
        if (!IsLocked)
        {
            float mouseX = mouse.x * sensitivity * _mouseSensitivityMod * delta;
            float mouseY = mouse.y * sensitivity * _mouseSensitivityMod * delta;

            Vector3 callerLocal = (GetChild(0) as Spatial).ToLocal(caller.GlobalTransform.origin);
            bool IsCallerAtFront = callerLocal.z > 0;

            float globalRot = (GetChild(0) as Spatial).GlobalTransform.basis.GetEuler().y;
            float callerRot = caller.Rotation.y + Mathf.Pi / 2.0f;

            Vector2 tempGlo = new Vector2(Mathf.Cos(globalRot), Mathf.Sin(globalRot));
            Vector2 tempCal = new Vector2(Mathf.Cos(callerRot), Mathf.Sin(callerRot));

            float angle = tempGlo.AngleTo(tempCal);

            float finalAngleX = -mouseX * Mathf.Cos(angle);
            float finalAngleY = -mouseY * Mathf.Sin(angle);

            ApplyTorqueImpulse(Vector3.Up * finalAngleX * delta);
            ApplyTorqueImpulse(Vector3.Up * finalAngleY * delta);
        }
    }

    public override void BeginInteraction()
    {
        Logger.Log("Begin Interaction");

        if (!IsLocked)
        {
            this.Mode = RigidBody.ModeEnum.Rigid;
            IsClosed = false;
            SetPhysicsProcess(false);
            EmitSignal(nameof(HandlePressed), _onHandlePressEvent);
        }
        else
        {
            IsClosed = true;
            Logger.Log("Door is locked!");
            EmitSignal(nameof(HandlePressed), _onDoorLockedEvent);
        }
    }

    public override void EndedInteraction()
    {
        Logger.Log("Ended Interaction");

        SetPhysicsProcess(true);
        if (!IsLocked)
        {
            EmitSignal(nameof(HandleReleased), _onHandleReleaseEvent);
        }
        else
        {
            EmitSignal(nameof(HandleReleased), _onDoorLockedEvent);
        }
    }

     public override void _Process(float delta)
    {
        Vector3 tempRot = this.RotationDegrees;
        Vector3 tempAng = this.AngularVelocity;

        for (int i = 0; i < 3; i++)
        {
            if (tempRot[i] < _rotationMinValues[i])
            {
                tempRot[i] = _rotationMinValues[i];
                this.RotationDegrees = tempRot;
                tempAng[i] = 0;
                this.AngularVelocity = tempAng;
            }
            if (tempRot[i] > _rotationMaxValues[i])
            {
                tempRot[i] = _rotationMaxValues[i];
                this.RotationDegrees = tempRot;
                tempAng[i] = 0;
                this.AngularVelocity = tempAng;
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!IsClosed && this.RotationDegrees.y < 0.01f && this.Mode == RigidBody.ModeEnum.Rigid)
        {
            this.Mode = RigidBody.ModeEnum.Static;
            Vector3 temp = this.RotationDegrees;
            temp.y = 0;
            this.RotationDegrees = temp;
            IsClosed = true;
        }
    }

    public override bool UseItem(string itemId)
    {
        Logger.Log("Using item: " + itemId);

        if (IsLocked && itemId == itemIdToUnlock)
        {
            IsLocked = false;
            EmitSignal(nameof(Unlocked), _onUnlockEvent);
            return true;
        }
        return false;
    }

    public override Dictionary<string, object> ExportToDictionary()
    {
        return new Dictionary<string, object>() {
            {
                nameof(IsClosed),
                GD.Var2Str(IsClosed)
            },
            {
                nameof(IsLocked),
                GD.Var2Str(IsLocked)
            },
            {
                nameof(Rotation),
                GD.Var2Str(this.Rotation)
            }
        };
    }

    public override void LoadFromDictionary(Dictionary<string, object> dataDictionary)
    {
        foreach (var pair in dataDictionary)
        {
            Set(pair.Key, GD.Str2Var(GD.Str(pair.Value)));
        }
    }
}
