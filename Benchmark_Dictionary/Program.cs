using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// 模擬 Rect（struct 版本）
public struct RectStruct
{
    public double X;
    public double Y;
    public double Width;
    public double Height;
    public RectStruct(double x, double y, double w, double h) { X = x; Y = y; Width = w; Height = h; }
}

// 模擬 Rect（class 版本）
public class RectClass
{
    public double X;
    public double Y;
    public double Width;
    public double Height;
    public RectClass(double x, double y, double w, double h) { X = x; Y = y; Width = w; Height = h; }
}

[MemoryDiagnoser]
public class DictionaryRectBench
{
    // Configurable size
    [Params(100, 1000, 10000)]
    public int N;

    private Dictionary<int, RectStruct> dictStruct = [];
    private Dictionary<int, RectClass> dictClass = [];
    private double Gap = 2.0;

    //[GlobalSetup]
    //public void Setup()
    //{
    //    var rnd = new Random(42);

    //    dictStruct = new Dictionary<int, RectStruct>(N);
    //    dictClass = new Dictionary<int, RectClass>(N);

    //    for (int i = 0; i < N; i++)
    //    {
    //        double w = 5 + rnd.NextDouble() * 20.0;
    //        dictStruct[i] = new RectStruct(0, 0, w, 10);
    //        dictClass[i] = new RectClass(0, 0, w, 10);
    //    }
    //}

    // 1) struct: foreach kvp, read copy, modify, write back
    [Benchmark(Baseline = true)]
    public void Struct_ReadModifyWrite()
    {
        this.SetupStruct();
        double currentX = 0;
        foreach (var kvp in dictStruct)
        {
            int key = kvp.Key;
            var r = kvp.Value;      // struct copy
            r.X = currentX;
            currentX += r.Width + Gap;
            dictStruct[key] = r;    // write back (dictionary indexer)
        }
    }

    // 2) class: foreach kvp, modify in-place (no write back)
    [Benchmark]
    public void Class_InPlaceModify()
    {
        SetupClass();
        double currentX = 0;
        foreach (var kvp in dictClass)
        {
            var r = kvp.Value;      // reference
            r.X = currentX;         // modify in-place
            currentX += r.Width + Gap;
            // no dict write-back
        }
    }

    // 3) struct: use enumerator and dictionary indexing for read + write (explicit)
    [Benchmark]
    public void Struct_Enumerator_Indexer()
    {
        this.SetupStruct();
        double currentX = 0;
        var e = dictStruct.GetEnumerator();
        while (e.MoveNext())
        {
            var key = e.Current.Key;
            var r = dictStruct[key]; // read via indexer (copy)
            r.X = currentX;
            currentX += r.Width + Gap;
            dictStruct[key] = r;     // write back
        }
    }

    // 4) class: enumerator loop modify in-place
    [Benchmark]
    public void Class_Enumerator_InPlace()
    {
        SetupClass();
        double currentX = 0;
        var e = dictClass.GetEnumerator();
        while (e.MoveNext())
        {
            var r = e.Current.Value; // reference
            r.X = currentX;
            currentX += r.Width + Gap;
        }
    }
    void SetupClass()
    {
        var rnd = new Random(42);

        dictClass = new Dictionary<int, RectClass>(N);

        for (int i = 0; i < N; i++)
        {
            double w = 5 + rnd.NextDouble() * 20.0;
            dictClass[i] = new RectClass(0, 0, w, 10);
        }
    }

    public void SetupStruct()
    {
        var rnd = new Random(42);

        dictStruct = new Dictionary<int, RectStruct>(N);

        for (int i = 0; i < N; i++)
        {
            double w = 5 + rnd.NextDouble() * 20.0;
            dictStruct[i] = new RectStruct(0, 0, w, 10);
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<DictionaryRectBench>();
    }
}
