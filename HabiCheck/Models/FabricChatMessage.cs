// FabricChatMessage.cs
// Represents a single message in the fabric-specific AI chatbot conversation.

namespace HabiCheck.Models;

public class FabricChatMessage
{
    /// <summary>
    /// Role of the sender, typically "user" or "assistant".
    /// </summary>
    public string Role { get; set; } = "user";

    /// <summary>
    /// Content of the message.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of FabricChatMessage.
    /// </summary>
    public FabricChatMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }
}
