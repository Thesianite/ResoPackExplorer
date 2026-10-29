using Godot;
using ResoPackExplorer.Core.Models;
using ResoPackExplorer.Core.Readers;
using System.Collections.Generic;
using System.IO;

[GlobalClass]
public partial class PackageViewer : Control
{
	// Node references
	[Export] private Label _nameLabel;
	[Export] private Label _recordTypeLabel;
	[Export] private Label _ownerIdLabel;
	[Export] private Button _openButton;
	[Export] private FileDialog _fileDialog;
	[Export] private Tree _assetTree;


    public override void _Ready()
    {
		if (_openButton != null)
		{
			_openButton.Pressed += OnOpenButtonPressed;
		}

		if (_fileDialog != null)
		{
			_fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
			_fileDialog.Access = FileDialog.AccessEnum.Filesystem;
			_fileDialog.ClearFilters();
			_fileDialog.AddFilter("*.resonitepackage", "Resonite Package");
			_fileDialog.FileSelected += OnFileSelected;
		}

        base._Ready();
    }

	/// <summary>
	/// Load package by path.
	/// </summary>
	/// <param name="packagePath">Package path to load.</param>
	public void LoadPackage(string packagePath)
	{
		if (!File.Exists(packagePath))
		{
			GD.PrintErr($"ResoPack not found: {packagePath}");
			return;
		}

		using var packageReader = new PackageReader(packagePath);
		using var recordStream = packageReader.GetEntryStream("R-Main.record");

		if (recordStream != null)
		{
			var json = RecordParser.ParseRecordStream(recordStream);
			var record = Record.FromJson(json);

			_nameLabel.Text = $"Name: {record.Name ?? "Name: N/A"}";
			_recordTypeLabel.Text = $"Type: { record.RecordType ?? "Type: N/A"}";
			_ownerIdLabel.Text = $"Owner: {record.OwnerId ?? "Owner: N/A"}";

			_assetTree.Clear();
			TreeItem treeRoot = _assetTree.CreateItem();
			_assetTree.HideRoot = true;
			
			var assetTypeDetector = new AssetTypeDetector();
			var folderMap = new Dictionary<string, TreeItem>();
			var typeSubTreeMap = new Dictionary<string, TreeItem>();

			foreach (var (key, size) in packageReader.GetEntries())
            {
                string[] segments;
				if (key.StartsWith("Variants/") && key.IndexOf('/', "Variants/".Length) != -1)
				{
					segments = new string[] { "Variants", key.Substring("Variants/".Length) };
				}
				else
				{
					segments = key.Split('/');
				}

				TreeItem currentParent = treeRoot;
				string currentPath = "";
		
				for (int i = 0; i < segments.Length - 1; i++)
				{
					string segment = segments[i];
					currentPath = string.IsNullOrEmpty(currentPath) ? segment : $"{currentPath}/{segment}";

					
					if (!folderMap.TryGetValue(currentPath, out var folderItem))
					{
						folderItem = _assetTree.CreateItem(currentParent);
						folderItem.SetText(0, segment);
						folderMap[currentPath] = folderItem;
					}

					currentParent = folderItem;
				}

				string fileTypeName = "Unknown";

				if (size > 0)
				{
					using var entryStream = packageReader.GetEntryStream(key);
					if (entryStream != null)
					{
						var detectedType = assetTypeDetector.DetectType(entryStream);
						if (detectedType != null)
						{
							fileTypeName = detectedType.Name;
						}
					}
				}

				string typeMapKey = $"{currentPath}::{fileTypeName}";

				if (!typeSubTreeMap.TryGetValue(typeMapKey, out var typeSubFolderItem))
				{
					typeSubFolderItem = _assetTree.CreateItem(currentParent);
					typeSubFolderItem.SetText(0, fileTypeName);
					typeSubTreeMap[typeMapKey] = typeSubFolderItem;
				}


				TreeItem fileItem = _assetTree.CreateItem(typeSubFolderItem);
				string fileName = segments[segments.Length - 1];
				fileItem.SetText(0, $"{fileName} [{fileTypeName}] ({size} bytes)");
            }
		}
	}

	private void OnFileSelected(string path)
	{
		LoadPackage(path);
	}

	private void OnOpenButtonPressed()
	{
		_fileDialog.PopupCentered(new Vector2I(800, 650));
	}
}
