using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class CS_FirstPersonController : NetworkBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private CS_MoteLook m_MouseLook;
        //[SerializeField] private MouseLook m_MouseLookBackUp;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private TextMesh m_NameTag;
        [SerializeField] public Image[] m_arrowImages;
        [SerializeField] private Text PowerUpText;


        // Player Settings
        [SerializeField] private int m_iMaxHealth = 100;
        [SyncVar] public int m_iHealth;
        [SerializeField] private int m_iMaxDamage = 10; 
        [SyncVar]
        private int m_iDamage;
        [SerializeField] private int m_iMaxShootingDistance = 2000; // For the raycast
        private GameObject SpawnManager;
        [SerializeField] ParticleSystem m_pGunShot;
        [SyncVar(hook = "OnPlayerIDChange")]public string m_ssName;

        [SyncVar] public int iKills = 0;
        [SerializeField] private GameObject WinningDisplay;
        [SerializeField] private GameObject LosingDisplay;

        private Camera m_Camera;
        private bool m_Jump;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        public GameObject[] Trains;
        private float fTimer;
        private bool bStartDeathTimer;
        private bool bShootPrevent;

        // HUD
        private Canvas m_PlayerHUD;
        private Slider m_HealthBar;

        // Use this for initialization
        private void Start()
        {

            m_AudioSource = GetComponent<AudioSource>();
            m_ssName = "Player: " + netId; // Default
            fTimer = 0;
            m_iHealth = m_iMaxHealth;
            m_iDamage = m_iMaxDamage;
            bStartDeathTimer = false;
            m_PlayerHUD = GetComponentInChildren<Canvas>(true); // Gets PlayerHUD
            m_HealthBar = m_PlayerHUD.GetComponentInChildren<Slider>(); // Cache the health bar
            m_HealthBar.value = m_iHealth; // Set value to start health
            bShootPrevent = true;
            m_pGunShot = GetComponentInChildren<ParticleSystem>();
            SpawnManager = GameObject.FindGameObjectWithTag("Spawnpoint"); // Find a faster way of doing this


            foreach(Image image in m_arrowImages)
            {
                image.enabled = false;
            }
            Trains = GameObject.FindGameObjectsWithTag("Train1");
            if (isLocalPlayer)
            {
                // Enable the HUD for the local player
                m_PlayerHUD.gameObject.SetActive(true);
                // Set player name as recorded name
                m_ssName = CS_SessionManager.player.UserName;
                CmdSetPlayerName(m_ssName); // Set name on the server
                PowerUpText.enabled = false;
                m_CharacterController = GetComponent<CharacterController>();
                m_Camera = GetComponentInChildren<Camera>();
                m_OriginalCameraPosition = m_Camera.transform.localPosition;
                m_FovKick.Setup(m_Camera);
                m_HeadBob.Setup(m_Camera, m_StepInterval);
                m_StepCycle = 0f;
                m_NextStep = m_StepCycle / 2f;
                m_Jumping = false;

                //m_MouseLookBackUp.Init(transform, m_Camera.transform);
                m_MouseLook.Init(transform, m_Camera.transform);
            }
        }

        
        // This is called on the winning player
        public void EndGame(string a_winningPlayer)
        {
            WinningDisplay.SetActive(true);
            LosingDisplay.SetActive(false);

            if (CS_GameManager.GetPlayer(a_winningPlayer) == this)
            {
                // Display winner
                WinningDisplay.SetActive(true);
                LosingDisplay.SetActive(false);
            }
        }

        [Command]
        public void CmdEndGame(string a_winningPlayer)
        {
            RpcEndGame(a_winningPlayer);
        }

        [ClientRpc]
        public void RpcEndGame(string a_winningPlayer)
        {
            if(CS_GameManager.GetPlayer(a_winningPlayer).gameObject != this)
            {
                // Display Loser
                WinningDisplay.SetActive(false);
                LosingDisplay.SetActive(true);
            }
        }

        [Command]
        void CmdSetPlayerName(string a_nameTag)
        {
            m_ssName = a_nameTag;
        }
        private void LateUpdate()
        {
            /*
            BodyTopInitRotation.z = BodyTop.transform.eulerAngles.z;
            BodyTop.transform.eulerAngles = BodyTopInitRotation;

            BodyCannonInitRotation.x = BodyCannon.transform.eulerAngles.x;
            BodyCannonInitRotation.y = BodyCannon.transform.eulerAngles.y;
            BodyCannon.transform.eulerAngles = BodyCannonInitRotation;

            BodyBottom.transform.eulerAngles = BodyBottomInitRotation;
            */
        }


        void OnPlayerIDChange(string a_theirName)
        {
            m_NameTag.text = a_theirName;
        }

        public void SetName(string a_ssNewName)
        {
            m_ssName = a_ssNewName;
        }

        public string GetName()
        {
            return m_ssName;
        }

        public void SetWalkSpeed(int a_iSpeed)
        {
            m_WalkSpeed = a_iSpeed;
        }
        public float GetWalkSpeed()
        {
            return m_WalkSpeed;
        }

        public void SetDamage(int a_iDamage)
        {
            m_iDamage = a_iDamage;
        }
        public int GetDamage()
        {
            return m_iDamage;
        }


        public void SetHealth(int a_iHealth)
        {
            m_iHealth = a_iHealth;
        }
        public int GetHealth()
        {
            return m_iHealth;
        }
        public void SetMaxHealth(int a_iHealth)
        {
            m_iMaxHealth = a_iHealth;
        }
        public int GetMaxHealth()
        {
            return m_iMaxHealth;
        }


        public void AimAssist()
        {
            RaycastHit outHit;
            if (Physics.Raycast(transform.position, m_Camera.transform.forward, out outHit, m_iMaxShootingDistance))
            {
                if (outHit.transform.gameObject.tag == "Player")
                {
                    ReduceSensitivity();
                }
                else
                {
                    OriginalSensitivity();
                }
            }
        }
        public void ReduceSensitivity()
        {
            m_MouseLook.XSensitivity = 1.0f;
            m_MouseLook.YSensitivity = 1.0f;
        }
        public void OriginalSensitivity()
        {
            m_MouseLook.XSensitivity = m_MouseLook.StartXSensitivity;
            m_MouseLook.YSensitivity = m_MouseLook.StartYSensitivity;
        }


        public override void OnStartClient()
        {
            base.OnStartClient();

            string netID = GetComponent<NetworkIdentity>().netId.ToString();
            Debug.Log(m_ssName + " Joined");
            CS_GameManager.RegisterPlayer(netID, GetComponent<CS_FirstPersonController>());
        }

        private void OnDisable()
        {
            Debug.Log(m_ssName + " Left");
            CS_GameManager.RemovePlayer(transform.name);
            
        }

        public void SetText(string a_sPower)
        {
            PowerUpText.enabled = true;
            PowerUpText.text = a_sPower;
            PowerUpText.fontSize = 14;
        }

        public void BlowText()
        {
            if (PowerUpText.enabled && PowerUpText.fontSize < 130)
            {
                PowerUpText.fontSize += 1;
            }
            else if(PowerUpText.fontSize >= 130)
            {
                PowerUpText.enabled = false;
            }
        }

        [Client]
        private void Shoot()
        {
            Debug.Log("Client Shoot");

            CS_SoundTest.PlaySoundOnObject(gameObject, "SFX/Player/PlayerShoot");

            RaycastHit outHit;
            if (Physics.Raycast(transform.position, m_Camera.transform.forward, out outHit, m_iMaxShootingDistance))
            {
                if (outHit.transform.gameObject.tag == "Player")
                {
                    CmdPlayerShot(gameObject.name, outHit.transform.gameObject.name, m_iDamage);
                }
            }
        }

        [Command] // Commands are called by clients to execute following function on server
        void CmdPlayerShot(string a_instigatorID, string a_playerHitID, int a_damage)
        {
            Debug.Log("CmdPlayerShot");

            CS_GameManager.GetPlayer(a_playerHitID).RpcTakeDamage(a_instigatorID, a_damage);

        }

        [Client]
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Train1")
            {
                gameObject.transform.SetParent(other.transform);
                bStartDeathTimer = true;
                gameObject.transform.SetParent(null);
                CS_GameManager.GetPlayer(gameObject.name).CmdPlayerShot(gameObject.name, other.gameObject.name, 100);

            }
        }


        // Called by the server to run on all clients
        [ClientRpc]
        private void RpcTakeDamage(string a_instigatorID, int a_iDamageToTake)
        {
            Debug.Log("RPCTakeDamage");
            m_iHealth -= a_iDamageToTake;
            m_HealthBar.value = m_iHealth;
            if(m_iHealth <= 0)
            {
                m_iHealth = m_iMaxHealth;
                m_HealthBar.value = m_iHealth;
                RpcDie(a_instigatorID);
            }
        }

        [ClientRpc]
        public void RpcAddHealth(int a_iDamageToTake)
        {
            Debug.Log("RPCAddHealth");
            m_iHealth += a_iDamageToTake;
            m_HealthBar.value = m_iHealth;
            if (m_iHealth >= m_iMaxHealth)
            {
                m_iHealth = m_iMaxHealth;
                m_HealthBar.value = m_iHealth;
            }
        }

        [ClientRpc]
        private void RpcDie(string a_instigatorID)
        {
            Debug.Log("RPCDIE");
            CS_GameManager.GetPlayer(a_instigatorID).iKills++;
            CS_GameManager.CheckForEnd();

            CS_SoundTest.PlaySoundOnObject(gameObject, "SFX/Player/PlayerDeath");
            transform.position = SpawnManager.GetComponent<CS_Spawnpoints>().GetValidSpawnPoint();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            BlowText();
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
            if(!m_MouseLook.MoteScene.wiimote.Nunchuck.c)
            {
                bShootPrevent = true;
            }
            if ((m_MouseLook.MoteScene.wiimote.Nunchuck.c && bShootPrevent))
            {
                Shoot();
                bShootPrevent = false;
            }

        }



        private void PlayLandingSound()
        {
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;
            AimAssist();

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            //m_MouseLookBackUp.UpdateCursorLock();
            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {

        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0

        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            //m_MouseLookBackUp.LookRotation(transform, m_Camera.transform);
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
