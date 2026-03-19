using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TraversableTile : Tile
{
    private Sprite sprite;
    
    public enum OccupiedState
    {
        UNKNOWN,
        UNOCCUPIED,
        OCCUPIED
    }

    public enum TTType
    {
        UNKNOWN,
        DOOR_TT,
        GENERIC_TT,
    }

    private OccupiedState occupiedState = OccupiedState.UNOCCUPIED;
    private TTType ttType = TTType.UNKNOWN;
    
    public override void Start()
    {
        tileType = TileType.TT;
        sprite =  GetComponent<SpriteRenderer>().sprite;
        
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(".../Assets/Textures/Test/TestPlayer.png");
        handle.Completed += loadSpritesWhenReady;
    }
    
    void Update()
    {
        
    }

    public OccupiedState getOccupationState()
    {
        return occupiedState;
    }

    public TTType getTTType()
    {
        return ttType;
    }

    public void setOccupationState(OccupiedState state)
    {
        occupiedState = state;
    }

    public void setTTType(TTType type)
    {
        ttType = type;
    }

    private void loadSpritesWhenReady(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite = handle.Result;
        }
    }
    
}
