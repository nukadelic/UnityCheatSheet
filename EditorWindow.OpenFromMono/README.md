# Open editor window from component

Attached script to game object in this example is using on validate to open custom editor window 

### Mono Behaviour

```cs
using UnityEngine;

[ExecuteInEditMode]
public class MyMono : MonoBehaviour
{
        public static System.Action onValidate;

        private void OnValidate()
        {
            onValidate?.Invoke();
        }
}
```

### Editor

```cs
using UnityEditor;
using UnityEngine;

[InitializeOnLoad] class MyMonoWindow : EditorWindow
{
    static MyMonoWindow()
    {
        MyMono.onValidate += OpenWindow;
        AssemblyReloadEvents.beforeAssemblyReload += Kill;
    }
    static void Kill()
    {
        if ( instance != null) instance.Close();
        AssemblyReloadEvents.beforeAssemblyReload -= Kill;
        MyMono.onValidate -= OpenWindow;
    }

    public static MyMonoWindow instance;

    [MenuItem("Window/My Mono Editor")]
    public static void OpenWindow()
    {
        if (instance != null) instance?.Close();

        var window = GetWindow<MyMonoWindow>("My Mono Editor");
        window.minSize = new Vector2(200, 50);
        instance = window;
    }
}
```
