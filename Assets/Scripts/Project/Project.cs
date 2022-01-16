using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using UnityEngine;

public class Project : MonoBehaviour
{
    public static ReportField<bool> Saved { get; private set; }

    //C:\Example
    public static string Name { get; set; } //Example (Project name)
    public static string MainFileName { get; set; } //Example.ndr (C:\Example\Example.ndr)
    public static string RootPath { get; set; } //C:\
    public static string ProjectPath => Path.GetFullPath($@"{RootPath}\{Name}"); //C:\Example
    public static string DataPath => ProjectPath + @"\Data"; //C:\Example\Data
    public static string VoxelsPath => $@"{DataPath}\Voxels"; //C:\Example\Data\Voxels
    public static string VerticesPath => $@"{DataPath}\Vertices"; //C:\Example\Data\Vertices
    public static string VoxelChunkPath => $@"{VoxelsPath}\{VoxelChunkName}"; //C:\Example\Data\Voxels\VoxelChunk
    public static string VertexChunkPath => $@"{VerticesPath}\{VertexChunkName}"; //C:\Example\Data\Vertices\VertexChunk
    public static string FileExtension => "ndr";
    public static string VoxelChunkName => "VoxelChunk";
    public static string VertexChunkName => "VertexChunk";

    private static bool _saved;

    private static Thread _savingThread;
    private static readonly ConcurrentQueue<Chunk<VoxelUnit>> _voxelChunksQueue = new ConcurrentQueue<Chunk<VoxelUnit>>();
    private static readonly ConcurrentQueue<Chunk<VertexUnit>> _vertexChunksQueue = new ConcurrentQueue<Chunk<VertexUnit>>();
    private static LinkedList<Chunk<VoxelUnit>> _voxelChunksList = new LinkedList<Chunk<VoxelUnit>>();
    private static LinkedList<Chunk<VertexUnit>> _vertexChunksList = new LinkedList<Chunk<VertexUnit>>();

    static Project()
    {
        Application.wantsToQuit += OnQuit;

        Saved = new ReportField<bool>(true);
    }

    public static void Init()
    {
        Saved.Value = _saved = true;

        _savingThread = new Thread(Saving)
        {
            IsBackground = true,
            Priority = System.Threading.ThreadPriority.Normal
        };

        _savingThread.Start();

        Voxelator.VoxelChunkManager.UpdateChunkEvent += OnUpdateVoxelChunk;
        Voxelator.VertexChunkManager.UpdateChunkEvent += OnUpdateVertexChunk;
    }

    public static bool Exists(string name, string rootPath)
    {
        return Directory.Exists(Path.GetFullPath($@"{rootPath}\{name}"));
    }

    public static bool TryCreate(string name, string rootPath)
    {
        if (!Directory.Exists(rootPath)) return false;

        Name = name;
        MainFileName = $@"{name}.{FileExtension}";
        RootPath = Path.GetFullPath(rootPath);

        CreateDirectories();

        return true;
    }

    public static bool TryLoad(string path)
    {
        if (!File.Exists(path)) return false;

        Name = Path.GetFileName(Directory.GetParent(path).FullName);
        MainFileName = Path.GetFileName(path);
        RootPath = Directory.GetParent(Directory.GetParent(path).FullName).FullName;
        Directory.CreateDirectory(DataPath);
        Directory.CreateDirectory(VoxelsPath);
        Directory.CreateDirectory(VerticesPath);

        //voxelator data reading
        using (StreamManager stream = new StreamManager(path))
        {
            Vector3Int fieldSize;
            int incrementOption;

            if (!stream.TryRead() ||
            !stream.TryGetVector3Int(out fieldSize) ||
            !stream.TryGetInt(out incrementOption)) return false;

            Release();
            Voxelator.Release();
            Voxelator.Init(fieldSize, (Voxelator.IncrementOptionType)incrementOption);
        }

        //abstract reading function 
        void readChunk<C, U>(string chunkPath, ChunkManager<C, U> chunkManager, Func<StreamManager, bool> readFunc) where C : Chunk<U> where U : Unit
        {
            for(int i = 0; i < chunkManager.Chunks.Length; i++)
            {
                using (StreamManager stream = new StreamManager($@"{chunkPath}{i}"))
                {
                    if(stream.TryRead())
                    {
                        while(true)
                        {
                            if (!readFunc(stream)) break;
                        }
                    }
                }
            }
        }

        //voxel data reading
        readChunk(VoxelChunkPath, Voxelator.VoxelChunkManager, (stream) =>
        {
            int voxelIndex;
            Vector3Byte color;
            if (!stream.TryGetInt(out voxelIndex) || !stream.TryGetVector3Byte(out color)) return false;

            Voxelator.CreateVoxel(voxelIndex, color);
            return true;
        });

        //vertex data reading
        readChunk(VertexChunkPath, Voxelator.VertexChunkManager, (stream) =>
        {
            int vertexIndex;
            Vector3Byte vertexOffset;

            if (!stream.TryGetInt(out vertexIndex) || !stream.TryGetVector3Byte(out vertexOffset)) return false;

            Vector3Int vertexPosition = VoxelatorArray.GetPosition(Voxelator.FieldSize + Vector3Int.one, vertexIndex);
            Voxelator.VertexChunkManager.SetVertexOffset(vertexPosition, vertexOffset.ToVector3() / Voxelator.IncrementOption);
            return true;
        });

        Voxelator.UpdateChunks();
        Init();

        return true;
    }

    public static bool TrySaveAs(string name, string rootPath)
    {
        if (!TryCreate(name, rootPath)) return false;

        foreach (Chunk<VoxelUnit> voxelChunk in Voxelator.VoxelChunkManager.Chunks)
            if (voxelChunk != null)
                OnUpdateVoxelChunk(voxelChunk);

        foreach (Chunk<VertexUnit> vertexChunk in Voxelator.VertexChunkManager.Chunks)
            if (vertexChunk != null)
                OnUpdateVertexChunk(vertexChunk);

        return true;
    }

    private static void CreateDirectories()
    {
        if (Directory.Exists(ProjectPath)) Directory.Delete(ProjectPath, true);

        Directory.CreateDirectory(ProjectPath);
        Directory.CreateDirectory(DataPath);
        Directory.CreateDirectory(VoxelsPath);
        Directory.CreateDirectory(VerticesPath);

        using (StreamManager stream = new StreamManager($@"{ProjectPath}\{MainFileName}"))
        {
            if(stream.OpenWrite())
            {
                stream.Write(BitConverterWrapper.GetBytes(Voxelator.FieldSize));
                stream.Write(BitConverter.GetBytes(Voxelator.IncrementOption));
            }
        }
    }

    public static void OpenDirectory()
    {
        if (!Directory.Exists(ProjectPath)) return;

        Process.Start("explorer.exe", ProjectPath);
    }

    public static void Ouit()
    {
        Application.Quit();
    }

    public static void Release()
    {
        if(Voxelator.VoxelChunkManager != null) Voxelator.VoxelChunkManager.UpdateChunkEvent -= OnUpdateVoxelChunk;
        if (Voxelator.VertexChunkManager != null) Voxelator.VertexChunkManager.UpdateChunkEvent -= OnUpdateVertexChunk;

        if(_savingThread != null) _savingThread.Abort();
    }

    private static bool OnQuit()
    {
        if(_savingThread.IsAlive) while (!_saved) { }
        return true;
    }

    private static async void OnUpdateVoxelChunk(Chunk<VoxelUnit> chunk)
    {
        if (_voxelChunksList.Contains(chunk)) return;

        Saved.Value = false;

        LinkedListNode<Chunk<VoxelUnit>> node = new LinkedListNode<Chunk<VoxelUnit>>(chunk);
        _voxelChunksList.AddFirst(node);
        await Task.Delay(1000);
        _voxelChunksList.Remove(node);

        _voxelChunksQueue.Enqueue(chunk);
        OnUpdateChunk();
    }

    private static async void OnUpdateVertexChunk(Chunk<VertexUnit> chunk)
    {
        if (_vertexChunksList.Contains(chunk)) return;

        Saved.Value = false;

        LinkedListNode<Chunk<VertexUnit>> node = new LinkedListNode<Chunk<VertexUnit>>(chunk);
        _vertexChunksList.AddFirst(node);
        await Task.Delay(1000);
        _vertexChunksList.Remove(node);

        _vertexChunksQueue.Enqueue(chunk);
        OnUpdateChunk();
    }

    private static async void OnUpdateChunk()
    {
        if (!_saved) return;

        Saved.Value = false;
        Saved.Value = false;

        _saved = false;
        await Task.Run(() =>
        {
            while (!_saved && _voxelChunksList.Count != 0 && _vertexChunksList.Count != 0) { }
        });

        Saved.Value = true;
    }

    private static void Saving()
    {
        //abstract writing function 
        void write<C, U>(string path, string chunkName, ChunkManager<C, U> chunkManageer, ConcurrentQueue< Chunk<U>> chunks, Action<int, U, StreamManager> writeAction) where C : Chunk<U> where U : Unit
        {
            if (!chunks.IsEmpty)
            {
                Chunk<U> chunk;
                if (chunks.TryDequeue(out chunk))
                {
                    int chunkIndex = chunkManageer.GetChunkIndex(chunk.Position);
                    //using (BinaryWriter writer = new BinaryWriter(File.Open($@"{path}\{chunkName}{chunkIndex}", FileMode.Create), Encoding.UTF8))
                    //{
                    //    for (int i = 0; i < chunk.Units.Length; i++)
                    //    {
                    //        if (chunk.Units[i] != null)
                    //        {
                    //            byte[] bytes = BitConverter.GetBytes(VoxelatorArray.GetIndex(chunkManageer.FieldSize, chunk.Units[i].Position));
                    //            writer.BaseStream.Write(bytes, 0, bytes.Length);
                    //            writeAction(i, chunk.Units[i], writer);
                    //        }
                    //    }
                    //}
                    using (StreamManager stream = new StreamManager($@"{path}\{chunkName}{chunkIndex}"))
                    {
                        if(stream.OpenWrite())
                        {
                            for (int i = 0; i < chunk.Units.Length; i++)
                            {
                                if (chunk.Units[i] != null)
                                {
                                    stream.Write(BitConverter.GetBytes(VoxelatorArray.GetIndex(chunkManageer.FieldSize, chunk.Units[i].Position)));
                                    writeAction(i, chunk.Units[i], stream);
                                }
                            }
                        }
                    }
                }
            }
        }

        //writing
        while(true)
        {
            if (!_saved)
            {
                write(VoxelsPath, VoxelChunkName, Voxelator.VoxelChunkManager, _voxelChunksQueue, (index, voxel, stream) =>
                {
                    stream.Write(BitConverterWrapper.GetBytes(voxel.Color));
                });

                write(VerticesPath, VertexChunkName, Voxelator.VertexChunkManager, _vertexChunksQueue, (index, vertex, stream) =>
                {
                    byte[] bytes = BitConverterWrapper.GetBytes((vertex.GetOffset() * Voxelator.IncrementOption).ToVector3Int().ToVector3Byte());
                    stream.Write(bytes);
                });

                if (_voxelChunksQueue.IsEmpty && _vertexChunksQueue.IsEmpty)
                    _saved = true;
            }

            Thread.Sleep(100);
        }
    }
}


