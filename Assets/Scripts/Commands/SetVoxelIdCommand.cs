public class SetVoxelIdCommand : Command
{
    private int _voxelId = 0;

    public void SetVoxelId(int id)
    {
        _voxelId = id;
    }

    public override void Execute()
    {
        ChunkManager.VoxelId = _voxelId;
    }
}
