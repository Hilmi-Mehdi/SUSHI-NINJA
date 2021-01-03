using UnityEngine;

namespace SushiNinja.ThirdPersonSystem
{
    public class FallAbility : ThirdPersonAbility
    {
        [Tooltip("Vertical height higher than this value will cause a hard land")][SerializeField] private float m_HeightToHardLand = 3f;
        [Tooltip("Name of the hard land state")] [SerializeField] private string m_HardLandState = "Air.Hard Landing";

        [SerializeField] private float m_MinHeightToCauseDamagee = 4f;
        [SerializeField] private float m_HeightToDie = 10f;

        private bool isHardLand = false;
        private float initialHeight = 0;
        
        public override bool TryEnterAbility()
        {
            return (!m_System.IsGrounded && m_System.m_Rigidbody.velocity.y < 0f);
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();
            initialHeight = transform.position.y;
        }


        public override bool TryExitAbility()
        {
            float height = initialHeight - transform.position.y;
            if (m_System.IsGrounded)
            {
                if (height >= m_HeightToHardLand) 
                {
                    SetState(m_HardLandState, 0.05f); 

                    if (!isHardLand)
                    {
                        float damageAmount = ((height - m_MinHeightToCauseDamagee) / (m_HeightToDie - m_MinHeightToCauseDamagee)) * 100f;
                        if(damageAmount > 0)
                            GlobalEvents.ExecuteEvent("Damage", gameObject, damageAmount);
                    }

                    isHardLand = true;
                    m_UseRootMotion = true;
                    m_FinishOnAnimationEnd = true;
                    return false;
                }

                return true;
            }

            return false;
        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();
            
            isHardLand = false;
            m_UseRootMotion = false;
        }


        private void Reset()
        {
            m_EnterState = "Air.Falling";
            m_TransitionDuration = 0.3f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = false;
        }
    }
}
