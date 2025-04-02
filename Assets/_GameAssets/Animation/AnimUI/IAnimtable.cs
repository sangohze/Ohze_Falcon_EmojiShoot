using System;


public interface IAnimatable
{
    void Show(Action onComplete = null);
    void Hide(Action onComplete = null);
}
