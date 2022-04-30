# FishNet.CodeAnalysis
 
A set of annotations, Roslyn analyzers & source generators for Fish-Networking (https://github.com/FirstGearGames/FishNet).

## Analyzers' Requirements

- Unity 2020 or later

## Source Generators' Requirements

- Unity 2021 or later

## Installing The Analyzers

- Download the latest release
- Extract the downloaded archive
- Copy `FishNet.CodeAnalysis.Analyzers.dll` to your project's `Assets` folder
- Select the imported dll
- Make sure all platforms are ***unchecked***
- Add the RoslynAnalyzer label to your dll

<div align="center">
 <img src="https://user-images.githubusercontent.com/37028872/165657976-e4451df8-6d75-46ec-b6c1-9a12fbc71a5a.gif" alt="getting-started"/>
</div>

## Installing The Source Generators

- TBA

## Included Analyzers

- `DontDestroyOnLoadUsageAnalyzer` → Detects invalid usage of `Object.DontDestroyOnLoad` on `NetworkObject`s and `NetworkBehaviour`s
- `NetworkBehaviourCallbackBaseMethodCallAnalyzer` → Detects missing and unordered calls to the base method of `NetworkBehaviour` callbacks

## Included Source Generators

- TBA
