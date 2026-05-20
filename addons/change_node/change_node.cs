#if TOOLS
using Godot;
using System.Diagnostics;

[Tool]
public partial class change_node : EditorPlugin
{
	private Tree customSceneTree;
	private Tree customFileSystem;
	private Node sceneTreeRoot;
	private DirAccess fileSystem; 
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
		customSceneTree = GetNode<Tree>("VBoxContainer/HBoxContainer/SceneTree/VBoxContainer/Tree");
		sceneTreeRoot = GetNode(".");
		TreeItem treeRoot = customSceneTree.CreateItem(); 
		treeRoot.SetText(0, sceneTreeRoot.Name);
		treeRoot.SetMetadata(0, sceneTreeRoot.GetPath());
		buildTree(sceneTreeRoot, treeRoot);

		customFileSystem = GetNode<Tree>("VBoxContainer/HBoxContainer/FileSystem/Tree");

		
		buildCustomFilesystem("res://");
	}

	private void buildCustomFilesystem(string path, TreeItem parent = null)
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
					TreeItem treeItem = customFileSystem.CreateItem(parent);
					treeItem.SetText(0, elementName);
					treeItem.SetMetadata(0, elementPath);
					Debug.Print($"{elementPath}/{elementName}");
					buildCustomFilesystem($"{elementPath}/{elementName}", treeItem);
				}

				else
				{
					TreeItem treeItem = customFileSystem.CreateItem(parent);
					treeItem.SetText(0, elementName);
					Debug.Print($"{elementPath}/{elementName}");
					treeItem.SetMetadata(0, $"{elementPath}/{elementName}");
				}
				elementName = dir.GetNext();
			}
		}
	}

	private void buildTree(Node node, TreeItem treeItem)
	{
		for(int i = 0; i < node.GetChildCount(); i++)
		{
			Node child = node.GetChild(i);
			TreeItem newTreeItem = customSceneTree.CreateItem(treeItem);
			newTreeItem.SetText(0, child.Name);
			newTreeItem.SetMetadata(0, child.GetPath());
			if (node.GetChildCount() > 0)
			{
				buildTree(child, newTreeItem);
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

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
	}
}
#endif
