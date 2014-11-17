using EnvDTE;

namespace TortoiseGitToolbar.UnitTests.Services
{
    /// <summary>
    /// ActiveDocument implementation for tests.
    /// </summary>
    public class DocumentMock : Document
    {
        public DocumentMock(int currentLine, string fullName)
        {
            Selection = new SelectionMock(currentLine);
            FullName = fullName;
        }

        public void Activate()
        {
            throw new System.NotImplementedException();
        }

        public void Close(vsSaveChanges Save = vsSaveChanges.vsSaveChangesPrompt)
        {
            throw new System.NotImplementedException();
        }

        public Window NewWindow()
        {
            throw new System.NotImplementedException();
        }

        public bool Redo()
        {
            throw new System.NotImplementedException();
        }

        public bool Undo()
        {
            throw new System.NotImplementedException();
        }

        public vsSaveStatus Save(string FileName = "")
        {
            throw new System.NotImplementedException();
        }

        public object Object(string ModelKind = "")
        {
            throw new System.NotImplementedException();
        }

        public void PrintOut()
        {
            throw new System.NotImplementedException();
        }

        public void ClearBookmarks()
        {
            throw new System.NotImplementedException();
        }

        public bool MarkText(string Pattern, int Flags = 0)
        {
            throw new System.NotImplementedException();
        }

        public bool ReplaceText(string FindText, string ReplaceText, int Flags = 0)
        {
            throw new System.NotImplementedException();
        }

        public DTE DTE { get; private set; }
        public string Kind { get; private set; }
        public Documents Collection { get; private set; }
        public Window ActiveWindow { get; private set; }

        public string FullName { get; private set; }

        public string Name { get; private set; }
        public string Path { get; private set; }
        public bool ReadOnly { get; set; }
        public bool Saved { get; set; }
        public Windows Windows { get; private set; }
        public ProjectItem ProjectItem { get; private set; }

        public dynamic Selection { get; private set; }

        public class SelectionMock
        {
            public SelectionMock(int currentLine)
            {
                CurrentLine = currentLine;
            }
            public int CurrentLine { get; private set; }
        }

        public object get_Extender(string ExtenderName)
        {
            throw new System.NotImplementedException();
        }

        public object ExtenderNames { get; private set; }
        public string ExtenderCATID { get; private set; }
        public int IndentSize { get; private set; }
        public string Language { get; set; }
        public int TabSize { get; private set; }
        public string Type { get; private set; }
    }
}