# Asset List
`ScriptableObject` that automatically aggregates assets in a project using
`AssetDatabase.FindAssets`.
The list is automatically updated when entering Play mode or building the
project, so there is no need to manually update it.
The list is ordered by the assets' GUID, avoiding file diffs if a list update
contains the same assets as before.

![](Extras~/demo.gif)


## Installing
Install via [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html)
using this repository URL:

```
https://github.com/gilzoide/AssetList.git
```
