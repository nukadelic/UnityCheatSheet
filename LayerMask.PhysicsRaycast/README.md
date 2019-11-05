Cast all: `~0` or `-1`  
 
Cast only against colliders in layer index of 8:
```cs
var layerMask = 1 << 8;
```

Collide against everything expect 8
```cs
var layerMask = ~(1 << 8);
```

Cast only on layers A & B: 
```cs
var layerMask = ~( (1 << LayerMask.NameToLayer ("B")) | (1 << LayerMask.NameToLayer ("B")))
```

Get inspector like layer dropdown in custom editor:
```cs
// using System.Linq;
var layers = new int[32]
    .Select( (x,i) => i + ": " + LayerMask.LayerToName( i ) )
    .Where( x => ! string.IsNullOrEmpty( x.Substring( x.IndexOf(":") + 2 ) ) )
    .ToArray();
var layers_indexes = layers.Select( x => LayerMask.NameToLayer( x.Substring( x.IndexOf(":") + 2 ) ) ).ToArray();
EditorGUILayout.IntPopup( "Layers", 0, layers, layers_indexes );
```
