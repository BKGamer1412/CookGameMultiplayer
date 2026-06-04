using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerController : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPlayerPickedUpSomething;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
        OnAnyPlayerPickedUpSomething = null;
    }
    public event EventHandler OnPickedUpSomething;
    public static PlayerController LocalInstance { get; private set; }
    public event EventHandler<OnSelectedCounterChangedeventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedeventArgs : EventArgs
    {
        public BaseCounter seletectedCounter;
    }
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> playerSpawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;

    private bool isWalking;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    // private void Awake()
    // {
    //     Instance = this;
    // }
    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }


    //(this same as Start and Awaken method but for netcode multiplayer)
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        //player spawn position
        transform.position = playerSpawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) { return; }

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) { return; }

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        // HandleMovementServerAuth();
        HandleMovement();
        HandleInteraction();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastInteractDirection = moveDir;
        }

        float interactDistance = 2f;

        //nếu muốn lấy list object thì xài RaycastAll hoặc thêm LayerMask để lọc
        //nếu có nhiều object khi raycast thì sẽ lấy cái đầu tiên
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, counterLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Has Clear Coutner
                if (baseCounter != selectedCounter)
                {
                    SelectedCounter(baseCounter);
                }
            }
            else
            {
                SelectedCounter(null);
            }
        }
        else
        {
            SelectedCounter(null);
        }
    }

    //người chơi gọi hàm này
    private void HandleMovementServerAuth()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        HandleMovementServerRpc(inputVector);
    }


    //server (host) sẽ xử lý, use case: nếu game mang tính cạnh tranh và chống lại việc user xài hack
    [ServerRpc(RequireOwnership = false)]
    private void HandleMovementServerRpc(Vector2 inputVector)
    {
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;

        float playerRadius = .7f;
        float playerHeight = 2f;
        //nếu raycast ko va chạm thì kết quả là true
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        if (!canMove)
        {
            //cannot move towards moveDir

            //Attempt move in the X direction
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > .5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                //can only move in the X direction
                moveDir = moveDirX;
            }
            else
            {
                //Attempt move in the Z direction
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > .5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    //can only move in the Z direction
                    moveDir = moveDirZ;
                }
                else
                {
                    //cannot move in any direction
                    // canMove = false;
                }
            }


        }
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;

        float playerRadius = .7f;
        // float playerHeight = 2f;
        //nếu raycast ko va chạm thì kết quả là true 
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionLayerMask);
        if (!canMove)
        {
            //cannot move towards moveDir

            //Attempt move in the X direction
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > .5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionLayerMask);
            if (canMove)
            {
                //can only move in the X direction
                moveDir = moveDirX;
            }
            else
            {
                //Attempt move in the Z direction
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > .5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionLayerMask);
                if (canMove)
                {
                    //can only move in the Z direction
                    moveDir = moveDirZ;
                }
                else
                {
                    //cannot move in any direction
                    // canMove = false;
                }
            }


        }
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
    }

    private void SelectedCounter(BaseCounter clearCounter)
    {
        selectedCounter = clearCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedeventArgs
        {
            seletectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjecFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedUpSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPlayerPickedUpSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
