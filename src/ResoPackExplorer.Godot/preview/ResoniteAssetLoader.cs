using Godot;
using ResoPackExplorer.Core.Readers;
using System;
using System.IO;

public partial class ResoniteAssetLoader : Node
{
	public void LoadPackageAssets(string packagePath)
	{
		GD.Print($"loading {packagePath}");
		using var reader = new PackageReader(packagePath);
		
		
	}
}
