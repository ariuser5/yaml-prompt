namespace YamlPrompt.Specs;

public class FileSystemDependentTestFixture : IDisposable
{
	private const string tempDirSuffix = "yaml-prompt";
	
    private readonly DirectoryInfo _tempDir;
	
	public FileSystemDependentTestFixture()
	{
		var tempDirPath = Path.GetTempPath() + tempDirSuffix;
		var tempDirFileName = Guid.NewGuid().ToString();
		_tempDir = Directory.CreateDirectory(Path.Combine(tempDirPath, tempDirFileName));
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