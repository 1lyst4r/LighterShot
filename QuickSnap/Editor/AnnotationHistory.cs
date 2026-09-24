using System;
using System.Collections.Generic;
using LightlyShot.Editor.Annotations;

namespace LightlyShot.Editor;
internal sealed class AnnotationHistory
{
    private readonly List<Annotation> appliedAnnotations = new();
    private readonly Stack<Annotation> undoneAnnotations = new();

    public event Action? Changed;

    public IReadOnlyList<Annotation> Applied => appliedAnnotations;

    public bool CanUndo => appliedAnnotations.Count > 0;

    public bool CanRedo => undoneAnnotations.Count > 0;

    public void Add(Annotation annotation)
    {
        appliedAnnotations.Add(annotation);
        undoneAnnotations.Clear();      // a new action makes the old redo trail meaningless
        Changed?.Invoke();
    }

    public void Undo()
    {
        if (!CanUndo) return;

        Annotation lastAnnotation = appliedAnnotations[^1];
        appliedAnnotations.RemoveAt(appliedAnnotations.Count - 1);
        undoneAnnotations.Push(lastAnnotation);
        Changed?.Invoke();
    }

    public void Redo()
    {
        if (!CanRedo) return;

        appliedAnnotations.Add(undoneAnnotations.Pop());
        Changed?.Invoke();
    }

    public void Clear()
    {
        appliedAnnotations.Clear();
        undoneAnnotations.Clear();
        Changed?.Invoke();
    }
    public void Restore(IReadOnlyList<Annotation> annotations)
    {
        appliedAnnotations.Clear();
        appliedAnnotations.AddRange(annotations);
        undoneAnnotations.Clear();
        Changed?.Invoke();
    }
}
