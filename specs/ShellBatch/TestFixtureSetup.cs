namespace YamlPrompt.Specs.ShellCommands;

public class FileSystemDependentTestFixture : IDisposable
{
	private const string tempDirName = "temp";
	
    private readonly DirectoryInfo _tempDir;
	
	public FileSystemDependentTestFixture()
	{
		string workingDir = Directory.GetCurrentDirectory();
		
		_tempDir = new DirectoryInfo(Path.Combine(workingDir, tempDirName));
		if (_tempDir.Exists)
		{
			_tempDir.Delete(true);
		}
		_tempDir.Create();
	}

	public virtual void Dispose()
	{
		if (_tempDir.Exists)
		{
			_tempDir.Delete(true);
		}
	}
	
	public string CreateFile(string fileName, string content = "")
	{
		var filePath = Path.Combine(_tempDir.FullName, fileName);
		File.WriteAllText(filePath, content);
		return filePath;
	}
}