index | values
---|---
0 | 10550, 40816, 70093, 79489, 43900, 32200, 52441, 20246, 33094, 89554, 59843, 45345, 49856, 82405, 67826, 54761, 88738, 75139, 55672, 52983, 10550, 40816, 70093, 79489, 43900
1 | 31005, 72154, 55180, 22999, 31005
2 | 14689, 17458, 39020, 72925, 14689 
3 | 56019, 68269, 57104, 50965, 56019
4 | 14983, 18989, 79050, 50734, 14983
5 | 68148, 20209, 53736, 10293, 68148

While NativeMultiHashMap does have an `Add()` method it still requires to have a predefined fixed 
size and does not supports growing on demand ( `1` ) ( adding more elements that its size can hold will 
throw an `System.InvalidOperationException: HashMap is full` exception )

On the other hand a native multi hash map does not care if the keys are sized differenly ( `2` ) and cam hold any amount of values below its capacity limit.

ParallelWriter type is needed when scheduling a parallel job with the hash map, make sure to include this ( `3` ) else you 
would a get `InvalidOperationException` requesting you to declare a [ReadOnly] in a IJobParallelFor job.

```cs
[BurstCompile( CompileSynchronously = true ) ]
public struct TestMultiHash : IJobParallelFor
{
    public Random random;
    
    /// (3) : Parallel writer required 
    [WriteOnly] public NativeMultiHashMap<int,int>.ParallelWriter map;

    public void Execute( int index )
    {
        var val = random.NextInt( 10000,90000 );
        
        /// (2) : write anywhere 
        if( index % 10 < 5 ) map.Add( 0, val );
        else map.Add( ( index % 10 ) - 4 , val ); 
    }
}
    
void Start( )
{
    /// (1) : needs fixed size 
    var map = new NativeMultiHashMap<int,int>( 50, Allocator.TempJob );

    var task = new TestMultiHash
    {
        map = map.AsParallelWriter(), /// (3)
        random = new Random( 7104 )
    };

    /// (1)
    task.Schedule( 50, 10 ).Complete( );
        
    for( var i = 0; i < 6; ++i )
    { 
        var txt = "";

        var vals = map.GetValuesForKey( i );

        /// (2): unknown size
        while( vals.MoveNext() ) txt += vals.Current + ", ";        
            
        Debug.Log( txt );
    }

    map.Dispose();
}
```
