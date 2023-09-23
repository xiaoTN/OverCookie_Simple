using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public static class RoleUtils
{
    public static IObservable<Unit> MoveToAsObservable(this Transform root,Vector3 target,float speed, float stopDistance = 0.5f)
    {
        return Observable.Create<Unit>(observer =>
        {
            CompositeDisposable disposable = new CompositeDisposable();
            root.UpdateAsObservable()
                .Subscribe(unit =>
                {
                    if (Vector3.SqrMagnitude(root.position - target) < stopDistance * stopDistance)
                    {
                        root.position = target;
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                    }
                    root.position= Vector3.MoveTowards(root.position, target, Time.deltaTime*speed);
                })
                .AddTo(disposable);
            return disposable;
        });
    }

    public static bool MoveToUpdate(this Transform root, Vector3 target,float speed, float stopDistance = 1.5f)
    {
        if (Vector3.SqrMagnitude(root.position - target) < stopDistance * stopDistance)
        {
            return true;
        }
        root.position= Vector3.MoveTowards(root.position, target, Time.deltaTime* speed);
        if (Vector3.SqrMagnitude(root.position - target) < stopDistance * stopDistance)
        {
            return true;
        }
        return false;
    }
}