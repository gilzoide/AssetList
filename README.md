# Asset List
`ScriptableObject` that automatically aggregates all assets using an
`AssetDatabase` search filter.
The list is automatically updated when opening Unity or entering Play mode, so
there is no need to manually update it every time a new asset is created or
deleted.
The list is ordered by the assets' GUID, avoiding file diffs if a list update
contains the same assets as before.

![](Extras~/demo.gif)


## Installing
Either:

- Copy the [Runtime/AssetList.cs](Runtime/AssetList.cs) file into your project
- Install via [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html)
  using this repository URL:

```
https://github.com/gilzoide/AssetList.git
```
