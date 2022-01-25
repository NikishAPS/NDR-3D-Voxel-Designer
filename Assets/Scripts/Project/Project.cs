using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using UnityEngine;

public class Project : MonoBehaviour
{
    public static ref ReportField<bool> Saved { get => ref _saved; }

    //C:\Example
    public static string Name { get; set; } //Example (Project name)
    public static string MainFileName { get; set; } //Example.ndr (C:\Example\Example.ndr)
    public static string RootPath { get; set; } //C:\
    public static string ProjectPath => Path.GetFullPath($@"{RootPath}\{Name}"); //C:\Example
    public static string DataPath => ProjectPath + @"\Data"; //C:\Example\Data
    public static string VoxelsPath => $@"{DataPath}\Voxels"; //C:\Example\Data\Voxels
    public static string VerticesPath => $@"{DataPath}\Vertices"; //C:\Example\Data\Vertices
    public static string ModelsPath => $@"{DataPath}\Models"; //C:\Example\Data.Models
    public static string VoxelChunkPath => $@"{VoxelsPath}\{VoxelChunkName}"; //C:\Example\Data\Voxels\VoxelChunk
    public static string VertexChunkPath => $@"{VerticesPath}\{VertexChunkName}"; //C:\Example\Data\Vertices\VertexChunk
    public static string FileExtension => "ndr";
    public static string VoxelChunkName => "VoxelChunk";
    public static string VertexChunkName => "VertexChunk";

    private static ReportField<bool> _saved;
    private static bool _savedAsync;

    private static Thread _savingThread;
    private static readonly ConcurrentQueue<Chunk<VoxelUnit>> _voxelChunksQueue = new ConcurrentQueue<Chunk<VoxelUnit>>();
    private static readonly ConcurrentQueue<Chunk<VertexUnit>> _vertexChunksQueue = new ConcurrentQueue<Chunk<VertexUnit>>();
    private static readonly ConcurrentQueue<ObjModel> _objModelsQueue = new ConcurrentQueue<ObjModel>();
    private static LinkedList<Chunk<VoxelUnit>> _voxelChunksList = new LinkedList<Chunk<VoxelUnit>>();
    private static LinkedList<Chunk<VertexUnit>> _vertexChunksList = new LinkedList<Chunk<VertexUnit>>();
    private static LinkedList<ObjModel> _objModelsList = new LinkedList<ObjModel>();

    static Project()
    {
        Application.wantsToQuit += OnQuit;

        Saved = new ReportField<bool>(true);
    }

    public static void Init()
    {
        Saved.Value = _savedAsync = true;

        _savingThread = new Thread(Saving)
        {
            IsBackground = true,
            Priority = System.Threading.ThreadPriority.Normal
        };

        _savingThread.Start();

        Voxelator.VoxelChunkManager.UpdateChunkEvent += OnUpdateVoxelChunk;
        Voxelator.VertexChunkManager.UpdateChunkEvent += OnUpdateVertexChunk;
        ObjModelManager.UpdateModelEvent += OnUpdateObjModel;
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
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            Vector3Int fieldSize;
            int incrementOption;

            if (!reader.TryRead(out fieldSize) ||
            !reader.TryRead(out incrementOption)) return false;

            Release();
            Voxelator.Release();
            Voxelator.Init(fieldSize, (Voxelator.IncrementOptionType)incrementOption);
        }

        //abstract reading function 
        void readChunk<C, U>(string unitsPath, ChunkManager<C, U> chunkManager, Func<BinaryReader, bool> readFunc) where C : Chunk<U> where U : Unit
        {
            for (int i = 0; i < chunkManager.Chunks.Length; i++)
            {
                foreach (string currentChunkPath in Directory.GetFiles(unitsPath))
                {
                    //using (BinaryReader reader = new BinaryReader(File.OpenRead($@"{chunkPath}{i}")))
                    using (BinaryReader reader = new BinaryReader(File.OpenRead(currentChunkPath)))
                    {
                        while (true)
                        {
                            if (!readFunc(reader)) break;
                        }
                    }
                }
            }
        }

        //voxel data reading
        readChunk(VoxelsPath, Voxelator.VoxelChunkManager, (reader) =>
        {
            int voxelIndex;
            Vector3Byte color;
            if (!reader.TryRead(out voxelIndex) || !reader.TryRead(out color)) return false;

            Voxelator.CreateVoxel(voxelIndex, color);
            return true;
        });

        //vertex data reading
        readChunk(VerticesPath, Voxelator.VertexChunkManager, (reader) =>
        {
            int vertexIndex;
            Vector3Byte vertexOffset;

            if (!reader.TryRead(out vertexIndex) || !reader.TryRead(out vertexOffset)) return false;

            Vector3Int vertexPosition = VoxelatorArray.GetPosition(Voxelator.FieldSize + Vector3Int.one, vertexIndex);
            Voxelator.VertexChunkManager.SetVertexOffset(vertexPosition, vertexOffset.ToVector3() / Voxelator.IncrementOption);
            return true;
        });

        Voxelator.UpdateChunks();
        Init();

        //obj models data reading
        foreach (string currentPath in Directory.GetFiles(ModelsPath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(currentPath)))
            {
                string modelPath;
                if (reader.TryRead(out modelPath))
                {
                    Vector3 modelPosition = Voxelator.Center;
                    float modelSize = 1;
                    if (reader.TryRead(out modelPosition))
                        reader.TryRead(out modelSize);

                    ObjModelManager.Import(modelPath, modelPosition, modelSize);
                }
            }
        }

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
        Directory.CreateDirectory(ModelsPath);

        using (BinaryWriter writer = new BinaryWriter(File.Create($@"{ProjectPath}\{MainFileName}")))
        {
            writer.Write(Voxelator.FieldSize);
            writer.Write(Voxelator.IncrementOption);
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
        ObjModelManager.UpdateModelEvent -= OnUpdateObjModel;

        if(_savingThread != null) _savingThread.Abort();
    }

    private static bool OnQuit()
    {
        if(_savingThread.IsAlive) while (!_savedAsync) { }
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
        OnSave();
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
        OnSave();
    }

    private static async void OnUpdateObjModel(ObjModel model)
    {
        if (_objModelsList.Contains(model)) return;

        Saved.Value = false;

        LinkedListNode<ObjModel> node = new LinkedListNode<ObjModel>(model);
        _objModelsList.AddFirst(node);
        await Task.Delay(1000);
        _objModelsList.Remove(node);

        _objModelsQueue.Enqueue(model);
        OnSave();
    }

    private static async void OnSave()
    {
        if (!_savedAsync) return;

        Saved.Value = false;
        _savedAsync = false;

        await Task.Run(() =>
        {
            //while (!_savedAsync && _voxelChunksList.Count != 0 && _vertexChunksList.Count != 0) { }
            //while (!_savedAsync && _voxelChunksQueue.Count != 0 && _vertexChunksQueue.Count != 0) { }
            while (!_savedAsync) { }
        });

        Saved.Value = true;
    }

    private static void Saving()
    {
        //abstract writing function 
        void write<C, U>(string path, string chunkName, ChunkManager<C, U> chunkManageer, ConcurrentQueue< Chunk<U>> chunks, Action<int, U, BinaryWriter> writeAction) where C : Chunk<U> where U : Unit
        {
            if (!chunks.IsEmpty)
            {
                Chunk<U> chunk;
                if (chunks.TryDequeue(out chunk))
                {
                    int chunkIndex = chunkManageer.GetChunkIndex(chunk.Position);
                    using (BinaryWriter writer = new BinaryWriter(File.Open($@"{path}\{chunkName}{chunkIndex}", FileMode.Create)))
                    {
                        for (int i = 0; i < chunk.Units.Length; i++)
                        {
                            if (chunk.Units[i] != null)
                            {
                                writer.Write(VoxelatorArray.GetIndex(chunkManageer.FieldSize, chunk.Units[i].Position));
                                writeAction(i, chunk.Units[i], writer);
                            }
                        }
                    }
                }
            }
        }

        //writing
        while(true)
        {
            if (!_savedAsync)
            {
                //voxel saving
                write(VoxelsPath, VoxelChunkName, Voxelator.VoxelChunkManager, _voxelChunksQueue, (index, voxel, writer) =>
                {
                    writer.Write(BitConverterWrapper.GetBytes(voxel.Color));
                });

                //vertex saving
                write(VerticesPath, VertexChunkName, Voxelator.VertexChunkManager, _vertexChunksQueue, (index, vertex, writer) =>
                {
                    byte[] bytes = BitConverterWrapper.GetBytes((vertex.GetOffset() * Voxelator.IncrementOption).ToVector3Int().ToVector3Byte());
                    writer.Write(bytes);
                });

                //obj model saving
                if (!_objModelsQueue.IsEmpty)
                {
                    ObjModel model;
                    if (_objModelsQueue.TryDequeue(out model))
                    {
                        using (BinaryWriter writer = new BinaryWriter(File.Create($@"{ModelsPath}\{Path.GetFileNameWithoutExtension(model.Path)}")))
                        {
                            writer.Write(model.Path);
                            writer.Write(model.Position);
                            writer.Write(model.Size);
                        }
                    }
                }

                if (_voxelChunksQueue.IsEmpty && _vertexChunksQueue.IsEmpty && _objModelsQueue.IsEmpty)
                    _savedAsync = true;
            }

            Thread.Sleep(100);
        }
    }
}


