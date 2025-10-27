using System.Reflection;

namespace NoteApp.Server.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; } = DateTime.Now;
        public string? Author { get; set; }
        public string? Tags { get; set; } = string.Empty;
        public bool IsArchived { get; set; } = false;
        public string? ImagePath { get; set; }
        public bool IsPinned { get; set; } = false;

        public Note(string title, string content, string author, string imagePath, string tags)
        {
            Title = title;
            Content = content;
            Author = author;
            ImagePath = imagePath;
            IsPinned = false;
            IsArchived = false;
            Tags = tags;
        }
        
        public Note(string title, string content, string author, string imagePath, bool isPinned = false, bool isArchived = false)
        {
            Title = title;
            Content = content;
            Author = author;
            ImagePath = imagePath;
            IsPinned = isPinned;
            IsArchived = isArchived;
            Tags = string.Empty;
        }

        public Note()
        {
            Tags = string.Empty;
        }
    }
}
