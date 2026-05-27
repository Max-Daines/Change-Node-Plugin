#if TOOLS
using Godot;
using System.Diagnostics;
using System.Collections.Generic;

[Tool]
public partial class change_node : EditorPlugin
{
	private Tree customFileSystem;
	private List<(string, string)> filePaths = new();
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.

		customFileSystem = GetNode<Tree>("VBoxContainer/HBoxContainer/VBoxContainer/FileSystem/Tree");

		buildCustomFilesystem("res://");
	}

	private void buildCustomFilesystem(string path)
	{
		DirAccess dir = DirAccess.Open(path);
		dir.IncludeHidden = false;
		if(dir != null)
		{
			dir.ListDirBegin();
			string elementName = dir.GetNext();
			while (elementName.StartsWith("."))
			{
				elementName = dir.GetNext();
			}
			string elementPath = dir.GetCurrentDir();
			
			
			while(elementName != "")
			{
				if (dir.CurrentIsDir())
				{
					buildCustomFilesystem($"{elementPath}/{elementName}");
				}

				else
				{
					(string, string) file;
					if (elementPath.EndsWith("/"))
					{
						file = (elementName, $"{elementPath}{elementName}");
					}
					else
					{
						file = (elementName, $"{elementPath}/{elementName}");
					}
					
					filePaths.Add(file);
				}
				elementName = dir.GetNext();
			}
		}
		buildFileTree();
	}

	private void buildFileTree(string searchTerm = "")
	{
		customFileSystem.Clear();
		foreach ((string, string) path in filePaths)
		{
			if (searchTerm == "")
			{
				TreeItem treeItem = customFileSystem.CreateItem();
				treeItem.SetText(0, path.Item1);
				treeItem.SetMetadata(0, path.Item2);
			}
			else if (path.Item1.ToLower().Contains(searchTerm.ToLower()))
			{
				TreeItem treeItem = customFileSystem.CreateItem();
				treeItem.SetText(0, path.Item1);
				treeItem.SetMetadata(0, path.Item2);
			}
		}
	}

	private void _on_file_system_tree_multi_selected(TreeItem item, int column, bool selected)
	{
		if (selected)
		{
		Debug.Print("" + item.GetMetadata(0));
		}
	}

	private void _on_line_edit_text_changed(string text)
	{
		buildFileTree(text);
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
	}
}
#endif
