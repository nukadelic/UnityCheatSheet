### ECS Authoring Template
This is a template to convert an existing GameObject in scene to ECS.  
The script name ( ScriptName.cs ) should have a matching name to first class in the code below.  
( In this example we use `ScriptName` )  

`ScriptName` is a mono behaviour that will be attached to the top level game object ( every other nested / child GameObjects will be converted automaticlly, there is no need to add conversion scripts to them )   
Note: Since `ConvertToEntity` is required for this script to work, the `RequireComponent` attribute will automaticlly add this component to the game object 

```cs
// Avoid namespace confusion
// using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

// --------------------------------
// .    Convert : Mono -> Entity 
// --------------------------------

[ RequiresEntityConversion, UnityEngine.DisallowMultipleComponent ]
[ UnityEngine.RequireComponent( typeof( ConvertToEntity ) ) ]
public class ScriptName : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    // Editor exposed variable to be set on start
    public float value = 1f;

    // IConvertGameObjectToEntity will intersept the conversion process, in this step 
    // we can add any number of ecs-components to the entitiy
    
    public void Convert( Entity id, EntityManager manager, GameObjectConversionSystem convSys )
    {
        var data = new ScriptName_Data {
        
            value = math.abs( value )

        };
    
        manager.AddComponentData( id, data );
    }
}

// --------------------------------
// .    Component : Data
// --------------------------------

// Serializable attribute for editor support
[System.Serializable]  public struct ScriptName_Data : IComponentData
{
    public float value;
}

// --------------------------------
// .    System : Behaviour
// --------------------------------

// This system updates all entities in the scene with both a `ScriptName_Data` and `Rotation` component.
public class ScriptName_System : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Entities.ForEach processes each set of ComponentData on the main thread 
        // ( not the recommended for best performance )
        
        Entities.ForEach( ( ref ScriptName_Data data, ref Rotation rotation ) => 
        {
            // Simple example using the value that was set in the editor
            // to rotate the entity around the x-axis
          
            float dt = UnityEngine.Time.deltaTime;
            // data value is the connected component for the current entitiy in the loop 
            var delta_rot = data.value * dt;
            
            rotation.Value = math.mul(
                math.normalize( rotation.Value ), 
                quaternion.AxisAngle( xAxis, delta_rot )
            );
        });
    }
    
    static float3 xAxis = new float3( 1, 0, 0 );
}
```
