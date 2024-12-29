namespace Pikol93.CJ13;

public interface IInteractable
{
    void Interact();
    bool IsDisplayable { get; }
    string DisplayableName { get; }
    string DisplayableDescription { get; }
}