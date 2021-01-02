using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SushiNinja.ThirdPersonSystem
{
    public enum InputEnterType { ButtonDown, ButtonPressing, ButtonReleased, Noone }

    public abstract class ThirdPersonAbility : MonoBehaviour
    {
        public bool Active { get; protected set; } = false;
        public bool Blocked { get; protected set; } = false;


        [Tooltip("Name of enter state of ability")] [SerializeField] protected string m_EnterState = "";
        [Tooltip("Animation transition duration to play this animation")] [SerializeField] protected float m_TransitionDuration = 0.1f;
        [Tooltip("Multiplier of root motion velocity in all axis")] [SerializeField] protected Vector3 m_RootMotionMultiplier = Vector3.one;
        [Tooltip("Should exit this ability when animation ends?")] [SerializeField] protected bool m_FinishOnAnimationEnd = false;

        [Space(5)]

        [Tooltip("Abilities that must be ignored to enter this ability. Means that this ability has higher priority than these ignored abilities")]
        public List<ThirdPersonAbility> IgnoreAbilities;



        [Tooltip("Should this ability uses root motion?")] [SerializeField] protected bool m_UseRootMotion = true;
        [Tooltip("Should this ability uses rotation root motion?")] [SerializeField] protected bool m_UseRotationRootMotion = false;
        [Tooltip("Should this ability uses root motion in vertical direction?")] [SerializeField] protected bool m_UseVerticalRootMotion = false;

        [SerializeField] protected bool m_AllowCameraZoom = false;

        [SerializeField] protected PhysicMaterial m_AbilityPhysicMaterial;


        public bool UseRootMotion { get { return m_UseRootMotion; } }
        public bool UseRotationRootMotion { get { return m_UseRotationRootMotion; } }
        public bool UseVerticalRootMotion { get { return m_UseVerticalRootMotion; } }
        public Vector3 RootMotionMultiplier { get { return m_RootMotionMultiplier; } }

        public bool AllowCameraZoom { get { return m_AllowCameraZoom; } }
        public float TransitionDuration { get { return m_TransitionDuration; } }

        public PhysicMaterial AbilityPhysicMaterial { get { return m_AbilityPhysicMaterial; } set { m_AbilityPhysicMaterial = value; } }
        public float AbilityEnterFixedTime { get; private set; } = 0;
        public float AbilityExitFixedTime { get; private set; } = 0;

        protected ThirdPersonSystem m_System;
        protected AnimatorManager m_AnimatorManager;
        protected UnityInputManager m_InputManager;

        public UnityEvent OnEnterAbilityEvent, OnExitAbilityEvent;

        protected string m_CurrentStatePlaying = "";
        public string CurrentStatePlaying { get { return m_CurrentStatePlaying; } }

        protected InputButton m_InputToEnter = null;

        [SerializeField] protected InputEnterType m_UseInputStateToEnter = InputEnterType.Noone;
        [SerializeField] protected InputReference InputButton;

        protected bool m_InputStateSet = false;

        public virtual void Initialize(ThirdPersonSystem mainSystem, AnimatorManager animatorManager, UnityInputManager inputManager)
        {
            m_System = mainSystem;
            m_AnimatorManager = animatorManager;
            m_InputManager = inputManager;

            m_InputToEnter = m_InputManager.GetInputReference(InputButton);
        }

        protected virtual void FixedUpdate()
        {
            if (Blocked)
            {
                if (Active)
                    m_System.ExitAbility(this);

                return;
            }

            if (Active)
            {
                if (m_System.enabled == false)
                {
                    m_System.ExitAbility(this);
                    return;
                }

                if (m_FinishOnAnimationEnd && m_AnimatorManager.HasFinishedAnimation(m_CurrentStatePlaying))
                {
                    m_System.ExitAbility(this);
                    return;
                }

                if (TryExitAbility())
                    m_System.ExitAbility(this);
            }
            else
            {
                if (ForceEnterAbility())
                    m_System.OnTryEnterAbility(this);
                else
                {
                    if (m_InputStateSet)
                    {
                        if (TryEnterAbility())
                            m_System.OnTryEnterAbility(this);
                    }
                }
            }

            m_InputStateSet = false;
        }

        protected virtual void Update()
        {
            if (Time.timeScale <= 0.05f || Blocked)
                return;

            SetButtonState();
        }

        protected void SetButtonState()
        {
            if (m_InputToEnter == null)
                return;

            switch (m_UseInputStateToEnter)
            {
                case InputEnterType.ButtonDown:
                    if (m_InputToEnter.WasPressed)
                        m_InputStateSet = true;
                    break;
                case InputEnterType.ButtonPressing:
                    m_InputStateSet = m_InputToEnter.IsPressed;
                    break;
                case InputEnterType.ButtonReleased:
                    if (m_InputToEnter.WasReleased)
                        m_InputStateSet = true;
                    break;
                case InputEnterType.Noone:
                    m_InputStateSet = true;
                    break;
            }
        }

        public virtual bool TryEnterAbility() { return false; }

        public virtual bool ForceEnterAbility() { return false; }

        public virtual void OnEnterAbility()
        {
            OnEnterAbilityEvent.Invoke();
            Active = true;
            SetState(GetEnterState());

            if (m_AbilityPhysicMaterial != null)
                m_System.m_Capsule.sharedMaterial = m_AbilityPhysicMaterial;

            AbilityEnterFixedTime = Time.fixedTime;
        }

        public virtual void FixedUpdateAbility()
        {
        }

        public virtual void UpdateAbility()
        {
        }


        public virtual bool TryExitAbility() { return false; }

        public virtual void OnExitAbility()
        {
            OnExitAbilityEvent.Invoke();
            Active = false;
            m_CurrentStatePlaying = string.Empty;
            m_System.ChangeCapsuleSize(m_System.CapsuleOriginalHeight);

            AbilityExitFixedTime = Time.fixedTime;
        }

        public virtual string GetEnterState() { return m_EnterState; }

        protected void SetState(string newState)
        {
            SetState(newState, m_TransitionDuration);
        }
        protected virtual void SetState(string newState, float transitionDuration)
        {
            if (m_CurrentStatePlaying == newState)
                return;

            m_AnimatorManager.SetAnimatorState(newState, transitionDuration, AnimatorManager.BaseLayerIndex);
            m_CurrentStatePlaying = newState;
        }

        protected Quaternion GetRotationFromDirection(Vector3 direction)
        {
            float yaw = Mathf.Atan2(direction.x, direction.z);
            return Quaternion.Euler(0, yaw * Mathf.Rad2Deg, 0);
        }

        protected Quaternion GetRotationFromDirection(Vector3 direction, float yawOffset)
        {
            float yaw = Mathf.Atan2(direction.x, direction.z);
            return Quaternion.Euler(0, (yaw * Mathf.Rad2Deg) + yawOffset, 0);
        }

        protected bool FreeOnMove()
        {
            Vector3 p1 = transform.position + Vector3.up * m_System.m_Capsule.radius * 2;
            Vector3 p2 = transform.position + Vector3.up * (m_System.m_Capsule.height - m_System.m_Capsule.radius);

            RaycastHit[] hits = Physics.CapsuleCastAll(p1, p2, m_System.m_Capsule.radius * 0.5f, transform.forward,
                                                        m_System.m_Rigidbody.velocity.sqrMagnitude * Time.fixedDeltaTime + 0.25f, m_System.GroundMask,
                                                        QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.normal.y < 0.5f && hit.collider.tag != "Player")
                    return false;
            }

            return true;
        }

        protected bool FreeOnMove(Vector3 direction)
        {
            Vector3 p1 = transform.position + Vector3.up * m_System.m_Capsule.radius * 2;
            Vector3 p2 = transform.position + Vector3.up * (m_System.m_Capsule.height - m_System.m_Capsule.radius);

            RaycastHit[] hits = Physics.CapsuleCastAll(p1, p2, m_System.m_Capsule.radius * 0.5f, direction,
                                                        m_System.m_Rigidbody.velocity.sqrMagnitude * Time.fixedDeltaTime + 0.25f, m_System.GroundMask,
                                                        QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.normal.y <= Mathf.Cos(m_System.MaxAngleSlope * Mathf.Deg2Rad) && hit.collider.tag != "Player")
                    return false;
            }

            return true;
        }

        public void BlockAbility()
        {
            Blocked = true;
        }

        public void UnblockAbility()
        {
            Blocked = false;
        }


        private void Reset()
        {
            m_InputToEnter = GetComponent<UnityInputManager>().jumpButton;
        }
    }
}
