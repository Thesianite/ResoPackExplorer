using Godot;
using ResPackExplorer.Core.Models;
using ResPackExplorer.Core.Readers;
using System;
using System.IO;
using System.Numerics;

[GlobalClass]
public partial class PackageViewer : Control
{
	[Export] public Label _nameLabel;
	[Export] public Label _recordTypeLable;
	[Export] private Button _openButton;
	[Export] private FileDialog _fileDialog;

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
